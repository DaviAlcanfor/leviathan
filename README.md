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

## Setup

```bash
make setup   # create venv, install dependencies
make server  # start C2 server (https://0.0.0.0:5000)
```

Requires Python 3.10+, .NET 8.0 SDK, MongoDB running locally.

TLS certs are expected at `server/certs/cert.pem` and `server/certs/key.pem`. Generate self-signed ones:

```bash
openssl req -x509 -newkey rsa:2048 -nodes \
    -keyout server/certs/key.pem -out server/certs/cert.pem -days 365 \
    -subj "/CN=localhost"
```

## CLI

Interactive operator interface:

```bash
make cli
```

```
leviathan> agents            # list connected agents
leviathan> cmd <id> whoami   # queue a command
leviathan> result <task_id>  # retrieve output
leviathan> exit
```

## Build pipeline

```bash
python build.py
```

Compiles Agent (.NET) → generates shellcode via Donut → compiles Loader. Output: `server/agent.bin`.

## Architecture

| Component | Language | Description |
|---|---|---|
| `server/` | Python / Flask | C2 server, operator CLI, MongoDB persistence |
| `implant/Agent/` | C# (.NET 8) | Agent: system info collection, command execution, HTTP polling |
| `implant/Loader/` | C# (.NET 9) | Loader: fetches and executes agent shellcode in memory |
| `build.py` | Python | Compiles Agent → Donut → shellcode → compiles Loader |
| `Makefile` | — | `setup`, `server`, `cli`, `clean` targets |

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

| Variable | Default | Description |
|---|---|---|
| `SERVER_URL` | `https://127.0.0.1:5000` | Public URL of the C2 server |
| `MONGO_URI` | `mongodb://localhost:27017` | MongoDB connection string |
| `MONGO_DB_NAME` | `leviathan` | Database name |

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
│   ├── cli.py
│   ├── config.py
│   ├── db.py
│   ├── main.py
│   ├── requirements.txt
│   ├── routes.py
│   └── shellcode_generator.py
├── build.py
├── Makefile
├── .env
├── .gitignore
├── LICENSE
└── README.md
```

## Disclaimer

This project is intended for educational purposes and authorized security research only. Do not deploy against systems you do not own or have explicit written permission to test. The author is not responsible for any misuse.
