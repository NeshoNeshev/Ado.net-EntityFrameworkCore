﻿
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.Data.Models
{
    public class User
    {
        public User()
        {
            this.Cards= new HashSet<Card>();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Username  { get; set; }
        [Required]
        public string 	FullName  { get; set; }
        [Required]
        public string 	Email  { get; set; }
        [Range(3,103)]
        [Required]
        public int Age { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
