using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tennis_Card_Game.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int Level { get; set; }
        public int Experience { get; set; }
        public int MaxEnergy { get; set; }
        public int CurrentEnergy { get; set; }

        [ForeignKey("PlayingStyle")]
        public int PlayingStyleId { get; set; }
        public virtual PlayingStyle PlayingStyle { get; set; }

        [ForeignKey("SpecialAbility")]
        public int SpecialAbilityId { get; set; }
        public virtual SpecialAbility SpecialAbility { get; set; }

        public bool SpecialAbilityUsed { get; set; }
        public string Momentum { get; set; } // "InForm", "UnderPressure", "Neutral"

        public virtual ICollection<PlayerCard> PlayerCards { get; set; }
        public virtual ICollection<Match> MatchesAsPlayer1 { get; set; }
        public virtual ICollection<Match> MatchesAsPlayer2 { get; set; }

        public Player()
        {
            this.PlayerCards = new HashSet<PlayerCard>();
            this.MatchesAsPlayer1 = new HashSet<Match>();
            this.MatchesAsPlayer2 = new HashSet<Match>();
            this.Level = 1;
            this.Experience = 0;
            this.MaxEnergy = 100;
            this.CurrentEnergy = 100;
            this.Momentum = "Neutral";
            this.SpecialAbilityUsed = false;

            this.Name = string.Empty; 
            this.PlayingStyle = new PlayingStyle(); 
            this.SpecialAbility = new SpecialAbility(); 
        }

        public Player(string name, PlayingStyle playingStyle, SpecialAbility specialAbility)
            : this() 
        {
            this.Name = name;
            this.PlayingStyle = playingStyle;
            this.PlayingStyleId = playingStyle.Id;
            this.SpecialAbility = specialAbility;
            this.SpecialAbilityId = specialAbility.Id;
        }
    }
}