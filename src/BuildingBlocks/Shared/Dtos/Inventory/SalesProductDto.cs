using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Inventory
{
    public record SalesProductDto(string ExternalDocumentNo, int Quanity)
    {
        public EDocumentType DocumentType = EDocumentType.Sale;
        public string ItemNo { get; set; }
        public void SetItemNo(string itemNo)
        {
            ItemNo = itemNo;
        }
    }
}
