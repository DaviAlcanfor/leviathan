from pydantic_settings import BaseSettings


class Config(BaseSettings):
    SERVER_URL: str = "https://127.0.0.1:5000"
    MONGO_URI: str = "mongodb://localhost:27017"
    MONGO_DB_NAME: str = "leviathan"

    class model_config:
        env_file = ".env"