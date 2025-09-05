# OData Kullanım Kılavuzu - BusinessReviews

## Genel Bakış

Bu kılavuz, GuidePlatform API'sinde BusinessReviews için OData sorgularının nasıl kullanılacağını açıklar.

## Temel URL Yapısı

```
http://localhost:5263/api/BusinessReviews/odata
```

## Önemli Notlar

- **String değerler için tek tırnak (') kullanın, çift tırnak (") değil**
- Tüm sorgular case-sensitive'dir
- GUID değerleri string olarak kullanılabilir

## Desteklenen Sorgu Parametreleri

### 1. $filter - Filtreleme

Temel karşılaştırma operatörleri:

- `eq` (eşittir)
- `ne` (eşit değildir)
- `gt` (büyüktür)
- `ge` (büyük eşittir)
- `lt` (küçüktür)
- `le` (küçük eşittir)

#### String Filtreleme Örnekleri:

```bash
# İş yeri adına göre filtreleme
http://localhost:5263/api/BusinessReviews/odata?$filter=BusinessName eq 'Aleppo'

# Yorum yapan kişi adına göre filtreleme
http://localhost:5263/api/BusinessReviews/odata?$filter=ReviewerName eq 'john.doe@example.com'

# Yorum metninde arama (contains)
http://localhost:5263/api/BusinessReviews/odata?$filter=contains(Comment,'güzel')

# Yorum metninde başlangıç arama (startswith)
http://localhost:5263/api/BusinessReviews/odata?$filter=startswith(Comment,'Çok')

# Yorum metninde bitiş arama (endswith)
http://localhost:5263/api/BusinessReviews/odata?$filter=endswith(Comment,'tavsiye ederim')

# Kullanıcı adına göre filtreleme (YENİ!)
http://localhost:5263/api/BusinessReviews/odata?$filter=AuthUserName eq 'admin@example.com'

# Oluşturan kullanıcıya göre filtreleme (YENİ!)
http://localhost:5263/api/BusinessReviews/odata?$filter=CreateUserName eq 'john.doe@example.com'

# Güncelleyen kullanıcıya göre filtreleme (YENİ!)
http://localhost:5263/api/BusinessReviews/odata?$filter=UpdateUserName eq 'jane.smith@example.com'
```

#### Numeric Filtreleme Örnekleri:

```bash
# Belirli bir puan ve üzeri
http://localhost:5263/api/BusinessReviews/odata?$filter=Rating ge 4

# Belirli bir puan aralığı
http://localhost:5263/api/BusinessReviews/odata?$filter=Rating ge 3 and Rating le 5

# Tam puan
http://localhost:5263/api/BusinessReviews/odata?$filter=Rating eq 5
```

#### Boolean Filtreleme Örnekleri:

```bash
# Onaylanmış yorumlar
http://localhost:5263/api/BusinessReviews/odata?$filter=IsApproved eq true

# Doğrulanmış yorumlar
http://localhost:5263/api/BusinessReviews/odata?$filter=IsVerified eq true

# Aktif kayıtlar
http://localhost:5263/api/BusinessReviews/odata?$filter=RowIsActive eq true
```

#### GUID Filtreleme Örnekleri:

```bash
# Belirli iş yeri ID'sine göre
http://localhost:5263/api/BusinessReviews/odata?$filter=BusinessId eq '12345678-1234-1234-1234-123456789012'

# Belirli kullanıcı ID'sine göre
http://localhost:5263/api/BusinessReviews/odata?$filter=ReviewerId eq '12345678-1234-1234-1234-123456789012'
```

#### Karmaşık Filtreleme Örnekleri:

```bash
# İş yeri adı ve puan kombinasyonu
http://localhost:5263/api/BusinessReviews/odata?$filter=BusinessName eq 'Aleppo' and Rating ge 4

# Onaylanmış ve doğrulanmış yorumlar
http://localhost:5263/api/BusinessReviews/odata?$filter=IsApproved eq true and IsVerified eq true

# Belirli tarih aralığında oluşturulan yorumlar
http://localhost:5263/api/BusinessReviews/odata?$filter=RowCreatedDate ge 2024-01-01T00:00:00Z and RowCreatedDate le 2024-12-31T23:59:59Z
```

### 2. $orderby - Sıralama

```bash
# Puanına göre azalan sıralama
http://localhost:5263/api/BusinessReviews/odata?$orderby=Rating desc

# Oluşturulma tarihine göre azalan sıralama
http://localhost:5263/api/BusinessReviews/odata?$orderby=RowCreatedDate desc

# Çoklu sıralama (önce puan, sonra tarih)
http://localhost:5263/api/BusinessReviews/odata?$orderby=Rating desc,RowCreatedDate desc

# İş yeri adına göre artan sıralama
http://localhost:5263/api/BusinessReviews/odata?$orderby=BusinessName asc
```

### 3. $select - Alan Seçimi

```bash
# Sadece belirli alanları getir
http://localhost:5263/api/BusinessReviews/odata?$select=Id,BusinessName,Rating,Comment

# Temel bilgiler
http://localhost:5263/api/BusinessReviews/odata?$select=Id,BusinessName,ReviewerName,Rating

# Tarih bilgileri
http://localhost:5263/api/BusinessReviews/odata?$select=Id,RowCreatedDate,RowUpdatedDate

# Kullanıcı bilgileri (YENİ!)
http://localhost:5263/api/BusinessReviews/odata?$select=Id,AuthUserName,CreateUserName,UpdateUserName,ReviewerName
```

### 4. $top ve $skip - Sayfalama

```bash
# İlk 10 kayıt
http://localhost:5263/api/BusinessReviews/odata?$top=10

# 20 kayıt atla, sonraki 10 kaydı getir (3. sayfa, sayfa başına 10 kayıt)
http://localhost:5263/api/BusinessReviews/odata?$skip=20&$top=10

# Sayfalama ile filtreleme
http://localhost:5263/api/BusinessReviews/odata?$filter=Rating ge 4&$top=5&$skip=0
```

### 5. $count - Toplam Sayı

```bash
# Toplam kayıt sayısını da getir
http://localhost:5263/api/BusinessReviews/odata?$count=true

# Filtrelenmiş sonuçların sayısı
http://localhost:5263/api/BusinessReviews/odata?$filter=Rating ge 4&$count=true
```

## Karmaşık Sorgu Örnekleri

### 1. Dashboard İçin Yüksek Puanlı Yorumlar

```bash
http://localhost:5263/api/BusinessReviews/odata?$filter=Rating ge 4 and IsApproved eq true&$orderby=Rating desc,RowCreatedDate desc&$top=10&$select=Id,BusinessName,ReviewerName,Rating,Comment
```

### 2. Belirli İş Yeri İçin Tüm Yorumlar

```bash
http://localhost:5263/api/BusinessReviews/odata?$filter=BusinessName eq 'Aleppo'&$orderby=RowCreatedDate desc&$count=true
```

### 3. Son 30 Günün Yorumları

```bash
http://localhost:5263/api/BusinessReviews/odata?$filter=RowCreatedDate ge 2024-01-01T00:00:00Z&$orderby=RowCreatedDate desc&$top=50
```

### 4. Bekleyen Onay Yorumları

```bash
http://localhost:5263/api/BusinessReviews/odata?$filter=IsApproved eq false&$orderby=RowCreatedDate asc&$select=Id,BusinessName,ReviewerName,Rating,Comment,RowCreatedDate
```

## Hata Durumları ve Çözümleri

### 1. Syntax Error - Quote Karakteri

**Hata:** `Syntax error: character '"' is not valid at position 16`
**Çözüm:** Çift tırnak yerine tek tırnak kullanın

```bash
# Yanlış
$filter=BusinessName eq "Aleppo"

# Doğru
$filter=BusinessName eq 'Aleppo'
```

### 2. Property Not Found

**Hata:** `Property 'BusinessName' does not exist`
**Çözüm:** Doğru property adını kullanın (case-sensitive)

### 3. Invalid Operator

**Hata:** `Operator 'contains' is not supported`
**Çözüm:** Desteklenen operatörleri kullanın

## Performans İpuçları

1. **Index'li alanları kullanın:** BusinessId, ReviewerId gibi
2. **$top ile limit koyun:** Büyük veri setleri için
3. **$select ile gereksiz alanları çıkarın**
4. **Karmaşık filtreleri basitleştirin**

## Test Etme

Postman veya curl ile test edebilirsiniz:

```bash
curl -X GET "http://localhost:5263/api/BusinessReviews/odata?$filter=BusinessName eq 'Aleppo'&$top=5" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## Güvenlik Notları

- Tüm OData endpoint'leri authentication gerektirir
- Admin yetkisi gerekli
- Maksimum 1000 node count limiti var
- Maksimum 100 top limiti var
