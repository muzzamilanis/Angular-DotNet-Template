# 🧩 SmartBizOps Database Backup & Restore Guide

This document explains how to **back up and restore the SmartBizOps PostgreSQL database** hosted on **Neon.tech**, using a simple, automated **Python script**.

---

## ⚙️ Prerequisites

Before running backups, ensure you have the following installed:

1. **Python 3.8+**
2. **pgAdmin 4** or **PostgreSQL Client Tools** (for `pg_dump` and `pg_restore`)
3. A `.env` file named **`credentials.env`** located in the same directory as the backup script

---

## 🧾 Environment File (`credentials.env`)

The script reads configuration values from this environment file.  
Example:

```ini
PG_HOST=ep-yourid.ap-southeast-1.aws.neon.tech
PG_PORT=5432
PG_USER=your_username
PG_PASSWORD=your_password
PG_DB=smartbizops
BACKUP_DIR=D:/Projects/Muzzamil/Personal/Angular-DotNet-Template/DbBackup
RETENTION_DAYS=7
PGSSLMODE=require
````

---

## 💾 Backup Instructions

### 1️⃣ Install Required Dependency

```bash
pip install python-dotenv
```

### 2️⃣ Ensure `pg_dump` is Available

Test in your terminal:

```bash
pg_dump --version
```

If you get an error, add PostgreSQL’s **bin folder** to your PATH, for example:

```
C:\Program Files\PostgreSQL\16\bin
```

Then re-open your terminal.

---

### 3️⃣ Run Backup Manually

From the directory containing your script and `.env` file:

```bash
python backup_smartbizops.py
```

You’ll see:

```
[INFO] Loaded environment variables from credentials.env
[SUCCESS] Backup completed successfully!
```

Backups are saved in the folder defined by `BACKUP_DIR` with names like:

```
smartbizops_2025-10-06_15-06.dump
```

Old backups are automatically deleted after the number of days specified in `RETENTION_DAYS`.

---

## 🕓 Automate Daily Backups

### 🪟 Windows (Task Scheduler)

1. Open **Task Scheduler → Create Basic Task**
2. **Trigger:** Daily → Choose a time (e.g., 2 AM)
3. **Action:**

   * **Program/script:** `python`
   * **Arguments:**

     ```
     D:\Projects\Muzzamil\Personal\Angular-DotNet-Template\DbBackup\backup_smartbizops.py
     ```
   * **Start in:**

     ```
     D:\Projects\Muzzamil\Personal\Angular-DotNet-Template\DbBackup
     ```
4. Finish setup ✅

Your database will now back up automatically every day.

---

### 🐧 Linux / macOS (cron)

Edit your crontab:

```bash
crontab -e
```

Add the following line (runs daily at 2 AM):

```bash
0 2 * * * /usr/bin/python3 /path/to/backup_smartbizops.py >> /path/to/backup.log 2>&1
```

---

## 🔁 Restore Instructions

To restore a backup created by this script:

1. Ensure PostgreSQL client tools or pgAdmin are installed.

2. Run this command in your terminal:

   ```bash
   pg_restore -h <host> -p 5432 -U <username> -d postgres -C -v smartbizops_YYYY-MM-DD_HH-MM.dump
   ```

   Example:

   ```bash
   pg_restore -h ep-yourid.ap-southeast-1.aws.neon.tech -U your_username -d postgres -C -v smartbizops_2025-10-06_15-06.dump
   ```

3. Add `--sslmode=require` if your Neon database requires SSL.

After restore, verify data using **pgAdmin** or your SmartBizOps application.

---

## 🧹 Maintenance Notes

* 🧾 Backups older than the number of days in `RETENTION_DAYS` are auto-deleted.
* 💽 Ensure enough disk space in the backup directory.
* 🔐 Consider encrypting backups if sensitive data is stored.
* ✅ Test restores periodically to ensure backup integrity.
* ☁️ Optionally upload backups to Google Drive or S3 for off-site redundancy.

---

## ⚠️ Common Errors & Fixes

| Error Message                                            | Cause                             | Solution                                                                    |
| -------------------------------------------------------- | --------------------------------- | --------------------------------------------------------------------------- |
| `[WinError 2] The system cannot find the file specified` | `pg_dump` not found in PATH       | Add PostgreSQL `bin` folder to PATH or hardcode its full path in the script |
| `pg_dump: error: connection to database failed`          | Wrong credentials or SSL mode     | Verify credentials in `credentials.env`, set `PGSSLMODE=require`            |
| `Permission denied` when saving backup                   | Restricted backup folder path     | Use a writable directory (e.g., `D:\SmartBizOpsBackups`)                    |
| Backup file is empty                                     | Connection timed out or SSL issue | Ensure Neon DB allows external connections and `sslmode=require` is set     |

---

## ✅ Quick Reference Summary

| Action                  | Command                                                      | Description                             |
| ----------------------- | ------------------------------------------------------------ | --------------------------------------- |
| Run backup manually     | `python backup_smartbizops.py`                               | Creates a new `.dump` file              |
| Check `pg_dump` version | `pg_dump --version`                                          | Confirms PostgreSQL tools are available |
| Restore backup          | `pg_restore -h <host> -U <user> -d postgres -C -v file.dump` | Restores the database                   |
| Automate backup         | Task Scheduler / cron                                        | Runs backup daily                       |
| Env file location       | `credentials.env`                                            | Must be in same folder as script        |

---

🧠 **Pro Tip:**
You can also extend this script to:

* Compress backups (`.zip` or `.gpg`)
* Upload to Google Drive, Dropbox, or AWS S3
* Send email notifications after backup completion

---

📦 **SmartBizOps DB Backup Utility**
Created & maintained by **Muhammad Muzzamil Nagda**
© 2025 – All rights reserved.

```

---

Would you like me to also generate a **shorter “Quick Setup” version** (like a 10-line summary) for your main `README.md` so users know there’s a full backup guide in `/DbBackup/README.md`?
```
