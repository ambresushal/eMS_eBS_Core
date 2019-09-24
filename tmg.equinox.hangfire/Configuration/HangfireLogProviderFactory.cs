using Hangfire.Logging;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire.Logger;

namespace tmg.equinox.hangfire.Configuration
{

    public class HangfireLogProviderFactory : ILogProviderFactory
    {

        public void CreateLogProvider()
        {
            LogProvider.SetCurrentLogProvider(new CustomHangfireLogProvider());
        }

    }

}
