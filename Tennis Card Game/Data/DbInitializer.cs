using Tennis_Card_Game.Data;
using Tennis_Card_Game.Models;

namespace TennisCardBattle.Data
{
    public static class DbInitializer
    {
        public static void Initialize(Tennis_Card_GameContext context)
        {
            if (context.Surfaces.Any()) return;
            // Populate playing surfaces
            var surfaces = new List<Surface>
            {
                new("Clay", "Slow surface that favors defensive players. The ball has a high bounce."),
                new("Grass", "Fast surface that favors offensive players and those with powerful serves."),
                new("Hard Court", "Medium-speed surface that offers a balance between attack and defense."),
                new("Indoor", "Fast surface, without environmental factors influencing the game.")
            };
            context.Surfaces.AddRange(surfaces);

            // Populate weather conditions
            var weatherConditions = new List<WeatherCondition>
            {
                new("Sunny", 5, 10) { Description = "Ideal conditions, slight advantage for offensive players." },
                new("Rainy", -10, 15) { Description = "Court becomes slower, ball becomes heavier." },
                new("Windy", 0, 20) { Description = "Wind makes ball control more difficult, increases energy consumption." },
                new("Hot", -5, 25) { Description = "Extreme heat increases fatigue and energy consumption." },
                new("Cold", 0, 10) { Description = "Low temperatures make the ball slower." }
            };
            context.WeatherConditions.AddRange(weatherConditions);

            // Populate card categories
            var cardCategories = new List<CardCategory>
            {
                new("Shot", "Serve") { },
                new("Shot", "Forehand") { },
                new("Shot", "Backhand") { },
                new("Shot", "Volley") { },
                new("Shot", "Smash") { },
                new("Shot", "Drop Shot") { },
                new("Shot", "Lob") { },
                new("Shot", "Slice") { },
                new("Positioning", "Net") { },
                new("Positioning", "Baseline") { },
                new("Positioning", "Middle Court") { },
                new("Strategy", "Defensive") { },
                new("Strategy", "Offensive") { },
                new("Strategy", "Counter") { },
                new("Strategy", "Rally") { },
                new("Mental", "Focus") { },
                new("Mental", "Comeback") { },
                new("Mental", "Clutch") { }
            };
            context.CardCategories.AddRange(cardCategories);

            // Populate playing styles
            var playingStyles = new List<PlayingStyle>
            {
                new("All-Court", 5, 5, 0) { Description = "Balanced style, without major weaknesses but without specific advantages." },
                new("Aggressive Baseliner", 15, -5, -5) { Description = "Hits hard from the baseline, but consumes more energy." },
                new("Defensive Baseliner", -5, 15, 0) { Description = "Excels in defense and counterattack, but with limited offensive power." },
                new("Serve and Volleyer", 10, 0, -10) { Description = "Powerful serve followed by net attack, but with high energy consumption." },
                new("Counter-Puncher", 0, 10, 5) { Description = "Uses opponent's mistakes, conserving energy." },
                new("Big Server", 20, -10, -5) { Description = "Devastating serve, but weak defenses." }
            };
            context.PlayingStyles.AddRange(playingStyles);

            // Populate special abilities
            var specialAbilities = new List<SpecialAbility>
            {
                new("Second Wind", "EnergyRestore", 30) { Description = "Restores 30 energy points once per match." },
                new("Focus Mode", "PrecisionBoost", 25) { Description = "Increases precision by 25% for the next 3 cards." },
                new("Power Surge", "PowerBoost", 25) { Description = "Increases power by 25% for the next card." },
                new("Card Draw", "NewCards", 2) { Description = "Draws two new cards to your hand." },
                new("Mind Games", "OpponentDebuff", 15) { Description = "Reduces opponent's precision by 15% for the next 2 cards." },
                new("Momentum Shift", "ChangeMomentum", 1) { Description = "Changes momentum in your favor." },
                new("Surface Master", "SurfaceBonus", 20) { Description = "Activates a 20% bonus on the current surface for 3 cards." },
                new("Ace King", "ServeBonus", 30) { Description = "Increases serve power by 30% for the next game." }
            };
            context.SpecialAbilities.AddRange(specialAbilities);

            // Populate cards
            var cards = new List<Card>
            {
                // Serve cards
                new("Powerful First Serve", cardCategories[0], 80, 60, 15) { Description = "A powerful serve with good chances of winning the point directly.", ClayBonus = 0, GrassBonus = 15, HardCourtBonus = 10 },
                new("Precise Second Serve", cardCategories[0], 50, 85, 10) { Description = "A safety serve with high precision.", ClayBonus = 5, GrassBonus = 5, HardCourtBonus = 5 },
                new("Slice Serve", cardCategories[0], 60, 75, 12) { Description = "A serve with side spin that pulls the opponent out of position.", ClayBonus = 5, GrassBonus = 15, HardCourtBonus = 10 },
                new("Kick Serve", cardCategories[0], 65, 70, 13) { Description = "A serve with high bounce, difficult to return.", ClayBonus = 15, GrassBonus = 0, HardCourtBonus = 10 },
                new("Body Serve", cardCategories[0], 70, 65, 12) { Description = "A serve directed at the opponent's body, limiting return options.", ClayBonus = 5, GrassBonus = 10, HardCourtBonus = 10 },
                
                // Forehand cards
                new("Inside-Out Forehand", cardCategories[1], 75, 70, 15) { Description = "A powerful cross-court forehand shot used to open up the court.", ClayBonus = 10, GrassBonus = 5, HardCourtBonus = 10 },
                new("Forehand Down the Line", cardCategories[1], 80, 65, 17) { Description = "A risky forehand shot down the line.", ClayBonus = 5, GrassBonus = 15, HardCourtBonus = 10 },
                new("Defensive Forehand", cardCategories[1], 40, 85, 10) { Description = "A safety shot to stay in the point.", ClayBonus = 15, GrassBonus = 0, HardCourtBonus = 5 },
                new("Forehand Winner", cardCategories[1], 90, 60, 20) { Description = "An offensive forehand shot with the intention to win the point directly.", ClayBonus = 0, GrassBonus = 15, HardCourtBonus = 10 },
                new("Running Forehand", cardCategories[1], 60, 70, 18) { Description = "A forehand shot hit while on the move, difficult to execute.", ClayBonus = 10, GrassBonus = 5, HardCourtBonus = 10 },
                
                // Backhand cards
                new("Two-Handed Backhand", cardCategories[2], 70, 75, 14) { Description = "A solid two-handed backhand shot.", ClayBonus = 10, GrassBonus = 5, HardCourtBonus = 10 },
                new("One-Handed Backhand", cardCategories[2], 75, 65, 15) { Description = "An elegant one-handed backhand shot.", ClayBonus = 5, GrassBonus = 10, HardCourtBonus = 10 },
                new("Backhand Slice", cardCategories[2], 45, 85, 12) { Description = "A defensive slice shot that keeps the ball low.", ClayBonus = 5, GrassBonus = 15, HardCourtBonus = 10 },
                new("Backhand Down the Line", cardCategories[2], 80, 60, 16) { Description = "An offensive backhand shot down the line.", ClayBonus = 5, GrassBonus = 10, HardCourtBonus = 10 },
                new("Backhand Cross", cardCategories[2], 65, 75, 14) { Description = "A cross-court backhand shot, combining safety with angle.", ClayBonus = 10, GrassBonus = 5, HardCourtBonus = 10 },
                
                // Volley cards
                new("Forehand Volley", cardCategories[3], 65, 75, 12) { Description = "A forehand volley shot at the net.", ClayBonus = 0, GrassBonus = 15, HardCourtBonus = 10 },
                new("Backhand Volley", cardCategories[3], 60, 80, 12) { Description = "A backhand volley shot at the net.", ClayBonus = 0, GrassBonus = 15, HardCourtBonus = 10 },
                new("Drop Volley", cardCategories[3], 55, 85, 14) { Description = "A soft volley that lands close to the net.", ClayBonus = 5, GrassBonus = 15, HardCourtBonus = 10 },
                new("Reflex Volley", cardCategories[3], 60, 70, 15) { Description = "A reactive volley to a powerful shot.", ClayBonus = 0, GrassBonus = 15, HardCourtBonus = 10 },
                new("Swinging Volley", cardCategories[3], 75, 65, 16) { Description = "An aggressive volley executed with a full swing.", ClayBonus = 5, GrassBonus = 10, HardCourtBonus = 10 },
                
                // Smash cards
                new("Overhead Smash", cardCategories[4], 85, 70, 16) { Description = "A powerful overhead shot.", ClayBonus = 5, GrassBonus = 10, HardCourtBonus = 10 },
                new("Jumping Smash", cardCategories[4], 90, 60, 18) { Description = "A smash executed with a jump for maximum power.", ClayBonus = 5, GrassBonus = 15, HardCourtBonus = 10 },
                
                // Drop Shot cards
                new("Forehand Drop Shot", cardCategories[5], 40, 90, 15) { Description = "A delicate shot that lands close to the net on the forehand side.", ClayBonus = 15, GrassBonus = 10, HardCourtBonus = 5 },
                new("Backhand Drop Shot", cardCategories[5], 35, 95, 15) { Description = "A delicate shot that lands close to the net on the backhand side.", ClayBonus = 15, GrassBonus = 10, HardCourtBonus = 5 },
                
                // Lob cards
                new("Defensive Lob", cardCategories[6], 45, 85, 14) { Description = "A high shot to gain time and return to position.", ClayBonus = 15, GrassBonus = 5, HardCourtBonus = 10 },
                new("Offensive Lob", cardCategories[6], 60, 75, 16) { Description = "A high shot over an opponent at the net.", ClayBonus = 10, GrassBonus = 5, HardCourtBonus = 10 },
                
                // Slice cards
                new("Deep Slice", cardCategories[7], 55, 80, 13) { Description = "A slice shot that lands deep near the baseline.", ClayBonus = 10, GrassBonus = 15, HardCourtBonus = 10 },
                new("Approach Slice", cardCategories[7], 60, 75, 14) { Description = "A slice shot used to prepare for approaching the net.", ClayBonus = 5, GrassBonus = 15, HardCourtBonus = 10 },
                
                // Positioning cards
                new("Net Position", cardCategories[8], 65, 70, 15) { Description = "Positioning at the net to intercept the ball.", ClayBonus = 0, GrassBonus = 15, HardCourtBonus = 10 },
                new("Baseline Position", cardCategories[9], 60, 75, 12) { Description = "Positioning at the baseline for solid shots.", ClayBonus = 15, GrassBonus = 5, HardCourtBonus = 10 },
                new("Center Court Coverage", cardCategories[10], 65, 80, 15) { Description = "Central positioning for maximum court coverage.", ClayBonus = 10, GrassBonus = 10, HardCourtBonus = 10 },
                
                // Strategy cards
                new("Defensive Play", cardCategories[11], 50, 85, 10) { Description = "Defensive play tactic, waiting for opponent's mistake.", ClayBonus = 15, GrassBonus = 0, HardCourtBonus = 5 },
                new("Aggressive Play", cardCategories[12], 85, 60, 18) { Description = "Aggressive play tactic, looking for winning shots.", ClayBonus = 0, GrassBonus = 15, HardCourtBonus = 10 },
                new("Counter Attack", cardCategories[13], 75, 70, 15) { Description = "Transforms defense into quick attack.", ClayBonus = 10, GrassBonus = 5, HardCourtBonus = 10 },
                new("Extended Rally", cardCategories[14], 55, 80, 15) { Description = "Keeps the ball in play to tire the opponent.", ClayBonus = 15, GrassBonus = 0, HardCourtBonus = 5 },
                
                // Mental cards
                new("Mental Focus", cardCategories[15], 60, 90, 10) { Description = "Mental concentration to reduce errors.", ClayBonus = 10, GrassBonus = 10, HardCourtBonus = 10 },
                new("Comeback Spirit", cardCategories[16], 75, 75, 15) { Description = "Determination to come back when behind.", ClayBonus = 10, GrassBonus = 10, HardCourtBonus = 10 },
                new("Clutch Performance", cardCategories[17], 80, 80, 20) { Description = "High performance in important moments of the match.", ClayBonus = 10, GrassBonus = 10, HardCourtBonus = 10 },
                
                // Wild Cards
                new("Tweener", cardCategories[1], 70, 50, 25) { Description = "Spectacular shot executed between the legs.", IsWildCard = true, ClayBonus = 5, GrassBonus = 5, HardCourtBonus = 5 },
                new("Behind-the-Back Shot", cardCategories[1], 65, 55, 25) { Description = "Shot executed behind the back, surprising the opponent.", IsWildCard = true, ClayBonus = 5, GrassBonus = 5, HardCourtBonus = 5 },
                new("Underarm Serve", cardCategories[0], 40, 80, 15) { Description = "A serve executed underhand, surprising the opponent.", IsWildCard = true, ClayBonus = 10, GrassBonus = 5, HardCourtBonus = 5 }
            };
            context.Cards.AddRange(cards);

            // Populate card synergies
            var cardSynergies = new List<CardSynergy>
            {
                new(cards[0], cards[15], 20, "Powerful first serve followed by forehand volley"),
                new(cards[2], cards[16], 20, "Slice serve followed by backhand volley"),
                new(cards[5], cards[10], 15, "Inside-out forehand combined with two-handed backhand"),
                new(cards[6], cards[28], 25, "Forehand down the line after net positioning"),
                new(cards[30], cards[8], 30, "Aggressive play with forehand winner"),
                new(cards[29], cards[32], 20, "Defensive play with extended rally"),
                new(cards[23], cards[19], 25, "Drop shot followed by smash"),
                new(cards[3], cards[31], 15, "Kick serve followed by counter attack"),
                new(cards[14], cards[25], 20, "Backhand cross combined with deep slice"),
                new(cards[34], cards[35], 35, "Mental focus in clutch moments"),
                new(cards[28], cards[17], 20, "Net position with drop volley"),
                new(cards[7], cards[24], 15, "Defensive forehand followed by defensive lob"),
                new(cards[27], cards[31], 25, "Approach slice followed by counter attack"),
                new(cards[20], cards[28], 20, "Overhead smash after net positioning"),
                new(cards[2], cards[11], 15, "Slice serve followed by one-handed backhand")
            };
            context.CardSynergies.AddRange(cardSynergies);

            // Populate players
            var players = new List<Player>
            {
                new("Roger F.", playingStyles[0], specialAbilities[5]) { Level = 5, Experience = 2500, MaxEnergy = 120, CurrentEnergy = 120 },
                new("Rafa N.", playingStyles[2], specialAbilities[6]) { Level = 5, Experience = 2400, MaxEnergy = 130, CurrentEnergy = 130 },
                new("Novak D.", playingStyles[0], specialAbilities[0]) { Level = 5, Experience = 2600, MaxEnergy = 125, CurrentEnergy = 125 },
                new("Andy M.", playingStyles[2], specialAbilities[2]) { Level = 4, Experience = 1800, MaxEnergy = 115, CurrentEnergy = 115 },
                new("Serena W.", playingStyles[1], specialAbilities[1]) { Level = 5, Experience = 2700, MaxEnergy = 110, CurrentEnergy = 110 },
                new("Simona H.", playingStyles[2], specialAbilities[3]) { Level = 4, Experience = 1900, MaxEnergy = 115, CurrentEnergy = 115 },
                new("Maria S.", playingStyles[1], specialAbilities[4]) { Level = 4, Experience = 1700, MaxEnergy = 105, CurrentEnergy = 105 },
                new("Pete S.", playingStyles[3], specialAbilities[7]) { Level = 4, Experience = 2000, MaxEnergy = 110, CurrentEnergy = 110 },
                new("Andre A.", playingStyles[1], specialAbilities[1]) { Level = 4, Experience = 1950, MaxEnergy = 110, CurrentEnergy = 110 },
                new("Steffi G.", playingStyles[0], specialAbilities[2]) { Level = 4, Experience = 2100, MaxEnergy = 105, CurrentEnergy = 105 },
                new("Björn B.", playingStyles[2], specialAbilities[6]) { Level = 3, Experience = 1500, MaxEnergy = 105, CurrentEnergy = 105 },
                new("John M.", playingStyles[3], specialAbilities[7]) { Level = 3, Experience = 1400, MaxEnergy = 100, CurrentEnergy = 100 },
                new("Jimmy C.", playingStyles[4], specialAbilities[3]) { Level = 3, Experience = 1350, MaxEnergy = 100, CurrentEnergy = 100 },
                new("Monica S.", playingStyles[1], specialAbilities[2]) { Level = 3, Experience = 1550, MaxEnergy = 100, CurrentEnergy = 100 },
                new("Martina N.", playingStyles[3], specialAbilities[0]) { Level = 3, Experience = 1600, MaxEnergy = 100, CurrentEnergy = 100 },
                new("Chris E.", playingStyles[2], specialAbilities[4]) { Level = 3, Experience = 1450, MaxEnergy = 100, CurrentEnergy = 100 },
                new("Boris B.", playingStyles[3], specialAbilities[7]) { Level = 2, Experience = 950, MaxEnergy = 95, CurrentEnergy = 95 },
                new("Player", playingStyles[0], specialAbilities[0]) { Level = 1, Experience = 0, MaxEnergy = 100, CurrentEnergy = 100 }
            };
            context.Players.AddRange(players);

            // Populate tournaments
            var tournaments = new List<Tournament>
            {
                new("Australian Open", new DateTime(2023, 1, 15), new DateTime(2023, 1, 29), surfaces[2], "Grand Slam", 1000, 5000),
                new("Roland Garros", new DateTime(2023, 5, 28), new DateTime(2023, 6, 11), surfaces[0], "Grand Slam", 1000, 5000),
                new("Wimbledon", new DateTime(2023, 7, 3), new DateTime(2023, 7, 16), surfaces[1], "Grand Slam", 1000, 5000),
                new("US Open", new DateTime(2023, 8, 28), new DateTime(2023, 9, 10), surfaces[2], "Grand Slam", 1000, 5000),
                new("Madrid Open", new DateTime(2023, 4, 25), new DateTime(2023, 5, 7), surfaces[0], "Masters", 500, 2500),
                new("Italian Open", new DateTime(2023, 5, 10), new DateTime(2023, 5, 21), surfaces[0], "Masters", 500, 2500),
                new("Miami Open", new DateTime(2023, 3, 22), new DateTime(2023, 4, 2), surfaces[2], "Masters", 500, 2500),
                new("Indian Wells", new DateTime(2023, 3, 8), new DateTime(2023, 3, 19), surfaces[2], "Masters", 500, 2500),
                new("Cincinnati Masters", new DateTime(2023, 8, 14), new DateTime(2023, 8, 20), surfaces[2], "Masters", 500, 2500),
                new("Queen's Club", new DateTime(2023, 6, 19), new DateTime(2023, 6, 25), surfaces[1], "Regular", 250, 1000),
                new("Dubai Tennis Championships", new DateTime(2023, 2, 20), new DateTime(2023, 2, 26), surfaces[2], "Regular", 250, 1000),
                new("Stuttgart Open", new DateTime(2023, 6, 12), new DateTime(2023, 6, 18), surfaces[1], "Regular", 250, 1000)
            };
            context.Tournaments.AddRange(tournaments);

            context.SaveChanges();
        }
    }
}