-- =====================================================
-- Syria Guide Offers & Promotions System
-- نظام العروض والكوبونات لدليلك في سوريا
-- =====================================================

-- =====================================================
-- OFFERS AND PROMOTIONS TABLES - جداول العروض والكوبونات
-- =====================================================

-- offers table - جدول العروض
CREATE TABLE offers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    business_location_id UUID, -- Yeni: Müşteri lokasyonu için
    title VARCHAR(255) NOT NULL,
    description TEXT,
    discount_type VARCHAR(20) NOT NULL, -- percentage, fixed_amount, free_item, no_download
    discount_value DECIMAL(10,2) NOT NULL,
    original_price DECIMAL(12,2),
    discounted_price DECIMAL(12,2),
    currency VARCHAR(3) DEFAULT 'SYP',
    offer_type VARCHAR(20) DEFAULT 'regular', -- regular, gold, premium
    is_download_required BOOLEAN DEFAULT true, -- Yeni: İndirme gerekli mi?
    start_date TIMESTAMPTZ NOT NULL,
    end_date TIMESTAMPTZ NOT NULL,
    is_active BOOLEAN DEFAULT true,
    is_featured BOOLEAN DEFAULT false,
    max_uses INTEGER, -- null = unlimited
    current_uses INTEGER DEFAULT 0,
    min_order_amount DECIMAL(12,2) DEFAULT 0,
    terms_conditions TEXT,
    photo bytea NULL,
    thumbnail bytea NULL,
    photo_content_type varchar(50) NULL,
    icon VARCHAR(100) DEFAULT 'local_offer',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (business_location_id) REFERENCES business_locations(id) ON DELETE SET NULL,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- business_locations table - Yeni: Müşteri lokasyonları tablosu
CREATE TABLE business_locations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    location_name VARCHAR(255) NOT NULL,
    address TEXT,
    city VARCHAR(100),
    district VARCHAR(100),  
    phone VARCHAR(20),
    email VARCHAR(255),
    latitude DECIMAL(10,8),
    longitude DECIMAL(11,8),
    is_main_location BOOLEAN DEFAULT false,
    is_active BOOLEAN DEFAULT true,
    working_hours JSONB, -- {"monday": {"open": "09:00", "close": "22:00"}, ...}
    icon VARCHAR(100) DEFAULT 'location_on',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- offer_locations table - Yeni: Teklif lokasyon ilişkisi
CREATE TABLE offer_locations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    offer_id UUID NOT NULL,
    business_location_id UUID NOT NULL,
    is_primary_location BOOLEAN DEFAULT false,
    icon VARCHAR(100) DEFAULT 'location_on',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (offer_id) REFERENCES offers(id) ON DELETE CASCADE,
    FOREIGN KEY (business_location_id) REFERENCES business_locations(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id"),
    UNIQUE(offer_id, business_location_id)
);

