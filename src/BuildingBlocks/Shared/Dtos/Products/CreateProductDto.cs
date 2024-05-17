﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Products
{
    public class CreateProductDto : CreateOrUpdateProductDto
    {
        [Required]
        public string No { get; set; }
    }
}
