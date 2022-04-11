namespace Parlivote.Web.Configurations
{
    public class LocalConfigurations
    {
        public ApiConfiguratons ApiConfigurations { get; set; }
        public string SyncfusionApiKey { get; set; }
        public string AuthTokenStorageKey { get; set; }
        public string AuthTokenExpirationStorageKey { get; set; }
        public string RefreshTokenStorageKey { get; set; }
    }
}
