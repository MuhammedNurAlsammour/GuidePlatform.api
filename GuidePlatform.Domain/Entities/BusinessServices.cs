using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
    public class BusinessServicesViewModel : BaseEntity
    {
        // İşletme ID'si - hangi işletmeye ait hizmet
        public Guid BusinessId { get; set; }
        
        // Hizmet adı
        public string ServiceName { get; set; } = string.Empty;
        
        // Hizmet açıklaması
        public string? ServiceDescription { get; set; }
        
        // Fiyat
        public decimal? Price { get; set; }
        
        // Para birimi (varsayılan: SYP)
        public string Currency { get; set; } = "SYP";
        
        // Hizmet mevcut mu?
        public bool IsAvailable { get; set; } = true;
        
        // İkon türü (varsayılan: miscellaneous_services)
        public string Icon { get; set; } = "miscellaneous_services";
    }
}