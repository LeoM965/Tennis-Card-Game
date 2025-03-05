using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tennis_Card_Game.Data;
using Tennis_Card_Game.Models;
using System.Linq;

namespace Tennis_Card_Game.Controllers
{
    public class PlayerController : Controller
    {
        private const int BASE_XP_PER_LEVEL = 1000;
        private const double XP_GROWTH_MULTIPLIER = 1.75;
        private const int BASE_SKILL_POINTS = 1;
        private const int SKILL_POINTS_GROWTH_RATE = 2;
        private readonly Tennis_Card_GameContext _context;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(Tennis_Card_GameContext context, ILogger<PlayerController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> PlayerDetails(int id)
        {
            var player = await _context.Players
                .AsNoTracking()
                .Include(p => p.PlayingStyle)
                .Include(p => p.SpecialAbility)
                .Include(p => p.PlayerCards)
                    .ThenInclude(pc => pc.Card)
                        .ThenInclude(c => c.CardCategory)
                .Include(p => p.MatchesAsPlayer1)
                    .ThenInclude(m => m.Tournament)
                .Include(p => p.MatchesAsPlayer2)
                    .ThenInclude(m => m.Tournament)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
            {
                return NotFound();
            }

            var recentMatches = player.MatchesAsPlayer1
                .Concat(player.MatchesAsPlayer2)
                .OrderByDescending(m => m.StartTime)
                .Take(5)
                .ToList();

            var playerCards = player.PlayerCards
                .Where(pc => pc.InDeck)
                .ToList();

            int wins = CalculatePlayerWins(player);
            int losses = CalculatePlayerLosses(player);

            var recommendedCards = await _context.Cards
                .AsNoTracking()
                .Include(c => c.CardCategory)
                .Where(c => !player.PlayerCards.Select(pc => pc.CardId).Contains(c.Id))
                .Take(4)
                .ToListAsync();

            return View(new PlayerDetailsViewModel
            {
                Player = player,
                PlayerCards = playerCards,
                RecentMatches = recentMatches,
                Wins = wins,
                Losses = losses,
                RecommendedCards = recommendedCards
            });
        }

        private int CalculatePlayerWins(Player player) =>
            player.MatchesAsPlayer1.Count(m => m.IsCompleted && m.WinnerId == player.Id) +
            player.MatchesAsPlayer2.Count(m => m.IsCompleted && m.WinnerId == player.Id);

        private int CalculatePlayerLosses(Player player) =>
            player.MatchesAsPlayer1.Count(m => m.IsCompleted && m.WinnerId != null && m.WinnerId != player.Id) +
            player.MatchesAsPlayer2.Count(m => m.IsCompleted && m.WinnerId != null && m.WinnerId != player.Id);

        private async Task<int> EnsureValidPlayerId(int id)
        {
            if (id == 0)
            {
                var firstPlayer = await _context.Players
                    .OrderBy(p => p.Id)
                    .FirstOrDefaultAsync();

                if (firstPlayer == null)
                {
                    _logger.LogWarning("No players found in the database");
                    throw new InvalidOperationException("No players exist in the system");
                }

                return firstPlayer.Id;
            }
            return id;
        }

        [HttpPost]
        public async Task<IActionResult> UpgradePlayerSkill(int playerId, string upgradeType, int upgradeId)
        {
            var player = await _context.Players
                .Include(p => p.PlayerCards)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
            {
                return NotFound();
            }

            var skillPointsCost = CalculateSkillUpgradeCost(player);

            if (player.Experience < skillPointsCost)
            {
                ModelState.AddModelError("", "Not enough experience points for upgrade.");
                return RedirectToAction(nameof(PlayerProgression), new { playerId });
            }

            await PerformSkillUpgrade(player, upgradeType, upgradeId);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PlayerProgression), new { playerId });
        }

