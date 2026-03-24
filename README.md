# TuneTweak

A C# Windows Forms audio file converter that is a work-in-progress, similar to many of my other projects. It is a simple application 
that I wanted to write to experiment with FFmpeg and Akka.NET.

<b>What it does:</b>
- Allows users to select individual audio files or entire folders
- Displays them in a grid showing filename, title, artist, album, and album art
- Converts audio files to different audio formats: mp3, wav, aac, ogg, or flac
- Shows conversion progress with a progress bar and status label
- Reads and displays existing metadata (title, artist, album) from audio files

<b>Under the hood:</b>
- FFmpeg + FFprobe do the actual audio conversion and metadata extraction — they're expected as binaries alongside the app.
- Akka.NET with dependency injection manages the actor system
- An IUIUpdater interface decouples the actors from the form, so actors can update the UI without directly referencing it

<b>Tech stack:</b> C# / .NET, Windows Forms, Akka.NET, FFmpeg/FFprobe binaries
