from pydantic_settings import BaseSettings

class Config(BaseSettings):
    SERVER_URL: str 
    DB_PATH = "leviathan.db"

    class model_config:
        env_file = ".env"