        private async Task PerformSkillUpgrade(Player player, string upgradeType, int upgradeId)
        {
            var skillPointsCost = CalculateSkillUpgradeCost(player);

            switch (upgradeType)
            {
                case "PlayingStyle":
                    var newPlayingStyle = await _context.PlayingStyles.FindAsync(upgradeId);
                    if (newPlayingStyle != null)
                    {
                        player.PlayingStyle = newPlayingStyle;
                        player.PlayingStyleId = newPlayingStyle.Id;
                        player.Experience -= skillPointsCost;
                        player.MaxEnergy += 5;
                    }
                    break;

                case "SpecialAbility":
                    var newSpecialAbility = await _context.SpecialAbilities.FindAsync(upgradeId);
                    if (newSpecialAbility != null)
                    {
                        player.SpecialAbility = newSpecialAbility;
                        player.SpecialAbilityId = newSpecialAbility.Id;
                        player.Experience -= skillPointsCost;
                        player.CurrentEnergy = player.MaxEnergy;
                    }
                    break;
            }
        }

        private int CalculateXpForNextLevel(int targetLevel)
        {
            return targetLevel == 1 ? 1000 : (int)(BASE_XP_PER_LEVEL * Math.Pow(XP_GROWTH_MULTIPLIER, targetLevel - 1));
        }
        private int CalculateXpToNextLevel(Player player)
        {
            if (player.Level == 1)
            {
                return 1000 - player.Experience;
            }

            int currentLevelXp = CalculateXpForNextLevel(player.Level);
            int nextLevelXp = CalculateXpForNextLevel(player.Level + 1);

            return nextLevelXp - player.Experience;
        }

        private int CalculateAvailableSkillPoints(Player player)
        {
            int basePoints = BASE_SKILL_POINTS + (player.Level / SKILL_POINTS_GROWTH_RATE);

            return player.PlayingStyle.Name switch
            {
                "Aggressive Baseliner" => basePoints + 1,
                "Serve and Volleyer" => basePoints + 1,
                "Defensive Baseliner" => basePoints,
                "Counter-Puncher" => basePoints + 1,
                "Big Server" => basePoints + 1,
                _ => basePoints
            };
        }

        private int CalculateSkillUpgradeCost(Player player)
        {
            if (player.Level <= 1)
                return 0;

            int baseCost = 750;
            int levelMultiplier = player.Level * 150;

            return player.PlayingStyle.Name switch
            {
                "Aggressive Baseliner" => baseCost + levelMultiplier + 250,
                "Serve and Volleyer" => baseCost + levelMultiplier + 300,
                "Defensive Baseliner" => baseCost + levelMultiplier + 200,
                "Counter-Puncher" => baseCost + levelMultiplier + 150,
                "Big Server" => baseCost + levelMultiplier + 350,
                _ => baseCost + levelMultiplier
            };
        }

        private void LevelUpPlayer(Player player)
        {
            int currentLevelXp = CalculateXpForNextLevel(player.Level);

            if (player.Experience >= currentLevelXp)
            {
                player.Level++;

                int energyBonus = player.PlayingStyle.Name switch
                {
                    "Aggressive Baseliner" => 8,
                    "Serve and Volleyer" => 10,
                    "Defensive Baseliner" => 6,
                    "Counter-Puncher" => 7,
                    "Big Server" => 9,
                    _ => 5
                };

                player.MaxEnergy += energyBonus + (player.Level * 2);
                player.CurrentEnergy = player.MaxEnergy;

                switch (player.Level)
                {
                    case 5:
                        player.Experience += 500;
                        break;
                }

                if (new Random().Next(20) == 0)
                {
                    player.Experience += 250;
                }
            }
        }
        private double CalculateLevelProgressPercentage(Player player)
        {
            if (player.Level == 1)
            {
                return Math.Min(100, (player.Experience / 1000.0) * 100);
            }

            int currentLevelXp = CalculateXpForNextLevel(player.Level);
            int nextLevelXp = CalculateXpForNextLevel(player.Level + 1);

            return Math.Min(100, (double)(player.Experience - currentLevelXp) / (nextLevelXp - currentLevelXp) * 100);
        }

