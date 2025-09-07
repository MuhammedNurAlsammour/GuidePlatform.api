# OData Base Classes Kullanım Kılavuzu

## Genel Bakış

Bu proje için OData query'leri için kapsamlı base classes oluşturulmuştur. Bu base classes sayesinde her entity için OData handler'ları hızlı ve tutarlı bir şekilde oluşturabilirsiniz.

## Base Classes

### 1. BaseODataQueryRequest<TResponse>

OData query parametrelerini içeren base request class'ı.

**Özellikler:**

- `Filter`: OData filter parametresi
- `OrderBy`: OData orderby parametresi
- `Select`: OData select parametresi
- `Top`: Kaç kayıt alınacağı
- `Skip`: Kaç kayıt atlanacağı
- `Count`: Toplam kayıt sayısı istenip istenmediği
- `AuthUserId` ve `AuthCustomerId`: Auth filtreleri

**Kullanım:**

```csharp
public class GetODataBusinessesQueryRequest : BaseODataQueryRequest<GetODataBusinessesQueryResponse>
{
    // Ek özellikler gerekirse buraya eklenebilir
}
```

### 2. BaseODataQueryResponse<TDto>

OData response'ları için base class.

**Özellikler:**

- `StatusCode`: HTTP status code
- `Message`: Response mesajı
- `Timestamp`: Response timestamp
- `TotalCount`: Toplam kayıt sayısı
- `Data`: DTO listesi
- `Count`: OData count değeri
- `Pagination`: Sayfalama bilgileri

**Kullanım:**

```csharp
public class GetODataBusinessesQueryResponse : BaseODataQueryResponse<BusinessesDTO>
{
    public List<BusinessesDTO> Businesses
    {
        get => Data;
        set => Data = value;
    }
}
```

### 3. BaseODataQueryHandler<TRequest, TResponse, TDto>

OData işlemlerini yöneten base handler class'ı.

**Özellikler:**

- Otomatik OData filter parsing
- Kullanıcı adlarını otomatik doldurma
- Error handling
- Context validation
- Pagination support

## Yeni OData Handler Oluşturma

### 1. Plop Template Kullanarak

```bash
# Plop template ile otomatik oluşturma
npm run plop
# "OData Query" seçeneğini seçin
```

### 2. Manuel Oluşturma

#### Request Class

```csharp
public class GetODataBusinessesQueryRequest : BaseODataQueryRequest<GetODataBusinessesQueryResponse>
{
}
```

#### Response Class

```csharp
public class GetODataBusinessesQueryResponse : BaseODataQueryResponse<BusinessesDTO>
{
    public List<BusinessesDTO> Businesses
    {
        get => Data;
        set => Data = value;
    }
}
```

#### Handler Class

```csharp
public class GetODataBusinessesQueryHandler : BaseODataQueryHandler<GetODataBusinessesQueryRequest, GetODataBusinessesQueryResponse, BusinessesDTO>
{
    public GetODataBusinessesQueryHandler(
        IApplicationDbContext context,
        AuthDbContext authContext,
        UserManager<AppUser> userManager,
        ICurrentUserService currentUserService,
        IAuthUserDetailService authUserService) : base(context, authContext, userManager, currentUserService, authUserService)
    {
    }

    protected override IQueryable<BusinessesDTO> GetBaseODataQuery()
    {
        var query = from business in _context.businesses
                    where business.RowIsActive && !business.RowIsDeleted
                    select new BusinessesDTO
                    {
                        Id = business.Id,
                        Name = business.Name,
                        // Diğer property'ler...
                    };
        return query;
    }

    protected override IQueryable<BusinessesDTO> ApplyOrderBy(IQueryable<BusinessesDTO> query, string orderBy)
    {
        if (orderBy.Contains("Name desc"))
        {
            query = query.OrderByDescending(x => x.Name);
        }
        else if (orderBy.Contains("Name asc"))
        {
            query = query.OrderBy(x => x.Name);
        }
        else
        {
            query = query.OrderByDescending(x => x.RowCreatedDate);
        }
        return query;
    }

    protected override IQueryable<BusinessesDTO> ApplyContainsFilter(IQueryable<BusinessesDTO> query, string condition)
    {
        var match = System.Text.RegularExpressions.Regex.Match(condition, @"contains\((\w+),\s*'([^']+)'\)");
        if (match.Success)
        {
            var property = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            return property.ToLower() switch
            {
                "name" => query.Where(x => x.Name.Contains(value)),
                "description" => query.Where(x => x.Description.Contains(value)),
                _ => query
            };
        }
        return query;
    }

    protected override IQueryable<BusinessesDTO> ApplyEqualsFilter(IQueryable<BusinessesDTO> query, string condition)
    {
        var parts = condition.Split(new string[] { " eq " }, 2, StringSplitOptions.None);
        if (parts.Length == 2)
        {
            var property = parts[0].Trim();
            var value = parts[1].Trim().Trim('\'');
            return property.ToLower() switch
            {
                "id" => query.Where(x => x.Id == Guid.Parse(value)),
                "name" => query.Where(x => x.Name == value),
                "isactive" => query.Where(x => x.IsActive == bool.Parse(value)),
                _ => query
            };
        }
        return query;
    }
}
```

