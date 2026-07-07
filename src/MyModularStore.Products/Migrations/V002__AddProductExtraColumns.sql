ALTER TABLE public.products
    ADD COLUMN IF NOT EXISTS sku              VARCHAR(100)  NOT NULL DEFAULT '',
    ADD COLUMN IF NOT EXISTS manufacturer    VARCHAR(200)  NOT NULL DEFAULT '',
    ADD COLUMN IF NOT EXISTS warranty_months INT           NOT NULL DEFAULT 0,
    ADD COLUMN IF NOT EXISTS description     TEXT          NOT NULL DEFAULT '';
