# Leviathan

C2 framework consisting of a Flask server (controller) and C# agent (implant) 
compiled to position-independent shellcode via Donut for payload delivery.

## Architecture

```
        Operator
            |
        Flask C2
       /        \
   Agent.dll  Shellcode (via Donut)
      |            |
   Victim Host  Injected Process
```

## Components

| Component | Status | Notes |
|-----------|--------|-------|
| SystemInfo | ✓ | Victim data collection |
| CommandExecution | - | Shell command execution |
| Screenshot | - | Screen capture |
| Audio | - | Microphone recording |
| Keylog | - | Keyboard monitoring |
| Protocol | - | C2 communication |
| Flask Server | - | Command & control |
| Donut Integration | - | Shellcode generation |

## Disclaimer

Educational and authorized testing only.