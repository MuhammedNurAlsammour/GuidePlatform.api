

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.BusinessContacts
{
  public class BusinessContactsDTO : BaseResponseDTO
  {
    // İşletme ID'si - hangi işletmeye ait iletişim bilgisi
    public Guid BusinessId { get; set; }

    // İletişim türü (telefon, email, adres, vb.)
    public string ContactType { get; set; } = string.Empty;

    // İletişim değeri (telefon numarası, email adresi, vb.)
    public string ContactValue { get; set; } = string.Empty;

    // Birincil iletişim bilgisi mi?
    public bool IsPrimary { get; set; } = false;

    // İkon türü (varsayılan: contact_phone)
    public string Icon { get; set; } = "contact_phone";
  }
}
