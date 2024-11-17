using System;

namespace eCoinAccountingApp.Models
{
    public class EventLog
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public DateTime EventTime { get; set; }
    }
}
