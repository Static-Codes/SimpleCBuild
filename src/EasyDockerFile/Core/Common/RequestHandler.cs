namespace EasyDockerFile.Core.Common;
public class RequestManager()
{
    public static readonly string DefaultUserAgent = "Mozilla/5.5 (Windows NT 10.0; Win64; x64; rv:136.0) Gecko/20100101 Firefox/146.0";
    public HttpClient Client { get; } = NetworkClient.Instance;

    public static class NetworkClient
    {
        private static readonly HttpClient _Instance;

        static NetworkClient()
        {
            var defaultHandler = new HttpClientHandler { AllowAutoRedirect = true };
            _Instance = new HttpClient(defaultHandler) { Timeout = TimeSpan.FromSeconds(30) };
            _Instance.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
        }

        public static HttpClient Instance => _Instance;
    }
    
}