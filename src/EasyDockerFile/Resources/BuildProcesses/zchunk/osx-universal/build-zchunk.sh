#!/bin/bash

# EXIT CODE LIST:
# https://medium.com/@himanshurahangdale153/list-of-exit-status-codes-in-linux-f4c00c46c9e0
# 3 => User rejected homebrew setup.
# 4 => Failed to install build dependencies
# 5 => Unable to create temporary directory
# 6 => Failed to navigate to build directory
# 7 => Failed to clone zchunk repository
# 8 => Failed to start the library build process
# 9 => Failed to compile static library
# 10 => Failed to remove build dependencies

TMP_DIR="$HOME/.tmp"
TMP_CLONE_DIR="$TMP_DIR/zchunk"

# --- Checking for homebrew installation status, prompting the user to install if needbe. ---
if command -v brew >/dev/null 2>&1; then
    echo -e "[SUCCESS]: Homebrew v$(brew --version | awk 'NR==1{print $2}') detected\n"
else
    echo -e "[INFO]: Homebrew was not found on your system.\n"
    read -p "[INPUT]: Would you like to install it now? (y/n): " confirm
    if [[ "$confirm" == [yY] || "$confirm" == [yY][eE][sS] ]]; then
        echo -e "[INFO]: Starting Homebrew installation"
        echo -e "[COMMAND]: /bin/bash -c \"$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\"\n"
        /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

        # Creating a shell environment for both the current and future sessions.
        if [[ $(uname -m) == "arm64" ]]; then
            # M-Series path setup
            (echo; echo 'eval "$(/opt/homebrew/bin/brew shellenv)"') >> "$HOME/.zprofile"
            eval "$(/opt/homebrew/bin/brew shellenv)"
        else
            # Intel path setup
            (echo; echo 'eval "$(/usr/local/bin/brew shellenv)"') >> "$HOME/.zprofile"
            eval "$(/usr/local/bin/brew shellenv)"
        fi
        echo -e "[SUCCESS]: Homebrew installation complete!\n"
    else
        echo -e "[WARNING]: Homebrew is required to install build dependencies.\n"
        echo -e "[INFO]: Please install homebrew, or install meson and ninja.\n"
        exit 3
    fi
fi

echo -e "[INFO]: Installing build dependencies\n"
brew install meson ninja git || { echo -e "[ERROR]: Failed to install build dependencies"; exit 4; }
echo -e "[SUCCESS]: Installed build dependencies\n"

echo -e "[INFO]: Creating temporary directory at $TMP_DIR/\n"
mkdir -p "$TMP_DIR" || { echo -e "[ERROR]: Failed to created temporary directory\n"; exit 5; }
echo -e "[SUCCESS]: Created temporary directory.\n"

cd "$TMP_DIR" || { echo -e "[ERROR]: Failed to navigate to $TMP_DIR\n"; exit 6; }

if [ -d "$TMP_CLONE_DIR" ]; then
    echo -e "[INFO]: Existing repository found at $TMP_CLONE_DIR.\n"
    echo -e "[INFO]: Removing for a clean build\n"
    rm -rf "$TMP_CLONE_DIR" || { echo -e "[ERROR]: Failed to remove existing directory\n"; exit 5; }
fi

echo -e "[INFO]: Cloning the zchunk git repository to $TMP_CLONE_DIR\n"
echo -e "[COMMAND]: git clone https://github.com/zchunk/zchunk\n"

git clone https://github.com/zchunk/zchunk || { echo "[ERROR]: Failed to clone the zchunk git repository\n"; exit 7; }
echo -e "[SUCCESS]: Cloned repo to $TMP_CLONE_DIR\n"

echo -e "[INFO]: Navigating to $TMP_CLONE_DIR\n"
cd "$TMP_CLONE_DIR" || { echo -e "[ERROR]: Unable to navigate to $TMP_CLONE_DIR\n"; exit 6; }
echo -e "[SUCCESS]: Navigated to $TMP_CLONE_DIR\n"

echo -e "[INFO]: Starting the Universal build process for zchunk\n"

rm -rf build-universal

env CFLAGS="-arch arm64 -arch x86_64" \
    LDFLAGS="-arch arm64 -arch x86_64"

meson setup build-universal --default-library=static -Dbuildtype=release || { echo "[ERROR]: Unable to start the build process"; exit 8; }

ninja -C build-universal || { echo "[ERROR]: Unable to build a Universal/Fat binary for zchunk"; exit 9; }

echo -e "[SUCCESS]: Universal binary created at $TMP_CLONE_DIR/universal/src/unzck\n"

read -p "[INPUT]: Would you like to remove the build dependencies now? (git, meson, ninja) [y/n]: " confirm
    if [[ "$confirm" == [yY] || "$confirm" == [yY][eE][sS] ]]; then
        echo -e "[INFO]: Removing build dependencies\n"
        echo -e "[COMMAND]: brew uninstall meson ninja git\n"
        brew uninstall meson ninja git || { echo -e "[ERROR]: Failed to remove build dependencies.\n"; exit 10; } 
        echo -e "[SUCCESS]: Removed build dependencies\n"
    fi



