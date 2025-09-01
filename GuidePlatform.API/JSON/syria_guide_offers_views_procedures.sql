-- =====================================================
-- Syria Guide Offers Views & Stored Procedures
-- أوامر SQL للعروض والكوبونات
-- =====================================================

-- =====================================================
-- VIEWS - العروض
-- =====================================================

-- View for active offers with business and category information
CREATE OR REPLACE VIEW v_active_offers AS
SELECT 
    o.id,
    o.business_id,
    b.name as business_name,
    b.category_id,
    c.name as category_name,
    o.title,
    o.description,
    o.discount_type,
    o.discount_value,
    o.original_price,
    o.discounted_price,
    o.currency,
    o.start_date,
    o.end_date,
    o.is_featured,
    o.max_uses,
    o.current_uses,
    o.min_order_amount,
    o.terms_conditions,
    o.photo,
    o.thumbnail,
    o.photo_content_type,
    o.icon,
    o.row_created_date,
    o.row_updated_date,
    CASE 
        WHEN o.discount_type = 'percentage' THEN CONCAT(o.discount_value, '%')
        WHEN o.discount_type = 'fixed_amount' THEN CONCAT(o.discount_value, ' ', o.currency)
        WHEN o.discount_type = 'free_item' THEN 'Bedava'
        ELSE 'İndirim'
    END as discount_display,
    CASE 
        WHEN o.max_uses IS NULL THEN 'Sınırsız'
        ELSE CONCAT(o.current_uses, '/', o.max_uses)
    END as usage_display,
    CASE 
        WHEN CURRENT_TIMESTAMP BETWEEN o.start_date AND o.end_date THEN 'Aktif'
        WHEN CURRENT_TIMESTAMP < o.start_date THEN 'Yakında'
        ELSE 'Süresi Dolmuş'
    END as status
FROM offers o
INNER JOIN businesses b ON o.business_id = b.id
LEFT JOIN categories c ON b.category_id = c.id
WHERE o.is_active = true 
    AND o.row_is_active = true 
    AND o.row_is_deleted = false
    AND b.row_is_active = true 
    AND b.row_is_deleted = false;

-- View for offers by category
CREATE OR REPLACE VIEW v_offers_by_category AS
SELECT 
    c.id as category_id,
    c.name as category_name,
    c.description as category_description,
    c.icon as category_icon,
    COUNT(o.id) as offers_count,
    COUNT(CASE WHEN o.is_featured = true THEN 1 END) as featured_offers_count
FROM categories c
LEFT JOIN businesses b ON c.id = b.category_id
LEFT JOIN offers o ON b.id = o.business_id 
    AND o.is_active = true 
    AND o.row_is_active = true 
    AND o.row_is_deleted = false
    AND CURRENT_TIMESTAMP BETWEEN o.start_date AND o.end_date
WHERE c.row_is_active = true 
    AND c.row_is_deleted = false
    AND b.row_is_active = true 
    AND b.row_is_deleted = false
GROUP BY c.id, c.name, c.description, c.icon;

-- View for user's favorite offers
CREATE OR REPLACE VIEW v_user_favorite_offers AS
SELECT 
    ufo.id,
    ufo.user_id,
    o.id as offer_id,
    o.business_id,
    b.name as business_name,
    c.name as category_name,
    o.title,
    o.description,
    o.discount_type,
    o.discount_value,
    o.original_price,
    o.discounted_price,
    o.currency,
    o.start_date,
    o.end_date,
    o.is_featured,
    o.photo,
    o.thumbnail,
    o.photo_content_type,
    o.icon,
    ufo.row_created_date as favorited_date
FROM user_favorite_offers ufo
INNER JOIN offers o ON ufo.offer_id = o.id
INNER JOIN businesses b ON o.business_id = b.id
LEFT JOIN categories c ON b.category_id = c.id
WHERE ufo.row_is_active = true 
    AND ufo.row_is_deleted = false
    AND o.is_active = true 
    AND o.row_is_active = true 
    AND o.row_is_deleted = false
    AND b.row_is_active = true 
    AND b.row_is_deleted = false;

