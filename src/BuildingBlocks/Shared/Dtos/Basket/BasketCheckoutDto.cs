using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Basket
{
    public class BasketCheckoutDto
    {
        [Required]
        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        private string _invoiceAddress;

        public string? InvoiceAddress { get=>_invoiceAddress; set =>_invoiceAddress=value ?? ShippingAddress; }
    }
}
