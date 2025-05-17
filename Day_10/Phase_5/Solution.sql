DO $$
DECLARE
    rec RECORD;
BEGIN
    FOR rec IN
        SELECT s.first_name, s.last_name, s.email
        FROM students s
        JOIN enrollments e ON s.student_id = e.student_id
        WHERE e.course_id = 1
        AND e.enrollment_id NOT IN (SELECT enrollment_id FROM certificates)
    LOOP
        RAISE NOTICE 'Student: % %, Email: %', rec.first_name, rec.last_name, rec.email;
    END LOOP;
END $$;
