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
                    .OrderByDescending(t => t.StartDate)
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

    }
}
        