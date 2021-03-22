using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetStore.Common;
using PetStore.Models.Enumerations;

namespace PetStore.Models
{
    public class Pet
    {
        public Pet()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        [Key]
        public string Id { get; set; }

        [Required]
        [MinLength(GlobalConstants.PetNameMinLength)]
        [MaxLength(GlobalConstants.PetNameMaxLength)]
        public string Name { get; set; }

        [Range(GlobalConstants.PetMinAge, GlobalConstants.PetMaxAge)]
        public byte Age { get; set; }
        public bool IsSold { get; set; }

        [Range(GlobalConstants.PetMinPrice, Double.MaxValue)]
        public decimal Price { get; set; }

        public Gender Gender { get; set; }

        [Required]
        [ForeignKey("Breed")]
        public int BreedId { get; set; }
        public virtual Breed Breed { get; set; }
        [ForeignKey("Client")]
        public string ClientId { get; set; }
        public virtual Client Client { get; set; }

    }
}
