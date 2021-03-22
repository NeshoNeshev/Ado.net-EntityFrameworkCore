

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PetStore.Common;

namespace PetStore.Models
{
    public class Client
    {
        public Client()
        {
            this.Id = Guid.NewGuid().ToString();
            this.PetsBuyed = new HashSet<Pet>();
        }
        [Key]
        public string Id { get; set; }

        [Required]
        [MinLength(GlobalConstants.UserNameMinLength)]
        [MaxLength(GlobalConstants.UserNameMaxLength)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MinLength(GlobalConstants.EmailNameMinLength)]
        [MaxLength(GlobalConstants.EmailNameMaxLength)]
        public string Email { get; set; }

        [Required]
        [MinLength(GlobalConstants.ClientFirstNameMinLength)]
        [MaxLength(GlobalConstants.ClientFirstNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(GlobalConstants.ClientLastNameMinLength)]
        [MaxLength(GlobalConstants.ClientLastNameMaxLength)]
        public string LastName { get; set; }
        public virtual ICollection<Pet> PetsBuyed { get; set; }
    }

}
