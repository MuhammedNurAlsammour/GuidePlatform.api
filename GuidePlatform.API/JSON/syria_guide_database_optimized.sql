-- =====================================================
-- Syria Guide Database - Optimized Version (No Data Duplication)
-- قاعدة بيانات دليلك في سوريا - النسخة المحسنة (بدون تكرار البيانات)
-- =====================================================
-- =====================================================
-- CORE TABLES - الجداول الأساسية
-- =====================================================

-- categories table - جدول الفئات
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    parent_id UUID,
    icon VARCHAR(100) DEFAULT 'category',
    sort_order INTEGER DEFAULT 0,
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,    
    auth_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (parent_id) REFERENCES categories(id),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- =====================================================
-- BUSINESS DIRECTORY TABLES - جداول دليل الأعمال
-- =====================================================

-- businesses table - جدول الأعمال
CREATE TABLE businesses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category_id UUID,
    sub_category_id UUID,
    province_id UUID,
    countries_id UUID,
    district_id UUID,
    address TEXT,
    phone VARCHAR(20),
    mobile VARCHAR(20),
    email VARCHAR(255),
    website VARCHAR(500),
    facebook_url VARCHAR(500),
    instagram_url VARCHAR(500),
    whatsapp VARCHAR(20),
    telegram VARCHAR(100),
    latitude DECIMAL(10,8),
    longitude DECIMAL(11,8),
    rating DECIMAL(3,2) DEFAULT 0.00,
    total_reviews INTEGER DEFAULT 0,
    view_count INTEGER DEFAULT 0,
    subscription_type VARCHAR(20) DEFAULT 'FREE', -- FREE, SILVER, GOLD
    is_verified BOOLEAN DEFAULT false,
    is_featured BOOLEAN DEFAULT false,
    working_hours TEXT,
    icon VARCHAR(100) DEFAULT 'business',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    owner_id UUID, -- Reference to auth."AspNetUsers"."Id" (business owner)
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (category_id) REFERENCES categories(id),
    FOREIGN KEY (sub_category_id) REFERENCES categories(id),
    FOREIGN KEY (province_id) REFERENCES storeplatformh."provinces"("id"),
    FOREIGN KEY (countries_id) REFERENCES storeplatformh."countries"("id"),
    FOREIGN KEY (district_id) REFERENCES storeplatformh."districts"("id"),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (owner_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- business_images table - جدول صور الأعمال
CREATE TABLE business_images (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    photo bytea NULL,
    thumbnail bytea NULL,
    photo_content_type varchar(50) NULL,    
    alt_text VARCHAR(255),
    image_type VARCHAR(50) DEFAULT 'gallery', -- profile, gallery, menu, etc.
    is_primary BOOLEAN DEFAULT false,
    sort_order INTEGER DEFAULT 0,
    icon VARCHAR(100) DEFAULT 'image',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- business_contacts table - جدول معلومات الاتصال
CREATE TABLE business_contacts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    contact_type VARCHAR(50) NOT NULL, -- phone, email, whatsapp, facebook, instagram, website
    contact_value VARCHAR(255) NOT NULL,
    is_primary BOOLEAN DEFAULT false,
    icon VARCHAR(100) DEFAULT 'contact_phone',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- business_services table - جدول خدمات الأعمال
CREATE TABLE business_services (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    service_name VARCHAR(255) NOT NULL,
    service_description TEXT,
    price DECIMAL(12,2),
    currency VARCHAR(3) DEFAULT 'SYP',
    is_available BOOLEAN DEFAULT true,
    icon VARCHAR(100) DEFAULT 'miscellaneous_services',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- business_working_hours table - جدول ساعات العمل
CREATE TABLE business_working_hours (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    day_of_week INTEGER NOT NULL CHECK (day_of_week >= 1 AND day_of_week <= 7), -- 1=Monday, 7=Sunday
    open_time TIME,
    close_time TIME,
    is_closed BOOLEAN DEFAULT false,
    icon VARCHAR(100) DEFAULT 'schedule',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- business_reviews table - جدول تقييمات الأعمال
CREATE TABLE business_reviews (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    reviewer_id UUID NOT NULL, -- Reference to auth."AspNetUsers"."Id"
    rating INTEGER NOT NULL CHECK (rating >= 1 AND rating <= 5),
    comment TEXT,
    is_verified BOOLEAN DEFAULT false,
    is_approved BOOLEAN DEFAULT true,
    icon VARCHAR(100) DEFAULT 'rate_review',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (reviewer_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- =====================================================
-- USER MANAGEMENT TABLES - جداول إدارة المستخدمين
-- =====================================================


-- user_favorites table - جدول المفضلة
CREATE TABLE user_favorites (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    icon VARCHAR(100) DEFAULT 'favorite',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- user_visits table - جدول زيارات المستخدمين
CREATE TABLE user_visits (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    visit_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    visit_type VARCHAR(50) DEFAULT 'view', -- view, contact, review
    icon VARCHAR(100) DEFAULT 'visibility',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- =====================================================
-- NOTIFICATION SYSTEM TABLES - جداول نظام الإشعارات
-- =====================================================

-- notifications table - جدول الإشعارات
CREATE TABLE notifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    recipient_user_id UUID NOT NULL, -- المستلم
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    notification_type VARCHAR(50) DEFAULT 'info', -- info, success, warning, error
    is_read BOOLEAN DEFAULT false,
    read_date TIMESTAMPTZ,
    action_url VARCHAR(500),
    related_entity_id UUID, -- معرف الكيان المرتبط
    related_entity_type VARCHAR(50), -- business, article, review, etc.
    icon VARCHAR(100) DEFAULT 'notifications',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID, -- المستخدم الذي أنشأ الإشعار
    auth_customer_id UUID,
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (recipient_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- notification_settings table - جدول إعدادات الإشعارات
CREATE TABLE notification_settings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    setting_type VARCHAR(50) NOT NULL, -- email, push, sms
    is_enabled BOOLEAN DEFAULT true,
    icon VARCHAR(100) DEFAULT 'settings',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id"),
    UNIQUE(user_id, setting_type)
);


-- =====================================================
-- SEARCH AND ANALYTICS TABLES - جداول البحث والتحليلات
-- =====================================================

-- search_logs table - جدول سجلات البحث
CREATE TABLE search_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    search_term VARCHAR(255) NOT NULL,
    search_filters JSONB,
    results_count INTEGER,
    search_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    ip_address INET,
    user_agent TEXT,
    icon VARCHAR(100) DEFAULT 'search',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID,
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);
-- business_analytics table - جدول تحليلات الأعمال
CREATE TABLE business_analytics (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    date DATE NOT NULL,
    views_count INTEGER DEFAULT 0,
    contacts_count INTEGER DEFAULT 0,
    reviews_count INTEGER DEFAULT 0,
    favorites_count INTEGER DEFAULT 0,
    icon VARCHAR(100) DEFAULT 'analytics',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id"),
    UNIQUE(business_id, date)
);

-- =====================================================
-- SUBSCRIPTION AND PAYMENT TABLES - جداول الاشتراكات والمدفوعات
-- =====================================================

-- subscriptions table - جدول الاشتراكات
CREATE TABLE subscriptions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL,
    subscription_type VARCHAR(20) NOT NULL, -- FREE, SILVER, GOLD
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'SYP',
    status VARCHAR(20) DEFAULT 'active', -- active, expired, cancelled
    payment_status VARCHAR(20) DEFAULT 'pending', -- pending, paid, failed
    icon VARCHAR(100) DEFAULT 'subscriptions',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- payments table - جدول المدفوعات
CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    subscription_id UUID NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'SYP',
    payment_method VARCHAR(50), -- cash, bank_transfer, online
    transaction_id VARCHAR(255),
    payment_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(20) DEFAULT 'pending', -- pending, completed, failed, refunded
    notes TEXT,
    icon VARCHAR(100) DEFAULT 'payment',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (subscription_id) REFERENCES subscriptions(id) ON DELETE CASCADE,
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- =====================================================
-- CONTENT MANAGEMENT TABLES - جداول إدارة المحتوى
-- =====================================================

-- articles table - جدول المقالات
CREATE TABLE articles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(500) NOT NULL,
    content TEXT,
    excerpt TEXT,
    photo bytea NULL,
    thumbnail bytea NULL,
    photo_content_type varchar(50) NULL,
    author_id UUID NOT NULL, -- Reference to auth."AspNetUsers"."Id"
    category_id UUID REFERENCES categories(id),
    is_featured BOOLEAN DEFAULT FALSE,
    is_published BOOLEAN DEFAULT FALSE,
    published_at TIMESTAMPTZ,
    view_count INTEGER DEFAULT 0,
    seo_title VARCHAR(255),
    seo_description VARCHAR(500),
    slug VARCHAR(255) UNIQUE,
    icon VARCHAR(100) DEFAULT 'article',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (category_id) REFERENCES categories(id),
    FOREIGN KEY (author_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- pages table - جدول الصفحات
CREATE TABLE pages (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    slug VARCHAR(255) NOT NULL UNIQUE,
    content TEXT,
    meta_description TEXT,
    meta_keywords TEXT,
    is_published BOOLEAN DEFAULT false,
    published_date TIMESTAMPTZ,
    icon VARCHAR(100) DEFAULT 'article',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- banners table - جدول البانرات
CREATE TABLE banners (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    photo bytea NULL,
    thumbnail bytea NULL,
    photo_content_type varchar(50) NULL,
    link_url VARCHAR(500),
    start_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    end_date TIMESTAMPTZ,
    is_active BOOLEAN DEFAULT true,
    order_index INTEGER DEFAULT 0,
    icon VARCHAR(100) DEFAULT 'image',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- announcements table - جدول الإعلانات
CREATE TABLE announcements (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    content TEXT NOT NULL,
    priority INTEGER DEFAULT 1, -- 1=High, 2=Medium, 3=Low
    is_published BOOLEAN DEFAULT false,
    published_date TIMESTAMPTZ,
    icon VARCHAR(100) DEFAULT 'announcement',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- =====================================================
-- SYSTEM TABLES - جداول النظام
-- =====================================================

-- parameters table - جدول المعاملات
CREATE TABLE parameters (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) UNIQUE NOT NULL,
    value TEXT NOT NULL,
    description TEXT,
    data_type VARCHAR(50) DEFAULT 'string', -- string, integer, boolean, json
    is_system BOOLEAN DEFAULT false,
    icon VARCHAR(100) DEFAULT 'settings',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- files table - جدول الملفات
CREATE TABLE files (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    file_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    file_size BIGINT,
    mime_type VARCHAR(100),
    file_type VARCHAR(50), -- image, document, video, audio
    is_public BOOLEAN DEFAULT false,
    icon VARCHAR(100) DEFAULT 'file_copy',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID,
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- logs table - جدول السجلات
CREATE TABLE logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    level VARCHAR(20) NOT NULL, -- info, warning, error, debug
    message TEXT NOT NULL,
    exception TEXT,
    source VARCHAR(255),
    ip_address INET,
    user_agent TEXT,
    icon VARCHAR(100) DEFAULT 'bug_report',
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID, -- Reference to auth."AspNetUsers"."Id" (nullable for system logs)
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- =====================================================
-- INDEXES - الفهارس
-- =====================================================

-- Categories indexes
CREATE INDEX idx_categories_active_deleted ON categories USING btree (row_is_active, row_is_deleted);
CREATE INDEX idx_categories_auth_customer ON categories USING btree (auth_customer_id) WHERE (auth_customer_id IS NOT NULL);
CREATE INDEX idx_categories_auth_user ON categories USING btree (auth_user_id) WHERE (auth_user_id IS NOT NULL);
CREATE INDEX idx_categories_created_date ON categories USING btree (row_created_date);
CREATE INDEX idx_categories_parent_id ON categories USING btree (parent_id) WHERE (parent_id IS NOT NULL);

-- User management indexes
CREATE INDEX idx_user_favorites_user_id ON user_favorites(auth_user_id);
CREATE INDEX idx_user_favorites_business_id ON user_favorites(business_id);
CREATE INDEX idx_user_visits_user_id ON user_visits(auth_user_id);
CREATE INDEX idx_user_visits_business_id ON user_visits(business_id);
CREATE INDEX idx_user_visits_visit_date ON user_visits(visit_date);

-- Notification indexes
CREATE INDEX idx_notifications_user_id ON notifications(auth_user_id);
CREATE INDEX idx_notifications_is_read ON notifications(is_read);
CREATE INDEX idx_notifications_notification_type ON notifications(notification_type);
CREATE INDEX idx_notification_settings_user_id ON notification_settings(auth_user_id);

-- Search and analytics indexes
CREATE INDEX idx_search_logs_user_id ON search_logs(auth_user_id);
CREATE INDEX idx_search_logs_search_date ON search_logs(search_date);
CREATE INDEX idx_business_analytics_business_id ON business_analytics(business_id);
CREATE INDEX idx_business_analytics_date ON business_analytics(date);

-- Subscription and payment indexes
CREATE INDEX idx_subscriptions_business_id ON subscriptions(business_id);
CREATE INDEX idx_subscriptions_status ON subscriptions(status);
CREATE INDEX idx_subscriptions_end_date ON subscriptions(end_date);
CREATE INDEX idx_payments_subscription_id ON payments(subscription_id);
CREATE INDEX idx_payments_status ON payments(status);
CREATE INDEX idx_payments_payment_date ON payments(payment_date);

-- Business indexes
CREATE INDEX idx_businesses_category ON businesses(category_id);
CREATE INDEX idx_businesses_location ON businesses(province_id, countries_id, district_id);
CREATE INDEX idx_businesses_featured ON businesses(is_featured);
CREATE INDEX idx_businesses_verified ON businesses(is_verified);
CREATE INDEX idx_businesses_active ON businesses(row_is_active, row_is_deleted);
CREATE INDEX idx_businesses_owner ON businesses(owner_id);
CREATE INDEX idx_business_contacts_business_id ON business_contacts(business_id);
CREATE INDEX idx_business_contacts_contact_type ON business_contacts(contact_type);
CREATE INDEX idx_business_services_business_id ON business_services(business_id);
CREATE INDEX idx_business_services_is_available ON business_services(is_available);
CREATE INDEX idx_business_working_hours_business_id ON business_working_hours(business_id);
CREATE INDEX idx_business_working_hours_day ON business_working_hours(day_of_week);
CREATE INDEX idx_business_reviews_business ON business_reviews(business_id);
CREATE INDEX idx_business_reviews_reviewer ON business_reviews(reviewer_id);
CREATE INDEX idx_business_reviews_approved ON business_reviews(is_approved);

-- Content management indexes
CREATE INDEX idx_articles_author ON articles(author_id);
CREATE INDEX idx_articles_category ON articles(category_id);
CREATE INDEX idx_articles_slug ON articles(slug);
CREATE INDEX idx_articles_published ON articles(is_published);
CREATE INDEX idx_pages_slug ON pages(slug);
CREATE INDEX idx_pages_is_published ON pages(is_published);
CREATE INDEX idx_banners_is_active ON banners(is_active);
CREATE INDEX idx_banners_order_index ON banners(order_index);
CREATE INDEX idx_banners_start_date ON banners(start_date);
CREATE INDEX idx_banners_end_date ON banners(end_date);
CREATE INDEX idx_announcements_is_published ON announcements(is_published);
CREATE INDEX idx_announcements_priority ON announcements(priority);

-- System indexes
CREATE INDEX idx_parameters_name ON parameters(name);
CREATE INDEX idx_parameters_is_system ON parameters(is_system);
CREATE INDEX idx_files_file_type ON files(file_type);
CREATE INDEX idx_files_is_public ON files(is_public);
CREATE INDEX idx_logs_level ON logs(level);
CREATE INDEX idx_logs_source ON logs(source);
CREATE INDEX idx_logs_user_id ON logs(auth_user_id);

-- Customer indexes
CREATE INDEX idx_categories_customer_id ON categories(auth_customer_id);
CREATE INDEX idx_businesses_customer_id ON businesses(auth_customer_id);
CREATE INDEX idx_user_favorites_customer_id ON user_favorites(auth_customer_id);
CREATE INDEX idx_user_visits_customer_id ON user_visits(auth_customer_id);
CREATE INDEX idx_notifications_customer_id ON notifications(auth_customer_id);
CREATE INDEX idx_parameters_customer_id ON parameters(auth_customer_id);

-- Create and Update User indexes
CREATE INDEX idx_categories_create_user ON categories(create_user_id);
CREATE INDEX idx_categories_update_user ON categories(update_user_id);
CREATE INDEX idx_businesses_create_user ON businesses(create_user_id);
CREATE INDEX idx_businesses_update_user ON businesses(update_user_id);
CREATE INDEX idx_business_images_create_user ON business_images(create_user_id);
CREATE INDEX idx_business_images_update_user ON business_images(update_user_id);
CREATE INDEX idx_business_contacts_create_user ON business_contacts(create_user_id);
CREATE INDEX idx_business_contacts_update_user ON business_contacts(update_user_id);
CREATE INDEX idx_business_services_create_user ON business_services(create_user_id);
CREATE INDEX idx_business_services_update_user ON business_services(update_user_id);
CREATE INDEX idx_business_working_hours_create_user ON business_working_hours(create_user_id);
CREATE INDEX idx_business_working_hours_update_user ON business_working_hours(update_user_id);
CREATE INDEX idx_business_reviews_create_user ON business_reviews(create_user_id);
CREATE INDEX idx_business_reviews_update_user ON business_reviews(update_user_id);
CREATE INDEX idx_user_favorites_create_user ON user_favorites(create_user_id);
CREATE INDEX idx_user_favorites_update_user ON user_favorites(update_user_id);
CREATE INDEX idx_user_visits_create_user ON user_visits(create_user_id);
CREATE INDEX idx_user_visits_update_user ON user_visits(update_user_id);
CREATE INDEX idx_notifications_create_user ON notifications(create_user_id);
CREATE INDEX idx_notifications_update_user ON notifications(update_user_id);
CREATE INDEX idx_notification_settings_create_user ON notification_settings(create_user_id);
CREATE INDEX idx_notification_settings_update_user ON notification_settings(update_user_id);
CREATE INDEX idx_search_logs_create_user ON search_logs(create_user_id);
CREATE INDEX idx_search_logs_update_user ON search_logs(update_user_id);
CREATE INDEX idx_business_analytics_create_user ON business_analytics(create_user_id);
CREATE INDEX idx_business_analytics_update_user ON business_analytics(update_user_id);
CREATE INDEX idx_subscriptions_create_user ON subscriptions(create_user_id);
CREATE INDEX idx_subscriptions_update_user ON subscriptions(update_user_id);
CREATE INDEX idx_payments_create_user ON payments(create_user_id);
CREATE INDEX idx_payments_update_user ON payments(update_user_id);
CREATE INDEX idx_articles_create_user ON articles(create_user_id);
CREATE INDEX idx_articles_update_user ON articles(update_user_id);
CREATE INDEX idx_pages_create_user ON pages(create_user_id);
CREATE INDEX idx_pages_update_user ON pages(update_user_id);
CREATE INDEX idx_banners_create_user ON banners(create_user_id);
CREATE INDEX idx_banners_update_user ON banners(update_user_id);
CREATE INDEX idx_announcements_create_user ON announcements(create_user_id);
CREATE INDEX idx_announcements_update_user ON announcements(update_user_id);
CREATE INDEX idx_parameters_create_user ON parameters(create_user_id);
CREATE INDEX idx_parameters_update_user ON parameters(update_user_id);
CREATE INDEX idx_files_create_user ON files(create_user_id);
CREATE INDEX idx_files_update_user ON files(update_user_id);
CREATE INDEX idx_logs_create_user ON logs(create_user_id);
CREATE INDEX idx_logs_update_user ON logs(update_user_id);


-- =====================================================
-- SAMPLE DATA - البيانات النموذجية
-- =====================================================
-- Temel Kategoriler
INSERT INTO categories (name, description, sort_order, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES
('Restoranlar ve Kafeler', 'Restaurants & Cafes', 1, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'), 
('Alışveriş', 'Shopping', 2, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),     
('Sağlık Hizmetleri', 'Medical Services', 3, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Eğitim', 'Education', 4, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Finansal Hizmetler', 'Financial Services', 5, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Otomotiv Hizmetleri', 'Automotive Services', 6, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Emlak', 'Real Estate', 7, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Turizm ve Seyahat', 'Tourism & Travel', 8, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Teknoloji', 'Technology', 9, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Ev Hizmetleri', 'Home Services', 10, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- Temel Parametreler
INSERT INTO parameters (name, value, description, data_type, is_system, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES
('max_file_size', '10485760', 'Maksimum dosya boyutu (10MB)', 'integer', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('allowed_file_types', 'jpg,jpeg,png,gif,pdf,doc,docx', 'İzin verilen dosya türleri', 'string', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('business_verification_required', 'true', 'İşletme doğrulaması gerekli mi?', 'boolean', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('auto_approve_reviews', 'false', 'Yorumları otomatik onayla', 'boolean', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('subscription_prices', '{"FREE": 0, "SILVER": 50000, "GOLD": 100000}', 'Abonelik fiyatları', 'json', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('contact_form_enabled', 'true', 'İletişim formu aktif mi?', 'boolean', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('search_suggestions_limit', '10', 'Arama önerileri limiti', 'integer', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('analytics_retention_days', '365', 'Analitik verileri saklama süresi (gün)', 'integer', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- Temel Sayfalar
INSERT INTO pages (title, slug, content, meta_description, is_published, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES
('Hakkımızda', 'about', 'Syria Guide platformu hakkında bilgiler...', 'Syria Guide platformu hakkında detaylı bilgi', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('İletişim', 'contact', 'Bizimle iletişime geçin...', 'Syria Guide ile iletişim bilgileri', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Gizlilik Politikası', 'privacy', 'Gizlilik politikamız...', 'Gizlilik politikası ve veri koruma', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Kullanım Şartları', 'terms', 'Kullanım şartları ve koşulları...', 'Platform kullanım şartları', true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- Temel Duyurular
INSERT INTO announcements (title, content, priority, is_published, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES
('Hoş Geldiniz!', 'Syria Guide platformuna hoş geldiniz. En iyi işletmeleri keşfedin!', 1, true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Yeni Özellikler', 'Platformumuzda yeni özellikler eklendi. Kontrol edin!', 2, true, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('Bakım Bildirimi', 'Sistem bakımı nedeniyle kısa süreli kesinti yaşanabilir.', 3, false, '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- =====================================================
-- END OF OPTIMIZED DATABASE
-- =====================================================
