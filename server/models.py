from pydantic import BaseModel


class AgentInfo(BaseModel):
    hostname: str
    username: str
    is_admin: bool
    os_version: str
    arch: str
    private_ip: str
    public_ip: str


class TaskResult(BaseModel):
    task_id: str
    output: str


class CommandRequest(BaseModel):
    agent_id: str
    command: str