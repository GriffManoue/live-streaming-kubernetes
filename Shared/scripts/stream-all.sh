#!/bin/bash
# This script starts 10 ffmpeg streams to rtmp://localhost:1935/live using real media files, looping each stream indefinitely
# It retrieves stream keys dynamically from the StreamDbHandler service

# Configuration
API_BASE_URL="http://localhost:8083"  # StreamDbHandler service URL
STREAMS_ENDPOINT="/api/streamdbhandler/all"
JWT_TOKEN=""  # Will be populated below
MAX_STREAMS=10

# Colors for better readability
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}[INFO]${NC} Getting JWT token for API access..."

# Function to get JWT token for API access
get_jwt_token() {
  # Get JWT token from auth service
  # This assumes you have your auth service running locally on port 8080
  local login_response=$(curl -s -X POST "http://localhost:8080/api/auth/login" \
    -H "Content-Type: application/json" \
    -d '{"username":"admin", "password":"admin123"}')
  
  # Extract token from response
  JWT_TOKEN=$(echo $login_response | grep -o '"token":"[^"]*' | sed 's/"token":"//')
  
  if [ -z "$JWT_TOKEN" ]; then
    echo -e "${RED}[ERROR]${NC} Failed to get JWT token. Make sure the auth service is running."
    exit 1
  else
    echo -e "${GREEN}[SUCCESS]${NC} JWT token acquired"
  fi
}

# Function to fetch stream keys from API
fetch_stream_keys() {
  echo -e "${BLUE}[INFO]${NC} Fetching stream keys from StreamDbHandler service..."
  
  # Fetch all streams from the API
  local response=$(curl -s -X GET "$API_BASE_URL$STREAMS_ENDPOINT" \
    -H "Authorization: Bearer $JWT_TOKEN" \
    -H "Accept: application/json")
  
  # Check if response contains streams
  if [[ "$response" == *"streamKey"* ]]; then
    echo -e "${GREEN}[SUCCESS]${NC} Retrieved streams from API"
    
    # Parse stream keys and IDs from JSON response using jq if available
    if command -v jq &> /dev/null; then
      local stream_data=$(echo "$response" | jq -r '.[] | select(.streamKey != null) | .id + ":" + .streamKey + ":" + (.username // "unknown")')
      
      # Reset arrays
      declare -g -a STREAM_IDS=()
      declare -g -a STREAM_KEYS=()
      declare -g -a STREAM_USERNAMES=()
      
      # Process each line
      while IFS=: read -r id key username; do
        STREAM_IDS+=("$id")
        STREAM_KEYS+=("$key")
        STREAM_USERNAMES+=("$username")
      done <<< "$stream_data"
      
    else
      # Basic parsing without jq (less reliable)
      echo -e "${YELLOW}[WARNING]${NC} jq not found, using basic parsing (consider installing jq for better reliability)"
      
      # Extract stream keys using pattern matching
      local pattern='"streamKey":"([^"]*)"'
      local i=0
      
      declare -g -a STREAM_KEYS=()
      
      while [[ $response =~ $pattern ]]; do
        STREAM_KEYS+=("${BASH_REMATCH[1]}")
        response=${response#*"${BASH_REMATCH[0]}"}
        i=$((i+1))
        if [ $i -ge $MAX_STREAMS ]; then
          break
        fi
      done
    fi
    
    # Check if we got enough stream keys
    if [ ${#STREAM_KEYS[@]} -eq 0 ]; then
      echo -e "${RED}[ERROR]${NC} No stream keys found in the response."
      exit 1
    elif [ ${#STREAM_KEYS[@]} -lt $MAX_STREAMS ]; then
      echo -e "${YELLOW}[WARNING]${NC} Found only ${#STREAM_KEYS[@]} streams with keys, wanted $MAX_STREAMS"
    else
      echo -e "${GREEN}[SUCCESS]${NC} Found ${#STREAM_KEYS[@]} streams with keys"
    fi
  else
    echo -e "${RED}[ERROR]${NC} Failed to retrieve streams or invalid response format."
    echo "Response: $response"
    exit 1
  fi
}

# Start streams
for i in {1..10}; do
  # Use the correct index for arrays (0-based)
  index=$((i-1))
  stream_key=${STREAM_KEYS[$index]}
  
  echo "Starting stream $i with key $stream_key"
  
  # Start the streaming process in the background
  (
    while true; do
      ffmpeg -re -i "F:\\stream$i.mkv" -c:v libx264 -preset veryfast -b:v 1500k -c:a aac -b:a 128k -f flv "rtmp://localhost:1935/live/$stream_key"
      sleep 1
    done
  ) &
  
  # Small delay between starting streams to avoid overwhelming the system
  sleep 0.5
done

# Print status message
echo "Started 10 ffmpeg streams to rtmp://localhost:1935/live/ (looping each file)"
echo "Press Ctrl+C to stop all streams"

# Wait for all background processes
wait
