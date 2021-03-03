
using P03_FootballBetting.Data.Models.Enumerations;

namespace P03_FootballBetting.Data.Models.Models
{
    public class Bet
    {
        public int BetId { get; set; }
        public decimal Amount { get; set; }
        public Prediction Prediction { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public User User { get; set; }
        public Game Game { get; set; }
    }
}
