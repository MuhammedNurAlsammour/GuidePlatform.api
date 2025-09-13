# 🎯 Ana İletişim Bilgileri Kullanım Kılavuzu - Primary Contact Information Usage Guide

## 📋 Genel Bakış - Overview

Bu sistem, işletmelerin en önemli 2 iletişim yöntemini belirlemesine olanak tanır.
This system allows businesses to specify their 2 most important contact methods.

## 🔢 İletişim Türleri - Contact Types

| Değer - Value | Tür - Type | Açıklama - Description |
|---------------|------------|------------------------|
| 1 | WhatsApp | WhatsApp numarası - WhatsApp number |
| 2 | Phone | Sabit telefon - Landline phone |
| 3 | Mobile | Cep telefonu - Mobile phone |
| 4 | Email | E-posta adresi - Email address |
| 5 | Facebook | Facebook sayfası - Facebook page |
| 6 | Instagram | Instagram hesabı - Instagram account |
| 7 | Telegram | Telegram hesabı - Telegram account |
| 8 | Website | Web sitesi - Website |

## 📝 Kullanım Örnekleri - Usage Examples

### Örnek 1: WhatsApp + Website
```json
{
  "primary_contact_type_1": 1,
  "primary_contact_value_1": "055334455",
  "primary_contact_type_2": 8,
  "primary_contact_value_2": "www.store.com"
}
```

### Örnek 2: Phone + Email
```json
{
  "primary_contact_type_1": 2,
  "primary_contact_value_1": "0112345678",
  "primary_contact_type_2": 4,
  "primary_contact_value_2": "info@business.com"
}
```

### Örnek 3: Instagram + WhatsApp
```json
{
  "primary_contact_type_1": 6,
  "primary_contact_value_1": "@business_instagram",
  "primary_contact_type_2": 1,
  "primary_contact_value_2": "055334455"
}
```

## 🚀 API Kullanımı - API Usage

### Create Business
```http
POST /api/businesses
Content-Type: application/json

{
  "name": "كوكتيل الآغا",
  "description": "أشهى الكوكتيلات والعصائر الطبيعية",
  "primary_contact_type_1": 1,
  "primary_contact_value_1": "055334455",
  "primary_contact_type_2": 8,
  "primary_contact_value_2": "www.cocktail-alagha.com"
}
```

### Update Business
```http
PUT /api/businesses/{id}
Content-Type: application/json

{
  "primary_contact_type_1": 1,
  "primary_contact_value_1": "055334455",
  "primary_contact_type_2": 5,
  "primary_contact_value_2": "https://facebook.com/cocktail-alagha"
}
```

### Get Business Response
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "كوكتيل الآغا",
  "description": "أشهى الكوكتيلات والعصائر الطبيعية",
  "primary_contact_type_1": 1,
  "primary_contact_value_1": "055334455",
  "primary_contact_type_2": 8,
  "primary_contact_value_2": "www.cocktail-alagha.com",
  "phone": "0112345678",
  "mobile": "055334455",
  "email": "info@cocktail-alagha.com",
  "whatsapp": "055334455",
  "website": "www.cocktail-alagha.com",
  "facebook_url": "https://facebook.com/cocktail-alagha",
  "instagram_url": "@cocktail_alagha"
}
```

## 📱 Flutter Kullanımı - Flutter Usage

```dart
// Ana iletişim bilgilerini göster
Widget buildPrimaryContact(BusinessesDTO business) {
  return Column(
    children: [
      if (business.primaryContactType1 != null)
        _buildContactButton(
          business.primaryContactType1!,
          business.primaryContactValue1!,
          isPrimary: true,
        ),
      if (business.primaryContactType2 != null)
        _buildContactButton(
          business.primaryContactType2!,
          business.primaryContactValue2!,
          isPrimary: false,
        ),
    ],
  );
}

Widget _buildContactButton(int type, String value, {required bool isPrimary}) {
  switch (type) {
    case 1: // WhatsApp
      return WhatsAppButton(value: value, isPrimary: isPrimary);
    case 2: // Phone
      return PhoneButton(value: value, isPrimary: isPrimary);
    case 3: // Mobile
      return MobileButton(value: value, isPrimary: isPrimary);
    case 4: // Email
      return EmailButton(value: value, isPrimary: isPrimary);
    case 5: // Facebook
      return FacebookButton(value: value, isPrimary: isPrimary);
    case 6: // Instagram
      return InstagramButton(value: value, isPrimary: isPrimary);
    case 7: // Telegram
      return TelegramButton(value: value, isPrimary: isPrimary);
    case 8: // Website
      return WebsiteButton(value: value, isPrimary: isPrimary);
    default:
      return SizedBox.shrink();
  }
}
```

## ✅ Avantajlar - Advantages

1. **Esneklik - Flexibility**: İşletme sahibi en uygun 2 iletişim yöntemini seçebilir
2. **Performans - Performance**: Flutter uygulamasında sadece gerekli butonlar gösterilir
3. **Kullanıcı Deneyimi - UX**: Müşteriler en hızlı şekilde iletişime geçebilir
4. **Veri Tutarlılığı - Data Consistency**: Integer değerler daha güvenilir
5. **Genişletilebilirlik - Scalability**: Yeni iletişim türleri kolayca eklenebilir

## 🔧 Veritabanı Değişiklikleri - Database Changes

```sql
-- Yeni alanları ekle
ALTER TABLE guideplatform.businesses 
ADD COLUMN primary_contact_type_1 int4 NULL,
ADD COLUMN primary_contact_value_1 varchar(500) NULL,
ADD COLUMN primary_contact_type_2 int4 NULL,
ADD COLUMN primary_contact_value_2 varchar(500) NULL;
```

## 📊 Validasyon Kuralları - Validation Rules

- `primary_contact_type_1` ve `primary_contact_type_2`: 1-8 arası integer
- `primary_contact_value_1` ve `primary_contact_value_2`: Maksimum 500 karakter
- Her iki alan da opsiyonel (nullable)
- Aynı tür birden fazla kez kullanılabilir
