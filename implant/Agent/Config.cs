namespace Agent
{
    static class Config
    {
        private static readonly string ServerUrl = "https://127.0.0.1:5000";

        private static readonly string[] UserAgents =
        [
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/124.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:125.0) Gecko/20100101 Firefox/125.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Edg/124.0.0.0 Safari/537.36",
        ];

        private static readonly int PollingInterval = 5000;
        private static readonly int JitterMax = 3000;
    }
}