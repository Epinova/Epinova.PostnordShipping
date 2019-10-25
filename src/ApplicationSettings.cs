using System;
using System.Configuration;

namespace Epinova.PostnordShipping
{
    public static class ApplicationSettings
    {
        private const int DefaultTimeout = 3;

        static ApplicationSettings()
        {
            string timeoutSetting = ConfigurationManager.AppSettings.Get("Postnord.Api.TimeOutInSeconds");
            if (String.IsNullOrWhiteSpace(timeoutSetting) || !Int32.TryParse(timeoutSetting, out int seconds))
                seconds = DefaultTimeout;
            TimeOut = TimeSpan.FromSeconds(seconds);
        }

        public static TimeSpan TimeOut { get; set; }
    }
}
