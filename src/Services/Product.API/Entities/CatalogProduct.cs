﻿using Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Product.API.Entities
{
    public class CatalogProduct : EntityAuditBase<long>
    {
        public long Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(150)")]
        public string No { get; set; }
        [Required]
        [Column(TypeName = "varchar(250)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Summary { get; set; }
        [Column(TypeName = "text")]
        public string Description { get; set; }
        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }
    }



}

