CREATE TABLE IF NOT EXISTS public.employees (
    id           SERIAL          PRIMARY KEY,
    first_name   VARCHAR(100)    NOT NULL,
    last_name    VARCHAR(100)    NOT NULL,
    role         VARCHAR(100)    NOT NULL,
    hourly_rate  NUMERIC(10, 2)  NOT NULL CHECK (hourly_rate >= 0),
    hire_date    TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);
