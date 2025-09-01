# Syria Guide Offers & Promotions System
# نظام العروض والكوبونات لدليلك في سوريا

## نظرة عامة - Genel Bakış

هذا النظام يتيح للمطاعم والأعمال التجارية إنشاء وإدارة العروض والكوبونات، ويتيح للمستخدمين البحث عن العروض حسب الفئات والاستفادة منها.

Bu sistem, restoranların ve işletmelerin indirim ve kupon oluşturmasına, kullanıcıların kategorilere göre arama yapmasına ve bu tekliflerden yararlanmasına olanak tanır.

## الملفات المطلوبة - Gerekli Dosyalar

1. `syria_guide_offers_system.sql` - الجداول الأساسية للعروض
2. `syria_guide_offers_views_procedures.sql` - العروض والإجراءات المخزنة

## الجداول الرئيسية - Ana Tablolar

### 1. offers - العروض
```sql
-- مثال على إنشاء عرض
INSERT INTO offers (
    business_id, 
    title, 
    description, 
    discount_type, 
    discount_value, 
    original_price, 
    discounted_price, 
    start_date, 
    end_date, 
    is_featured, 
    max_uses, 
    terms_conditions
) VALUES (
    'business-uuid-here',
    'Pizza %50 İndirim',
    'Tüm pizzalarda %50 indirim fırsatı!',
    'percentage',
    50.00,
    100.00,
    50.00,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP + INTERVAL '30 days',
    true,
    100,
    'Bu indirim sadece pizzalar için geçerlidir.'
);
```

### 2. coupons - الكوبونات
```sql
-- مثال على إنشاء كوبون
INSERT INTO coupons (
    business_id,
    code,
    title,
    description,
    discount_type,
    discount_value,
    start_date,
    end_date,
    max_uses,
    min_order_amount,
    terms_conditions
) VALUES (
    'business-uuid-here',
    'HOSGELDIN10',
    'Hoş Geldin İndirimi',
    'İlk alışverişinizde %10 indirim',
    'percentage',
    10.00,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP + INTERVAL '90 days',
    1000,
    50.00,
    'Bu kupon sadece ilk alışverişlerde kullanılabilir.'
);
```

## أنواع الخصومات - İndirim Türleri

### للعروض - Teklifler için:
- `percentage` - نسبة مئوية
- `fixed_amount` - مبلغ ثابت
- `free_item` - منتج مجاني

### للكوبونات - Kuponlar için:
- `percentage` - نسبة مئوية
- `fixed_amount` - مبلغ ثابت
- `free_shipping` - شحن مجاني

## الاستعلامات المفيدة - Faydalı Sorgular

### 1. الحصول على العروض حسب الفئة
```sql
-- Get offers by category
SELECT * FROM get_offers_by_category(
    p_category_id := 'category-uuid-here',
    p_limit := 10,
    p_offset := 0,
    p_featured_only := false,
    p_user_id := 'user-uuid-here'
);
```

### 2. البحث في العروض
```sql
-- Search offers
SELECT * FROM search_offers(
    p_search_term := 'pizza',
    p_category_id := 'category-uuid-here',
    p_discount_type := 'percentage',
    p_min_discount := 10.00,
    p_max_discount := 50.00,
    p_featured_only := true,
    p_limit := 10,
    p_offset := 0,
    p_user_id := 'user-uuid-here'
);
```

### 3. العروض المميزة
```sql
-- Get featured offers
SELECT * FROM v_active_offers 
WHERE is_featured = true 
    AND status = 'Aktif'
ORDER BY row_created_date DESC;
```

### 4. العروض حسب الأعمال التجارية
```sql
-- Get offers by business
SELECT * FROM get_offers_by_business(
    p_business_id := 'business-uuid-here',
    p_limit := 10,
    p_offset := 0,
    p_user_id := 'user-uuid-here'
);
```

### 5. العروض المفضلة للمستخدم
```sql
-- Get user's favorite offers
SELECT * FROM get_user_favorite_offers(
    p_user_id := 'user-uuid-here',
    p_limit := 10,
    p_offset := 0
);
```

## إضافة/إزالة من المفضلة - Favorilere Ekleme/Çıkarma

### إضافة عرض للمفضلة
```sql
-- Add offer to favorites
SELECT add_offer_to_favorites(
    p_user_id := 'user-uuid-here',
    p_offer_id := 'offer-uuid-here',
    p_auth_user_id := 'auth-user-uuid-here',
    p_auth_customer_id := 'customer-uuid-here',
    p_create_user_id := 'create-user-uuid-here',
    p_update_user_id := 'update-user-uuid-here'
);
```

### إزالة عرض من المفضلة
```sql
-- Remove offer from favorites
SELECT remove_offer_from_favorites(
    p_user_id := 'user-uuid-here',
    p_offer_id := 'offer-uuid-here',
    p_update_user_id := 'update-user-uuid-here'
);
```

## العروض المفيدة - Faydalı Görünümler

### 1. v_active_offers
عرض جميع العروض النشطة مع معلومات الأعمال التجارية والفئات

### 2. v_offers_by_category
عرض عدد العروض لكل فئة

### 3. v_user_favorite_offers
عرض العروض المفضلة للمستخدم

### 4. v_active_coupons
عرض جميع الكوبونات النشطة

## مثال على الاستخدام في التطبيق - Uygulama Örneği

### عند الضغط على فئة "المطاعم":
```sql
-- Get restaurant offers
SELECT * FROM get_offers_by_category(
    p_category_id := (SELECT id FROM categories WHERE name = 'Restoranlar ve Kafeler'),
    p_limit := 20,
    p_offset := 0,
    p_featured_only := false,
    p_user_id := 'current-user-uuid'
);
```

### البحث عن عروض البيتزا:
```sql
-- Search for pizza offers
SELECT * FROM search_offers(
    p_search_term := 'pizza',
    p_category_id := (SELECT id FROM categories WHERE name = 'Restoranlar ve Kafeler'),
    p_limit := 10,
    p_offset := 0,
    p_user_id := 'current-user-uuid'
);
```

## ملاحظات مهمة - Önemli Notlar

1. **التواريخ**: تأكد من أن `start_date` و `end_date` صحيحة
2. **الحد الأقصى للاستخدام**: إذا كان `max_uses` فارغاً، فهذا يعني استخدام غير محدود
3. **الحالة**: العروض تظهر فقط إذا كانت `is_active = true` وفي الفترة الزمنية الصحيحة
4. **المستخدمين**: تأكد من وجود المستخدمين في جداول `auth."AspNetUsers"` و `auth."Customers"`

## التثبيت - Kurulum

1. قم بتشغيل `syria_guide_offers_system.sql` أولاً
2. ثم قم بتشغيل `syria_guide_offers_views_procedures.sql`
3. تأكد من وجود بيانات في جداول `businesses` و `categories`

## الدعم - Destek

إذا واجهت أي مشاكل أو لديك أسئلة، يرجى التواصل مع فريق التطوير.
