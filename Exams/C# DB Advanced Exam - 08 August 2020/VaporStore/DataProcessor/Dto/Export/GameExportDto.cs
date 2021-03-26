    
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("Game")]
    public class GameExportDto
    {
        [XmlAttribute("title")]
        public string Name { get; set; }
        [XmlElement("Genre")]
        public string GenreName { get; set; }
        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
