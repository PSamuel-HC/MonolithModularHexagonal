CREATE TABLE IF NOT EXISTS public.products (
    id           SERIAL          PRIMARY KEY,
    name         VARCHAR(200)    NOT NULL,
    price        NUMERIC(10, 2)  NOT NULL CHECK (price >= 0),
    stock_qty    INT             NOT NULL DEFAULT 0,
    created_at   TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
    category_id  INT             NULL
);
