namespace Agent.Models;

public record CheckinPayload(
    string Uuid,
    string Hostname,
    string Username,
    bool IsAdmin,
    string OsVersion,
    string Arch,
    string PrivateIp,
    string PublicIp
);