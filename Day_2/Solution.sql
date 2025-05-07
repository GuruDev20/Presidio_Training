use employee

CREATE TABLE EMP (
    empno INT PRIMARY KEY,
    empname VARCHAR(100),
    salary INT,
    deptname VARCHAR(50),
    bossno INT
);

INSERT INTO EMP VALUES
(1, 'Alice', 75000, 'Management', NULL),
(2, 'Ned', 45000, 'Marketing', 1),
(3, 'Andrew', 25000, 'Marketing', 2),
(4, 'Clare', 22000, 'Marketing', 2),
(5, 'Todd', 38000, 'Accounting', 1),
(6, 'Nancy', 22000, 'Accounting', 5),
(7, 'Brier', 43000, 'Purchasing', 1),
(8, 'Sarah', 56000, 'Purchasing', 7),
(9, 'Sophile', 35000, 'Personnel', 1),
(10, 'Sanjay', 15000, 'Navigation', 3),
(11, 'Rita', 15000, 'Books', 4),
(12, 'Gigi', 16000, 'Clothes', 4),
(13, 'Maggie', 11000, 'Clothes', 4),
(14, 'Paul', 15000, 'Equipment', 3),
(15, 'James', 15000, 'Equipment', 3),
(16, 'Pat', 15000, 'Furniture', 3),
(17, 'Mark', 15000, 'Recreation', 3);

CREATE TABLE DEPARTMENT (
    deptname VARCHAR(50) PRIMARY KEY,
    floor INT,
    phone INT,
    empno INT NOT NULL
);

INSERT INTO DEPARTMENT VALUES
('Management', 5, 34, 1),
('Books', 1, 81, 4),
('Clothes', 2, 24, 4),
('Equipment', 3, 57, 3),
('Furniture', 4, 14, 3),
('Navigation', 1, 41, 3),
('Recreation', 2, 29, 4),
('Accounting', 5, 35, 5),
('Purchasing', 5, 36, 7),
('Personnel', 5, 37, 9),
('Marketing', 5, 38, 2);

CREATE TABLE ITEM (
    itemname VARCHAR(100) PRIMARY KEY,
    itemtype CHAR(1),
    itemcolor VARCHAR(20)
);

INSERT INTO ITEM VALUES
('Pocket Knife-Nile', 'E', 'Brown'),
('Pocket Knife-Avon', 'E', 'Brown'),
('Compass', 'N', NULL),
('Geo positioning system', 'N', NULL),
('Elephant Polo stick', 'R', 'Bamboo'),
('Camel Saddle', 'R', 'Brown'),
('Sextant', 'N', NULL),
('Map Measure', 'N', NULL),
('Boots-snake proof', 'C', 'Green'),
('Pith Helmet', 'C', 'Khaki'),
('Hat-polar Explorer', 'C', 'White'),
('Exploring in 10 Easy Lessons', 'B', NULL),
('Hammock', 'F', 'Khaki'),
('How to win Foreign Friends', 'B', NULL),
('Map case', 'E', 'Brown'),
('Safari Chair', 'F', 'Khaki'),
('Safari cooking kit', 'F', 'Khaki'),
('Stetson', 'C', 'Black'),
('Tent - 2 person', 'F', 'Khaki'),
('Tent -8 person', 'F', 'Khaki');

CREATE TABLE SALES (
    salesno INT PRIMARY KEY,
    saleqty INT,
    itemname VARCHAR(100),
    deptname VARCHAR(50)
);

INSERT INTO SALES VALUES
(101, 2, 'Boots-snake proof', 'Clothes'),
(102, 1, 'Pith Helmet', 'Clothes'),
(103, 1, 'Sextant', 'Navigation'),
(104, 3, 'Hat-polar Explorer', 'Clothes'),
(105, 5, 'Pith Helmet', 'Equipment'),
(106, 2, 'Pocket Knife-Nile', 'Clothes'),
(107, 3, 'Pocket Knife-Nile', 'Recreation'),
(108, 1, 'Compass', 'Navigation'),
(109, 2, 'Geo positioning system', 'Navigation'),
(110, 5, 'Map Measure', 'Navigation'),
(111, 1, 'Geo positioning system', 'Books'),
(112, 1, 'Sextant', 'Books'),
(113, 3, 'Pocket Knife-Nile', 'Books'),
(114, 1, 'Pocket Knife-Nile', 'Navigation'),
(115, 1, 'Pocket Knife-Nile', 'Equipment'),
(116, 1, 'Sextant', 'Clothes'),
(117, 1, NULL, 'Equipment'),
(118, 1, NULL, 'Recreation'),
(119, 1, NULL, 'Furniture'),
(120, 1, 'Pocket Knife-Nile', NULL),
(121, 1, 'Exploring in 10 Easy Lessons', 'Books'),
(122, 1, 'How to win Foreign Friends', NULL),
(123, 1, 'Compass', NULL),
(124, 1, 'Pith Helmet', NULL),
(125, 1, 'Elephant Polo stick', 'Recreation'),
(126, 1, 'Camel Saddle', 'Recreation');



ALTER TABLE EMP
ADD CONSTRAINT fk_emp_dept
FOREIGN KEY (deptname) REFERENCES DEPARTMENT(deptname);

ALTER TABLE EMP
ADD CONSTRAINT fk_emp_boss
FOREIGN KEY (bossno) REFERENCES EMP(empno);

ALTER TABLE DEPARTMENT
ADD CONSTRAINT fk_dept_mgr
FOREIGN KEY (empno) REFERENCES EMP(empno);

ALTER TABLE SALES
ADD CONSTRAINT fk_sales_item
FOREIGN KEY (itemname) REFERENCES ITEM(itemname);

ALTER TABLE SALES
ADD CONSTRAINT fk_sales_dept
FOREIGN KEY (deptname) REFERENCES DEPARTMENT(deptname);