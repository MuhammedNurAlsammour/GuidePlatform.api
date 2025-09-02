-- =====================================================
-- CONVERT IMAGE_TYPE FROM VARCHAR TO INT - تحويل نوع الصورة من نص إلى رقم
-- =====================================================

-- 1. إضافة عمود جديد من نوع INT
ALTER TABLE guideplatform.business_images ADD COLUMN image_type_new INT DEFAULT 1;

-- 2. تحديث البيانات الموجودة
UPDATE guideplatform.business_images SET image_type_new = 
    CASE image_type
        WHEN 'profile' THEN 0
        WHEN 'gallery' THEN 1
        WHEN 'menu' THEN 2
        WHEN 'banner' THEN 3
        WHEN 'logo' THEN 4
        ELSE 1  -- القيمة الافتراضية
    END;

-- 3. حذف العمود القديم
ALTER TABLE guideplatform.business_images DROP COLUMN image_type;

-- 4. إعادة تسمية العمود الجديد
ALTER TABLE guideplatform.business_images RENAME COLUMN image_type_new TO image_type;

-- 5. إضافة قيود على العمود الجديد
ALTER TABLE guideplatform.business_images ALTER COLUMN image_type SET NOT NULL;
ALTER TABLE guideplatform.business_images ALTER COLUMN image_type SET DEFAULT 1;

-- 6. إضافة قيود لضمان صحة البيانات
ALTER TABLE guideplatform.business_images ADD CONSTRAINT chk_image_type_valid 
CHECK (image_type IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11));

-- 7. إضافة فهرس على العمود الجديد
CREATE INDEX idx_business_images_image_type ON guideplatform.business_images(image_type);

-- 8. إضافة التعليق
COMMENT ON COLUMN guideplatform.business_images.image_type IS 'نوع الصورة (0:profile, 1:gallery, 2:menu, 3:banner, 4:logo, 5:interior, 6:exterior, 7:food, 8:kitchen, 9:atmosphere, 10:design, 11:dessert)';

-- 9. إضافة تعليق على القيد
COMMENT ON CONSTRAINT chk_image_type_valid ON guideplatform.business_images IS 'نوع الصورة يجب أن يكون من القيم المسموحة (0-11)';

-- 10. إضافة تعليق على الفهرس
COMMENT ON INDEX idx_business_images_image_type IS 'فهرس على نوع الصورة لتحسين أداء الاستعلامات';

-- 11. عرض معلومات الجدول بعد التعديل
\d+ guideplatform.business_images

-- =====================================================
-- END OF CONVERT IMAGE_TYPE
-- =====================================================
