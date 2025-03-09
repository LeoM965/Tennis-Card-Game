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

        // GET: Tournaments
        public async Task<IActionResult> Index()
        {
            // Obține toate turneele ordonate descrescător după ora de începere
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

        // GET: Tournaments/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obține detaliile unui turneu specific
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

        // GET: Tournaments/Current
        public async Task<IActionResult> Current()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            DateTime today = DateTime.Today;

            // Obține turneele active în momentul curent
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

        public async Task<IActionResult> Join(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .Include(t => t.Surface)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return NotFound();
            }

            // Obține ora curentă
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            // Verifică dacă este în intervalul de înscriere (cu o oră înainte de începere și până cu 5 minute înainte)
            TimeSpan registrationStartTime = tournament.StartTime.Subtract(TimeSpan.FromHours(1));
            TimeSpan registrationEndTime = tournament.StartTime.Subtract(TimeSpan.FromMinutes(5));

            if (currentTime < registrationStartTime || currentTime > registrationEndTime)
            {
                TempData["ErrorMessage"] = "Înscrierea este permisă doar cu o oră înainte de începerea turneului și se închide cu 5 minute înainte de start.";
                return RedirectToAction(nameof(Details), new { id = tournament.Id });
            }

            // Obține ID-ul utilizatorului curent
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verifică dacă utilizatorul este deja înscris la acest turneu
            var existingRegistration = await _context.TournamentRegistrations
                .FirstOrDefaultAsync(r => r.TournamentId == id && r.UserId == userId);

            if (existingRegistration != null)
            {
                TempData["InfoMessage"] = "Sunteți deja înscris la acest turneu.";
                return RedirectToAction(nameof(Details), new { id = tournament.Id });
            }

            // Creează înregistrarea pentru turneu
            var registration = new TournamentRegistration
            {
                TournamentId = tournament.Id,
                UserId = userId,
                RegistrationTime = DateTime.Now
            };

            _context.TournamentRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "V-ați înscris cu succes la turneul " + tournament.Name;
            return RedirectToAction(nameof(Details), new { id = tournament.Id });
        }
    }
}