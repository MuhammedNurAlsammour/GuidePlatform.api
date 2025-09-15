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
    subscription_type INTEGER DEFAULT 0, -- 0: FREE, 1: SILVER, 2: GOLD
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
    photo bytea NULL, -- Eski sistem için korunuyor - Kept for old system
    thumbnail bytea NULL, -- Eski sistem için korunuyor - Kept for old system
    photo_url VARCHAR(500) NULL, -- Yeni sistem: Fotoğraf URL'si - New system: Photo URL
    thumbnail_url VARCHAR(500) NULL, -- Yeni sistem: Küçük resim URL'si - New system: Thumbnail URL
    photo_content_type varchar(50) NULL,    
    alt_text VARCHAR(255),
    image_type INT DEFAULT 1, -- 0:profile, 1:gallery, 2:menu, 3:banner, 4:logo, 5:interior, 6:exterior, 7:food, 8:kitchen, 9:atmosphere, 10:design, 11:dessert
    is_primary BOOLEAN DEFAULT false,
    sort_order INT DEFAULT 0,
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
    setting_type INTEGER NOT NULL, -- 0:email, 1:push, 2:sms, 3:whatsapp, 4:telegram, etc.
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

-- guideplatform.subscriptions definition

-- Drop table

-- DROP TABLE guideplatform.subscriptions;
-- DROP TABLE guideplatform.subscriptions;
CREATE TABLE subscriptions (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	business_id uuid NOT NULL, -- İşletme kimliği
	start_date date NOT NULL, -- Başlangıç tarihi
	end_date date NOT NULL, -- Bitiş tarihi
	amount numeric(12, 2) NOT NULL, -- Abonelik tutarı
	payment_status int4 DEFAULT 0 NULL, -- Ödeme durumu (0: bekliyor, 1: tamamlandı, 2: başarısız, 3: iade edildi)
	icon varchar(100) DEFAULT 'subscriptions'::character varying NULL, -- Bu satır kaldırıldı
	row_created_date timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL, -- Oluşturulma tarihi
	row_updated_date timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL, -- Güncellenme tarihi
	row_is_active bool DEFAULT true NOT NULL, -- Aktif mi?
	row_is_deleted bool DEFAULT false NOT NULL, -- Silindi mi?
	auth_user_id uuid NULL, -- Kimlik doğrulama kullanıcı kimliği
	auth_customer_id uuid NULL, -- Müşteri kimliği
	create_user_id uuid NULL, -- Oluşturan kullanıcı kimliği
	update_user_id uuid NULL, -- Güncelleyen kullanıcı kimliği
	currency int4 DEFAULT 1 NULL, -- Para birimi (1: SYP, 2: TRY, 3: USD, 4: EUR)
	status int4 DEFAULT 1 NULL, -- Abonelik durumu (1: aktif, 2: pasif, 3: iptal edildi)
	subscription_type int4 NOT NULL, -- Abonelik türü (1: ücretsiz, 2: gümüş, 3: altın)
	CONSTRAINT chk_currency_values CHECK ((currency = ANY (ARRAY[1, 2, 3, 4]))),
	CONSTRAINT chk_status_values CHECK ((status = ANY (ARRAY[1, 2, 3]))),
	CONSTRAINT chk_subscription_type_values CHECK ((subscription_type = ANY (ARRAY[1, 2, 3]))),
	CONSTRAINT subscriptions_pkey PRIMARY KEY (id)
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
    photo bytea NULL, -- Eski sistem için korunuyor - Kept for old system
    thumbnail bytea NULL, -- Eski sistem için korunuyor - Kept for old system
    photo_url VARCHAR(500) NULL, -- Yeni sistem: Fotoğraf URL'si - New system: Photo URL
    thumbnail_url VARCHAR(500) NULL, -- Yeni sistem: Küçük resim URL'si - New system: Thumbnail URL
    photo_content_type VARCHAR(50) NULL,
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
    province_id UUID, -- Reference to provinces."Id"
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (province_id) REFERENCES provinces("Id")
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
    file_path BYTEA NOT NULL, -- Binary data for file content
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

CREATE INDEX idx_subscriptions_business_id ON guideplatform.subscriptions USING btree (business_id);
CREATE INDEX idx_subscriptions_create_user ON guideplatform.subscriptions USING btree (create_user_id);
CREATE INDEX idx_subscriptions_end_date ON guideplatform.subscriptions USING btree (end_date);
CREATE INDEX idx_subscriptions_update_user ON guideplatform.subscriptions USING btree (update_user_id);


-- guideplatform.subscriptions foreign keys

ALTER TABLE guideplatform.subscriptions ADD CONSTRAINT subscriptions_auth_customer_id_fkey FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id");
ALTER TABLE guideplatform.subscriptions ADD CONSTRAINT subscriptions_auth_user_id_fkey FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id");
ALTER TABLE guideplatform.subscriptions ADD CONSTRAINT subscriptions_business_id_fkey FOREIGN KEY (business_id) REFERENCES guideplatform.businesses(id) ON DELETE CASCADE;
ALTER TABLE guideplatform.subscriptions ADD CONSTRAINT subscriptions_create_user_id_fkey FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id");
ALTER TABLE guideplatform.subscriptions ADD CONSTRAINT subscriptions_update_user_id_fkey FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id");
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
CREATE INDEX idx_business_images_photo_url ON business_images(photo_url) WHERE photo_url IS NOT NULL;
CREATE INDEX idx_business_images_thumbnail_url ON business_images(thumbnail_url) WHERE thumbnail_url IS NOT NULL;
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
CREATE INDEX idx_banners_photo_url ON banners(photo_url) WHERE photo_url IS NOT NULL;
CREATE INDEX idx_banners_thumbnail_url ON banners(thumbnail_url) WHERE thumbnail_url IS NOT NULL;
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

INSERT INTO businesses (name, description, category_id, sub_category_id, province_id, countries_id, district_id, address, phone, mobile, email, website, facebook_url, instagram_url, whatsapp, telegram, latitude, longitude, rating, total_reviews, view_count, subscription_type, is_verified, is_featured, working_hours, icon, auth_user_id, owner_id, auth_customer_id, create_user_id, update_user_id) VALUES
('Aleppo Kebap House', 'Geleneksel Halep kebap ve mezelerinin sunulduğu restoran', '2a5ce345-d636-437f-9795-1144fcaf7665', null, '6ae34069-3dcf-461c-8c53-af9730699493', '129b3784-b9e7-4872-bdab-1666625890b6', 'dd9cd864-8c5f-47a6-a3f3-bdbb5b961477', 'Al-Bab Merkez Mah.', '+90 555 123 4567', '+90 555 123 4567', 'info@aleppokebap.com', 'www.aleppokebap.com', 'facebook.com/aleppokebap', 'instagram.com/aleppokebap', '+90 555 123 4567', '@aleppokebap', 36.37321, 37.51672, 4.50, 120, 1500, 2, true, true, '10:00-22:00', 'restaurant', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

('Manbij Lokantası', 'Suriye mutfağının en güzel örneklerini sunan lokanta', '2a5ce345-d636-437f-9795-1144fcaf7665', null, '6ae34069-3dcf-461c-8c53-af9730699493', 'a2db09e5-910e-4800-a226-68b70c43926e', '26610b3a-59bf-49df-953a-ca930d3f1464', 'Manbij Ana Caddesi', '+90 555 234 5678', '+90 555 234 5678', 'info@manbijlokanta.com', 'www.manbijlokanta.com', 'facebook.com/manbijlokanta', 'instagram.com/manbijlokanta', '+90 555 234 5678', '@manbijlokanta', 36.52815, 37.95495, 4.20, 85, 1200, 1, true, false, '09:00-23:00', 'restaurant', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

('Azaz Sofra', 'Ev yapımı Suriye yemekleri sunan aile restoranı', '2a5ce345-d636-437f-9795-1144fcaf7665', null, '6ae34069-3dcf-461c-8c53-af9730699493', 'a2db09e5-910e-4800-a226-68b70c43926e', '94b32542-3c72-45bb-ae5c-37a1b19dc5f3', 'Azaz Çarşı', '+90 555 345 6789', '+90 555 345 6789', 'info@azazsofra.com', 'www.azazsofra.com', 'facebook.com/azazsofra', 'instagram.com/azazsofra', '+90 555 345 6789', '@azazsofra', 36.58671, 37.04751, 4.00, 65, 800, 0, false, false, '11:00-21:00', 'restaurant', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

('Afrin Lezzet', 'Modern Suriye mutfağı konseptli restoran', '2a5ce345-d636-437f-9795-1144fcaf7665', null, '6ae34069-3dcf-461c-8c53-af9730699493', 'a2db09e5-910e-4800-a226-68b70c43926e', 'fae9f919-3e32-42f0-915f-adf23387a17a', 'Afrin Merkez', '+90 555 456 7890', '+90 555 456 7890', 'info@afrinlezzet.com', 'www.afrinlezzet.com', 'facebook.com/afrinlezzet', 'instagram.com/afrinlezzet', '+90 555 456 7890', '@afrinlezzet', 36.51231, 36.86982, 4.30, 95, 1100, 1, true, true, '10:00-22:00', 'restaurant', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

('Jarabulus Mutfak', 'Otantik Suriye lezzetleri ve tatlıları', '2a5ce345-d636-437f-9795-1144fcaf7665', null, '6ae34069-3dcf-461c-8c53-af9730699493', 'a2db09e5-910e-4800-a226-68b70c43926e', '9f83ef0a-48a3-4c4f-83ad-170efe61e8e3', 'Jarabulus Ana Yol', '+90 555 567 8901', '+90 555 567 8901', 'info@jarabulusmutfak.com', 'www.jarabulusmutfak.com', 'facebook.com/jarabulusmutfak', 'instagram.com/jarabulusmutfak', '+90 555 567 8901', '@jarabulusmutfak', 36.81752, 38.01165, 4.10, 75, 950, 0, false, false, '09:00-21:00', 'restaurant', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');
-- =====================================================
-- END OF OPTIMIZED DATABASE
-- =====================================================

-- SAMPLE DATA - البيانات النموذجية

-- business_contacts table için örnek veriler - بيانات نموذجية لجدول معلومات الاتصال
INSERT INTO business_contacts (business_id, contact_type, contact_value, is_primary, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES

-- Aleppo Kebap House için iletişim bilgileri - معلومات الاتصال لمطعم حلب كباب هاوس
('978bea96-ec7f-461b-aefa-deaac61df09e', 'phone', '+90 555 123 4567', true, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'phone', '+90 555 123 4567', false, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'email', 'info@aleppokebap.com', false, 'contact_mail', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'website', 'www.aleppokebap.com', false, 'contact_web', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'facebook', 'facebook.com/aleppokebap', false, 'contact_facebook', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'instagram', 'instagram.com/aleppokebap', false, 'contact_instagram', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'whatsapp', '+90 555 123 4567', false, 'contact_whatsapp', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'twitter', '@aleppokebap', false, 'contact_twitter', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Manbij Lokantası için iletişim bilgileri - معلومات الاتصال لمطعم منبج لوقنتاسي
('005542a0-ce29-4aa9-b71e-442dd007de67', 'phone', '+90 555 234 5678', true, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'phone', '+90 555 234 5678', false, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'email', 'info@manbijlokanta.com', false, 'contact_mail', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'website', 'www.manbijlokanta.com', false, 'contact_web', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'facebook', 'facebook.com/manbijlokanta', false, 'contact_facebook', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'instagram', 'instagram.com/manbijlokanta', false, 'contact_instagram', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'whatsapp', '+90 555 234 5678', false, 'contact_whatsapp', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'twitter', '@manbijlokanta', false, 'contact_twitter', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Azaz Sofra için iletişim bilgileri - معلومات الاتصال لمطعم أعزاز سفرة
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'phone', '+90 555 345 6789', true, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'phone', '+90 555 345 6789', false, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'email', 'info@azazsofra.com', false, 'contact_mail', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'website', 'www.azazsofra.com', false, 'contact_web', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'facebook', 'facebook.com/azazsofra', false, 'contact_facebook', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'instagram', 'instagram.com/azazsofra', false, 'contact_instagram', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'whatsapp', '+90 555 345 6789', false, 'contact_whatsapp', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'twitter', '@azazsofra', false, 'contact_twitter', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Afrin Lezzet için iletişim bilgileri - معلومات الاتصال لمطعم عفرين لذة
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'phone', '+90 555 456 7890', true, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'phone', '+90 555 456 7890', false, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'email', 'info@afrinlezzet.com', false, 'contact_mail', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'website', 'www.afrinlezzet.com', false, 'contact_web', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'facebook', 'facebook.com/afrinlezzet', false, 'contact_facebook', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'instagram', 'instagram.com/afrinlezzet', false, 'contact_instagram', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'whatsapp', '+90 555 456 7890', false, 'contact_whatsapp', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'twitter', '@afrinlezzet', false, 'contact_twitter', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Jarabulus Mutfak için iletişim bilgileri - معلومات الاتصال لمطعم جرابلس مطبخ
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'phone', '+90 555 567 8901', true, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'phone', '+90 555 567 8901', false, 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'email', 'info@jarabulusmutfak.com', false, 'contact_mail', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'website', 'www.jarabulusmutfak.com', false, 'contact_web', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'facebook', 'facebook.com/jarabulusmutfak', false, 'contact_facebook', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'instagram', 'instagram.com/jarabulusmutfak', false, 'contact_instagram', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'whatsapp', '+90 555 567 8901', false, 'contact_whatsapp', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'twitter', '@jarabulusmutfak', false, 'contact_twitter', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- business_contacts için örnek veri ekleme tamamlandı - تم إضافة البيانات النموذجية لجدول معلومات الاتصال

-- business_services table için örnek veriler - بيانات نموذجية لجدول خدمات الأعمال
INSERT INTO business_services (business_id, service_name, service_description, price, currency, is_available, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES

-- Aleppo Kebap House için hizmetler - خدمات مطعم حلب كباب هاوس
('978bea96-ec7f-461b-aefa-deaac61df09e', 'Halep Kebap', 'Geleneksel Halep usulü kebap, özel baharatlarla marine edilmiş et', 45.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'Hummus', 'Nohut püresi, tahin, zeytinyağı ve limon suyu ile', 15.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'Baba Ganoush', 'Patlıcan püresi, tahin ve baharatlarla', 18.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'Adana Kebap', 'Kıyma et, baharatlar ve soğan ile hazırlanan kebap', 42.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'Ev Yapımı Ekmek', 'Geleneksel Suriye ekmeği, sıcak servis', 5.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'Paket Servis', 'Ev ve işyeri adreslerine teslimat hizmeti', 8.00, 'TRY', true, 'delivery_dining', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Manbij Lokantası için hizmetler - خدمات مطعم منبج لوقنتاسي
('005542a0-ce29-4aa9-b71e-442dd007de67', 'Suriye Kahvaltısı', 'Zeytin, peynir, domates, salatalık ve bal ile', 25.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'Kuzu Pirzola', 'Marine edilmiş kuzu pirzola, özel sos ile', 55.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'Falafel', 'Nohut ve baharatlarla hazırlanan kızartma', 20.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'Mercimek Çorbası', 'Geleneksel Suriye mercimek çorbası', 18.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'Tavuk Şiş', 'Marine edilmiş tavuk eti, sebzelerle', 35.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'Rezervasyon', 'Özel günler için masa rezervasyonu', 0.00, 'TRY', true, 'event_available', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Azaz Sofra için hizmetler - خدمات مطعم أعزاز سفرة
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'Aile Menüsü', '4 kişilik aile paketi, çeşitli mezeler ve ana yemek', 120.00, 'TRY', true, 'family_restroom', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'Ev Yapımı Yoğurt', 'Geleneksel Suriye yoğurdu, taze ve doğal', 12.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'Kısır', 'Bulgur, domates, salatalık ve baharatlarla', 16.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'Tavuk Sote', 'Sebzelerle sote edilmiş tavuk eti', 28.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'Çay Servisi', 'Geleneksel Suriye çayı, sıcak servis', 8.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'Toplu Sipariş', 'Kurum ve organizasyonlar için toplu yemek', 0.00, 'TRY', true, 'group', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Afrin Lezzet için hizmetler - خدمات مطعم عفرين لذة
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'Modern Suriye Menüsü', 'Çağdaş sunum teknikleriyle hazırlanan geleneksel lezzetler', 85.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'Deniz Mahsülleri', 'Taze balık ve deniz ürünleri, özel soslarla', 65.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'Vejetaryen Menü', 'Sebze ağırlıklı, protein açısından zengin yemekler', 45.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'Özel Gün Menüsü', 'Doğum günü, evlilik teklifi gibi özel anlar için', 150.00, 'TRY', true, 'celebration', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'Şef Özel Menüsü', 'Şefin günlük önerileri, mevsimlik malzemelerle', 95.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'Wine Pairing', 'Yemeklerle uyumlu şarap önerileri', 35.00, 'TRY', true, 'wine_bar', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Jarabulus Mutfak için hizmetler - خدمات مطعم جرابلس مطبخ
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'Otantik Suriye Menüsü', 'Geleneksel tariflerle hazırlanan otantik lezzetler', 75.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'Tatlı Menüsü', 'Baklava, künefe ve diğer geleneksel tatlılar', 25.00, 'TRY', true, 'cake', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'Çorba Çeşitleri', 'Günlük çorba önerileri, taze malzemelerle', 20.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'Izgara Köfte', 'Özel baharatlarla hazırlanan ızgara köfte', 32.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'Salata Çeşitleri', 'Taze sebzeler, zeytinyağı ve limon sosu ile', 18.00, 'TRY', true, 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'Kahve Servisi', 'Geleneksel Suriye kahvesi, Türk kahvesi tarzında', 12.00, 'TRY', true, 'coffee', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- business_services için örnek veri ekleme tamamlandı - تم إضافة البيانات النموذجية لجدول خدمات الأعمال

-- business_working_hours table için örnek veriler - بيانات نموذجية لجدول ساعات العمل
INSERT INTO business_working_hours (business_id, day_of_week, open_time, close_time, is_closed, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES

-- Aleppo Kebap House için çalışma saatleri - ساعات عمل مطعم حلب كباب هاوس
('978bea96-ec7f-461b-aefa-deaac61df09e', 1, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 2, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 3, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 4, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 5, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 6, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 7, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Manbij Lokantası için çalışma saatleri - ساعات عمل مطعم منبج لوقنتاسي
('005542a0-ce29-4aa9-b71e-442dd007de67', 1, '09:00:00', '23:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 2, '09:00:00', '23:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 3, '09:00:00', '23:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 4, '09:00:00', '23:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 5, '09:00:00', '23:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 6, '09:00:00', '23:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 7, '09:00:00', '23:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Azaz Sofra için çalışma saatleri - ساعات عمل مطعم أعزاز سفرة
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 1, '11:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 2, '11:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 3, '11:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 4, '11:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 5, '11:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 6, '11:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 7, '11:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Afrin Lezzet için çalışma saatleri - ساعات عمل مطعم عفرين لذة
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 1, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 2, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 3, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 4, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 5, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 6, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 7, '10:00:00', '22:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Jarabulus Mutfak için çalışma saatleri - ساعات عمل مطعم جرابلس مطبخ
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 1, '09:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 2, '09:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 3, '09:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 4, '09:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 5, '09:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 6, '09:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 7, '09:00:00', '21:00:00', false, 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- business_working_hours için örnek veri ekleme tamamlandı - تم إضافة البيانات النموذجية لجدول ساعات العمل

-- business_reviews table için örnek veriler - بيانات نموذجية لجدول تقييمات الأعمال
INSERT INTO business_reviews (business_id, reviewer_id, rating, comment, is_verified, is_approved, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES

-- Aleppo Kebap House için değerlendirmeler - تقييمات مطعم حلب كباب هاوس
('978bea96-ec7f-461b-aefa-deaac61df09e', '19a8b428-a57e-4a24-98e3-470258d3d83e', 5, 'Harika bir deneyim! Halep kebap gerçekten çok lezzetliydi. Servis hızlı ve personel çok nazik. Kesinlikle tekrar geleceğim.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Çok güzel bir restoran. Yemekler taze ve lezzetli. Fiyatlar da makul. Sadece biraz kalabalık olabiliyor.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '129b3784-b9e7-4872-bdab-1666625890b6', 5, 'Suriye mutfağının en güzel örneklerini burada buldum. Hummus ve baba ganoush muhteşemdi!', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Güzel bir atmosfer ve lezzetli yemekler. Adana kebap özellikle çok iyiydi.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '129b3784-b9e7-4872-bdab-1666625890b6', 3, 'Yemekler iyiydi ama servis biraz yavaştı. Belki çok kalabalık olduğu için olabilir.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Manbij Lokantası için değerlendirmeler - تقييمات مطعم منبج لوقنتاسي
('005542a0-ce29-4aa9-b71e-442dd007de67', '19a8b428-a57e-4a24-98e3-470258d3d83e', 4, 'Çok güzel bir lokanta. Suriye mutfağının en güzel örneklerini sunuyor. Kuzu pirzola harikaydı!', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '129b3784-b9e7-4872-bdab-1666625890b6', 5, 'Mükemmel! Hem yemekler hem de servis çok iyiydi. Falafel özellikle çok lezzetliydi.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Güzel bir deneyimdi. Mercimek çorbası çok lezzetliydi. Personel de çok ilgiliydi.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '129b3784-b9e7-4872-bdab-1666625890b6', 3, 'Yemekler iyiydi ama biraz pahalı. Kalite fiyata değer mi emin değilim.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Genel olarak memnun kaldım. Tavuk şiş çok lezzetliydi. Atmosfer de güzeldi.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Azaz Sofra için değerlendirmeler - تقييمات مطعم أعزاز سفرة
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '19a8b428-a57e-4a24-98e3-470258d3d83e', 4, 'Aile restoranı olarak çok güzel. Yemekler ev yapımı gibi lezzetli. Aile menüsü çok uygun fiyatlı.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '129b3784-b9e7-4872-bdab-1666625890b6', 3, 'Yemekler iyiydi ama servis biraz yavaştı. Belki aile restoranı olduğu için normal olabilir.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Çok güzel bir yer. Ev yapımı yoğurt harikaydı. Kısır da çok lezzetliydi.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '129b3784-b9e7-4872-bdab-1666625890b6', 5, 'Mükemmel! Hem yemekler hem de atmosfer çok güzeldi. Ailemle birlikte çok keyifli vakit geçirdik.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Güzel bir deneyimdi. Tavuk sote çok lezzetliydi. Fiyatlar da makul.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Afrin Lezzet için değerlendirmeler - تقييمات مطعم عفرين لذة
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '19a8b428-a57e-4a24-98e3-470258d3d83e', 5, 'Modern Suriye mutfağının en güzel örneklerini burada buldum. Sunum teknikleri harika!', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Çok güzel bir konsept. Deniz mahsülleri özellikle çok lezzetliydi. Şarap önerileri de çok iyiydi.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '129b3784-b9e7-4872-bdab-1666625890b6', 5, 'Mükemmel! Vejetaryen menü çok çeşitli ve lezzetliydi. Şef özel menüsü de harikaydı.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Güzel bir deneyimdi. Özel gün menüsü çok lezzetliydi. Atmosfer de çok romantikti.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '129b3784-b9e7-4872-bdab-1666625890b6', 3, 'Yemekler iyiydi ama biraz pahalı. Kalite iyi ama fiyat performans oranı biraz düşük.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Jarabulus Mutfak için değerlendirmeler - تقييمات مطعم جرابلس مطبخ
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '19a8b428-a57e-4a24-98e3-470258d3d83e', 4, 'Otantik Suriye lezzetlerini çok güzel sunuyor. Tatlı menüsü özellikle harikaydı!', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '129b3784-b9e7-4872-bdab-1666625890b6', 3, 'Yemekler iyiydi ama çorba çeşitleri biraz azdı. Izgara köfte çok lezzetliydi.', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Güzel bir atmosfer ve lezzetli yemekler. Salata çeşitleri çok taze ve lezzetliydi.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '129b3784-b9e7-4872-bdab-1666625890b6', 5, 'Mükemmel! Hem yemekler hem de kahve servisi çok iyiydi. Baklava özellikle harikaydı!', true, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '129b3784-b9e7-4872-bdab-1666625890b6', 4, 'Çok güzel bir deneyimdi. Geleneksel Suriye kahvesi harikaydı. Atmosfer de çok sıcaktı.', false, true, 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- business_reviews için örnek veri ekleme tamamlandı - تم إضافة البيانات النموذجية لجدول تقييمات الأعمال

-- user_favorites table için örnek veriler - بيانات نموذجية لجدول المفضلات
INSERT INTO user_favorites (business_id, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES

-- Kullanıcıların favori restoranları - المطاعم المفضلة للمستخدمين
('978bea96-ec7f-461b-aefa-deaac61df09e', 'favorite', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'favorite_border', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', 'favorite', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

('005542a0-ce29-4aa9-b71e-442dd007de67', 'favorite', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'favorite', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', 'favorite_border', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'favorite', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'favorite', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'favorite_border', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'favorite', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'favorite', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'favorite', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'favorite_border', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'favorite', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'favorite', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- user_favorites için örnek veri ekleme tamamlandı - تم إضافة البيانات النموذجية لجدول المفضلات

-- user_visits table için örnek veriler - بيانات نموذجية لجدول زيارات المستخدمين
INSERT INTO user_visits (business_id, visit_date, visit_type, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES

-- Aleppo Kebap House için kullanıcı ziyaretleri - زيارات المستخدمين لمطعم حلب كباب هاوس
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-15 14:30:00+03:00', 'view', 'visibility', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-16 19:45:00+03:00', 'contact', 'contact_phone', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-17 12:15:00+03:00', 'review', 'rate_review', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-18 20:00:00+03:00', 'view', 'visibility', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-19 16:30:00+03:00', 'contact', 'contact_mail', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Manbij Lokantası için kullanıcı ziyaretleri - زيارات المستخدمين لمطعم منبج لوقنتاسي
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-14 11:20:00+03:00', 'view', 'visibility', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-15 18:15:00+03:00', 'contact', 'contact_phone', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-16 13:45:00+03:00', 'view', 'visibility', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-17 21:30:00+03:00', 'review', 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-18 15:00:00+03:00', 'contact', 'contact_web', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Azaz Sofra için kullanıcı ziyaretleri - زيارات المستخدمين لمطعم أعزاز سفرة
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-13 10:45:00+03:00', 'view', 'visibility', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-14 17:30:00+03:00', 'view', 'visibility', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-15 12:00:00+03:00', 'contact', 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-16 19:15:00+03:00', 'review', 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-17 14:45:00+03:00', 'view', 'visibility', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Afrin Lezzet için kullanıcı ziyaretleri - زيارات المستخدمين لمطعم عفرين لذة
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-12 16:20:00+03:00', 'view', 'visibility', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-13 20:30:00+03:00', 'contact', 'contact_phone', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-14 13:15:00+03:00', 'view', 'visibility', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-15 18:45:00+03:00', 'review', 'rate_review', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-16 11:30:00+03:00', 'contact', 'contact_mail', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Jarabulus Mutfak için kullanıcı ziyaretleri - زيارات المستخدمين لمطعم جرابلس مطبخ
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-11 14:00:00+03:00', 'view', 'visibility', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-12 19:00:00+03:00', 'view', 'visibility', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-13 15:30:00+03:00', 'contact', 'contact_phone', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-14 12:15:00+03:00', 'review', 'rate_review', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-15 17:45:00+03:00', 'view', 'visibility', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- user_visits için örnek veri ekleme tamamlandı - تم إضافة البيانات النموذجية لجدول زيارات المستخدمين

-- notifications table için örnek veriler - بيانات نموذجية لجدول الإشعارات
INSERT INTO notifications (recipient_user_id, title, message, notification_type, is_read, read_date, action_url, related_entity_id, related_entity_type, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES

-- Aleppo Kebap House için bildirimler - إشعارات مطعم حلب كباب هاوس
('129b3784-b9e7-4872-bdab-1666625890b6', 'Yeni Değerlendirme', 'Aleppo Kebap House için yeni bir değerlendirme eklendi. 5 yıldız aldı!', 'info', false, NULL, '/businesses/978bea96-ec7f-461b-aefa-deaac61df09e', '978bea96-ec7f-461b-aefa-deaac61df09e', 'business', 'rate_review', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Çalışma Saatleri Güncellendi', 'Aleppo Kebap House çalışma saatleri güncellendi. Artık 10:00-22:00 arası açık.', 'success', true, '2025-01-20 09:15:00+03:00', '/businesses/978bea96-ec7f-461b-aefa-deaac61df09e', '978bea96-ec7f-461b-aefa-deaac61df09e', 'business', 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Yeni Hizmet Eklendi', 'Aleppo Kebap House menüsüne yeni hizmetler eklendi. Halep Kebap artık mevcut!', 'info', false, NULL, '/businesses/978bea96-ec7f-461b-aefa-deaac61df09e/services', '978bea96-ec7f-461b-aefa-deaac61df09e', 'business', 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Manbij Lokantası için bildirimler - إشعارات مطعم منبج لوقنتاسي
('129b3784-b9e7-4872-bdab-1666625890b6', 'Rezervasyon Onayı', 'Manbij Lokantası için yaptığınız rezervasyon onaylandı. 20 Ocak 19:00 için masa hazır.', 'success', true, '2025-01-19 14:30:00+03:00', '/businesses/005542a0-ce29-4aa9-b71e-442dd007de67', '005542a0-ce29-4aa9-b71e-442dd007de67', 'business', 'event_available', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Özel Menü Duyurusu', 'Manbij Lokantası özel Suriye menüsü sunuyor. Bu hafta sonu %20 indirim!', 'info', false, NULL, '/businesses/005542a0-ce29-4aa9-b71e-442dd007de67', '005542a0-ce29-4aa9-b71e-442dd007de67', 'business', 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Yeni İletişim Bilgisi', 'Manbij Lokantası WhatsApp numarası güncellendi. Artık +90 555 234 5678 üzerinden ulaşabilirsiniz.', 'info', false, NULL, '/businesses/005542a0-ce29-4aa9-b71e-442dd007de67/contact', '005542a0-ce29-4aa9-b71e-442dd007de67', 'business', 'contact_whatsapp', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Azaz Sofra için bildirimler - إشعارات مطعم أعزاز سفرة
('129b3784-b9e7-4872-bdab-1666625890b6', 'Aile Menüsü Hatırlatması', 'Azaz Sofra aile menüsü için rezervasyon yapmayı unutmayın. 4 kişilik paket sadece 120 TL!', 'warning', false, NULL, '/businesses/f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'business', 'family_restroom', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Yeni Çalışma Saatleri', 'Azaz Sofra artık 11:00-21:00 arası açık. Hafta sonu da aynı saatlerde hizmet veriyor.', 'info', true, '2025-01-18 16:45:00+03:00', '/businesses/f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'business', 'schedule', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Toplu Sipariş İndirimi', 'Azaz Sofra toplu siparişlerde %15 indirim sunuyor. 10 kişi ve üzeri siparişlerde geçerli.', 'success', false, NULL, '/businesses/f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'f60578b0-ca95-4b3f-b2e9-b66679e8f88c', 'business', 'group', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Afrin Lezzet için bildirimler - إشعارات مطعم عفرين لذة
('129b3784-b9e7-4872-bdab-1666625890b6', 'Özel Gün Menüsü', 'Afrin Lezzet özel günler için özel menü sunuyor. Doğum günü, evlilik teklifi gibi özel anlar için rezervasyon yapın.', 'info', false, NULL, '/businesses/f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'business', 'celebration', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Wine Pairing Etkinliği', 'Afrin Lezzet şarap eşleştirme etkinliği düzenliyor. 25 Ocak Cumartesi 20:00. Sınırlı kontenjan!', 'success', false, NULL, '/businesses/f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'business', 'wine_bar', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Vejetaryen Menü Güncellendi', 'Afrin Lezzet vejetaryen menüsü güncellendi. Yeni sebze yemekleri ve protein açısından zengin seçenekler eklendi.', 'info', true, '2025-01-17 12:20:00+03:00', '/businesses/f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'f76e31c4-3e4e-4a58-a5bb-3a57597603f6', 'business', 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Jarabulus Mutfak için bildirimler - إشعارات مطعم جرابلس مطبخ
('129b3784-b9e7-4872-bdab-1666625890b6', 'Tatlı Menüsü Genişletildi', 'Jarabulus Mutfak tatlı menüsü genişletildi. Yeni baklava çeşitleri ve künefe seçenekleri eklendi.', 'info', false, NULL, '/businesses/7fb6d642-528b-4dcd-8e58-785b7c9542f4', '7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'business', 'cake', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Kahve Servisi Hatırlatması', 'Jarabulus Mutfak geleneksel Suriye kahvesi servisi sunuyor. Türk kahvesi tarzında hazırlanan özel karışım.', 'warning', false, NULL, '/businesses/7fb6d642-528b-4dcd-8e58-785b7c9542f4', '7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'business', 'coffee', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 'Çorba Çeşitleri Güncellendi', 'Jarabulus Mutfak günlük çorba önerileri güncellendi. Her gün farklı çorba çeşidi sunuluyor.', 'info', true, '2025-01-16 18:10:00+03:00', '/businesses/7fb6d642-528b-4dcd-8e58-785b7c9542f4', '7fb6d642-528b-4dcd-8e58-785b7c9542f4', 'business', 'restaurant_menu', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');

-- notifications için örnek veri ekleme tamamlandı - تم إضافة البيانات النموذجية لجدول الإشعارات

-- notification_settings table için örnek veriler - بيانات نموذجية لجدول إعدادات الإشعارات
INSERT INTO notification_settings (user_id, setting_type, is_enabled, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES

-- Ana kullanıcı için bildirim ayarları - إعدادات الإشعارات للمستخدم الرئيسي
('19a8b428-a57e-4a24-98e3-470258d3d83e', 0, true, 'mail', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('19a8b428-a57e-4a24-98e3-470258d3d83e', 1, true, 'notifications_active', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('19a8b428-a57e-4a24-98e3-470258d3d83e', 2, false, 'sms', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Diğer kullanıcılar için bildirim ayarları - إعدادات الإشعارات للمستخدمين الآخرين
('129b3784-b9e7-4872-bdab-1666625890b6', 0, true, 'mail', '129b3784-b9e7-4872-bdab-1666625890b6', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '129b3784-b9e7-4872-bdab-1666625890b6', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 1, true, 'notifications_active', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('129b3784-b9e7-4872-bdab-1666625890b6', 2, true, 'sms', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e')
-- notification_settings için örnek veri ekleme tamamlandı - تم إضافة البيانات النموذجية لجدول إعدادات الإشعارات



-- Sample data for business_analytics - بيانات نموذجية لتحليلات الأعمال
INSERT INTO business_analytics (business_id, date, views_count, contacts_count, reviews_count, favorites_count, icon, auth_user_id, auth_customer_id, create_user_id, update_user_id) VALUES
-- Aleppo Kebap House - تحليلات مطعم حلب كباب هاوس
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-01', 156, 23, 8, 45, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-02', 189, 31, 12, 52, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-03', 234, 28, 15, 67, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-04', 198, 35, 9, 58, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('978bea96-ec7f-461b-aefa-deaac61df09e', '2025-01-05', 267, 42, 18, 73, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Manbij Lokantası - تحليلات مطعم منبج
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-01', 98, 15, 6, 28, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-02', 124, 22, 8, 35, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-03', 156, 19, 11, 42, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-04', 112, 26, 7, 31, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('005542a0-ce29-4aa9-b71e-442dd007de67', '2025-01-05', 189, 33, 14, 48, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Azaz Sofra - تحليلات مطعم أعزاز سفرة
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-01', 67, 8, 3, 15, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-02', 89, 12, 5, 22, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-03', 112, 16, 7, 28, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-04', 78, 9, 4, 18, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f60578b0-ca95-4b3f-b2e9-b66679e8f88c', '2025-01-05', 134, 21, 9, 35, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Afrin Lezzet - تحليلات مطعم عفرين لذة
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-01', 145, 19, 7, 38, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-02', 178, 25, 10, 45, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-03', 203, 31, 13, 52, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-04', 167, 22, 8, 41, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('f76e31c4-3e4e-4a58-a5bb-3a57597603f6', '2025-01-05', 234, 38, 16, 58, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),

-- Jarabulus Mutfak - تحليلات مطعم جرابلس مطبخ
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-01', 89, 12, 4, 23, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-02', 112, 18, 6, 29, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-03', 145, 24, 9, 36, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-04', 98, 14, 5, 25, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e'),
('7fb6d642-528b-4dcd-8e58-785b7c9542f4', '2025-01-05', 167, 28, 11, 42, 'analytics', '19a8b428-a57e-4a24-98e3-470258d3d83e', '72c54b1a-8e1c-45ea-8edd-b5da1091e325', '19a8b428-a57e-4a24-98e3-470258d3d83e', '19a8b428-a57e-4a24-98e3-470258d3d83e');


{
  "name": "Raqqa Mutfak Evi",
  "description": "Rakka bölgesinin geleneksel lezzetlerini sunan ev yapımı restoran",
  "categoryId": "2a5ce345-d636-437f-9795-1144fcaf7665",
  "subCategoryId": null,
  "provinceId": "6ae34069-3dcf-461c-8c53-af9730699493",
  "countriesId": "a2db09e5-910e-4800-a226-68b70c43926e",
  "districtId": "dd9cd864-8c5f-47a6-a3f3-bdbb5b961477",
  "address": "Raqqa Çarşı Mahallesi No:8",
  "phone": "+90 555 678 9012",
  "mobile": "+90 555 678 9012",
  "email": "info@raqqamutfak.com",
  "website": "www.raqqamutfak.com",
  "facebookUrl": "facebook.com/raqqamutfak",
  "instagramUrl": "instagram.com/raqqamutfak",
  "whatsapp": "+90 555 678 9012",
  "telegram": "@raqqamutfak",
  "latitude": 35.94944,
  "longitude": 39.00944,
  "rating": 4.4,
  "totalReviews": 110,
  "viewCount": 1350,
  "subscriptionType": 1,
  "isVerified": true,
  "isFeatured": false,
  "workingHours": "08:00-22:00",
  "icon": "restaurant",
  "authUserId": "19a8b428-a57e-4a24-98e3-470258d3d83e",
  "ownerId": "19a8b428-a57e-4a24-98e3-470258d3d83e",
  "authCustomerId": "72c54b1a-8e1c-45ea-8edd-b5da1091e325"
}

-- =====================================================
-- COLUMN COMMENTS - تعليقات الأعمدة
-- =====================================================

-- business_images table comments
COMMENT ON COLUMN guideplatform.business_images.photo IS 'Fotoğraf verisi (bytea) - Photo data (bytea) - Eski sistem için korunuyor';
COMMENT ON COLUMN guideplatform.business_images.thumbnail IS 'Küçük resim verisi (bytea) - Thumbnail data (bytea) - Eski sistem için korunuyor';
COMMENT ON COLUMN guideplatform.business_images.photo_url IS 'Fotoğraf URL''si - Photo URL - Yeni sistem için wwwroot''ta saklanan dosya yolu';
COMMENT ON COLUMN guideplatform.business_images.thumbnail_url IS 'Küçük resim URL''si - Thumbnail URL - Yeni sistem için wwwroot''ta saklanan dosya yolu';
COMMENT ON COLUMN guideplatform.business_images.photo_content_type IS 'Fotoğraf içerik tipi - Photo content type (image/jpeg, image/png, vb.)';
COMMENT ON COLUMN guideplatform.business_images.alt_text IS 'Alternatif metin - Alt text for accessibility';
COMMENT ON COLUMN guideplatform.business_images.image_type IS 'Resim tipi - Image type (0:profile, 1:gallery, 2:menu, 3:banner, 4:logo, 5:interior, 6:exterior, 7:food, 8:kitchen, 9:atmosphere, 10:design, 11:dessert)';
COMMENT ON COLUMN guideplatform.business_images.is_primary IS 'Ana fotoğraf mı - Is primary image';
COMMENT ON COLUMN guideplatform.business_images.sort_order IS 'Sıralama düzeni - Sort order';
COMMENT ON COLUMN guideplatform.business_images.icon IS 'İkon - Icon name';

-- banners table comments
COMMENT ON COLUMN guideplatform.banners.photo IS 'Fotoğraf verisi (bytea) - Photo data (bytea) - Eski sistem için korunuyor';
COMMENT ON COLUMN guideplatform.banners.thumbnail IS 'Küçük resim verisi (bytea) - Thumbnail data (bytea) - Eski sistem için korunuyor';
COMMENT ON COLUMN guideplatform.banners.photo_url IS 'Fotoğraf URL''si - Photo URL - Yeni sistem için wwwroot''ta saklanan dosya yolu';
COMMENT ON COLUMN guideplatform.banners.thumbnail_url IS 'Küçük resim URL''si - Thumbnail URL - Yeni sistem için wwwroot''ta saklanan dosya yolu';
COMMENT ON COLUMN guideplatform.banners.photo_content_type IS 'Fotoğraf içerik tipi - Photo content type (image/jpeg, image/png, vb.)';
COMMENT ON COLUMN guideplatform.banners.title IS 'Banner başlığı - Banner title';
COMMENT ON COLUMN guideplatform.banners.description IS 'Banner açıklaması - Banner description';
COMMENT ON COLUMN guideplatform.banners.link_url IS 'Banner link URL''si - Banner link URL';
COMMENT ON COLUMN guideplatform.banners.start_date IS 'Başlangıç tarihi - Start date';
COMMENT ON COLUMN guideplatform.banners.end_date IS 'Bitiş tarihi - End date';
COMMENT ON COLUMN guideplatform.banners.is_active IS 'Aktif mi - Is active';
COMMENT ON COLUMN guideplatform.banners.order_index IS 'Sıralama indeksi - Order index';

-- =====================================================
-- JOB SEEKERS TABLE - İş Arayanlar
-- =====================================================
CREATE TABLE job_seekers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID, -- İşletme referansı
    full_name VARCHAR(255) NOT NULL, -- İş arayanın tam adı
    description TEXT, -- İş arayan açıklaması
    phone VARCHAR(20), -- Telefon numarası
    is_sponsored BOOLEAN DEFAULT false, -- Sponsorlu ilan mı
    province_id UUID,
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (province_id) REFERENCES storeplatformh."provinces"("id"),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- =====================================================
-- JOB OPPORTUNITIES TABLE - İş İmkanları
-- =====================================================
CREATE TABLE job_opportunities (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    business_id UUID NOT NULL, -- İşletme referansı
    title VARCHAR(255) NOT NULL, -- İş ilanı başlığı
    description TEXT NOT NULL, -- İş tanımı
    phone VARCHAR(20), -- İletişim telefonu
    duration INT4 DEFAULT 0, -- İlan süresi (gün cinsinden)
    is_sponsored BOOLEAN DEFAULT false, -- Sponsorlu ilan mı
    province_id UUID,
    row_created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_updated_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    row_is_active BOOLEAN DEFAULT true NOT NULL,
    row_is_deleted BOOLEAN DEFAULT false NOT NULL,
    auth_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    auth_customer_id UUID, -- Reference to auth."Customers"."Id"
    create_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    update_user_id UUID, -- Reference to auth."AspNetUsers"."Id"
    FOREIGN KEY (business_id) REFERENCES businesses(id) ON DELETE CASCADE,
    FOREIGN KEY (province_id) REFERENCES storeplatformh."provinces"("id"),
    FOREIGN KEY (auth_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (auth_customer_id) REFERENCES auth."Customers"("Id"),
    FOREIGN KEY (create_user_id) REFERENCES auth."AspNetUsers"("Id"),
    FOREIGN KEY (update_user_id) REFERENCES auth."AspNetUsers"("Id")
);

-- =====================================================
-- INDEXES FOR JOB TABLES - İş Tabloları İndeksleri
-- =====================================================

-- job_seekers indexes
CREATE INDEX idx_job_seekers_business_id ON job_seekers(business_id);
CREATE INDEX idx_job_seekers_auth_user_id ON job_seekers(auth_user_id);
CREATE INDEX idx_job_seekers_active ON job_seekers(row_is_active, row_is_deleted);
CREATE INDEX idx_job_seekers_sponsored ON job_seekers(is_sponsored);

-- job_opportunities indexes
CREATE INDEX idx_job_opportunities_business_id ON job_opportunities(business_id);
CREATE INDEX idx_job_opportunities_auth_user_id ON job_opportunities(auth_user_id);
CREATE INDEX idx_job_opportunities_active ON job_opportunities(row_is_active, row_is_deleted);
CREATE INDEX idx_job_opportunities_sponsored ON job_opportunities(is_sponsored);

-- =====================================================
-- COMMENTS FOR JOB TABLES - İş Tabloları Yorumları
-- =====================================================

-- job_seekers table comments
COMMENT ON TABLE job_seekers IS 'İş arayanların bilgilerini saklayan tablo - Table storing job seekers information';
COMMENT ON COLUMN job_seekers.business_id IS 'İşletme referansı - Business reference';
COMMENT ON COLUMN job_seekers.full_name IS 'İş arayanın tam adı - Job seeker full name';
COMMENT ON COLUMN job_seekers.description IS 'İş arayan açıklaması - Job seeker description';
COMMENT ON COLUMN job_seekers.phone IS 'Telefon numarası - Phone number';
COMMENT ON COLUMN job_seekers.is_sponsored IS 'Sponsorlu ilan mı - Is sponsored advertisement';
COMMENT ON COLUMN job_seekers.province_id IS 'İl referansı - Province reference';

-- job_opportunities table comments
COMMENT ON TABLE job_opportunities IS 'İş ilanlarını saklayan tablo - Table storing job opportunities';
COMMENT ON COLUMN job_opportunities.business_id IS 'İşletme referansı - Business reference';
COMMENT ON COLUMN job_opportunities.title IS 'İş ilanı başlığı - Job title';
COMMENT ON COLUMN job_opportunities.description IS 'İş tanımı - Job description';
COMMENT ON COLUMN job_opportunities.phone IS 'İletişim telefonu - Contact phone';
COMMENT ON COLUMN job_opportunities.duration IS 'İlan süresi (gün) - Advertisement duration (days)';
COMMENT ON COLUMN job_opportunities.is_sponsored IS 'Sponsorlu ilan mı - Is sponsored advertisement';
COMMENT ON COLUMN job_opportunities.province_id IS 'İl referansı - Province reference';

-- =====================================================
-- SAMPLE DATA FOR JOB TABLES - İş Tabloları Örnek Verileri
-- =====================================================

-- Sample data for job_seekers
INSERT INTO job_seekers (
    business_id, 
    full_name, 
    description, 
    phone, 
    is_sponsored, 
    province_id, 
    auth_user_id, 
    auth_customer_id, 
    create_user_id, 
    update_user_id
) VALUES 
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Ahmed Hassan',
    'Merhaba, ben 25 yaşında genç bir adamım ve Mersin''de yaşıyorum. Türkçe ve İngilizce konuşabiliyorum. İş arıyorum.',
    '+90 552 123 4567',
    true, -- Sponsored
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Fatma Yılmaz',
    'Deneyimli muhasebeci. 5 yıl deneyim. Excel ve SAP biliyorum. Tam zamanlı iş arıyorum.',
    '+90 532 987 6543',
    false,
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Mehmet Özkan',
    'Grafik tasarımcı. Adobe programları kullanıyorum. Freelance veya tam zamanlı çalışabilirim.',
    '+90 505 456 7890',
    true, -- Sponsored
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Ayşe Demir',
    'İngilizce öğretmeni. Üniversite mezunu. Özel ders veya okul pozisyonu arıyorum.',
    '+90 543 234 5678',
    false,
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Omar Al-Rashid',
    'Yazılım geliştirici. React, Node.js, Python biliyorum. Remote çalışmayı tercih ediyorum.',
    '+90 555 678 9012',
    false,
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Sample data for job_opportunities
INSERT INTO job_opportunities (
    business_id, 
    title, 
    description, 
    phone, 
    duration, 
    is_sponsored, 
    province_id, 
    auth_user_id, 
    auth_customer_id, 
    create_user_id, 
    update_user_id
) VALUES 
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Satış Danışmanı',
    'Deneyimli satış danışmanı aranıyor. Müşteri ilişkileri konusunda deneyimli, iletişimi güçlü adaylar başvurabilir.',
    '+90 324 123 4567',
    30, -- 30 gün
    true, -- Sponsored
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Muhasebe Elemanı',
    'Tam zamanlı muhasebe elemanı aranıyor. Excel ve muhasebe programları bilgisi şart. Deneyimli adaylar tercih edilir.',
    '+90 324 234 5678',
    15, -- 15 gün
    false,
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Garson/Garson Kız',
    'Restoran için deneyimli garson aranıyor. Vardiyalı çalışma. İyi iletişim becerileri gerekli.',
    '+90 324 345 6789',
    7, -- 7 gün
    true, -- Sponsored
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Şoför (Ehliyet B)',
    'Şirket aracı için şoför aranıyor. B sınıfı ehliyet şart. Temiz sürücü belgesi olan adaylar başvurabilir.',
    '+90 324 456 7890',
    20, -- 20 gün
    false,
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
),
(
    '565d34c8-6bbf-4b68-9cd4-17511e6b954b',
    'Web Tasarımcı',
    'Kreatif web tasarımcı aranıyor. HTML, CSS, JavaScript bilgisi şart. Portfolio sahibi adaylar tercih edilir.',
    '+90 324 567 8901',
    45, -- 45 gün
    false,
    '6ae34069-3dcf-461c-8c53-af9730699493',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);