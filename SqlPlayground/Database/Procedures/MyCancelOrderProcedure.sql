CREATE OR REPLACE PROCEDURE my_cancel_order_prd(p_order_id INT)
LANGUAGE plpgsql
AS $$
BEGIN

UPDATE orders
SET status = 'Cancelled'
WHERE id = p_order_id;
IF NOT FOUND THEN
RAISE EXCEPTION 'Order % not found' , p_order_id;
END IF;

DELETE FROM order_items WHERE order_id = p_order_id;
RAISE NOTICE 'Order % has been cancelled.' , p_order_id;
END;
$$;