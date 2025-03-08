using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tennis_Card_Game.Data;
using Tennis_Card_Game.Models;
using Tennis_Card_Game.ViewModel;
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

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> JoinConfirm(int tournamentId)
        //    {
        //        var tournament = await _context.Tournaments
        //            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        //        if (tournament == null)
        //        {
        //            return NotFound();
        //        }

        //        // Check if user already has matches in this tournament
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        var userPlayer = await _context.Players
        //            .FirstOrDefaultAsync(p => p.UserId == userId);

        //        if (userPlayer == null)
        //        {
        //            TempData["Error"] = "Nu aveți un jucător pentru a participa la turneu!";
        //            return RedirectToAction(nameof(Details), new { id = tournamentId });
        //        }

        //        // Check if user is already in this tournament
        //        var existingMatches = await _context.Matches
        //            .AnyAsync(m => m.TournamentId == tournamentId &&
        //                           (m.Player1Id == userPlayer.Id || m.Player2Id == userPlayer.Id));

        //        if (existingMatches)
        //        {
        //            TempData["Error"] = "Sunteți deja înscris în acest turneu!";
        //            return RedirectToAction(nameof(Details), new { id = tournamentId });
        //        }

        //        // Get tournament surface
        //        var surface = await _context.Surfaces.FindAsync(tournament.SurfaceId);
        //        if (surface == null)
        //        {
        //            TempData["Error"] = "Suprafața de joc nu a fost găsită!";
        //            return RedirectToAction(nameof(Details), new { id = tournamentId });
        //        }

        //        // Generate 15 random AI opponents
        //        var randomOpponents = await _context.Players
        //            .Where(p => p.UserId == null) // Only AI players
        //            .OrderBy(p => Guid.NewGuid()) // Random order
        //            .Take(15)
        //            .ToListAsync();

        //        if (randomOpponents.Count < 15)
        //        {
        //            TempData["Error"] = "Nu sunt suficienți jucători AI în baza de date pentru a genera un turneu!";
        //            return RedirectToAction(nameof(Details), new { id = tournamentId });
        //        }

        //        using (var transaction = await _context.Database.BeginTransactionAsync())
        //        {
        //            try
        //            {
        //                // Generate first round matches (8 matches for 16 players)
        //                await GenerateTournamentMatches(tournament, userPlayer, randomOpponents, surface);

        //                await transaction.CommitAsync();

        //                TempData["Success"] = "V-ați înscris cu succes în turneu și au fost generate meciurile!";
        //                return RedirectToAction(nameof(Details), new { id = tournamentId });
        //            }
        //            catch (Exception ex)
        //            {
        //                await transaction.RollbackAsync();
        //                TempData["Error"] = $"Eroare la înscrierea în turneu: {ex.Message}";
        //                return RedirectToAction(nameof(Details), new { id = tournamentId });
        //            }
        //        }
        //    }

        //    // Helper method for generating tournament matches
        //    private async Task GenerateTournamentMatches(Tournament tournament, Player userPlayer, List<Player> opponents, Surface surface)
        //    {
        //        // Create a list with all participants (user and 15 opponents)
        //        var allPlayers = new List<Player> { userPlayer };
        //        allPlayers.AddRange(opponents);

        //        // Create matches for the first round (8 matches)
        //        var matches = new List<Match>();

        //        // First round - 16 player bracket
        //        for (int i = 0; i < 8; i++)
        //        {
        //            // Classic seeding: 1 vs 16, 2 vs 15, 3 vs 14, etc.
        //            var player1 = allPlayers[i];
        //            var player2 = allPlayers[15 - i];

        //            var match = new Match
        //            {
        //                TournamentId = tournament.Id,
        //                Player1Id = player1.Id,
        //                Player2Id = player2.Id,
        //                SurfaceId = surface.Id,
        //                Round = 1,
        //                MatchOrder = i + 1,
        //                StartTime = tournament.StartTime.Add(TimeSpan.FromHours(i)),
        //                IsCompleted = false
        //            };

        //            matches.Add(match);
        //        }

        //        _context.Matches.AddRange(matches);
        //        await _context.SaveChangesAsync();
        //    }
        //}

    }
}