import os
import subprocess
import datetime
import glob
import shutil
from dotenv import load_dotenv
from pathlib import Path

import os
from pathlib import Path
from dotenv import load_dotenv

# Get the directory where the script is located
BASE_DIR = Path(__file__).resolve().parent

# Path to your .env file in the same directory
ENV_PATH = BASE_DIR / "credentials.env"

# Load environment variables from the .env file
if ENV_PATH.exists():
    load_dotenv(dotenv_path=ENV_PATH)
    print(f"[INFO] Loaded environment variables from {ENV_PATH}")
else:
    raise FileNotFoundError(f"[ERROR] credentials.env not found in {BASE_DIR}")

PG_HOST = os.getenv("PG_HOST")
PG_PORT = os.getenv("PG_PORT", "5432")
PG_USER = os.getenv("PG_USER")
PG_PASSWORD = os.getenv("PG_PASSWORD")
PG_DB = os.getenv("PG_DB")
BACKUP_DIR = Path(os.getenv("BACKUP_DIR", "./backups"))
RETENTION_DAYS = int(os.getenv("RETENTION_DAYS", "7"))
PGSSLMODE = os.getenv("PGSSLMODE", "require")

# Ensure backup directory exists
BACKUP_DIR.mkdir(parents=True, exist_ok=True)

# Timestamp for filename
timestamp = datetime.datetime.now().strftime("%Y-%m-%d_%H-%M")
backup_file = BACKUP_DIR / f"smartbizops_{timestamp}.backup"

# Construct pg_dump command
command = [
    "pg_dump",
    "-h", PG_HOST,
    "-p", PG_PORT,
    "-U", PG_USER,
    "-d", PG_DB,
    "-Fc",  # custom compressed format
    "-f", str(backup_file)
]

# Add SSL mode if needed
os.environ["PGSSLMODE"] = PGSSLMODE
os.environ["PGPASSWORD"] = PG_PASSWORD

print(f"[INFO] Starting backup at {datetime.datetime.now()}")
print(f"[INFO] Saving to {backup_file}")

try:
    subprocess.run(command, check=True)
    print("[SUCCESS] Backup completed successfully!")

    # Delete old backups
    cutoff = datetime.datetime.now() - datetime.timedelta(days=RETENTION_DAYS)
    for file in BACKUP_DIR.glob("smartbizops_*.dump"):
        mtime = datetime.datetime.fromtimestamp(file.stat().st_mtime)
        if mtime < cutoff:
            file.unlink()
            print(f"[CLEANUP] Deleted old backup: {file.name}")

except subprocess.CalledProcessError as e:
    print(f"[ERROR] Backup failed: {e}")

except Exception as e:
    print(f"[ERROR] Unexpected error: {e}")

print("[DONE] Backup process finished.")
