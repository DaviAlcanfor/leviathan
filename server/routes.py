from flask import Blueprint, request
from models import TaskResult
from collections import deque

routes = Blueprint("routes", __name__)
_pending = {}


@routes.get("/")
def healthcheck():
    return "ok", 200 


# agent <- server
@routes.get("/task")
def task():
    agent_id = request.headers.get("X-Session-Id")
    cmd = _pending.get(agent_id)

    if cmd:
        _pending[agent_id] = None
        return cmd, 200

    return '', 204


# server -> agent
@routes.post("/cmd")
def command():
    data = request.get_json()

    agent_id = data.get("agent_id")
    cmd = data.get("cmd")

    if cmd:
        _pending[agent_id] = cmd
        return "ok", 200

    return '', 204



# TODO -> finish result, bin, bd


@routes.post("/result")
def result():
    data = request.get_json()
    output = data.get("output")


    return '', 204


