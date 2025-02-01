using Microsoft.Extensions.Logging;

namespace UserSystem.Services
{
    public class BaseLogger
    {
        protected readonly ILogger<BaseLogger> _logger;
        public BaseLogger(ILogger<BaseLogger> logger)
        {
            _logger = logger;
        }

        protected void LogException(Exception ex)
        {
            _logger.LogError("Exception Message: " + ex.Message);
            _logger.LogError("Stack Trace: " + ex.StackTrace);
            if (ex.InnerException != null)
            {
                _logger.LogError("Inner Exception Message: " + ex.InnerException.Message);
            }
        }
    }
}
