using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tennis_Card_Game.Data;

namespace Tennis_Card_Game.Controllers
{
    public class HomeController : Controller
    {
        private readonly Tennis_Card_GameContext _context;
        public HomeController(Tennis_Card_GameContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new GameIntroductionVM
            {
                BasicRules = new List<string>
                {
                    "Win 6 games with at least a 2-point difference",
                    "Manage energy to avoid fatigue",
                    "Combine cards for synergy bonuses",
                    "Use special abilities strategically"
                },
                StarterPlayers = await _context.Players
                    .Include(p => p.PlayingStyle)
                    .Where(p => p.Level <= 5)
                    .Take(3)
                    .ToListAsync(),
                EssentialCards = await _context.Cards
                    .Include(c => c.CardCategory)
                    .Where(c => c.EnergyConsumption <= 30)
                    .OrderBy(c => c.CardCategory.Name)
                    .Take(6)
                    .ToListAsync(),
                RecentTournaments = await _context.Tournaments
                    .Include(t => t.Surface)
                    .OrderByDescending(t => t.StartTime)
                    .Take(2)
                    .ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Browse(string name = null, string subCategory = null, string surface = null)
        {
            var query = _context.Cards
                .Include(c => c.CardCategory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.CardCategory.Name == name);
            }

            if (!string.IsNullOrEmpty(subCategory))
            {
                query = query.Where(c => c.CardCategory.SubCategory == subCategory);
            }

            var categories = await _context.CardCategories
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();

            var subcategories = await _context.CardCategories
                .Select(c => c.SubCategory)
                .Distinct()
                .ToListAsync();

            var surfaces = await _context.Surfaces.ToListAsync();

            var model = new BrowseCardsVM
            {
                Cards = await query.ToListAsync(),
                Categories = categories,
                SelectedCategory = name,
                SubCategories = subcategories,
                SelectedSubCategory = subCategory,
                Surfaces = surfaces,
                SelectedSurface = surface
            };

            return View(model);
        }

        public async Task<IActionResult> GameDashboard()
        {
            var topPlayers = await _context.Players
                .Include(p => p.PlayingStyle)
                .OrderByDescending(p => p.Level)
                .ThenByDescending(p => p.Experience)
                .Take(5)
                .ToListAsync();

            var popularCards = await _context.PlayedCards
                .Include(pc => pc.Card)
                .ThenInclude(c => c.CardCategory)
                .GroupBy(pc => new { pc.Card.Id, pc.Card.Name, CategoryName = pc.Card.CardCategory.Name })
                .Select(g => new CardStatistic
                {
                    CardId = g.Key.Id,
                    CardName = g.Key.Name,
                    CategoryName = g.Key.CategoryName,
                    UsageCount = g.Count()
                })
                .OrderByDescending(c => c.UsageCount)
                .Take(5)
                .ToListAsync();

            var surfaceStats = await _context.Matches
                .Include(m => m.Surface)
                .Include(m => m.Sets)
                .Where(m => m.IsCompleted)
                .GroupBy(m => new { m.Surface.Id, m.Surface.Name })
                .Select(g => new SurfaceStatistic
                {
                    SurfaceId = g.Key.Id,
                    SurfaceName = g.Key.Name,
                    MatchCount = g.Count(),
                    AverageGamesPerSet = g.SelectMany(m => m.Sets)
                                           .Average(s => s.Player1Games + s.Player2Games)
                })
                .ToListAsync();

            var recentMatches = await _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Include(m => m.Tournament)
                .Include(m => m.Surface)
                .Where(m => m.IsCompleted)
                .OrderByDescending(m => m.EndTime)
                .Take(10)
                .ToListAsync();

            var styleDistribution = await _context.Players
                .Include(p => p.PlayingStyle)
                .GroupBy(p => new { p.PlayingStyle.Id, p.PlayingStyle.Name })
                .Select(g => new PlayingStyleDistribution
                {
                    StyleId = g.Key.Id,
                    StyleName = g.Key.Name,
                    PlayerCount = g.Count()
                })
                .ToListAsync();

            var model = new GameDashboardViewModel
            {
                TopPlayers = topPlayers,
                PopularCards = popularCards,
                SurfaceStatistics = surfaceStats,
                RecentMatches = recentMatches,
                StyleDistribution = styleDistribution,
                TotalPlayers = await _context.Players.CountAsync(),
                TotalCards = await _context.Cards.CountAsync(),
                TotalMatches = await _context.Matches.CountAsync()
            };

            return View(model);
        }

    }
}
        