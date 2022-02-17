﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;


namespace BookShop.DataProcessor.ImportDto
{
    [XmlType("Book")]
    public class ImportBooksDto
    {
        [Required]
        
        [MinLength(3)]
        [MaxLength(30)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [Range(1, 3)]
        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }
        [XmlElement("Pages")]
        [Range(50,5000)]
        public int Pages { get; set; }
        [Required]
        [XmlElement("PublishedOn")]
        public string PublishedOn { get; set; }
    }
}


