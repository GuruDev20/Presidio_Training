CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE customer (
    customer_id SERIAL PRIMARY KEY,
    store_id INTEGER NOT NULL,
    first_name TEXT NOT NULL,
    email BYTEA NOT NULL,
    address_id INTEGER NOT NULL,
    create_date DATE NOT NULL
);

--Encrypt text
CREATE OR REPLACE FUNCTION sp_encrypt_text(input_text TEXT, secret_key TEXT)
RETURNS BYTEA AS $$
BEGIN
    RETURN pgp_sym_encrypt(input_text, secret_key);
END;
$$ LANGUAGE plpgsql;

--compare
CREATE OR REPLACE FUNCTION sp_compare_encrypted(enc1 BYTEA, enc2 BYTEA, secret_key TEXT)
RETURNS BOOLEAN AS $$
DECLARE
    dec1 TEXT;
    dec2 TEXT;
BEGIN
    dec1 := pgp_sym_decrypt(enc1, secret_key);
    dec2 := pgp_sym_decrypt(enc2, secret_key);
    RETURN dec1 = dec2;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE sp_test_compare_encrypted()
LANGUAGE plpgsql
AS $$
DECLARE
    enc1 BYTEA;
    enc2 BYTEA;
    is_equal BOOLEAN;
    key TEXT := 'mysecretkey';
BEGIN
    enc1 := sp_encrypt_text('Igris', key);
    enc2 := sp_encrypt_text('Bellion', key);
    is_equal := sp_compare_encrypted(enc1, enc2, key);
    RAISE NOTICE 'Encrypted comparison result: %', is_equal;
END;
$$;

CALL sp_test_compare_encrypted();

--mask
CREATE OR REPLACE FUNCTION sp_mask_text(input_text TEXT)
RETURNS TEXT AS $$
DECLARE
    input_length INT;
    masked_length INT;
    masked_text TEXT;
BEGIN
    input_length := LENGTH(input_text);
    IF input_length <= 4 THEN
        RETURN input_text;
    END IF;
    masked_length := input_length - 4;
    masked_text := SUBSTRING(input_text FROM 1 FOR 2) ||
                   REPEAT('*', masked_length) ||
                   SUBSTRING(input_text FROM input_length - 1 FOR 2);
    RETURN masked_text;
END;
$$ LANGUAGE plpgsql;


--insert
CREATE OR REPLACE PROCEDURE sp_insert_customer(p_first_name TEXT,p_email TEXT,p_address_id INTEGER,p_store_id INTEGER,secret_key TEXT)
LANGUAGE plpgsql
AS $$
DECLARE
    encrypted_email BYTEA;
    masked_name TEXT;
BEGIN
    encrypted_email := sp_encrypt_text(p_email, secret_key);
    masked_name := sp_mask_text(p_first_name);

    INSERT INTO customer (first_name, email, address_id, store_id, create_date)
    VALUES (masked_name, encrypted_email, p_address_id, p_store_id, CURRENT_DATE);
END;
$$;

CALL sp_insert_customer(p_first_name:='Achielles',p_email:='achielles@gmail.com',p_address_id:=1,p_store_id:=1,secret_key:='mysecretkey');
CALL sp_insert_customer(p_first_name:='Optimus',p_email:='optimus@gmail.com',p_address_id:=1,p_store_id:=2,secret_key:='mysecretkey');

SELECT * FROM customer;

--read data
CREATE OR REPLACE PROCEDURE sp_read_customer_masked(secret_key TEXT)
LANGUAGE plpgsql
AS $$
DECLARE
    rec RECORD;
BEGIN
    RAISE NOTICE 'Reading customer records...';
    FOR rec IN
        SELECT customer_id, first_name, pgp_sym_decrypt(email, secret_key) AS decrypted_email
        FROM customer
    LOOP
        RAISE NOTICE 'ID: %, Name: %, Email: %', rec.customer_id, rec.first_name, rec.decrypted_email;
    END LOOP;
    RAISE NOTICE 'Done.';
END;
$$;

CALL sp_read_customer_masked('mysecretkey');