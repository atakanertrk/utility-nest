from flask import Flask, request, jsonify
from moviepy.editor import VideoFileClip, AudioFileClip
from mediahelper import merge_audio_and_video
import base64
import io
import tempfile
import os

app = Flask(__name__)

@app.route("/")
def home():
    return "Home"

@app.route("/get-user/<user_id>") #/get-user/12
def get_user(user_id):
    user_data = {
        "description":f"UserId: {user_id}",
        "name":"test",
        "extra":""
    }
    extra = request.args.get("extra") # /get-user/12?extra=extraData
    if extra:
        user_data["extra"] = extra
    return jsonify(user_data), 200 # 200 status code

@app.route("/create-user",methods=["POST"])
def create_user():
    data = request.get_json()
    return data, 201

@app.route("/merge", methods=["POST"])
def merge():    
    return merge_audio_and_video()

if __name__ == "__main__":
    app.run(debug=True)


