using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Inventory
{
    public class SalesOrderDto
    {
        //Order's DocumentNo
        public string OrderNo { get; set; }
        public List<SaleItemDto> SaleItems { get; set; }
    }
}
