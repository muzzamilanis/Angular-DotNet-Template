import time
import requests

# Replace with your Render health endpoint
url = "https://angular-dotnet-template.onrender.com/api/Health/ping"

while True:
    try:
        r = requests.get(url, timeout=10)
        print(f"[{time.strftime('%H:%M:%S')}] Pinged {url}, Status: {r.status_code}")
    except Exception as e:
        print(f"[{time.strftime('%H:%M:%S')}] Ping failed: {e}")
    time.sleep(60)  # wait 60 seconds before next ping
