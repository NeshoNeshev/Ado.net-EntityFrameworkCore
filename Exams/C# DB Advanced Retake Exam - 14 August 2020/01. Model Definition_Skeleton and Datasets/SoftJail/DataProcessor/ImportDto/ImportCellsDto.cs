﻿
using System.ComponentModel.DataAnnotations;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportCellsDto
    {
        [Required]
        [Range(1, 1000)]
        public int CellNumber { get; set; }
        [Required]
        public bool HasWindow { get; set; }
    }
}
