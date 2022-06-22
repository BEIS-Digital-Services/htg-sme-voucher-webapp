
namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    public class FakeLogger : ILogger<IndesserConnection>
    {
        public static bool LogErrorCalled { get; set; }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel == LogLevel.Error)
            {
                LogErrorCalled = true;
            }
        }
    }
}