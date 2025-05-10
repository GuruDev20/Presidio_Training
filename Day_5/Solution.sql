-- cursor

--Write a cursor that loops through all films and prints titles longer than 120 minutes
DO $$
DECLARE
    film_record RECORD;
BEGIN
    FOR film_record IN SELECT title, length FROM film LOOP
        IF film_record.length > 120 THEN
            RAISE NOTICE 'Title: %', film_record.title;
        END IF;
    END LOOP;
END $$;


--Create a cursor that iterates through all customers and counts how many rentals each made.
DO $$
DECLARE
    customer_record RECORD;
    rental_count INT;
BEGIN
    FOR customer_record IN SELECT customer_id FROM customer LOOP
        SELECT COUNT(*) INTO rental_count FROM rental WHERE customer_id = customer_record.customer_id;
        RAISE NOTICE 'Customer % has % rentals', customer_record.customer_id, rental_count;
    END LOOP;
END $$;


--Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.
DO $$
DECLARE
    film_record RECORD;
    rental_count INT;
BEGIN
    FOR film_record IN SELECT film_id FROM film LOOP
        SELECT COUNT(*) INTO rental_count FROM inventory i JOIN rental r ON i.inventory_id = r.inventory_id WHERE i.film_id = film_record.film_id;
        IF rental_count < 5 THEN
            UPDATE film SET rental_rate = rental_rate + 1 WHERE film_id = film_record.film_id;
        END IF;
    END LOOP;
END $$;


--Create a function using a cursor that collects titles of all films from a particular category.
CREATE OR REPLACE FUNCTION get_titles_by_category(cat_name TEXT)
RETURNS TABLE(title TEXT) AS $$
DECLARE
    film_record RECORD;
BEGIN
    FOR film_record IN
        SELECT f.title
        FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        JOIN category c ON c.category_id = fc.category_id
        WHERE c.name = cat_name
    LOOP
        title := film_record.title;
        RETURN NEXT;
    END LOOP;
END;
$$ LANGUAGE plpgsql;
SELECT * FROM get_titles_by_category('Action');



--Loop through all stores and count how many distinct films are available in each store using a cursor.
DO $$
DECLARE
    store_record RECORD;
    film_count INT;
BEGIN
    FOR store_record IN SELECT store_id FROM store LOOP
        SELECT COUNT(DISTINCT inventory.film_id)
        INTO film_count
        FROM inventory
        WHERE store_id = store_record.store_id;

        RAISE NOTICE 'Store % has % distinct films', store_record.store_id, film_count;
    END LOOP;
END $$;




--trigger

--Write a trigger that logs whenever a new customer is inserted.
CREATE TABLE customer_log (
    log_id SERIAL PRIMARY KEY,
    customer_id INT,
    log_time TIMESTAMP DEFAULT now()
);

CREATE OR REPLACE FUNCTION log_new_customer()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO customer_log(customer_id) VALUES (NEW.customer_id);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_log_new_customer
AFTER INSERT ON customer
FOR EACH ROW
EXECUTE FUNCTION log_new_customer();



--Create a trigger that prevents inserting a payment of amount 0.
CREATE OR REPLACE FUNCTION prevent_zero_payment()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.amount = 0 THEN
        RAISE EXCEPTION 'Payment amount cannot be 0';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER trg_prevent_zero_payment_v2
BEFORE INSERT ON payment
FOR EACH ROW
EXECUTE FUNCTION prevent_zero_payment();



--Set up a trigger to automatically set last_update on the film table before update.
CREATE OR REPLACE FUNCTION update_last_update()
RETURNS TRIGGER AS $$
BEGIN
    NEW.last_update := now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_last_update
BEFORE UPDATE ON film
FOR EACH ROW
EXECUTE FUNCTION update_last_update();



--Create a trigger to log changes in the inventory table (insert/delete).
CREATE TABLE inventory_log (
    log_id SERIAL,
    inventory_id INTEGER,
    action TEXT,
    log_time TIMESTAMP DEFAULT now()
);

CREATE OR REPLACE FUNCTION log_inventory_changes()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        INSERT INTO inventory_log (inventory_id, action) VALUES (NEW.inventory_id, 'INSERT');
    ELSIF TG_OP = 'DELETE' THEN
        INSERT INTO inventory_log (inventory_id, action) VALUES (OLD.inventory_id, 'DELETE');
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_inventory_changes
AFTER INSERT OR DELETE ON inventory
FOR EACH ROW
EXECUTE FUNCTION log_inventory_changes();



--Write a trigger that ensures a rental can’t be made for a customer who owes more than $50.
CREATE OR REPLACE FUNCTION check_customer_balance()
RETURNS TRIGGER AS $$
DECLARE
    balance NUMERIC;
BEGIN
    SELECT SUM(amount) - SUM(p.amount) INTO balance
    FROM payment p
    WHERE p.customer_id = NEW.customer_id;

    IF balance > 50 THEN
        RAISE EXCEPTION 'Customer owes more than $50';
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_check_customer_balance
BEFORE INSERT ON rental
FOR EACH ROW
EXECUTE FUNCTION check_customer_balance();



--transactions

--Write a transaction that inserts a customer and an initial rental in one atomic operation.
BEGIN;

INSERT INTO customer (store_id, first_name, last_name, email, address_id, active, create_date)
VALUES (1, 'test', 'test', 'test@example.com', 1, true, now())
RETURNING customer_id INTO new_customer_id;

INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
VALUES (now(), 1, new_customer_id, 1);

COMMIT;


--Simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.
BEGIN;

UPDATE film SET rental_duration = rental_duration + 1 WHERE film_id = 1;

INSERT INTO inventory (film_id, store_id) VALUES (1, 1);

ROLLBACK;



--Create a transaction that transfers an inventory item from one store to another.
BEGIN;

UPDATE inventory SET store_id = 2 WHERE inventory_id = 5;

-- Insert a log if needed (optional)
INSERT INTO inventory_log (inventory_id, action) VALUES (5, 'TRANSFER');

COMMIT;



--Demonstrate SAVEPOINT and ROLLBACK TO SAVEPOINT by updating payment amounts, then undoing one.
BEGIN;

UPDATE payment SET amount = amount + 1 WHERE payment_id = 1;

SAVEPOINT before_second_update;

UPDATE payment SET amount = amount + 1 WHERE payment_id = 2;
		
ROLLBACK TO SAVEPOINT before_second_update;

COMMIT;



--Write a transaction that deletes a customer and all associated rentals and payments, ensuring atomicity.
BEGIN;

DELETE FROM payment WHERE customer_id = 1;
DELETE FROM rental WHERE customer_id = 1;
DELETE FROM customer WHERE customer_id = 1;

COMMIT;
