CREATE TABLE IF NOT EXISTS public.orders (
    id           SERIAL          PRIMARY KEY,
    customer_id  INT             NOT NULL,
    order_number VARCHAR(50)     NOT NULL UNIQUE,
    status       VARCHAR(20)     NOT NULL DEFAULT 'Pending',
    total_amount NUMERIC(10, 2)  NOT NULL CHECK (total_amount >= 0),
    created_at   TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);