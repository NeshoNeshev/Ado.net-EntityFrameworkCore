using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }
        public PurchaseType Type { get; set; }
        [Required]
        public string ProductKey { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [ForeignKey("Card")]
        public int CardId { get; set; }
        [Required]
        public Card Card { get; set; }
        [Required]
        [ForeignKey("Game")]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }
    }
}
