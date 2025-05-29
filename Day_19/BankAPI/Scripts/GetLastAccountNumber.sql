CREATE OR REPLACE FUNCTION get_last_account_number()
RETURNS TEXT AS $$
DECLARE
    last_acc TEXT;
BEGIN
    SELECT "AccountNumber"
    INTO last_acc
    FROM public."Accounts"
    ORDER BY "AccountNumber" DESC
    LIMIT 1;

    RETURN last_acc;
END;
$$ LANGUAGE plpgsql;

SELECT get_last_account_number() as Value;