-- coupons table - جدول الكوبونات
CREATE TABLE coupons (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    business_location_id UUID, -- Yeni: Müşteri lokasyonu için
    code VARCHAR(50) UNIQUE NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    discount_type VARCHAR(20) NOT NULL, -- percentage, fixed_amount, free_shipping
    discount_value DECIMAL(10,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'SYP',
    start_date TIMESTAMPTZ NOT NULL,
    end_date TIMESTAMPTZ NOT NULL,
    is_active BOOLEAN DEFAULT true,
    max_uses INTEGER, -- null = unlimited
    current_uses INTEGER DEFAULT 0,
    min_order_amount DECIMAL(12,2) DEFAULT 0,
    max_discount_amount DECIMAL(12,2), -- for percentage discounts
    is_first_time_only BOOLEAN DEFAULT false,
    terms_conditions TEXT,
    icon VARCHAR(100) DEFAULT 'card_giftcard',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (business_location_id) REFERENCES business_locations(id) ON DELETE SET NULL,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- offer_categories table - جدول فئات العروض
CREATE TABLE offer_categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    offer_id UUID NOT NULL,
    category_id UUID NOT NULL,
    icon VARCHAR(100) DEFAULT 'category',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (offer_id) REFERENCES offers(id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES categories(id),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id"),
    UNIQUE(offer_id, category_id)
);

-- user_offer_usage table - جدول استخدام العروض من قبل المستخدمين
CREATE TABLE user_offer_usage (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    offer_id UUID NOT NULL,
    business_location_id UUID, -- Yeni: Hangi lokasyonda kullanıldı
    usage_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    order_amount DECIMAL(12,2),
    discount_applied DECIMAL(12,2),
    is_used BOOLEAN DEFAULT true,
    is_downloaded BOOLEAN DEFAULT false, -- Yeni: İndirildi mi?
    download_date TIMESTAMPTZ, -- Yeni: İndirme tarihi
    icon VARCHAR(100) DEFAULT 'redeem',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (offer_id) REFERENCES offers(id) ON DELETE CASCADE,
    FOREIGN KEY (business_location_id) REFERENCES business_locations(id) ON DELETE SET NULL,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- user_coupon_usage table - جدول استخدام الكوبونات من قبل المستخدمين
CREATE TABLE user_coupon_usage (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    coupon_id UUID NOT NULL,
    business_location_id UUID, -- Yeni: Hangi lokasyonda kullanıldı
    usage_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    order_amount DECIMAL(12,2),
    discount_applied DECIMAL(12,2),
    is_used BOOLEAN DEFAULT true,
    icon VARCHAR(100) DEFAULT 'redeem',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (coupon_id) REFERENCES coupons(id) ON DELETE CASCADE,
    FOREIGN KEY (business_location_id) REFERENCES business_locations(id) ON DELETE SET NULL,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- user_favorite_offers table - جدول العروض المفضلة للمستخدمين
CREATE TABLE user_favorite_offers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    offer_id UUID NOT NULL,
    icon VARCHAR(100) DEFAULT 'favorite',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (offer_id) REFERENCES offers(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id"),
    UNIQUE(user_id, offer_id)
);

-- offer_analytics table - جدول تحليلات العروض
CREATE TABLE offer_analytics (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    offer_id UUID NOT NULL,
    business_location_id UUID, -- Yeni: Lokasyon bazlı analiz
    date DATE NOT NULL,
    views_count INTEGER DEFAULT 0,
    clicks_count INTEGER DEFAULT 0,
    uses_count INTEGER DEFAULT 0,
    downloads_count INTEGER DEFAULT 0, -- Yeni: İndirme sayısı
    revenue_generated DECIMAL(12,2) DEFAULT 0,
    icon VARCHAR(100) DEFAULT 'analytics',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID,
    update_user_id UUID,
    FOREIGN KEY (offer_id) REFERENCES offers(id) ON DELETE CASCADE,
    FOREIGN KEY (business_location_id) REFERENCES business_locations(id) ON DELETE SET NULL,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id"),
    UNIQUE(offer_id, business_location_id, date)
);

-- =====================================================
-- INDEXES - الفهارس
-- =====================================================

-- Business locations indexes
CREATE INDEX idx_business_locations_business_id ON business_locations(business_id);
CREATE INDEX idx_business_locations_active ON business_locations(is_active, row_is_active, row_is_deleted);
CREATE INDEX idx_business_locations_main ON business_locations(is_main_location);
CREATE INDEX idx_business_locations_customer_id ON business_locations(auth_customer_id);
CREATE INDEX idx_business_locations_create_user ON business_locations(create_user_id);
CREATE INDEX idx_business_locations_update_user ON business_locations(update_user_id);

-- Offer locations indexes
CREATE INDEX idx_offer_locations_offer_id ON offer_locations(offer_id);
CREATE INDEX idx_offer_locations_location_id ON offer_locations(business_location_id);
CREATE INDEX idx_offer_locations_primary ON offer_locations(is_primary_location);
CREATE INDEX idx_offer_locations_customer_id ON offer_locations(auth_customer_id);
CREATE INDEX idx_offer_locations_create_user ON offer_locations(create_user_id);
CREATE INDEX idx_offer_locations_update_user ON offer_locations(update_user_id);

-- Offers indexes
CREATE INDEX idx_offers_business_id ON offers(business_id);
CREATE INDEX idx_offers_location_id ON offers(business_location_id);
CREATE INDEX idx_offers_active ON offers(is_active, row_is_active, row_is_deleted);
CREATE INDEX idx_offers_featured ON offers(is_featured);
CREATE INDEX idx_offers_date_range ON offers(start_date, end_date);
CREATE INDEX idx_offers_discount_type ON offers(discount_type);
CREATE INDEX idx_offers_offer_type ON offers(offer_type);
CREATE INDEX idx_offers_download_required ON offers(is_download_required);
CREATE INDEX idx_offers_customer_id ON offers(auth_customer_id);
CREATE INDEX idx_offers_create_user ON offers(create_user_id);
CREATE INDEX idx_offers_update_user ON offers(update_user_id);

-- Coupons indexes
CREATE INDEX idx_coupons_business_id ON coupons(business_id);
CREATE INDEX idx_coupons_location_id ON coupons(business_location_id);
CREATE INDEX idx_coupons_code ON coupons(code);
CREATE INDEX idx_coupons_active ON coupons(is_active, row_is_active, row_is_deleted);
CREATE INDEX idx_coupons_date_range ON coupons(start_date, end_date);
CREATE INDEX idx_coupons_customer_id ON coupons(auth_customer_id);
CREATE INDEX idx_coupons_create_user ON coupons(create_user_id);
CREATE INDEX idx_coupons_update_user ON coupons(update_user_id);

-- Offer categories indexes
CREATE INDEX idx_offer_categories_offer_id ON offer_categories(offer_id);
CREATE INDEX idx_offer_categories_category_id ON offer_categories(category_id);
CREATE INDEX idx_offer_categories_customer_id ON offer_categories(auth_customer_id);
CREATE INDEX idx_offer_categories_create_user ON offer_categories(create_user_id);
CREATE INDEX idx_offer_categories_update_user ON offer_categories(update_user_id);

-- User usage indexes
CREATE INDEX idx_user_offer_usage_user_id ON user_offer_usage(user_id);
CREATE INDEX idx_user_offer_usage_offer_id ON user_offer_usage(offer_id);
CREATE INDEX idx_user_offer_usage_location_id ON user_offer_usage(business_location_id);
CREATE INDEX idx_user_offer_usage_date ON user_offer_usage(usage_date);
CREATE INDEX idx_user_offer_usage_downloaded ON user_offer_usage(is_downloaded);
CREATE INDEX idx_user_offer_usage_customer_id ON user_offer_usage(auth_customer_id);
CREATE INDEX idx_user_offer_usage_create_user ON user_offer_usage(create_user_id);
CREATE INDEX idx_user_offer_usage_update_user ON user_offer_usage(update_user_id);

CREATE INDEX idx_user_coupon_usage_user_id ON user_coupon_usage(user_id);
CREATE INDEX idx_user_coupon_usage_coupon_id ON user_coupon_usage(coupon_id);
CREATE INDEX idx_user_coupon_usage_location_id ON user_coupon_usage(business_location_id);
CREATE INDEX idx_user_coupon_usage_date ON user_coupon_usage(usage_date);
CREATE INDEX idx_user_coupon_usage_customer_id ON user_coupon_usage(auth_customer_id);
CREATE INDEX idx_user_coupon_usage_create_user ON user_coupon_usage(create_user_id);
CREATE INDEX idx_user_coupon_usage_update_user ON user_coupon_usage(update_user_id);

-- Favorite offers indexes
CREATE INDEX idx_user_favorite_offers_user_id ON user_favorite_offers(user_id);
CREATE INDEX idx_user_favorite_offers_offer_id ON user_favorite_offers(offer_id);
CREATE INDEX idx_user_favorite_offers_customer_id ON user_favorite_offers(auth_customer_id);
CREATE INDEX idx_user_favorite_offers_create_user ON user_favorite_offers(create_user_id);
CREATE INDEX idx_user_favorite_offers_update_user ON user_favorite_offers(update_user_id);

-- Analytics indexes
CREATE INDEX idx_offer_analytics_offer_id ON offer_analytics(offer_id);
CREATE INDEX idx_offer_analytics_location_id ON offer_analytics(business_location_id);
CREATE INDEX idx_offer_analytics_date ON offer_analytics(date);
CREATE INDEX idx_offer_analytics_customer_id ON offer_analytics(auth_customer_id);
CREATE INDEX idx_offer_analytics_create_user ON offer_analytics(create_user_id);
CREATE INDEX idx_offer_analytics_update_user ON offer_analytics(update_user_id);

-- =====================================================
-- SAMPLE DATA - البيانات النموذجية
-- =====================================================

-- Örnek Müşteri Lokasyonları
INSERT INTO business_locations (business_id, location_name, address, city, district, phone, email, latitude, longitude, is_main_location, working_hours, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES
(
    (SELECT id FROM businesses LIMIT 1), 
    'Merkez Şube', 
    'Atatürk Caddesi No:123, Merkez', 
    'İstanbul', 
    'Kadıköy', 
    '+90 212 555 0123', 
    'merkez@restaurant.com', 
    40.9909, 
    29.0303, 
    true, 
    '{"monday": {"open": "09:00", "close": "23:00"}, "tuesday": {"open": "09:00", "close": "23:00"}, "wednesday": {"open": "09:00", "close": "23:00"}, "thursday": {"open": "09:00", "close": "23:00"}, "friday": {"open": "09:00", "close": "00:00"}, "saturday": {"open": "10:00", "close": "00:00"}, "sunday": {"open": "10:00", "close": "22:00"}}', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    (SELECT id FROM businesses LIMIT 1), 
    'Şube 2', 
    'Bağdat Caddesi No:456, Kadıköy', 
    'İstanbul', 
    'Maltepe', 
    '+90 216 555 0456', 
    'sube2@restaurant.com', 
    40.9354, 
    29.1289, 
    false, 
    '{"monday": {"open": "10:00", "close": "22:00"}, "tuesday": {"open": "10:00", "close": "22:00"}, "wednesday": {"open": "10:00", "close": "22:00"}, "thursday": {"open": "10:00", "close": "22:00"}, "friday": {"open": "10:00", "close": "23:00"}, "saturday": {"open": "11:00", "close": "23:00"}, "sunday": {"open": "11:00", "close": "21:00"}}', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Örnek İndirimler (GOLD ve Normal)
INSERT INTO offers (business_id, business_location_id, title, description, discount_type, discount_value, original_price, discounted_price, offer_type, is_download_required, start_date, end_date, is_featured, max_uses, terms_conditions, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES
(
    (SELECT id FROM businesses LIMIT 1), 
    (SELECT id FROM business_locations WHERE is_main_location = true LIMIT 1), 
    'GOLD Pizza %50 İndirim', 
    'GOLD üyeler için özel %50 indirim fırsatı!', 
    'percentage', 
    50.00, 
    100.00, 
    50.00, 
    'gold', 
    true, 
    CURRENT_TIMESTAMP, 
    CURRENT_TIMESTAMP + INTERVAL '30 days', 
    true, 
    100, 
    'Bu indirim sadece GOLD üyeler için geçerlidir. Pizzalar için geçerlidir.', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    (SELECT id FROM businesses LIMIT 1), 
    (SELECT id FROM business_locations WHERE is_main_location = false LIMIT 1), 
    'Normal Pizza %30 İndirim', 
    'Tüm müşteriler için %30 indirim fırsatı!', 
    'percentage', 
    30.00, 
    100.00, 
    70.00, 
    'regular', 
    false, 
    CURRENT_TIMESTAMP, 
    CURRENT_TIMESTAMP + INTERVAL '30 days', 
    false, 
    200, 
    'Bu indirim tüm müşteriler için geçerlidir. İndirme gerekmez.', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    (SELECT id FROM businesses LIMIT 1), 
    (SELECT id FROM business_locations WHERE is_main_location = true LIMIT 1), 
    'İkinci Kahve Bedava', 
    'İkinci kahvenizi bedava alın!', 
    'free_item', 
    0.00, 
    25.00, 
    12.50, 
    'regular', 
    true, 
    CURRENT_TIMESTAMP, 
    CURRENT_TIMESTAMP + INTERVAL '15 days', 
    false, 
    50, 
    'Bu teklif sadece aynı gün içinde geçerlidir.', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    (SELECT id FROM businesses LIMIT 1), 
    (SELECT id FROM business_locations WHERE is_main_location = false LIMIT 1), 
    '5000 TL İndirim', 
    '100 TL üzeri alışverişlerde 5000 TL indirim!', 
    'fixed_amount', 
    5000.00, 
    15000.00, 
    10000.00, 
    'gold', 
    false, 
    CURRENT_TIMESTAMP, 
    CURRENT_TIMESTAMP + INTERVAL '7 days', 
    true, 
    20, 
    'Minimum 100 TL alışveriş şartı vardır. İndirme gerekmez.', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Örnek Kuponlar
INSERT INTO coupons (business_id, business_location_id, code, title, description, discount_type, discount_value, start_date, end_date, max_uses, min_order_amount, terms_conditions, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES
(
    (SELECT id FROM businesses LIMIT 1), 
    (SELECT id FROM business_locations WHERE is_main_location = true LIMIT 1), 
    'HOSGELDIN10', 
    'Hoş Geldin İndirimi', 
    'İlk alışverişinizde %10 indirim', 
    'percentage', 
    10.00, 
    CURRENT_TIMESTAMP, 
    CURRENT_TIMESTAMP + INTERVAL '90 days', 
    1000, 
    50.00, 
    'Bu kupon sadece ilk alışverişlerde kullanılabilir.', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    (SELECT id FROM businesses LIMIT 1), 
    (SELECT id FROM business_locations WHERE is_main_location = false LIMIT 1), 
    'YAZ2024', 
    'Yaz İndirimi', 
    'Yaz sezonu özel %20 indirim', 
    'percentage', 
    20.00, 
    CURRENT_TIMESTAMP, 
    CURRENT_TIMESTAMP + INTERVAL '60 days', 
    500, 
    100.00, 
    'Bu kupon sadece yaz sezonunda geçerlidir.', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    (SELECT id FROM businesses LIMIT 1), 
    (SELECT id FROM business_locations WHERE is_main_location = true LIMIT 1), 
    'BEDAVA', 
    'Ücretsiz Kargo', 
    '100 TL üzeri alışverişlerde ücretsiz kargo', 
    'free_shipping', 
    0.00, 
    CURRENT_TIMESTAMP, 
    CURRENT_TIMESTAMP + INTERVAL '30 days', 
    200, 
    100.00, 
    'Bu kupon sadece kargo ücreti için geçerlidir.', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e', 
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- =====================================================
-- END OF OFFERS SYSTEM
-- =====================================================
