using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BusinessContactsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.BusinessContacts.CreateBusinessContacts
{
  public class CreateBusinessContactsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBusinessContactsCommandResponse>>
  {

    // İşletme ID'si - hangi işletmeye ait iletişim bilgisi
    [Required(ErrorMessage = "BusinessId is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format for BusinessId")]
    public string BusinessId { get; set; } = string.Empty;

    // İletişim türü (telefon, email, adres, vb.)
    [Required(ErrorMessage = "ContactType is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "ContactType must be between 1 and 50 characters")]
    public string ContactType { get; set; } = string.Empty;

    // İletişim değeri (telefon numarası, email adresi, vb.)
    [Required(ErrorMessage = "ContactValue is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "ContactValue must be between 1 and 255 characters")]
    public string ContactValue { get; set; } = string.Empty;

    // Birincil iletişim bilgisi mi?
    public bool IsPrimary { get; set; } = false;

    // İkon türü (varsayılan: contact_phone)
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "contact_phone";


    public static O Map(CreateBusinessContactsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId
        BusinessId = Guid.Parse(request.BusinessId),
        ContactType = request.ContactType.Trim(),
        ContactValue = request.ContactValue.Trim(),
        IsPrimary = request.IsPrimary,
        Icon = string.IsNullOrWhiteSpace(request.Icon) ? "contact_phone" : request.Icon.Trim(),
      };
    }
  }
}

