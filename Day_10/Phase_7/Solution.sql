DO $$
DECLARE
    v_enroll_id INT;
BEGIN
    BEGIN
        INSERT INTO enrollments(student_id, course_id)
        VALUES (1, 2)
        RETURNING enrollment_id INTO v_enroll_id;
        IF v_enroll_id IS NOT NULL THEN
            INSERT INTO certificates(enrollment_id, serial_no)
            VALUES (v_enroll_id, 'CERT-' || LPAD(v_enroll_id::TEXT, 3, '0'));
        ELSE
            RAISE EXCEPTION 'Enrollment failed!';
        END IF;

        COMMIT;
        RAISE NOTICE 'Transaction completed successfully.';
    EXCEPTION WHEN OTHERS THEN
        ROLLBACK;
        RAISE NOTICE 'Transaction failed. Rolled back.';
    END;
END $$;
