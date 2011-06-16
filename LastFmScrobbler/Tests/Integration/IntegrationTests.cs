using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lpfm.LastFmScrobbler.Tests.Integration
{
    [TestClass]
    public abstract class IntegrationTests
    {
        protected string SessionKey; 
        protected string ApiKey; 
        protected string ApiSecret; 

        const string SessionKeyAppSettingKey = "LastFmSessionKey";
        const string ApiKeyAppSettingKey = "LastFmApiKey";
        const string ApiSecretAppSettingKey = "LastFmApiSecret";

        protected IntegrationTests()
        {
            SessionKey = GetAppSetting(SessionKeyAppSettingKey);
            ApiKey = GetAppSetting(ApiKeyAppSettingKey);
            ApiSecret = GetAppSetting(ApiSecretAppSettingKey);

            if (string.IsNullOrEmpty(SessionKey) || string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(ApiSecret))
            {
                throw new InvalidOperationException("The SessionKey, ApiKey and ApiSecret have not been set. All three keys must be set in the App settings of the integration project app.config file before running integration tests");
            }
        }

        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