        private int CalculateBonusXp(Player player, int baseXp)
        {
            double multiplier = 1.0;

            multiplier += player.PlayingStyle.Name switch
            {
                "Aggressive Baseliner" => 0.2,
                "Serve and Volleyer" => 0.15,
                "Defensive Baseliner" => 0.1,
                "Counter-Puncher" => 0.25,
                "Big Server" => 0.18,
                _ => 0.12
            };

            if (player.SpecialAbility != null)
            {
                multiplier += player.SpecialAbility.Name switch
                {
                    "Second Wind" => 0.1,
                    "Focus Mode" => 0.15,
                    "Power Surge" => 0.2,
                    "Card Draw" => 0.12,
                    _ => 0.05
                };
            }
            else
            {
                multiplier += 0.05;
            }

            return (int)(baseXp * multiplier);
        }

        private int CalculateTournamentXpBonus(Tournament tournament, bool isWinner)
        {
            int baseBonus = tournament.Level switch
            {
                "Grand Slam" => 1000,
                "Masters" => 500,
                "Regular" => 250,
                _ => 100
            };

            return isWinner ? baseBonus * 2 : baseBonus;
        }

        public async Task<IActionResult> PerformTraining(int playerId, string trainingModuleName)
        {
            var player = await _context.Players
                .Include(p => p.PlayingStyle)
                .Include(p => p.SpecialAbility)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
                return NotFound();

            // Ensure PlayingStyle is not null
            if (player.PlayingStyle == null)
                return BadRequest("Player's playing style is not set.");

            var trainingModule = GenerateTrainingModules(player)
                .FirstOrDefault(tm => tm.Name == trainingModuleName);

            if (trainingModule == null || player.CurrentEnergy < trainingModule.EnergyRequired)
                return BadRequest("Invalid training module or insufficient energy.");

            // Calculate bonus XP based on player's characteristics
            int baseXp = trainingModule.ExperienceReward;
            int bonusXp = CalculateBonusXp(player, baseXp);
            player.Experience += bonusXp;

            player.CurrentEnergy -= trainingModule.EnergyRequired;

            // Use advanced level up method
            LevelUpPlayer(player);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TrainingCamp), new { playerId });
        }

        public async Task<IActionResult> PlayerProgression(int id)
        {
            id = await EnsureValidPlayerId(id);

            var player = await _context.Players
                .AsNoTracking()
                .Include(p => p.PlayingStyle)
                .Include(p => p.SpecialAbility)
                .Include(p => p.PlayerCards)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
                return NotFound($"No player found with ID {id}");

            var progressionViewModel = new PlayerProgressionViewModel
            {
                Player = player,
                CurrentLevelXpRequirement = CalculateXpForNextLevel(player.Level),
                XpToNextLevel = CalculateXpToNextLevel(player),
                LevelProgressPercentage = CalculateLevelProgressPercentage(player),
                AvailableSkillPoints = CalculateAvailableSkillPoints(player),
                SkillUpgradeCost = CalculateSkillUpgradeCost(player),
                PossiblePlayingStyles = await _context.PlayingStyles
                    .AsNoTracking()
                    .Where(ps => ps.Id != player.PlayingStyleId)
                    .ToListAsync(),
                PossibleSpecialAbilities = await _context.SpecialAbilities
                    .AsNoTracking()
                    .Where(sa => sa.Id != player.SpecialAbilityId)
                    .ToListAsync(),
                RecommendedTrainingModules = GenerateRecommendedTrainingModules(player)
            };

            return View(progressionViewModel);
        }

        private List<TrainingModule> GenerateTrainingModules(Player player) =>
            new()
            {
                new TrainingModule
                {
                    Name = "Physical Conditioning",
                    Description = "Improve overall fitness and energy management",
                    ExperienceReward = 50 + (player.Level * 10),
                    EnergyRequired = 20
                },
                new TrainingModule
                {
                    Name = "Technical Skills",
                    Description = "Enhance precision and shot techniques",
                    ExperienceReward = 75 + (player.Level * 15),
                    EnergyRequired = 30
                },
                new TrainingModule
                {
                    Name = "Mental Training",
                    Description = "Develop focus and resilience",
                    ExperienceReward = 100 + (player.Level * 20),
                    EnergyRequired = 40
                }
            };

        private List<TrainingModule> GenerateRecommendedTrainingModules(Player player) =>
            GenerateTrainingModules(player)
                .Where(tm => tm.EnergyRequired <= player.CurrentEnergy)
                .ToList();

        public async Task<IActionResult> TrainingCamp(int id)
        {
            _logger.LogInformation($"TrainingCamp action called with playerId: {id}");

            var player = await _context.Players
                .Include(p => p.PlayingStyle)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
            {
                return RedirectToAction(nameof(PlayerList));
            }

            var trainingOptions = new TrainingCampViewModel
            {
                Player = player,
                AvailableTrainingModules = (id >= 201 && id <= 232)
                    ? new List<TrainingModule>()
                    : GenerateTrainingModules(player)
            };

            return View(trainingOptions);
        }

        public async Task<IActionResult> PlayerList()
        {
            var players = await _context.Players
                .Include(p => p.PlayingStyle)
                .ToListAsync();

            return View(players);
        }

        
        [HttpGet]
        public IActionResult CreatePlayer()
        {
            try
            {
                ViewBag.PlayingStyles = _context.PlayingStyles
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    })
                    .ToList();

                ViewBag.SpecialAbilities = _context.SpecialAbilities
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    })
                    .ToList();

                return View(new Player
                {
                    Level = 1,
                    Experience = 0,
                    MaxEnergy = 100,
                    CurrentEnergy = 100
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading CreatePlayer view");
                return StatusCode(500, "An error occurred while preparing the create player form");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlayer(Player player)
        {
            try
            {
                ViewBag.PlayingStyles = await _context.PlayingStyles
                    .Select(ps => new SelectListItem
                    {
                        Value = ps.Id.ToString(),
                        Text = ps.Name
                    })
                    .ToListAsync();

                ViewBag.SpecialAbilities = await _context.SpecialAbilities
                    .Select(sa => new SelectListItem
                    {
                        Value = sa.Id.ToString(),
                        Text = sa.Name
                    })
                    .ToListAsync();

                if (string.IsNullOrWhiteSpace(player.Name))
                {
                    ModelState.AddModelError(nameof(player.Name), "Player name is required.");
                    return View(player);
                }

                player.Name = player.Name.Trim();

                var existingPlayer = await _context.Players
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == player.Name.ToLower());

                if (existingPlayer != null)
                {
                    ModelState.AddModelError(nameof(player.Name), $"A player with the name '{player.Name}' already exists.");
                    return View(player);
                }

                var playingStyleExists = await _context.PlayingStyles
                    .AnyAsync(ps => ps.Id == player.PlayingStyleId);
                var specialAbilityExists = await _context.SpecialAbilities
                    .AnyAsync(sa => sa.Id == player.SpecialAbilityId);

                if (!playingStyleExists)
                {
                    ModelState.AddModelError(nameof(player.PlayingStyleId), "Selected Playing Style is invalid.");
                    return View(player);
                }

                if (!specialAbilityExists)
                {
                    ModelState.AddModelError(nameof(player.SpecialAbilityId), "Selected Special Ability is invalid.");
                    return View(player);
                }
                
                player.Level = 1;
                player.Experience = 0;
                player.MaxEnergy = 100;
                player.CurrentEnergy = 100;

                _context.Players.Add(player);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Player {player.Name} created successfully!";
                return RedirectToAction("PlayerList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating player: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred while creating the player.");
                return View(player);
            }
        }
    }
}