using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tennis_Card_Game.Models
{
    public class Tournament
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [StringLength(50)]
        public string Level { get; set; } // "Regular", "Masters", "Grand Slam"

        public int XpReward { get; set; }
        public int CoinReward { get; set; }

        [ForeignKey("Surface")]
        public int SurfaceId { get; set; }
        public virtual Surface Surface { get; set; }

        public virtual ICollection<Match> Matches { get; set; }

        public Tournament()
        {
            this.Matches = new HashSet<Match>();
            this.XpReward = 0;
            this.CoinReward = 0;

            this.Name = string.Empty; 
            this.Level = string.Empty;
            this.Surface = new Surface(); 
        }

        public Tournament(string name, DateTime startDate, DateTime endDate, Surface surface, string level, int xpReward, int coinReward)
            : this() 
        {
            this.Name = name;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Surface = surface;
            this.SurfaceId = surface.Id;
            this.Level = level;
            this.XpReward = xpReward;
            this.CoinReward = coinReward;
        }
    }
}