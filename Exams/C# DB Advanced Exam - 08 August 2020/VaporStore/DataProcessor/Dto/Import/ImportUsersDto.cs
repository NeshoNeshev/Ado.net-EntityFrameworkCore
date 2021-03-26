using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUsersDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        public string Username { get; set; }
        [Required]
        [RegularExpression(@"^([A-Z]{1}[a-z]{1,}) ([A-Z]{1}[a-z]{1,})$")]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Range(3, 103)]
        [Required]
        public int Age { get; set; }

        public ImportCardsDto[] Cards { get; set; }
    }
}