-- View for active coupons
CREATE OR REPLACE VIEW v_active_coupons AS
SELECT 
    cp.id,
    cp.business_id,
    b.name as business_name,
    b.category_id,
    c.name as category_name,
    cp.code,
    cp.title,
    cp.description,
    cp.discount_type,
    cp.discount_value,
    cp.currency,
    cp.start_date,
    cp.end_date,
    cp.max_uses,
    cp.current_uses,
    cp.min_order_amount,
    cp.max_discount_amount,
    cp.is_first_time_only,
    cp.terms_conditions,
    cp.icon,
    cp.row_created_date,
    cp.row_updated_date,
    CASE 
        WHEN cp.discount_type = 'percentage' THEN CONCAT(cp.discount_value, '%')
        WHEN cp.discount_type = 'fixed_amount' THEN CONCAT(cp.discount_value, ' ', cp.currency)
        WHEN cp.discount_type = 'free_shipping' THEN 'Ücretsiz Kargo'
        ELSE 'İndirim'
    END as discount_display,
    CASE 
        WHEN cp.max_uses IS NULL THEN 'Sınırsız'
        ELSE CONCAT(cp.current_uses, '/', cp.max_uses)
    END as usage_display,
    CASE 
        WHEN CURRENT_TIMESTAMP BETWEEN cp.start_date AND cp.end_date THEN 'Aktif'
        WHEN CURRENT_TIMESTAMP < cp.start_date THEN 'Yakında'
        ELSE 'Süresi Dolmuş'
    END as status
FROM coupons cp
INNER JOIN businesses b ON cp.business_id = b.id
LEFT JOIN categories c ON b.category_id = c.id
WHERE cp.is_active = true 
    AND cp.row_is_active = true 
    AND cp.row_is_deleted = false
    AND b.row_is_active = true 
    AND b.row_is_deleted = false;

-- =====================================================
-- STORED PROCEDURES - الإجراءات المخزنة
-- =====================================================

