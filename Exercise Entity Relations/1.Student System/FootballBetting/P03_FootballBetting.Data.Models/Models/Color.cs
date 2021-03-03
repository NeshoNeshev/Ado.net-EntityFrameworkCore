using System.Collections.Generic;
using P03_FootballBetting.Data.Models.Models;

namespace P03_FootballBetting.Data.Models
{
    public class Color
    {
        public Color()
        {
            this.PrimaryKitTeams = new HashSet<Team>();
            this.SecondaryKiTeams = new HashSet<Team>();
        }
        public int ColorId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Team> PrimaryKitTeams { get; set; }
        public virtual ICollection<Team> SecondaryKiTeams { get; set; }
    }
}
