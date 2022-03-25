namespace Parlivote.Shared.Models.Polls
{
    public class Poll
    {
        public Guid Id { get; set; }
        public string AgendaItem { get; set; }
        public string Text { get; set; }
    }
}