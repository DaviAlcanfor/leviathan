# Leviathan

C2 framework with a Flask server and C# agent compiled to position-independent shellcode via Donut.

## Architecture

Operator (cli.py)
|
Flask Server
|
/ping ← Agent checks in + sends output
/pong → Operator queues command
/assets → Serves shellcode


## Disclaimer

For educational purposes and authorized testing only.