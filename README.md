# INSTALLER Downloader

INSTALLER Downloader was a small utility designed to quickly configure **INSTALLER** on a USB drive or any other path without having to go through the initial setup process.  
It was primarily used to deploy INSTALLER on multiple USB drives when needed, but also served as a restoration tool if INSTALLER stopped working.

## Features

- **Quick Setup:** Deploy INSTALLER without repeating the full initial configuration.
- **Self-Updating:** Automatically downloads the latest INSTALLER version and database.
- **Automatic Extraction:** Unpacks the files and replaces the old installation.
- **Fallback Recovery:** Restores a broken or non-starting INSTALLER installation.
- **Portable:** Could run directly from the database folder of INSTALLER.

## How It Worked

When launched, INSTALLER Downloader would:
1. Check for the latest available INSTALLER release and its database.
2. Download both files.
3. Extract and overwrite the existing files in the target directory.
4. Confirm completion and allow immediate use.

It was included in the **main INSTALLER database** under a subfolder, so it was always available for quick deployment or recovery.

## Status

This tool has not been functional for several years due to broken download links and changes in file hosting.

## Requirements (at the time)

- Windows operating system.
- Internet connection to fetch updates.
- A USB drive or target path for deployment.

## License

Provided for archival and historical purposes only.
