ALTER TABLE public.customers
    ADD COLUMN IF NOT EXISTS points_balance      INT         NOT NULL DEFAULT 0,
    ADD COLUMN IF NOT EXISTS is_premium          BOOLEAN     NOT NULL DEFAULT FALSE,
    ADD COLUMN IF NOT EXISTS last_purchase_date  TIMESTAMPTZ NULL;
