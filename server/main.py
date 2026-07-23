from flask import Flask
from db import init_db
from routes import routes


app = Flask(__name__)
app.register_blueprint(routes)


@app.after_request
def hide_server_header(response):
    response.headers["Server"] = ""
    return response


@app.errorhandler(404)
def not_found(_e):
    return {"error": "Not Found"}, 404


@app.errorhandler(Exception)
def handle(_e):
    return {"error": "Internal Error"}, 500


if __name__ == "__main__":
    init_db()
    app.run(
        host="0.0.0.0",
        port=5000,
        ssl_context=("certs/cert.pem", "certs/key.pem"),
    )
