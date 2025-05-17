-- 1. List students and the courses they enrolled in
SELECT s.first_name, s.last_name, c.course_name, e.enroll_date
FROM students s
JOIN enrollments e ON s.student_id = e.student_id
JOIN courses c ON c.course_id = e.course_id;


-- 2. Students who received certificates with trainer names
SELECT s.first_name, s.last_name, c2.course_name, t.first_name AS trainer_fname, t.last_name AS trainer_lname
FROM students s
JOIN enrollments e ON s.student_id = e.student_id
JOIN certificates cert ON e.enrollment_id = cert.enrollment_id
JOIN courses c2 ON c2.course_id = e.course_id
JOIN trainers t ON t.trainer_id = c2.trainer_id;


-- 3. Count number of students per course
SELECT c.course_name, COUNT(e.student_id) AS total_students
FROM courses c
LEFT JOIN enrollments e ON c.course_id = e.course_id
GROUP BY c.course_name;
