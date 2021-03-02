﻿
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models
{
    public class Writer
    {
        public Writer()
        {
            this.Songs = new HashSet<Song>();
        }
        [Key]
        public int  Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public string Pseudonym  { get; set; }
        [Required]
        public ICollection<Song> Songs { get; set; }
    }
}
