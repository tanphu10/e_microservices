﻿using Contracts.Common.Events;
using Ordering.Domain.OrderAggregate.Events;
using Shared.Enums.Order;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain.Entities
{
    public class Order : AuditableEventEntity<long>
    {
        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string UserName { get; set; }

        public Guid DocumentNo { get; set; } = Guid.NewGuid();
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string FirstName { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(250)")]
        public string EmailAddress { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ShippingAddress { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string InvoiceAddress { get; set; }
        public EOrderStatus Status  { get; set; }
        public Order AddedOrder()
        {
            AddDomainEvent(new OrderCreatedEvent(Id, UserName, DocumentNo.ToString(), TotalPrice, EmailAddress, ShippingAddress, InvoiceAddress));
            return this;
        }
        public Order DeleteOrder()
        {
            AddDomainEvent(new OrderDeletedEvent(Id));
            return this;
        }
}
}
