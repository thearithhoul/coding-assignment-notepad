-- Dedicated least-privilege Postgres role for the application,
-- separate from the POSTGRES_USER superuser used to bootstrap the container.

CREATE ROLE notepad_app WITH LOGIN PASSWORD 'notepad_app';

GRANT CONNECT ON DATABASE notepad TO notepad_app;

GRANT USAGE ON SCHEMA public TO notepad_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO notepad_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO notepad_app;

-- Apply the same privileges to tables/sequences created later.
ALTER DEFAULT PRIVILEGES IN SCHEMA public
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO notepad_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public
    GRANT USAGE, SELECT ON SEQUENCES TO notepad_app;
