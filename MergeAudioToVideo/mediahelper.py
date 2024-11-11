from flask import Flask, request, jsonify
from moviepy.editor import VideoFileClip, AudioFileClip
import base64
import io
import tempfile
import os

def merge_audio_and_video():
        # Ensure video and audio files are uploaded
        if 'video' not in request.files or 'audio' not in request.files:
            raise Exception("Both video and audio files are required.")

        video_file = request.files['video']
        audio_file = request.files['audio']

        # Write the video file to a temporary file
        with tempfile.NamedTemporaryFile(delete=False, suffix='.mp4') as temp_video_file:
            video_file_path = temp_video_file.name
            video_file.save(video_file_path)

        # Write the audio file to a temporary file
        with tempfile.NamedTemporaryFile(delete=False, suffix='.mp3') as temp_audio_file:
            audio_file_path = temp_audio_file.name
            audio_file.save(audio_file_path)

        # Read the video and audio from the temporary files
        video = VideoFileClip(video_file_path)
        audio = AudioFileClip(audio_file_path)

        # Combine the video with the audio
        final_video = video.set_audio(audio)

        # Create a temporary file to store the merged video
        with tempfile.NamedTemporaryFile(delete=False, suffix='.mp4') as temp_file:
            temp_file_path = temp_file.name

        # Write the final video to the temporary file
        final_video.write_videofile(temp_file_path, codec="libx264", audio_codec="aac")

        # Read the content of the temporary video file
        with open(temp_file_path, 'rb') as temp_video_file:
            video_data = temp_video_file.read()

        # Encode the video data to base64
        encoded_video = base64.b64encode(video_data).decode('utf-8')

        # Remove the temporary files after use
        os.remove(video_file_path)
        os.remove(audio_file_path)
        os.remove(temp_file_path)

        # Return the base64-encoded video as JSON
        return jsonify({"video": encoded_video})