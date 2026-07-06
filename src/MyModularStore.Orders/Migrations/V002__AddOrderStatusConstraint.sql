ALTER TABLE public.orders
    DROP CONSTRAINT IF EXISTS chk_orders_status;

ALTER TABLE public.orders
    ADD CONSTRAINT chk_orders_status
        CHECK (status IN ('Pending', 'Shipped', 'Delivered', 'Processing', 'Cancelled'));