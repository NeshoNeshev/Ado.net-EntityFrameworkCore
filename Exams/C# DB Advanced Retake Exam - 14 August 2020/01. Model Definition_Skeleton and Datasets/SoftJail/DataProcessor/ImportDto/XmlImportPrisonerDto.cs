﻿
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Prisoner")]
    public class XmlImportPrisonerDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
