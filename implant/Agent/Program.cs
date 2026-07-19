class Program
{
    static void Main(string[] args)
    {
        var info = new SystemInfo();

        Console.WriteLine($"Hostname: {info.Hostname}");
        Console.WriteLine($"IP Privado: {info.IpPriv}");
        Console.WriteLine($"IP Público: {info.IpPub}");
        Console.WriteLine($"OS: {info.OsVersion}");
        Console.WriteLine($"Machine: {info.Machine}");
        Console.WriteLine($"Username: {info.Username}");
        Console.WriteLine($"Admin: {info.IsAdmin}");
        Console.WriteLine($"VicId: {info.VicId}");
    }
}