## Override Edilebilir Metodlar

### GetBaseODataQuery()

Ana OData query'sini oluşturur. **Mutlaka override edilmelidir.**

### EnrichWithUserNamesAsync()

Kullanıcı adlarını doldurur. Özel ihtiyaçlar için override edilebilir.

### ApplyOrderBy()

OrderBy işlemlerini yönetir. Entity'ye özel sıralama için override edilmelidir.

### ApplyContainsFilter()

Contains filter'larını yönetir. Entity'ye özel string filtreleri için override edilmelidir.

### ApplyEqualsFilter()

Equals filter'larını yönetir. Entity'ye özel eşitlik filtreleri için override edilmelidir.

### ApplyGreaterThanFilter(), ApplyLessThanFilter(), vb.

Diğer comparison filter'ları için override edilebilir.

## OData Query Örnekleri

### Filter Örnekleri

```
# Basit eşitlik
Rating eq 5

# String içerme
contains(BusinessName, 'Restaurant')

# Birden fazla koşul
Rating gt 3 and contains(BusinessName, 'Restaurant')

# Tarih karşılaştırması
RowCreatedDate gt 2024-01-01T00:00:00Z
```

### OrderBy Örnekleri

```
# Tek alan
Rating desc

# Birden fazla alan
Rating desc, BusinessName asc
```

### Pagination Örnekleri

```
# İlk 10 kayıt
$top=10

# 20 kayıt atla, 10 kayıt al
$skip=20&$top=10
```

## Avantajlar

1. **Tutarlılık**: Tüm OData handler'ları aynı yapıyı kullanır
2. **Hızlı Geliştirme**: Plop template ile otomatik oluşturma
3. **Bakım Kolaylığı**: Ortak kod base class'ta
4. **Hata Yönetimi**: Merkezi error handling
5. **Performans**: Optimize edilmiş query'ler
6. **Güvenlik**: Auth filtreleri otomatik uygulanır

## Dikkat Edilmesi Gerekenler

1. `GetBaseODataQuery()` metodunu mutlaka implement edin
2. Entity'ye özel filter'ları override edin
3. User ID'leri güvenli bir şekilde handle edin
4. Performance için gerekli index'leri oluşturun
5. Test yazmayı unutmayın

## Test Örnekleri

```csharp
[Test]
public async Task Handle_ValidRequest_ReturnsSuccessResult()
{
    // Arrange
    var request = new GetODataBusinessesQueryRequest
    {
        Filter = "Rating gt 3",
        OrderBy = "Rating desc",
        Top = 10
    };

    // Act
    var result = await _handler.Handle(request, CancellationToken.None);

    // Assert
    Assert.IsTrue(result.OperationStatus);
    Assert.IsNotNull(result.Result);
    Assert.IsTrue(result.Result.Data.Count <= 10);
}
```

Bu base classes sayesinde OData handler'larınızı hızlı ve tutarlı bir şekilde oluşturabilirsiniz.
