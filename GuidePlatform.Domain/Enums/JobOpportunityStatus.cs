namespace GuidePlatform.Domain.Enums
{
  /// <summary>
  /// JobOpportunity durumları - Job Opportunity Status
  /// </summary>
  public enum JobOpportunityStatus
  {
    /// <summary>
    /// Beklemede - Pending (Yeni oluşturulan, henüz onaylanmamış)
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Aktif - Active (Onaylanmış ve aktif)
    /// </summary>
    Active = 1,

    /// <summary>
    /// Onaylandı - Approved (İşveren tarafından onaylandı)
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Reddedildi - Denied (İşveren tarafından reddedildi)
    /// </summary>
    Denied = 3,

    /// <summary>
    /// Süresi doldu - Expired (Duration süresi doldu)
    /// </summary>
    Expired = 4,

    /// <summary>
    /// Pasif - Inactive (Manuel olarak deaktive edildi)
    /// </summary>
    Inactive = 5,

    /// <summary>
    /// Silindi - Deleted (Soft delete)
    /// </summary>
    Deleted = 6
  }
}
