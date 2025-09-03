-- Sample Announcements Data for Restaurants
-- GuidePlatform Syria-Turkey Business Guide Platform
-- Created: 2025-01-27

-- Sample data for Aleppo Kebap House (978bea96-ec7f-461b-aefa-deaac61df09e)
INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Yeni Halep Kebap Menüsü!',
    'Geleneksel Halep kebap ve mezelerinin sunulduğu restoranımızda yeni menü seçenekleri eklendi. Özel soslar ve taze malzemelerle hazırlanan kebap çeşitlerimizi deneyin.',
    1, -- Yüksek öncelik
    true,
    CURRENT_TIMESTAMP,
    'restaurant',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Hafta Sonu Özel İndirim',
    'Cumartesi ve Pazar günleri tüm kebap çeşitlerinde %20 indirim! Aile paketlerimizi de kaçırmayın.',
    2, -- Orta öncelik
    true,
    CURRENT_TIMESTAMP,
    'discount',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Sample data for Manbij Lokantası (005542a0-ce29-4aa9-b71e-442dd007de67)
INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Suriye Mutfağından Özel Lezzetler',
    'Suriye mutfağının en güzel örneklerini sunan lokantamızda, geleneksel tariflerle hazırlanan yemeklerimizi deneyin. Özellikle humus ve falafel çeşitlerimiz önerilir.',
    1, -- Yüksek öncelik
    true,
    CURRENT_TIMESTAMP,
    'food',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Öğle Menüsü Güncellemesi',
    'Günlük öğle menümüzde yeni seçenekler eklendi. Her gün farklı ana yemek ve çorba seçenekleriyle hizmet veriyoruz.',
    2, -- Orta öncelik
    true,
    CURRENT_TIMESTAMP,
    'menu',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Sample data for Azaz Sofra (f60578b0-ca95-4b3f-b2e9-b66679e8f88c)
INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Ev Yapımı Suriye Yemekleri',
    'Ev yapımı Suriye yemekleri sunan aile restoranımızda, annelerimizin tarifleriyle hazırlanan geleneksel lezzetleri bulabilirsiniz. Özellikle mercimek çorbası ve kısır çeşitlerimiz önerilir.',
    2, -- Orta öncelik
    true,
    CURRENT_TIMESTAMP,
    'home',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Aile Paketi Kampanyası',
    '4 kişilik aile paketimizde ana yemek, çorba, salata ve tatlı dahil sadece 150 TL! Rezervasyon için bizi arayın.',
    1, -- Yüksek öncelik
    true,
    CURRENT_TIMESTAMP,
    'family',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Sample data for Afrin Lezzet (f76e31c4-3e4e-4a58-a5bb-3a57597603f6)
INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Modern Suriye Mutfağı Konsepti',
    'Modern Suriye mutfağı konseptli restoranımızda, geleneksel lezzetleri çağdaş sunum teknikleriyle birleştiriyoruz. Özel şef menümüzü deneyin.',
    1, -- Yüksek öncelik
    true,
    CURRENT_TIMESTAMP,
    'chef',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Özel Etkinlik: Suriye Gecesi',
    'Her Cuma akşamı Suriye Gecesi etkinliğimizde canlı müzik eşliğinde geleneksel dans gösterileri ve özel menü seçenekleri sunuyoruz.',
    1, -- Yüksek öncelik
    true,
    CURRENT_TIMESTAMP,
    'event',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Sample data for Jarabulus Mutfak (7fb6d642-528b-4dcd-8e58-785b7c9542f4)
INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Otantik Suriye Lezzetleri',
    'Otantik Suriye lezzetleri ve tatlıları sunan restoranımızda, geleneksel tariflerle hazırlanan baklava ve künefe çeşitlerimizi deneyin.',
    2, -- Orta öncelik
    true,
    CURRENT_TIMESTAMP,
    'dessert',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Kahve ve Çay Kültürü',
    'Geleneksel Suriye kahvesi ve çay çeşitlerimizle birlikte sunulan tatlılarımızı deneyin. Özellikle nane çayı ve Türk kahvesi eşliğinde baklava keyfi yaşayın.',
    3, -- Düşük öncelik
    true,
    CURRENT_TIMESTAMP,
    'coffee',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Additional sample announcements for different scenarios

-- Draft announcement (not published yet)
INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Yakında: Online Sipariş Sistemi',
    'Çok yakında online sipariş sistemimiz devreye girecek. Mobil uygulamamızı indirerek evinizden sipariş verebileceksiniz.',
    2, -- Orta öncelik
    false, -- Henüz yayınlanmadı
    NULL,
    'mobile',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    true,
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Inactive announcement
INSERT INTO announcements (
    id,
    title,
    content,
    priority,
    is_published,
    published_date,
    icon,
    row_created_date,
    row_updated_date,
    row_is_active,
    row_is_deleted,
    auth_user_id,
    create_user_id,
    auth_customer_id,
    update_user_id
) VALUES (
    gen_random_uuid(),
    'Eski Kampanya (Pasif)',
    'Bu eski kampanya artık aktif değildir. Yeni kampanyalarımızı takip edin.',
    3, -- Düşük öncelik
    true,
    '2024-12-01 00:00:00+03',
    'archive',
    '2024-12-01 00:00:00+03',
    '2024-12-31 23:59:59+03',
    false, -- Pasif
    false,
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '19a8b428-a57e-4a24-98e3-470258d3d83e',
    '72c54b1a-8e1c-45ea-8edd-b5da1091e325',
    '19a8b428-a57e-4a24-98e3-470258d3d83e'
);

-- Comments in Turkish as requested:
-- Bu dosya, GuidePlatform için örnek duyuru verilerini içerir
-- Her restoran için farklı türde duyurular tanımlanmıştır
-- Öncelik seviyeleri: 1=yüksek, 2=orta, 3=düşük
-- Duyurular farklı durumlarda (yayınlanmış, taslak, pasif) örneklenmiştir
-- Her duyuru için uygun ikonlar ve içerikler belirlenmiştir
