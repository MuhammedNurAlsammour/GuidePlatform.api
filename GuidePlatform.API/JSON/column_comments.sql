-- =====================================================
-- TABLE COMMENTS - تعليقات الجداول
-- =====================================================

COMMENT ON TABLE categories IS 'Ana ve Alt Kategoriler';
COMMENT ON TABLE businesses IS 'İşletmeler ve Organizasyonlar';
COMMENT ON TABLE business_images IS 'İşletme Görselleri';
COMMENT ON TABLE business_contacts IS 'İşletme İletişim Bilgileri';
COMMENT ON TABLE business_services IS 'İşletme Hizmetleri';
COMMENT ON TABLE business_working_hours IS 'Çalışma Saatleri';
COMMENT ON TABLE business_reviews IS 'İşletme Değerlendirmeleri';
-- user_profiles kaldırıldı - doğrudan auth."AspNetUsers" kullanılıyor
COMMENT ON TABLE user_favorites IS 'Favoriler';
COMMENT ON TABLE user_visits IS 'Kullanıcı Ziyaretleri';
COMMENT ON TABLE notifications IS 'Bildirimler';
COMMENT ON TABLE notification_settings IS 'Bildirim Ayarları';
COMMENT ON TABLE search_logs IS 'Arama Kayıtları';
COMMENT ON TABLE business_analytics IS 'İşletme Analizleri';
COMMENT ON TABLE subscriptions IS 'Abonelikler';
COMMENT ON TABLE payments IS 'Ödemeler';
COMMENT ON TABLE articles IS 'Makaleler';
COMMENT ON TABLE pages IS 'Statik Sayfalar';
COMMENT ON TABLE banners IS 'Bannerlar';
COMMENT ON TABLE announcements IS 'Duyurular';
COMMENT ON TABLE parameters IS 'Parametreler ve Ayarlar';
COMMENT ON TABLE files IS 'Dosyalar';
COMMENT ON TABLE logs IS 'Kayıtlar';
-- =====================================================
-- COLUMN COMMENTS - تعليقات الأعمدة
-- =====================================================

