# Leviathan

A command-and-control framework built for educational purposes. Flask server in Python, agent in C# compiled to position-independent shellcode via Donut, delivered through a C# loader.

```
[Operator]
    |
    | POST /pong  (queue command)
    v
[Flask Server] <-----> [MongoDB]
    |
    | GET  /assets  (shellcode)
    v
[Loader.exe]
    |
    | VirtualAlloc + CreateThread
    v
[Agent shellcode]
    |
    | POST /ping  { info, output }
    | <-----------{ cmd }
    |
    | CommandExecution
    v
[Victim Host]
```

## Architecture

| Component | Language | Description |
|---|---|---|
| `server/` | Python / Flask | C2 server, operator API, MongoDB persistence |
| `implant/Agent/` | C# (.NET 8) | Agent: system info collection, command execution, HTTP polling |
| `implant/Loader/` | C# (.NET 8) | Loader: fetches and executes agent shellcode in memory |
| `build.py` | Python | Compiles Agent → Donut → shellcode → compiles Loader |

## Server endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/` | Healthcheck |
| `POST` | `/ping` | Agent sync: sends info + output, receives next command |
| `POST` | `/pong` | Operator queues a command for an agent |
| `GET` | `/assets` | Serves agent shellcode |

Agent identity is passed via `X-Session-Id` header (victim UUID) on every request.

## Agent capabilities

- System info collection: hostname, username, admin status, OS version, architecture, private IP, public IP
- Command execution via `cmd.exe` with persistent working directory state
- Single polling loop: sends output from previous command, receives next
- Configurable polling interval with jitter
- Random user-agent rotation per request

## Configuration

`.env` variables:

| Variable | Description |
|---|---|
| `SERVER_URL` | Public URL of the C2 server |
| `PORT` | Flask port (default: 5000) |
| `MONGO_URI` | MongoDB connection string |
| `AGENT_BIN` | Path to generated shellcode (default: `server/agent.bin`) |

Agent-side config (`implant/Agent/Config.cs`):

| Field | Description |
|---|---|
| `ServerUrl` | C2 server URL |
| `UserAgents` | Pool of user-agents for request rotation |
| `PollingInterval` | Base sleep between pings (ms) |
| `JitterMax` | Max random jitter added to polling interval (ms) |

## Project structure

```
Leviathan/
├── implant/
│   ├── Agent/
│   │   ├── Models/
│   │   │   ├── CheckinPayload.cs
│   │   │   └── ResultPayload.cs
│   │   ├── Modules/
│   │   │   ├── CommandExecution.cs
│   │   │   └── SystemInfo.cs
│   │   ├── Config.cs
│   │   └── Program.cs
│   ├── Loader/
│   │   ├── Config.cs
│   │   └── Program.cs
│   └── Leviathan.sln
├── server/
│   ├── certs/
│   ├── config.py
│   ├── db.py
│   ├── main.py
│   ├── requirements.txt
│   ├── routes.py
│   └── shellcode_generator.py
├── build.py
├── .env
├── .gitignore
├── LICENSE
└── README.md
```

## Disclaimer

This project is intended for educational purposes and authorized security research only. Do not deploy against systems you do not own or have explicit written permission to test. The author is not responsible for any misuse.
