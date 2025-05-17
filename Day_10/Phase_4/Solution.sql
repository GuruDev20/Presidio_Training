-- Function: Get Certified Students
CREATE OR REPLACE FUNCTION get_certified_students(p_course_id INT)
RETURNS TABLE(student_name TEXT, email TEXT) AS $$
BEGIN
    RETURN QUERY
    SELECT CONCAT(s.first_name, ' ', s.last_name), s.email
    FROM students s
    JOIN enrollments e ON s.student_id = e.student_id
    JOIN certificates c ON e.enrollment_id = c.enrollment_id
    WHERE e.course_id = p_course_id;
END;
$$ LANGUAGE plpgsql;


-----------------------------------------------------------------------

CREATE OR REPLACE PROCEDURE sp_enroll_student(
    p_student_id INT,
    p_course_id INT,
    p_issue_certificate BOOLEAN DEFAULT FALSE
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_enrollment_id INT;
    v_certificate_id INT := NULL;
    v_serial_number VARCHAR(100);
BEGIN
    BEGIN
        INSERT INTO enrollments (student_id, course_id, enroll_date)
        VALUES (p_student_id, p_course_id, CURRENT_DATE)
        RETURNING enrollment_id INTO v_enrollment_id;

        RAISE NOTICE 'Enrolled student % into course % with enrollment ID %.',p_student_id, p_course_id, v_enrollment_id;
        IF p_issue_certificate THEN
            v_serial_number := 'HW-NEW-' || v_enrollment_id::TEXT || '-' || TO_CHAR(CURRENT_DATE, 'YYYY-MM-DD');

            INSERT INTO certificates (enrollment_id, serial_no, issue_date)
            VALUES (v_enrollment_id, v_serial_number, CURRENT_DATE)
            RETURNING certificate_id INTO v_certificate_id;

            RAISE NOTICE '🏅 Certificate issued with ID % and Serial No: %.',v_certificate_id, v_serial_number;
        END IF;

    EXCEPTION WHEN OTHERS THEN
        RAISE NOTICE 'Enrollment failed: %', SQLERRM;
    END;
END;
$$;