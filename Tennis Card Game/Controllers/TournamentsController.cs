using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tennis_Card_Game.Data;
using Tennis_Card_Game.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TennisCardBattle.Controllers
{
    public class TournamentsController : Controller
    {
        private readonly Tennis_Card_GameContext _context;

        public TournamentsController(Tennis_Card_GameContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            List<TournamentViewModel> tournaments = await _context.Tournaments
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

            TournamentViewModel? tournament = await _context.Tournaments
                .Include(t => t.Surface)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.Player1)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.Player2)
                .Where(t => t.Id == id)
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
                .FirstOrDefaultAsync();

            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        public async Task<IActionResult> Current()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            DateTime today = DateTime.Today;

            List<TournamentViewModel> currentTournaments = await _context.Tournaments
                .Include(t => t.Surface)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.Player1)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.Player2)
                .Where(t => t.StartTime <= currentTime && t.EndTime >= currentTime)
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

            return View(currentTournaments);
        }
    }
}