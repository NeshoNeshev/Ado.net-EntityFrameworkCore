﻿using System;
using System.ComponentModel.DataAnnotations;
using PetStore.Common;
using PetStore.Models.Enumerations;

namespace PetStoreServiceModels.Products.ImputModels
{
    public class EditProductInputServiceModel 
    {
        [Required]
        [MinLength(GlobalConstants.ProductNameMinLength)]
        [MaxLength(GlobalConstants.ProductNameMaxLength)]
        public string Name { get; set; }

        [Range(GlobalConstants.ProductMinPrice, Double.MaxValue)]
        public decimal Price { get; set; }
        public string ProductType { get; set; }
    }
}
