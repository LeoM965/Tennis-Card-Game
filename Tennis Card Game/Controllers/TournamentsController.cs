using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tennis_Card_Game.Data;
using Tennis_Card_Game.ViewModel;

namespace TennisCardBattle.Controllers
{
    public class TournamentsController : Controller
    {
        private readonly Tennis_Card_GameContext _context;

        public TournamentsController(Tennis_Card_GameContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tournaments = await _context.Tournaments
                .Include(t => t.Surface)
                .OrderByDescending(t => t.StartTime)
                .Select(t => new TournamentViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    Surface = t.Surface.Name,
                    Level = t.Level,
                    XpReward = t.XpReward,
                    CoinReward = t.CoinReward,
                    MatchCount = t.Matches.Count,
                    Matches = t.Matches.Select(m => new MatchViewModel
                    {
                        Id = m.Id,
                        Player1Name = m.Player1.Name,
                        Player2Name = m.Player2.Name,
                        Player1Sets = m.Player1Sets,
                        Player2Sets = m.Player2Sets,
                        IsCompleted = m.IsCompleted,
                        StartTime = m.StartTime
                    }).ToList()
                })
                .ToListAsync();

            return View(tournaments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .Include(t => t.Surface)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.Player1)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.Player2)
                .Select(t => new TournamentViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    Surface = t.Surface.Name,
                    Level = t.Level,
                    XpReward = t.XpReward,
                    CoinReward = t.CoinReward,
                    MatchCount = t.Matches.Count,
                    Matches = t.Matches.Select(m => new MatchViewModel
                    {
                        Id = m.Id,
                        Player1Name = m.Player1.Name,
                        Player2Name = m.Player2.Name,
                        Player1Sets = m.Player1Sets,
                        Player2Sets = m.Player2Sets,
                        IsCompleted = m.IsCompleted,
                        StartTime = m.StartTime
                    }).ToList()
                })
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }
    }

}