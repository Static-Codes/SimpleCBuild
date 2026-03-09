namespace EasyDockerFile.Core.Common;
public static class RequestManager
{
    public static readonly string DefaultUserAgent = "Mozilla/5.5 (Windows NT 10.0; Win64; x64; rv:136.0) Gecko/20100101 Firefox/146.0";

    public static class NetworkClient
    {
        private static readonly HttpClient _Instance;
        
        public static HttpClient Instance => _Instance;

        public static void UpdateUserAgent(string userAgent) {
            Instance.DefaultRequestHeaders.Remove("User-Agent");
            Instance.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        public static void ResetUserAgentToDefault() {
            Instance.DefaultRequestHeaders.Remove("User-Agent");
            Instance.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
        }
        static NetworkClient()
        {
            var defaultHandler = new HttpClientHandler { AllowAutoRedirect = true };
            _Instance = new HttpClient(defaultHandler) { Timeout = TimeSpan.FromSeconds(30) };
            _Instance.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
        }

        

    }
    
}