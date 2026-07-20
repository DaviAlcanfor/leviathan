import sqlite3
from datetime import datetime


DB_PATH = "leviathan.db"


def get_connection() -> sqlite3.Connection:
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn


def init_db() -> None:
    with get_connection() as conn:
        conn.executescript("""
            CREATE TABLE IF NOT EXISTS agents (
                id            TEXT PRIMARY KEY,
                hostname      TEXT NOT NULL,
                username      TEXT NOT NULL,
                is_admin      INTEGER NOT NULL DEFAULT 0,
                os_version    TEXT,
                arch          TEXT,
                private_ip    TEXT,
                public_ip     TEXT,
                last_seen     TEXT NOT NULL,
                registered_at TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS results (
                id         TEXT PRIMARY KEY,
                agent_id   TEXT NOT NULL,
                command    TEXT NOT NULL,
                output     TEXT,
                created_at TEXT NOT NULL,
                FOREIGN KEY (agent_id) REFERENCES agents(id)
            );
        """)


def register_agent(agent_id: str, info: dict) -> None:
    with get_connection() as conn:
        conn.execute(
            """
            INSERT INTO agents
                (id, hostname, username, is_admin, os_version, arch,
                 private_ip, public_ip, last_seen, registered_at)
            VALUES
                (:id, :hostname, :username, :is_admin, :os_version, :arch,
                 :private_ip, :public_ip, :now, :now)
            ON CONFLICT(id) DO UPDATE SET last_seen = :now
            """,
            {
                "id":         agent_id,
                "hostname":   info.get("hostname"),
                "username":   info.get("username"),
                "is_admin":   int(info.get("is_admin", False)),
                "os_version": info.get("os_version"),
                "arch":       info.get("arch"),
                "private_ip": info.get("private_ip"),
                "public_ip":  info.get("public_ip"),
                "now":        datetime.utcnow().isoformat(),
            },
        )


def get_agent(agent_id: str) -> sqlite3.Row | None:
    with get_connection() as conn:
        return conn.execute(
            "SELECT * FROM agents WHERE id = ?", (agent_id,)
        ).fetchone()


def list_agents() -> list[sqlite3.Row]:
    with get_connection() as conn:
        return conn.execute(
            "SELECT * FROM agents ORDER BY last_seen DESC"
        ).fetchall()


def save_result(agent_id: str, task_id: str, command: str, output: str) -> None:
    with get_connection() as conn:
        conn.execute(
            """
            INSERT INTO results (id, agent_id, command, output, created_at)
            VALUES (?, ?, ?, ?, ?)
            """,
            (task_id, agent_id, command, output, datetime.utcnow().isoformat()),
        )


def get_result(task_id: str) -> sqlite3.Row | None:
    with get_connection() as conn:
        return conn.execute(
            "SELECT * FROM results WHERE id = ?", (task_id,)
        ).fetchone()