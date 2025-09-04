using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.BusinessContactsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessContacts.UpdateBusinessContacts
{
  public class UpdateBusinessContactsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateBusinessContactsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

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

    public static O Map(O entity, UpdateBusinessContactsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // İşletme ID'si güncelleme
      if (!string.IsNullOrWhiteSpace(request.BusinessId))
        entity.BusinessId = Guid.Parse(request.BusinessId);

      // İletişim türü güncelleme
      if (!string.IsNullOrWhiteSpace(request.ContactType))
        entity.ContactType = request.ContactType.Trim();

      // İletişim değeri güncelleme
      if (!string.IsNullOrWhiteSpace(request.ContactValue))
        entity.ContactValue = request.ContactValue.Trim();

      // Birincil iletişim bilgisi güncelleme
      entity.IsPrimary = request.IsPrimary;

      // İkon güncelleme
      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();
      else
        entity.Icon = "contact_phone"; // Varsayılan değer

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
