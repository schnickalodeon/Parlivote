using System;

namespace Parlivote.Web.Models.Views.Polls
{
    public class PollView
    {
        public Guid Id { get; set; }
        public string AgendaItem { get; set; }
        public string Text { get; set; }
    }
}
