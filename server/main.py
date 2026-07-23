from flask import Flask, request
from server.db import init_db
from routes import routes



app = Flask(__name__)
app.register_blueprint(routes)



@app.before_request
def before():
    print(request.method, request.path)


@app.after_request
def changeserver(response):
    print(response.headers)
    response.headers['Server'] = ''
    
    return response


@app.errorhandler(404)
def not_found(e):
    return {"error": "Not Found"}, 404


@app.errorhandler(Exception)
def handle(e):
    return {"error": "Internal Error"}, 500




if __name__ == "__main__":
    init_db()
    
    app.run(
        debug=True,
        host="0.0.0.0",
        port=5000,
        ssl_context=("certs/cert.pem", "certs/key.pem")
    )