--1. Try two concurrent updates to same row → see lock in action.

BEGIN;
UPDATE products SET price = 500 WHERE id = 1; -- Session A locks row with id = 1

BEGIN;
UPDATE products SET price = 600 WHERE id = 1; -- This query will BLOCK until A COMMITs or ROLLBACKs


--2. Write a query using SELECT...FOR UPDATE and check how it locks row.

BEGIN;
SELECT * FROM products WHERE id = 2 FOR UPDATE; -- Locks row with id = 2

UPDATE products SET price = 900 WHERE id = 2; -- This will block until A finishes COMMIT or ROLLBACK


--3. Intentionally create a deadlock and observe PostgreSQL cancel one transaction.

BEGIN;
UPDATE products SET price = 100 WHERE id = 1;

BEGIN;
UPDATE products SET price = 200 WHERE id = 2;

UPDATE products SET price = 100 WHERE id = 2; -- Waits, because B locked id = 2

UPDATE products SET price = 200 WHERE id = 1; -- Waits, because A locked id = 1


--4. Use pg_locks query to monitor active locks.

SELECT pid,mode,relation::regclass AS table,transactionid,virtualtransaction,granted
FROM pg_locks
WHERE NOT granted OR mode LIKE '%Exclusive%'
ORDER BY pid;


--5. Explore about Lock Modes.

BEGIN;
SELECT * FROM products; -- Access Share

BEGIN;
SELECT * FROM products WHERE id = 1 FOR UPDATE; -- Row Share

BEGIN;
LOCK TABLE products IN EXCLUSIVE MODE; -- Exclusive

BEGIN;
LOCK TABLE products IN ACCESS EXCLUSIVE MODE; -- Access Exclusive
