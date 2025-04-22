#!/bin/bash
# This script starts multiple ffmpeg streams to rtmp://localhost:1935/live using real media files, looping each stream indefinitely

# Stream 1
while true; do
  ffmpeg -re -i F:\stream1.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/a8a14555-f071-446e-a4b0-d6fd98ded20c
  sleep 1
done &

# Stream 2
while true; do
  ffmpeg -re -i F:\stream2.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/ae2c0da4-520e-46af-8a66-f26f422a94fa
  sleep 1
done &

# Stream 3
while true; do
  ffmpeg -re -i F:\stream3.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/2e5d499d-2fce-4242-89ca-a019733d52f9
  sleep 1
done &

# Stream 4
while true; do
  ffmpeg -re -i F:\stream4.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/6e09293a-9322-458b-ae19-9cf860b5d5ca
  sleep 1
done &

# Stream 5
while true; do
  ffmpeg -re -i F:\stream5.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/beb4ccaf-f1e8-41ab-8d1c-617f752d50fb
  sleep 1
done &

# Stream 6
while true; do
  ffmpeg -re -i F:\stream6.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/6579a589-a2a7-45c1-8c86-891760d82db6
  sleep 1
done &

# Stream 7
while true; do
  ffmpeg -re -i F:\stream7.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/9bd0d9da-6383-4d23-8e54-fecbb36a7d24
  sleep 1
done &

# Stream 8
while true; do
  ffmpeg -re -i F:\stream8.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/b7cdd17c-7d5c-497d-b070-3da3173dfb49
  sleep 1
done &

# Stream 9
while true; do
  ffmpeg -re -i F:\stream9.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/f50e9159-2e82-4d12-b4ba-bae32f3dcc04
  sleep 1
done &

# Stream 10
while true; do
  ffmpeg -re -i F:\stream10.mkv -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv rtmp://localhost:1935/live/00d0c1e0-eb48-4857-bb31-145a5cff63ec
  sleep 1
done &

echo "Started ffmpeg streams to rtmp://localhost:1935/live/ (looping each file)"
