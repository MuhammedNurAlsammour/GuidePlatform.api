-- Sample Subscription Data for Restaurants
-- GuidePlatform Syria-Turkey Business Guide Platform
-- Created: 2025-01-27

-- Sample data for Aleppo Kebap House (978bea96-ec7f-461b-aefa-deaac61df09e)
INSERT INTO guideplatform.subscriptions (
    id,
    business_id,
    start_date,
    end_date,
    amount,
    payment_status,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    auth_customer_id,
    create_user_id,
    update_user_id,
    currency,
    status,
    subscription_type
) VALUES (
    gen_random_uuid(),
    '978bea96-ec7f-461b-aefa-deaac61df09e',
    '2025-01-01',
    '2025-12-31',
    1200.00,
    1, -- Ödeme tamamlandı
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    2, -- TRY
    1, -- Aktif
    3  -- Altın abonelik
);

-- Sample data for Manbij Lokantası (005542a0-ce29-4aa9-b71e-442dd007de67)
INSERT INTO guideplatform.subscriptions (
    id,
    business_id,
    start_date,
    end_date,
    amount,
    payment_status,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    auth_customer_id,
    create_user_id,
    update_user_id,
    currency,
    status,
    subscription_type
) VALUES (
    gen_random_uuid(),
    '005542a0-ce29-4aa9-b71e-442dd007de67',
    '2025-01-01',
    '2025-06-30',
    600.00,
    1, -- Ödeme tamamlandı
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    2, -- TRY
    1, -- Aktif
    2  -- Gümüş abonelik
);

-- Sample data for Azaz Sofra (f60578b0-ca95-4b3f-b2e9-b66679e8f88c)
INSERT INTO guideplatform.subscriptions (
    id,
    business_id,
    start_date,
    end_date,
    amount,
    payment_status,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    auth_customer_id,
    create_user_id,
    update_user_id,
    currency,
    status,
    subscription_type
) VALUES (
    gen_random_uuid(),
    'f60578b0-ca95-4b3f-b2e9-b66679e8f88c',
    '2025-01-01',
    '2025-12-31',
    0.00,
    1, -- Ödeme tamamlandı (ücretsiz)
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    2, -- TRY
    1, -- Aktif
    1  -- Ücretsiz abonelik
);

-- Sample data for Afrin Lezzet (f76e31c4-3e4e-4a58-a5bb-3a57597603f6)
INSERT INTO guideplatform.subscriptions (
    id,
    business_id,
    start_date,
    end_date,
    amount,
    payment_status,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    auth_customer_id,
    create_user_id,
    update_user_id,
    currency,
    status,
    subscription_type
) VALUES (
    gen_random_uuid(),
    'f76e31c4-3e4e-4a58-a5bb-3a57597603f6',
    '2025-01-01',
    '2025-12-31',
    800.00,
    1, -- Ödeme tamamlandı
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    2, -- TRY
    1, -- Aktif
    2  -- Gümüş abonelik
);

-- Sample data for Jarabulus Mutfak (7fb6d642-528b-4dcd-8e58-785b7c9542f4)
INSERT INTO guideplatform.subscriptions (
    id,
    business_id,
    start_date,
    end_date,
    amount,
    payment_status,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    auth_customer_id,
    create_user_id,
    update_user_id,
    currency,
    status,
    subscription_type
) VALUES (
    gen_random_uuid(),
    '7fb6d642-528b-4dcd-8e58-785b7c9542f4',
    '2025-01-01',
    '2025-03-31',
    300.00,
    0, -- Ödeme bekliyor
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    2, -- TRY
    2, -- Pasif (ödeme bekliyor)
    1  -- Ücretsiz abonelik
);

-- Additional sample data for different subscription scenarios

-- Expired subscription example
INSERT INTO guideplatform.subscriptions (
    id,
    business_id,
    start_date,
    end_date,
    amount,
    payment_status,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    auth_customer_id,
    create_user_id,
    update_user_id,
    currency,
    status,
    subscription_type
) VALUES (
    gen_random_uuid(),
    '978bea96-ec7f-461b-aefa-deaac61df09e',
    '2024-01-01',
    '2024-12-31',
    1000.00,
    1, -- Ödeme tamamlandı
    '2024-01-01 00:00:00+03',
    '2024-12-31 23:59:59+03',
    false,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    2, -- TRY
    2, -- Pasif (süresi dolmuş)
    2  -- Gümüş abonelik
);

-- Failed payment example
INSERT INTO guideplatform.subscriptions (
    id,
    business_id,
    start_date,
    end_date,
    amount,
    payment_status,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    auth_customer_id,
    create_user_id,
    update_user_id,
    currency,
    status,
    subscription_type
) VALUES (
    gen_random_uuid(),
    '005542a0-ce29-4aa9-b71e-442dd007de67',
    '2025-01-01',
    '2025-12-31',
    1200.00,
    2, -- Ödeme başarısız
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    false,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    2, -- TRY
    3, -- İptal edildi
    3  -- Altın abonelik
);

-- Refunded subscription example
INSERT INTO guideplatform.subscriptions (
    id,
    business_id,
    start_date,
    end_date,
    amount,
    payment_status,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    auth_customer_id,
    create_user_id,
    update_user_id,
    currency,
    status,
    subscription_type
) VALUES (
    gen_random_uuid(),
    'f76e31c4-3e4e-4a58-a5bb-3a57597603f6',
    '2024-06-01',
    '2024-08-31',
    400.00,
    3, -- İade edildi
    '2024-06-01 00:00:00+03',
    '2024-08-31 23:59:59+03',
    false,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    2, -- TRY
    3, -- İptal edildi
    2  -- Gümüş abonelik
);

-- Comments in Turkish as requested:
-- Bu dosya, GuidePlatform için örnek abonelik verilerini içerir
-- Her restoran için farklı abonelik türleri ve durumları tanımlanmıştır
-- Abonelik türleri: 1=ücretsiz, 2=gümüş, 3=altın
-- Ödeme durumları: 0=bekliyor, 1=tamamlandı, 2=başarısız, 3=iade edildi
-- Abonelik durumları: 1=aktif, 2=pasif, 3=iptal edildi
