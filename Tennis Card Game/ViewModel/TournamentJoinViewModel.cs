namespace Tennis_Card_Game.ViewModel
{
    public class TournamentJoinViewModel
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public DateTime TournamentStartTime { get; set; }
        public DateTime RegistrationStartTime { get; set; }
        public DateTime RegistrationEndTime { get; set; }
        public bool IsRegistrationOpen { get; set; }
        public string Surface { get; set; }
        public string Level { get; set; }
    }
}