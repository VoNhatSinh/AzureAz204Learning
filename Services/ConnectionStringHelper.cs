namespace Azure_Az204.Services
{
    public static class ConnectionStringHelper
    {
        public static string GetServiceBusConnectionString()
        {
            return Environment.GetEnvironmentVariable(
                "SERVICEBUSCONNSTR_ServiceBusConnectionString");
        }

        public static string GetAppInsightsInstrumentationKey()
        {
            return Environment.GetEnvironmentVariable(
                "APPSETTING_APPLICATIONINSIGHTS_CONNECTION_STRING");
        }
    }
}