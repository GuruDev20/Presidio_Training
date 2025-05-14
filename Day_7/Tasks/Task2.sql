--Cursors
--Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.


CREATE TABLE customer_rental_summary (
    customer_id INT,
    full_name TEXT,
    rental_count INT
);

DO $$
DECLARE
    rec RECORD;
BEGIN
    FOR rec IN
        SELECT c.customer_id, CONCAT(c.first_name, ' ', c.last_name) AS full_name, COUNT(r.rental_id) AS rental_count
        FROM customer c
        LEFT JOIN rental r ON c.customer_id = r.customer_id
        GROUP BY c.customer_id, full_name
    LOOP
        INSERT INTO customer_rental_summary(customer_id, full_name, rental_count)
        VALUES (rec.customer_id, rec.full_name, rec.rental_count);
    END LOOP;
END $$;

SELECT * FROM customer_rental_summary


--Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.

DO $$
DECLARE
    film_title TEXT;
    film_cursor CURSOR FOR
        SELECT f.title
        FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        JOIN category c ON c.category_id = fc.category_id
        JOIN inventory i ON i.film_id = f.film_id
        JOIN rental r ON r.inventory_id = i.inventory_id
        WHERE c.name = 'Comedy'
        GROUP BY f.title
        HAVING COUNT(r.rental_id) > 10;
BEGIN
    OPEN film_cursor;
    LOOP
        FETCH film_cursor INTO film_title;
        EXIT WHEN NOT FOUND;
        RAISE NOTICE 'Comedy Film: %', film_title;
    END LOOP;
    CLOSE film_cursor;
END $$;


--Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.

CREATE TABLE store_film_report (
    store_id INT,
    distinct_film_count INT
);

DO $$
DECLARE
    rec RECORD;
BEGIN
    FOR rec IN
        SELECT s.store_id, COUNT(DISTINCT i.film_id) AS film_count
        FROM store s
        JOIN inventory i ON s.store_id = i.store_id
        GROUP BY s.store_id
    LOOP
        INSERT INTO store_film_report(store_id, distinct_film_count)
        VALUES (rec.store_id, rec.film_count);
    END LOOP;
END $$;

SELECT * FROM store_film_report


--Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.

CREATE TABLE inactive_customers (
    customer_id INT,
    full_name TEXT,
    email TEXT,
    last_rental_date DATE
);

DO $$
DECLARE
    rec RECORD;
BEGIN
    FOR rec IN
        SELECT DISTINCT c.customer_id,
               CONCAT(c.first_name, ' ', c.last_name) AS full_name,
               c.email,
               MAX(r.rental_date) AS last_rental
        FROM customer c
        LEFT JOIN rental r ON c.customer_id = r.customer_id
        GROUP BY c.customer_id, c.first_name, c.last_name, c.email
        HAVING MAX(r.rental_date) IS NULL OR MAX(r.rental_date) < CURRENT_DATE - INTERVAL '6 months'
    LOOP
        INSERT INTO inactive_customers(customer_id, full_name, email, last_rental_date)
        VALUES (rec.customer_id,rec.full_name,rec.email,rec.last_rental);
    END LOOP;
END $$;

SELECT * FROM inactive_customers

--Transactions
--Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.

BEGIN;

WITH new_cust AS (
  INSERT INTO customer (store_id, first_name, last_name, email, active)
  VALUES (1, 'Stuv', 'Wxyz', 'stuvwxyz@example.com', TRUE)
  RETURNING customer_id
),

new_rent AS (
  INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
  SELECT NOW(), 1, customer_id, 1 FROM new_cust
  RETURNING rental_id, customer_id
)

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
SELECT customer_id, 1, rental_id, 5.99, NOW() FROM new_rent;

COMMIT;


 
--Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.


BEGIN;

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (1, 1, 100, 2.99, NOW());

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (1, 1, 999999, 2.99, NOW()); 

COMMIT;
 
--Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.

BEGIN;

SAVEPOINT s1;
UPDATE payment SET amount = amount + 1 WHERE payment_id = 1;

SAVEPOINT s2;
UPDATE payment SET amount = amount + 1 WHERE payment_id = 99999;  -- invalid, will fail

ROLLBACK TO SAVEPOINT s2;

UPDATE payment SET amount = amount + 1 WHERE payment_id = 2;

COMMIT;

 
--Perform a transaction that transfers inventory from one store to another (delete + insert) safely.
SELECT * FROM inventory ORDER BY inventory_id;

BEGIN;

WITH deleted AS (
	DELETE FROM inventory
	WHERE inventory_id = 1
	RETURNING film_id
)

INSERT INTO inventory (film_id, store_id)
SELECT film_id, 2 FROM deleted;

COMMIT;


--Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.

BEGIN;

DELETE FROM payment WHERE customer_id = 1;

DELETE FROM rental WHERE customer_id = 1;

DELETE FROM customer WHERE customer_id = 1;

COMMIT;


--Triggers
--Create a trigger to prevent inserting payments of zero or negative amount.

CREATE OR REPLACE FUNCTION prevent_invalid_payments()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.amount <= 0 THEN
        RAISE EXCEPTION 'Payment amount must be positive';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER validate_payment_amount
BEFORE INSERT ON payment
FOR EACH ROW EXECUTE FUNCTION prevent_invalid_payments();
	
 
--Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.

CREATE OR REPLACE FUNCTION update_last_modified()
RETURNS TRIGGER AS $$
BEGIN
    NEW.last_update = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER film_auto_update
BEFORE UPDATE OF title, rental_rate ON film
FOR EACH ROW EXECUTE FUNCTION update_last_modified();

 
--Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.

CREATE TABLE rental_log (
    film_id INT,
    week_start DATE,
    rental_count INT,
    log_time TIMESTAMP DEFAULT NOW()
);

CREATE OR REPLACE FUNCTION log_popular_rentals()
RETURNS TRIGGER AS $$
DECLARE
    start_week DATE := date_trunc('week', NEW.rental_date)::date;
    film_count INT;
BEGIN
    SELECT COUNT(*) INTO film_count
    FROM rental r
    JOIN inventory i ON r.inventory_id = i.inventory_id
    WHERE i.film_id = (SELECT film_id FROM inventory WHERE inventory_id = NEW.inventory_id)
      AND date_trunc('week', r.rental_date) = start_week;

    IF film_count > 3 THEN
        INSERT INTO rental_log (film_id, week_start, rental_count)
        VALUES ((SELECT film_id FROM inventory WHERE inventory_id = NEW.inventory_id), start_week, film_count);
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER track_popular_rentals
AFTER INSERT ON rental
FOR EACH ROW EXECUTE FUNCTION log_popular_rentals();

SELECT * FROM rental_log
