using Service_Academy1.Models;

namespace Service_Academy1.Services
{
    public class LogSystemUsageService
    {
        private readonly ApplicationDbContext _context;

        public LogSystemUsageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogSystemUsageAsync(string userId, string actionType, int? targetId = null)
        {
            var log = new SystemUsageLogModel
            {
                UserId = userId,
                ActionType = actionType,
                Timestamp = DateTime.UtcNow,
                TargetId = targetId // Optional, e.g., ProgramId, ResourceId
            };

            _context.SystemUsageLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
