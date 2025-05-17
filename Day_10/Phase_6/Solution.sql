-- READ-ONLY USER
CREATE ROLE readonly_user LOGIN PASSWORD 'readonly';
GRANT CONNECT ON DATABASE your_database_name TO readonly_user;
GRANT USAGE ON SCHEMA public TO readonly_user;
GRANT SELECT ON students, courses, certificates TO readonly_user;

-- DATA ENTRY USER
CREATE ROLE data_entry_user LOGIN PASSWORD 'dataentry';
GRANT CONNECT ON DATABASE your_database_name TO data_entry_user;
GRANT USAGE ON SCHEMA public TO data_entry_user;
GRANT INSERT ON students, enrollments TO data_entry_user;
REVOKE INSERT, UPDATE, DELETE ON certificates FROM data_entry_user;
