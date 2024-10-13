using eCoinAccountingApp.Models;
using eCoinAccountingApp.Data;
using System;
using System.Threading.Tasks;

namespace eCoinAccountingApp.Services
{
    public class EventLogger
    {
        private readonly ApplicationDbContext _context;

        public EventLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogEventAsync(string description, string userId)
        {
            var log = new EventLog
            {
                Description = description,
                UserId = userId,
                EventTime = DateTime.Now
            };

            _context.EventLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