-- =====================================================
-- CATEGORIES TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN categories.id IS 'Benzersiz kategori kimliği';
COMMENT ON COLUMN categories.name IS 'Kategori adı';
COMMENT ON COLUMN categories.description IS 'Kategori açıklaması';
COMMENT ON COLUMN categories.parent_id IS 'Üst kategori kimliği (alt kategoriler için)';
COMMENT ON COLUMN categories.icon IS 'Kategori simgesi (Material Icons)';
COMMENT ON COLUMN categories.sort_order IS 'Sıralama düzeni';
COMMENT ON COLUMN categories.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN categories.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN categories.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN categories.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN categories.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN categories.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN categories.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN categories.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- BUSINESSES TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN businesses.id IS 'Benzersiz işletme kimliği';
COMMENT ON COLUMN businesses.name IS 'İşletme adı';
COMMENT ON COLUMN businesses.description IS 'İşletme açıklaması';
COMMENT ON COLUMN businesses.category_id IS 'Ana kategori kimliği';
COMMENT ON COLUMN businesses.sub_category_id IS 'Alt kategori kimliği';
COMMENT ON COLUMN businesses.province_id IS 'İl kimliği';
COMMENT ON COLUMN businesses.city_id IS 'Şehir kimliği';
COMMENT ON COLUMN businesses.district_id IS 'İlçe kimliği';
COMMENT ON COLUMN businesses.address IS 'İşletme adresi';
COMMENT ON COLUMN businesses.phone IS 'Telefon numarası';
COMMENT ON COLUMN businesses.mobile IS 'Cep telefonu numarası';
COMMENT ON COLUMN businesses.email IS 'E-posta adresi';
COMMENT ON COLUMN businesses.website IS 'Web sitesi URL''si';
COMMENT ON COLUMN businesses.facebook_url IS 'Facebook sayfası URL''si';
COMMENT ON COLUMN businesses.instagram_url IS 'Instagram sayfası URL''si';
COMMENT ON COLUMN businesses.whatsapp IS 'WhatsApp numarası';
COMMENT ON COLUMN businesses.telegram IS 'Telegram kullanıcı adı';
COMMENT ON COLUMN businesses.latitude IS 'Enlem koordinatı';
COMMENT ON COLUMN businesses.longitude IS 'Boylam koordinatı';
COMMENT ON COLUMN businesses.rating IS 'Ortalama değerlendirme puanı (1-5)';
COMMENT ON COLUMN businesses.total_reviews IS 'Toplam değerlendirme sayısı';
COMMENT ON COLUMN businesses.view_count IS 'Görüntülenme sayısı';
COMMENT ON COLUMN businesses.subscription_type IS 'Abonelik türü (FREE, SILVER, GOLD)';
COMMENT ON COLUMN businesses.is_verified IS 'İşletme doğrulanmış mı?';
COMMENT ON COLUMN businesses.is_featured IS 'İşletme öne çıkarılmış mı?';
COMMENT ON COLUMN businesses.working_hours IS 'Çalışma saatleri (JSON formatında)';
COMMENT ON COLUMN businesses.icon IS 'İşletme simgesi';
COMMENT ON COLUMN businesses.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN businesses.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN businesses.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN businesses.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN businesses.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN businesses.owner_id IS 'İşletme sahibi kullanıcı kimliği';
COMMENT ON COLUMN businesses.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN businesses.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN businesses.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- BUSINESS_IMAGES TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN business_images.id IS 'Benzersiz resim kimliği';
COMMENT ON COLUMN business_images.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN business_images.photo IS 'Resim verisi (bytea)';
COMMENT ON COLUMN business_images.thumbnail IS 'Küçük resim verisi (bytea)';
COMMENT ON COLUMN business_images.photo_content_type IS 'Resim içerik türü';
COMMENT ON COLUMN business_images.alt_text IS 'Resim alternatif metni';
COMMENT ON COLUMN business_images.image_type IS 'Resim türü (profile, gallery, menu)';
COMMENT ON COLUMN business_images.is_primary IS 'Ana resim mi?';
COMMENT ON COLUMN business_images.sort_order IS 'Sıralama düzeni';
COMMENT ON COLUMN business_images.icon IS 'Resim simgesi';
COMMENT ON COLUMN business_images.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN business_images.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN business_images.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN business_images.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN business_images.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN business_images.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN business_images.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN business_images.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- BUSINESS_CONTACTS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN business_contacts.id IS 'Benzersiz iletişim kimliği';
COMMENT ON COLUMN business_contacts.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN business_contacts.contact_type IS 'İletişim türü (phone, email, whatsapp, facebook, instagram, website)';
COMMENT ON COLUMN business_contacts.contact_value IS 'İletişim değeri';
COMMENT ON COLUMN business_contacts.is_primary IS 'Birincil iletişim bilgisi mi?';
COMMENT ON COLUMN business_contacts.icon IS 'İletişim simgesi';
COMMENT ON COLUMN business_contacts.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN business_contacts.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN business_contacts.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN business_contacts.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN business_contacts.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN business_contacts.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN business_contacts.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN business_contacts.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- BUSINESS_SERVICES TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN business_services.id IS 'Benzersiz hizmet kimliği';
COMMENT ON COLUMN business_services.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN business_services.service_name IS 'Hizmet adı';
COMMENT ON COLUMN business_services.service_description IS 'Hizmet açıklaması';
COMMENT ON COLUMN business_services.price IS 'Hizmet fiyatı';
COMMENT ON COLUMN business_services.currency IS 'Para birimi (SYP)';
COMMENT ON COLUMN business_services.is_available IS 'Hizmet mevcut mu?';
COMMENT ON COLUMN business_services.icon IS 'Hizmet simgesi';
COMMENT ON COLUMN business_services.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN business_services.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN business_services.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN business_services.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN business_services.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN business_services.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN business_services.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN business_services.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- BUSINESS_WORKING_HOURS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN business_working_hours.id IS 'Benzersiz çalışma saati kimliği';
COMMENT ON COLUMN business_working_hours.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN business_working_hours.day_of_week IS 'Haftanın günü (1=Pazartesi, 7=Pazar)';
COMMENT ON COLUMN business_working_hours.open_time IS 'Açılış saati';
COMMENT ON COLUMN business_working_hours.close_time IS 'Kapanış saati';
COMMENT ON COLUMN business_working_hours.is_closed IS 'O gün kapalı mı?';
COMMENT ON COLUMN business_working_hours.icon IS 'Çalışma saati simgesi';
COMMENT ON COLUMN business_working_hours.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN business_working_hours.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN business_working_hours.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN business_working_hours.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN business_working_hours.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN business_working_hours.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN business_working_hours.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN business_working_hours.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- BUSINESS_REVIEWS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN business_reviews.id IS 'Benzersiz değerlendirme kimliği';
COMMENT ON COLUMN business_reviews.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN business_reviews.reviewer_id IS 'Değerlendiren kullanıcı kimliği';
COMMENT ON COLUMN business_reviews.rating IS 'Değerlendirme puanı (1-5)';
COMMENT ON COLUMN business_reviews.comment IS 'Değerlendirme yorumu';
COMMENT ON COLUMN business_reviews.is_verified IS 'Değerlendirme doğrulanmış mı?';
COMMENT ON COLUMN business_reviews.is_approved IS 'Değerlendirme onaylanmış mı?';
COMMENT ON COLUMN business_reviews.icon IS 'Değerlendirme simgesi';
COMMENT ON COLUMN business_reviews.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN business_reviews.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN business_reviews.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN business_reviews.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN business_reviews.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN business_reviews.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN business_reviews.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN business_reviews.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- USER_FAVORITES TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN user_favorites.id IS 'Benzersiz favori kimliği';
COMMENT ON COLUMN user_favorites.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN user_favorites.icon IS 'Favori simgesi';
COMMENT ON COLUMN user_favorites.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN user_favorites.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN user_favorites.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN user_favorites.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN user_favorites.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN user_favorites.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN user_favorites.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN user_favorites.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- USER_VISITS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN user_visits.id IS 'Benzersiz ziyaret kimliği';
COMMENT ON COLUMN user_visits.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN user_visits.visit_date IS 'Ziyaret tarihi';
COMMENT ON COLUMN user_visits.visit_type IS 'Ziyaret türü (view, contact, review)';
COMMENT ON COLUMN user_visits.icon IS 'Ziyaret simgesi';
COMMENT ON COLUMN user_visits.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN user_visits.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN user_visits.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN user_visits.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN user_visits.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN user_visits.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN user_visits.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN user_visits.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- NOTIFICATIONS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN notifications.id IS 'Benzersiz bildirim kimliği';
COMMENT ON COLUMN notifications.recipient_user_id IS 'Alıcı kullanıcı kimliği';
COMMENT ON COLUMN notifications.title IS 'Bildirim başlığı';
COMMENT ON COLUMN notifications.message IS 'Bildirim mesajı';
COMMENT ON COLUMN notifications.notification_type IS 'Bildirim türü (info, success, warning, error)';
COMMENT ON COLUMN notifications.is_read IS 'Bildirim okunmuş mu?';
COMMENT ON COLUMN notifications.read_date IS 'Okunma tarihi';
COMMENT ON COLUMN notifications.action_url IS 'Eylem URL''si';
COMMENT ON COLUMN notifications.related_entity_id IS 'İlgili varlık kimliği';
COMMENT ON COLUMN notifications.related_entity_type IS 'İlgili varlık türü (business, article, review)';
COMMENT ON COLUMN notifications.icon IS 'Bildirim simgesi';
COMMENT ON COLUMN notifications.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN notifications.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN notifications.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN notifications.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN notifications.auth_user_id IS 'Bildirim oluşturan kullanıcı kimliği';
COMMENT ON COLUMN notifications.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN notifications.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN notifications.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- NOTIFICATION_SETTINGS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN notification_settings.id IS 'Benzersiz ayar kimliği';
COMMENT ON COLUMN notification_settings.user_id IS 'Kullanıcı kimliği';
COMMENT ON COLUMN notification_settings.setting_type IS 'Ayar türü (email, push, sms)';
COMMENT ON COLUMN notification_settings.is_enabled IS 'Ayar etkin mi?';
COMMENT ON COLUMN notification_settings.icon IS 'Ayar simgesi';
COMMENT ON COLUMN notification_settings.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN notification_settings.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN notification_settings.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN notification_settings.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN notification_settings.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN notification_settings.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN notification_settings.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN notification_settings.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- SEARCH_LOGS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN search_logs.id IS 'Benzersiz arama kaydı kimliği';
COMMENT ON COLUMN search_logs.search_term IS 'Arama terimi';
COMMENT ON COLUMN search_logs.search_filters IS 'Arama filtreleri (JSON)';
COMMENT ON COLUMN search_logs.results_count IS 'Sonuç sayısı';
COMMENT ON COLUMN search_logs.search_date IS 'Arama tarihi';
COMMENT ON COLUMN search_logs.ip_address IS 'IP adresi';
COMMENT ON COLUMN search_logs.user_agent IS 'Kullanıcı ajanı';
COMMENT ON COLUMN search_logs.icon IS 'Arama simgesi';
COMMENT ON COLUMN search_logs.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN search_logs.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN search_logs.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN search_logs.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN search_logs.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN search_logs.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN search_logs.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN search_logs.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- BUSINESS_ANALYTICS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN business_analytics.id IS 'Benzersiz analitik kaydı kimliği';
COMMENT ON COLUMN business_analytics.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN business_analytics.date IS 'Analitik tarihi';
COMMENT ON COLUMN business_analytics.views_count IS 'Görüntülenme sayısı';
COMMENT ON COLUMN business_analytics.contacts_count IS 'İletişim sayısı';
COMMENT ON COLUMN business_analytics.reviews_count IS 'Değerlendirme sayısı';
COMMENT ON COLUMN business_analytics.favorites_count IS 'Favori sayısı';
COMMENT ON COLUMN business_analytics.icon IS 'Analitik simgesi';
COMMENT ON COLUMN business_analytics.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN business_analytics.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN business_analytics.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN business_analytics.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN business_analytics.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN business_analytics.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN business_analytics.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN business_analytics.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- SUBSCRIPTIONS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN subscriptions.id IS 'Benzersiz abonelik kimliği';
COMMENT ON COLUMN subscriptions.business_id IS 'İşletme kimliği';
COMMENT ON COLUMN subscriptions.subscription_type IS 'Abonelik türü (FREE, SILVER, GOLD)';
COMMENT ON COLUMN subscriptions.start_date IS 'Başlangıç tarihi';
COMMENT ON COLUMN subscriptions.end_date IS 'Bitiş tarihi';
COMMENT ON COLUMN subscriptions.amount IS 'Abonelik tutarı';
COMMENT ON COLUMN subscriptions.currency IS 'Para birimi (SYP)';
COMMENT ON COLUMN subscriptions.status IS 'Abonelik durumu (active, expired, cancelled)';
COMMENT ON COLUMN subscriptions.payment_status IS 'Ödeme durumu (pending, paid, failed)';
COMMENT ON COLUMN subscriptions.icon IS 'Abonelik simgesi';
COMMENT ON COLUMN subscriptions.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN subscriptions.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN subscriptions.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN subscriptions.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN subscriptions.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN subscriptions.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN subscriptions.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN subscriptions.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- PAYMENTS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN payments.id IS 'Benzersiz ödeme kimliği';
COMMENT ON COLUMN payments.subscription_id IS 'Abonelik kimliği';
COMMENT ON COLUMN payments.amount IS 'Ödeme tutarı';
COMMENT ON COLUMN payments.currency IS 'Para birimi (SYP)';
COMMENT ON COLUMN payments.payment_method IS 'Ödeme yöntemi (cash, bank_transfer, online)';
COMMENT ON COLUMN payments.transaction_id IS 'İşlem kimliği';
COMMENT ON COLUMN payments.payment_date IS 'Ödeme tarihi';
COMMENT ON COLUMN payments.status IS 'Ödeme durumu (pending, completed, failed, refunded)';
COMMENT ON COLUMN payments.notes IS 'Ödeme notları';
COMMENT ON COLUMN payments.icon IS 'Ödeme simgesi';
COMMENT ON COLUMN payments.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN payments.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN payments.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN payments.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN payments.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN payments.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN payments.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN payments.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- ARTICLES TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN articles.id IS 'Benzersiz makale kimliği';
COMMENT ON COLUMN articles.title IS 'Makale başlığı';
COMMENT ON COLUMN articles.content IS 'Makale içeriği';
COMMENT ON COLUMN articles.excerpt IS 'Makale özeti';
COMMENT ON COLUMN articles.photo IS 'Makale resmi (bytea)';
COMMENT ON COLUMN articles.thumbnail IS 'Küçük resim (bytea)';
COMMENT ON COLUMN articles.photo_content_type IS 'Resim içerik türü';
COMMENT ON COLUMN articles.author_id IS 'Yazar kullanıcı kimliği';
COMMENT ON COLUMN articles.category_id IS 'Kategori kimliği';
COMMENT ON COLUMN articles.is_featured IS 'Öne çıkarılmış makale mi?';
COMMENT ON COLUMN articles.is_published IS 'Makale yayınlanmış mı?';
COMMENT ON COLUMN articles.published_at IS 'Yayınlanma tarihi';
COMMENT ON COLUMN articles.view_count IS 'Görüntülenme sayısı';
COMMENT ON COLUMN articles.seo_title IS 'SEO başlığı';
COMMENT ON COLUMN articles.seo_description IS 'SEO açıklaması';
COMMENT ON COLUMN articles.slug IS 'URL slug''ı';
COMMENT ON COLUMN articles.icon IS 'Makale simgesi';
COMMENT ON COLUMN articles.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN articles.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN articles.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN articles.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN articles.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN articles.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN articles.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN articles.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- PAGES TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN pages.id IS 'Benzersiz sayfa kimliği';
COMMENT ON COLUMN pages.title IS 'Sayfa başlığı';
COMMENT ON COLUMN pages.slug IS 'URL slug''ı';
COMMENT ON COLUMN pages.content IS 'Sayfa içeriği';
COMMENT ON COLUMN pages.meta_description IS 'Meta açıklaması';
COMMENT ON COLUMN pages.meta_keywords IS 'Meta anahtar kelimeleri';
COMMENT ON COLUMN pages.is_published IS 'Sayfa yayınlanmış mı?';
COMMENT ON COLUMN pages.published_date IS 'Yayınlanma tarihi';
COMMENT ON COLUMN pages.icon IS 'Sayfa simgesi';
COMMENT ON COLUMN pages.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN pages.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN pages.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN pages.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN pages.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN pages.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN pages.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN pages.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- BANNERS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN banners.id IS 'Benzersiz banner kimliği';
COMMENT ON COLUMN banners.title IS 'Banner başlığı';
COMMENT ON COLUMN banners.description IS 'Banner açıklaması';
COMMENT ON COLUMN banners.photo IS 'Banner resmi (bytea)';
COMMENT ON COLUMN banners.thumbnail IS 'Küçük resim (bytea)';
COMMENT ON COLUMN banners.photo_content_type IS 'Resim içerik türü';
COMMENT ON COLUMN banners.link_url IS 'Banner link URL''si';
COMMENT ON COLUMN banners.start_date IS 'Başlangıç tarihi';
COMMENT ON COLUMN banners.end_date IS 'Bitiş tarihi';
COMMENT ON COLUMN banners.is_active IS 'Banner aktif mi?';
COMMENT ON COLUMN banners.order_index IS 'Sıralama indeksi';
COMMENT ON COLUMN banners.icon IS 'Banner simgesi';
COMMENT ON COLUMN banners.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN banners.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN banners.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN banners.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN banners.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN banners.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN banners.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN banners.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- ANNOUNCEMENTS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN announcements.id IS 'Benzersiz duyuru kimliği';
COMMENT ON COLUMN announcements.title IS 'Duyuru başlığı';
COMMENT ON COLUMN announcements.content IS 'Duyuru içeriği';
COMMENT ON COLUMN announcements.priority IS 'Öncelik (1=Yüksek, 2=Orta, 3=Düşük)';
COMMENT ON COLUMN announcements.is_published IS 'Duyuru yayınlanmış mı?';
COMMENT ON COLUMN announcements.published_date IS 'Yayınlanma tarihi';
COMMENT ON COLUMN announcements.icon IS 'Duyuru simgesi';
COMMENT ON COLUMN announcements.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN announcements.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN announcements.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN announcements.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN announcements.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN announcements.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN announcements.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN announcements.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- PARAMETERS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN parameters.id IS 'Benzersiz parametre kimliği';
COMMENT ON COLUMN parameters.name IS 'Parametre adı';
COMMENT ON COLUMN parameters.value IS 'Parametre değeri';
COMMENT ON COLUMN parameters.description IS 'Parametre açıklaması';
COMMENT ON COLUMN parameters.data_type IS 'Veri türü (string, integer, boolean, json)';
COMMENT ON COLUMN parameters.is_system IS 'Sistem parametresi mi?';
COMMENT ON COLUMN parameters.icon IS 'Parametre simgesi';
COMMENT ON COLUMN parameters.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN parameters.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN parameters.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN parameters.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN parameters.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN parameters.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN parameters.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN parameters.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- FILES TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN files.id IS 'Benzersiz dosya kimliği';
COMMENT ON COLUMN files.file_name IS 'Dosya adı';
COMMENT ON COLUMN files.file_path IS 'Dosya yolu';
COMMENT ON COLUMN files.file_size IS 'Dosya boyutu (byte)';
COMMENT ON COLUMN files.mime_type IS 'MIME türü';
COMMENT ON COLUMN files.file_type IS 'Dosya türü (image, document, video, audio)';
COMMENT ON COLUMN files.is_public IS 'Dosya herkese açık mı?';
COMMENT ON COLUMN files.icon IS 'Dosya simgesi';
COMMENT ON COLUMN files.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN files.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN files.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN files.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN files.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği';
COMMENT ON COLUMN files.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN files.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN files.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- LOGS TABLE COMMENTS
-- =====================================================
COMMENT ON COLUMN logs.id IS 'Benzersiz log kimliği';
COMMENT ON COLUMN logs.level IS 'Log seviyesi (info, warning, error, debug)';
COMMENT ON COLUMN logs.message IS 'Log mesajı';
COMMENT ON COLUMN logs.exception IS 'Hata detayı';
COMMENT ON COLUMN logs.source IS 'Log kaynağı';
COMMENT ON COLUMN logs.ip_address IS 'IP adresi';
COMMENT ON COLUMN logs.user_agent IS 'Kullanıcı ajanı';
COMMENT ON COLUMN logs.icon IS 'Log simgesi';
COMMENT ON COLUMN logs.row_created_date IS 'Kayıt oluşturma tarihi';
COMMENT ON COLUMN logs.row_updated_date IS 'Kayıt güncelleme tarihi';
COMMENT ON COLUMN logs.row_is_active IS 'Kayıt aktif mi?';
COMMENT ON COLUMN logs.row_is_deleted IS 'Kayıt silinmiş mi?';
COMMENT ON COLUMN logs.auth_user_id IS 'Kimlik doğrulama kullanıcı kimliği (sistem logları için null olabilir)';
COMMENT ON COLUMN logs.auth_customer_id IS 'Müşteri kimliği';
COMMENT ON COLUMN logs.create_user_id IS 'Kayıt oluşturan kullanıcı kimliği';
COMMENT ON COLUMN logs.update_user_id IS 'Kayıt güncelleyen kullanıcı kimliği';

-- =====================================================
-- END OF COLUMN COMMENTS
-- =====================================================
