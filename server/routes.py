from flask import Blueprint

routes = Blueprint("routes", __name__)


@routes.get("/")
def healthcheck():
    return "wesh"