import time
from flask import Flask, request, jsonify
from moviepy.editor import VideoFileClip, AudioFileClip
import base64
import io
import tempfile
import os

def merge_audio_and_video():
        if 'video' not in request.files or 'audio' not in request.files:
            raise Exception("Both video and audio files are required.")

        video_file = request.files['video']
        audio_file = request.files['audio']

        with tempfile.NamedTemporaryFile(delete=False, suffix='.mp4') as temp_video_file:
            video_file_path = temp_video_file.name
            video_file.save(video_file_path)

        with tempfile.NamedTemporaryFile(delete=False, suffix='.mp3') as temp_audio_file:
            audio_file_path = temp_audio_file.name
            audio_file.save(audio_file_path)

        video = VideoFileClip(video_file_path)
        audio = AudioFileClip(audio_file_path)

        if audio.duration > video.duration:
            audio.duration = video.duration
        
        final_video = video.set_audio(audio)

        with tempfile.NamedTemporaryFile(delete=False, suffix='.mp4') as temp_file:
            temp_file_path = temp_file.name

        final_video.write_videofile(temp_file_path, codec="libx264", audio_codec="aac")

        with open(temp_file_path, 'rb') as temp_video_file:
            video_data = temp_video_file.read()

        encoded_video = base64.b64encode(video_data).decode('utf-8')
        
        time.sleep(5)
                
        try:
            if video:
                video.close()
            if audio:
                audio.close()
            if final_video:
                final_video.close()
            os.remove(video_file_path)
            os.remove(audio_file_path)
            os.remove(temp_file_path)
        except Exception as e:
            print(f"error occured while removing temp files. {e}")

        return jsonify({"video": encoded_video})
    
