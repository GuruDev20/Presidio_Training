-- CATEGORY
CREATE TABLE category (
    category_id SERIAL PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT
);

-- TRAINERS
CREATE TABLE trainers (
    trainer_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    expertise TEXT NOT NULL
);

-- COURSES
CREATE TABLE courses (
    course_id SERIAL PRIMARY KEY,
    course_name VARCHAR(100) NOT NULL,
    category_id INT NOT NULL REFERENCES category(category_id) ON DELETE CASCADE,
    description TEXT,
    duration_days INT CHECK (duration_days > 0),
    price DECIMAL(10,2) CHECK (price >= 0),
    trainer_id INT NOT NULL REFERENCES trainers(trainer_id) ON DELETE CASCADE
);

-- STUDENTS
CREATE TABLE students (
    student_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(15) UNIQUE NOT NULL
);

-- ENROLLMENTS
CREATE TABLE enrollments (
    enrollment_id SERIAL PRIMARY KEY,
    student_id INT NOT NULL REFERENCES students(student_id) ON DELETE CASCADE,
    course_id INT NOT NULL REFERENCES courses(course_id) ON DELETE CASCADE,
    enroll_date DATE DEFAULT CURRENT_DATE
);

-- CERTIFICATES
CREATE TABLE certificates (
    certificate_id SERIAL PRIMARY KEY,
    enrollment_id INT UNIQUE NOT NULL REFERENCES enrollments(enrollment_id) ON DELETE CASCADE,
    issue_date DATE DEFAULT CURRENT_DATE,
    serial_no VARCHAR(50) UNIQUE NOT NULL
);

-- COURSE_TRAINERS
CREATE TABLE course_trainers (
    course_id INT REFERENCES courses(course_id) ON DELETE CASCADE,
    trainer_id INT REFERENCES trainers(trainer_id) ON DELETE CASCADE,
    PRIMARY KEY(course_id, trainer_id)
);

-------------------------------------------------------------------------------------------------------------

-- CATEGORY
INSERT INTO category (category_name, description) VALUES
('Web Development', 'Learn HTML, CSS, JavaScript, and frontend/backend frameworks'),
('Data Science', 'Courses on ML, AI, and Python data stack'),
('DevOps', 'CI/CD, Docker, Kubernetes, AWS pipeline'),
('Cybersecurity', 'Ethical hacking, forensics, network security');

-- TRAINERS
INSERT INTO trainers (first_name, last_name, email, expertise) VALUES
('Kakashi', 'Hatake', 'kakashi@edtech.com', 'Web Development'),
('Satoru', 'Gojo', 'gojo@edtech.com', 'Cybersecurity'),
('Thorfinn', 'Karlsefni', 'thorfinn@edtech.com', 'DevOps'),
('Jinwoo', 'Sung', 'jinwoo@edtech.com', 'Data Science');

-- STUDENTS
INSERT INTO students (first_name, last_name, email, phone) VALUES
('Naruto', 'Uzumaki', 'naruto@leaf.com', '1234567890'),
('Sasuke', 'Uchiha', 'sasuke@leaf.com', '1234509876'),
('Luffy', 'Monkey D.', 'luffy@onepiece.com', '1112223333'),
('Zoro', 'Roronoa', 'zoro@onepiece.com', '4445556666'),
('Eren', 'Yeager', 'eren@paradise.com', '7778889999'),
('Mikasa', 'Ackerman', 'mikasa@paradise.com', '3332221111'),
('Itadori', 'Yuji', 'itadori@jjk.com', '8887776666'),
('Megumi', 'Fushiguro', 'megumi@jjk.com', '6667778888'),
('Anos', 'Voldigoad', 'anos@misfit.com', '1010101010'),
('Levi', 'Ackerman', 'levi@scout.com', '9998887777');

-- COURSES
INSERT INTO courses (course_name, category_id, description, duration_days, price, trainer_id) VALUES
('Full Stack Web Dev', 1, 'MERN Stack Project-Based Learning', 45, 299.99, 1),
('Cybersecurity Basics', 4, 'Network Security and Tools', 30, 249.99, 2),
('DevOps with AWS', 3, 'Automate Infrastructure on Cloud', 40, 279.99, 3),
('Machine Learning', 2, 'Regression, Classification, and Clustering', 50, 399.99, 4);

-- ENROLLMENTS
INSERT INTO enrollments (student_id, course_id) VALUES
(1, 1), (2, 1), (3, 2), (4, 3), (5, 4), (6, 1), (7, 2), (8, 4), (9, 3), (10, 2);

-- CERTIFICATES
INSERT INTO certificates (enrollment_id, serial_no) VALUES
(1, 'CERT-001'),
(3, 'CERT-002'),
(4, 'CERT-003'),
(5, 'CERT-004'),
(7, 'CERT-005');

-- INDEXING
CREATE INDEX idx_students_email ON students(email);
CREATE INDEX idx_students_id ON students(student_id);
CREATE INDEX idx_courses_id ON courses(course_id);