-- Procedure to get offers by category
CREATE OR REPLACE FUNCTION get_offers_by_category(
    p_category_id UUID DEFAULT NULL,
    p_limit INTEGER DEFAULT 10,
    p_offset INTEGER DEFAULT 0,
    p_featured_only BOOLEAN DEFAULT false,
    p_user_id UUID DEFAULT NULL
)
RETURNS TABLE (
    id UUID,
    business_id UUID,
    business_name VARCHAR(255),
    category_name VARCHAR(255),
    title VARCHAR(255),
    description TEXT,
    discount_type VARCHAR(20),
    discount_value DECIMAL(10,2),
    original_price DECIMAL(12,2),
    discounted_price DECIMAL(12,2),
    currency VARCHAR(3),
    start_date TIMESTAMPTZ,
    end_date TIMESTAMPTZ,
    is_featured BOOLEAN,
    max_uses INTEGER,
    current_uses INTEGER,
    min_order_amount DECIMAL(12,2),
    terms_conditions TEXT,
    photo bytea,
    thumbnail bytea,
    photo_content_type varchar(50),
    icon VARCHAR(100),
    discount_display TEXT,
    usage_display TEXT,
    status TEXT,
    is_favorited BOOLEAN,
    days_remaining INTEGER
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        o.id,
        o.business_id,
        o.business_name,
        o.category_name,
        o.title,
        o.description,
        o.discount_type,
        o.discount_value,
        o.original_price,
        o.discounted_price,
        o.currency,
        o.start_date,
        o.end_date,
        o.is_featured,
        o.max_uses,
        o.current_uses,
        o.min_order_amount,
        o.terms_conditions,
        o.photo,
        o.thumbnail,
        o.photo_content_type,
        o.icon,
        o.discount_display,
        o.usage_display,
        o.status,
        CASE WHEN p_user_id IS NOT NULL AND ufo.id IS NOT NULL THEN true ELSE false END as is_favorited,
        EXTRACT(DAY FROM (o.end_date - CURRENT_TIMESTAMP))::INTEGER as days_remaining
    FROM v_active_offers o
    LEFT JOIN user_favorite_offers ufo ON o.id = ufo.offer_id AND ufo.user_id = p_user_id
    WHERE (p_category_id IS NULL OR o.category_id = p_category_id)
        AND (NOT p_featured_only OR o.is_featured = true)
        AND o.status = 'Aktif'
    ORDER BY 
        o.is_featured DESC,
        o.row_created_date DESC
    LIMIT p_limit OFFSET p_offset;
END;
$$ LANGUAGE plpgsql;

-- Procedure to get offers by business
CREATE OR REPLACE FUNCTION get_offers_by_business(
    p_business_id UUID,
    p_limit INTEGER DEFAULT 10,
    p_offset INTEGER DEFAULT 0,
    p_user_id UUID DEFAULT NULL
)
RETURNS TABLE (
    id UUID,
    business_id UUID,
    business_name VARCHAR(255),
    category_name VARCHAR(255),
    title VARCHAR(255),
    description TEXT,
    discount_type VARCHAR(20),
    discount_value DECIMAL(10,2),
    original_price DECIMAL(12,2),
    discounted_price DECIMAL(12,2),
    currency VARCHAR(3),
    start_date TIMESTAMPTZ,
    end_date TIMESTAMPTZ,
    is_featured BOOLEAN,
    max_uses INTEGER,
    current_uses INTEGER,
    min_order_amount DECIMAL(12,2),
    terms_conditions TEXT,
    photo bytea,
    thumbnail bytea,
    photo_content_type varchar(50),
    icon VARCHAR(100),
    discount_display TEXT,
    usage_display TEXT,
    status TEXT,
    is_favorited BOOLEAN,
    days_remaining INTEGER
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        o.id,
        o.business_id,
        o.business_name,
        o.category_name,
        o.title,
        o.description,
        o.discount_type,
        o.discount_value,
        o.original_price,
        o.discounted_price,
        o.currency,
        o.start_date,
        o.end_date,
        o.is_featured,
        o.max_uses,
        o.current_uses,
        o.min_order_amount,
        o.terms_conditions,
        o.photo,
        o.thumbnail,
        o.photo_content_type,
        o.icon,
        o.discount_display,
        o.usage_display,
        o.status,
        CASE WHEN p_user_id IS NOT NULL AND ufo.id IS NOT NULL THEN true ELSE false END as is_favorited,
        EXTRACT(DAY FROM (o.end_date - CURRENT_TIMESTAMP))::INTEGER as days_remaining
    FROM v_active_offers o
    LEFT JOIN user_favorite_offers ufo ON o.id = ufo.offer_id AND ufo.user_id = p_user_id
    WHERE o.business_id = p_business_id
        AND o.status = 'Aktif'
    ORDER BY 
        o.is_featured DESC,
        o.row_created_date DESC
    LIMIT p_limit OFFSET p_offset;
END;
$$ LANGUAGE plpgsql;

-- Procedure to search offers
CREATE OR REPLACE FUNCTION search_offers(
    p_search_term TEXT DEFAULT '',
    p_category_id UUID DEFAULT NULL,
    p_business_id UUID DEFAULT NULL,
    p_discount_type VARCHAR(20) DEFAULT NULL,
    p_min_discount DECIMAL(10,2) DEFAULT NULL,
    p_max_discount DECIMAL(10,2) DEFAULT NULL,
    p_featured_only BOOLEAN DEFAULT false,
    p_limit INTEGER DEFAULT 10,
    p_offset INTEGER DEFAULT 0,
    p_user_id UUID DEFAULT NULL
)
RETURNS TABLE (
    id UUID,
    business_id UUID,
    business_name VARCHAR(255),
    category_name VARCHAR(255),
    title VARCHAR(255),
    description TEXT,
    discount_type VARCHAR(20),
    discount_value DECIMAL(10,2),
    original_price DECIMAL(12,2),
    discounted_price DECIMAL(12,2),
    currency VARCHAR(3),
    start_date TIMESTAMPTZ,
    end_date TIMESTAMPTZ,
    is_featured BOOLEAN,
    max_uses INTEGER,
    current_uses INTEGER,
    min_order_amount DECIMAL(12,2),
    terms_conditions TEXT,
    photo bytea,
    thumbnail bytea,
    photo_content_type varchar(50),
    icon VARCHAR(100),
    discount_display TEXT,
    usage_display TEXT,
    status TEXT,
    is_favorited BOOLEAN,
    days_remaining INTEGER,
    search_rank REAL
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        o.id,
        o.business_id,
        o.business_name,
        o.category_name,
        o.title,
        o.description,
        o.discount_type,
        o.discount_value,
        o.original_price,
        o.discounted_price,
        o.currency,
        o.start_date,
        o.end_date,
        o.is_featured,
        o.max_uses,
        o.current_uses,
        o.min_order_amount,
        o.terms_conditions,
        o.photo,
        o.thumbnail,
        o.photo_content_type,
        o.icon,
        o.discount_display,
        o.usage_display,
        o.status,
        CASE WHEN p_user_id IS NOT NULL AND ufo.id IS NOT NULL THEN true ELSE false END as is_favorited,
        EXTRACT(DAY FROM (o.end_date - CURRENT_TIMESTAMP))::INTEGER as days_remaining,
        ts_rank(
            to_tsvector('turkish', COALESCE(o.title, '') || ' ' || COALESCE(o.description, '') || ' ' || COALESCE(o.business_name, '')),
            plainto_tsquery('turkish', p_search_term)
        ) as search_rank
    FROM v_active_offers o
    LEFT JOIN user_favorite_offers ufo ON o.id = ufo.offer_id AND ufo.user_id = p_user_id
    WHERE o.status = 'Aktif'
        AND (p_category_id IS NULL OR o.category_id = p_category_id)
        AND (p_business_id IS NULL OR o.business_id = p_business_id)
        AND (p_discount_type IS NULL OR o.discount_type = p_discount_type)
        AND (p_min_discount IS NULL OR o.discount_value >= p_min_discount)
        AND (p_max_discount IS NULL OR o.discount_value <= p_max_discount)
        AND (NOT p_featured_only OR o.is_featured = true)
        AND (
            p_search_term = '' OR 
            to_tsvector('turkish', COALESCE(o.title, '') || ' ' || COALESCE(o.description, '') || ' ' || COALESCE(o.business_name, '')) @@ plainto_tsquery('turkish', p_search_term)
        )
    ORDER BY 
        search_rank DESC,
        o.is_featured DESC,
        o.row_created_date DESC
    LIMIT p_limit OFFSET p_offset;
END;
$$ LANGUAGE plpgsql;

-- Procedure to get user's favorite offers
CREATE OR REPLACE FUNCTION get_user_favorite_offers(
    p_user_id UUID,
    p_limit INTEGER DEFAULT 10,
    p_offset INTEGER DEFAULT 0
)
RETURNS TABLE (
    id UUID,
    offer_id UUID,
    business_id UUID,
    business_name VARCHAR(255),
    category_name VARCHAR(255),
    title VARCHAR(255),
    description TEXT,
    discount_type VARCHAR(20),
    discount_value DECIMAL(10,2),
    original_price DECIMAL(12,2),
    discounted_price DECIMAL(12,2),
    currency VARCHAR(3),
    start_date TIMESTAMPTZ,
    end_date TIMESTAMPTZ,
    is_featured BOOLEAN,
    photo bytea,
    thumbnail bytea,
    photo_content_type varchar(50),
    icon VARCHAR(100),
    favorited_date TIMESTAMPTZ,
    days_remaining INTEGER
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        ufo.id,
        ufo.offer_id,
        ufo.business_id,
        ufo.business_name,
        ufo.category_name,
        ufo.title,
        ufo.description,
        ufo.discount_type,
        ufo.discount_value,
        ufo.original_price,
        ufo.discounted_price,
        ufo.currency,
        ufo.start_date,
        ufo.end_date,
        ufo.is_featured,
        ufo.photo,
        ufo.thumbnail,
        ufo.photo_content_type,
        ufo.icon,
        ufo.favorited_date,
        EXTRACT(DAY FROM (ufo.end_date - CURRENT_TIMESTAMP))::INTEGER as days_remaining
    FROM v_user_favorite_offers ufo
    WHERE ufo.user_id = p_user_id
        AND CURRENT_TIMESTAMP BETWEEN ufo.start_date AND ufo.end_date
    ORDER BY ufo.favorited_date DESC
    LIMIT p_limit OFFSET p_offset;
END;
$$ LANGUAGE plpgsql;

-- Procedure to add offer to favorites
CREATE OR REPLACE FUNCTION add_offer_to_favorites(
    p_user_id UUID,
    p_offer_id UUID,
    p_auth_user_id UUID DEFAULT NULL,
    p_auth_customer_id UUID DEFAULT NULL,
    p_create_user_id UUID DEFAULT NULL,
    p_update_user_id UUID DEFAULT NULL
)
RETURNS BOOLEAN AS $$
DECLARE
    v_exists BOOLEAN;
BEGIN
    -- Check if already favorited
    SELECT EXISTS(
        SELECT 1 FROM user_favorite_offers 
        WHERE user_id = p_user_id 
            AND offer_id = p_offer_id 
            AND row_is_active = true 
            AND row_is_deleted = false
    ) INTO v_exists;
    
    IF v_exists THEN
        RETURN false; -- Already favorited
    END IF;
    
    -- Check if offer exists and is active
    IF NOT EXISTS(
        SELECT 1 FROM offers 
        WHERE id = p_offer_id 
            AND is_active = true 
            AND row_is_active = true 
            AND row_is_deleted = false
    ) THEN
        RETURN false; -- Offer not found or inactive
    END IF;
    
    -- Add to favorites
    INSERT INTO user_favorite_offers (
        user_id, 
        offer_id, 
        auth_user_id, 
        auth_customer_id, 
        create_user_id, 
        update_user_id
    ) VALUES (
        p_user_id, 
        p_offer_id, 
        p_auth_user_id, 
        p_auth_customer_id, 
        p_create_user_id, 
        p_update_user_id
    );
    
    RETURN true;
END;
$$ LANGUAGE plpgsql;

-- Procedure to remove offer from favorites
CREATE OR REPLACE FUNCTION remove_offer_from_favorites(
    p_user_id UUID,
    p_offer_id UUID,
    p_update_user_id UUID DEFAULT NULL
)
RETURNS BOOLEAN AS $$
BEGIN
    UPDATE user_favorite_offers 
    SET row_is_deleted = true,
        row_updated_date = CURRENT_TIMESTAMP,
        update_user_id = p_update_user_id
    WHERE user_id = p_user_id 
        AND offer_id = p_offer_id 
        AND row_is_active = true 
        AND row_is_deleted = false;
    
    RETURN FOUND;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- TRIGGERS - المشغلات
-- =====================================================

-- Trigger to update current_uses when offer is used
CREATE OR REPLACE FUNCTION update_offer_usage_count()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE offers 
        SET current_uses = current_uses + 1,
            row_updated_date = CURRENT_TIMESTAMP
        WHERE id = NEW.offer_id;
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE offers 
        SET current_uses = current_uses - 1,
            row_updated_date = CURRENT_TIMESTAMP
        WHERE id = OLD.offer_id;
        RETURN OLD;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_offer_usage_count
    AFTER INSERT OR DELETE ON user_offer_usage
    FOR EACH ROW
    EXECUTE FUNCTION update_offer_usage_count();

-- Trigger to update current_uses when coupon is used
CREATE OR REPLACE FUNCTION update_coupon_usage_count()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE coupons 
        SET current_uses = current_uses + 1,
            row_updated_date = CURRENT_TIMESTAMP
        WHERE id = NEW.coupon_id;
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE coupons 
        SET current_uses = current_uses - 1,
            row_updated_date = CURRENT_TIMESTAMP
        WHERE id = OLD.coupon_id;
        RETURN OLD;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_coupon_usage_count
    AFTER INSERT OR DELETE ON user_coupon_usage
    FOR EACH ROW
    EXECUTE FUNCTION update_coupon_usage_count();

-- =====================================================
-- END OF OFFERS VIEWS & PROCEDURES
-- =====================================================
