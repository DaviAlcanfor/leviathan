namespace Agent.Models;

public record ResultPayload(
    string AgentId,
    string Command,
    string Output
);