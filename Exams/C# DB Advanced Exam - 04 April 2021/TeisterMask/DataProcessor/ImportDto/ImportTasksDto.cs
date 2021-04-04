
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class ImportTasksDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; }
        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }
        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; }
        [Required]
        [Range(0,3)]
        [XmlElement("ExecutionType")]
        public int ExecutionType { get; set; }
        [Required]
        [XmlElement("LabelType")]
        [Range(0,4)]
        public int LabelType { get; set; }
    }
}
