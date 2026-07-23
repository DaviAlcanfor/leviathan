from datetime import datetime

from pymongo import DESCENDING, MongoClient
from pymongo.collection import Collection

from config import Config

_cfg = Config()
_client = MongoClient(_cfg.MONGO_URI)
_db = _client[_cfg.MONGO_DB_NAME]

_agents: Collection = _db["agents"]
_results: Collection = _db["results"]


def init_db() -> None:
    _results.create_index("agent_id")


def register_agent(agent_id: str, info: dict) -> None:
    now = datetime.utcnow().isoformat()
    _agents.update_one(
        {"_id": agent_id},
        {
            "$set": {"last_seen": now},
            "$setOnInsert": {
                "hostname":      info.get("hostname"),
                "username":      info.get("username"),
                "is_admin":      int(info.get("is_admin", False)),
                "os_version":    info.get("os_version"),
                "arch":          info.get("arch"),
                "private_ip":    info.get("private_ip"),
                "public_ip":     info.get("public_ip"),
                "registered_at": now,
            },
        },
        upsert=True,
    )


def get_agent(agent_id: str) -> dict | None:
    return _agents.find_one({"_id": agent_id})


def list_agents() -> list[dict]:
    return list(_agents.find().sort("last_seen", DESCENDING))


def save_result(agent_id: str, task_id: str, command: str, output: str) -> None:
    _results.insert_one(
        {
            "_id":        task_id,
            "agent_id":   agent_id,
            "command":    command,
            "output":     output,
            "created_at": datetime.utcnow().isoformat(),
        }
    )


def get_result(task_id: str) -> dict | None:
    return _results.find_one({"_id": task_id})