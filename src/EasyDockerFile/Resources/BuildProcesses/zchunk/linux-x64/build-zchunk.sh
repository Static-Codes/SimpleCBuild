#!/bin/bash

# EXIT CODE LIST:
# https://medium.com/@himanshurahangdale153/list-of-exit-status-codes-in-linux-f4c00c46c9e0
# 3 => Unable to install build dependencies
# 4 => Unable to create temporary directory
# 5 => Failed to navigate to build directory
# 6 => Failed to clone zchunk repository
# 7 => Failed to install build dependencies
# 8 => Failed to compile static library 
# 9 => Build tests failed.
# 10 => Failed to remove build dependencies

TMP_DIR="$HOME/.tmp"
TMP_CLONE_DIR="$TMP_DIR/zchunk"

echo -e "[INFO]: Installing build dependencies."
echo -e "[INFO]: You will be prompted for your super user password.\n"
echo -e "[COMMAND]: sudo apt update && sudo apt install meson ninja-build pkg-config libcurl4-openssl-dev libzstd-dev libssl-dev zlib1g-dev\n"
sudo apt update && sudo apt install git meson ninja-build pkg-config libcurl4-openssl-dev libzstd-dev libssl-dev zlib1g-dev || {  echo -e "[ERROR]: Failed to install build dependencies\n"; exit 3; }
echo -e "[SUCCESS]: Installed build depdendencies\n"

echo -e "[INFO]: Creating temporary directory at $TMP_DIR/\n"
mkdir -p "$TMP_DIR" || { echo -e "[ERROR]: Failed to created temporary directory\n"; exit 4; }
echo -e "[SUCCESS]: Created temporary directory.\n"

cd "$TMP_DIR" || { echo -e "[ERROR]: Failed to navigate to $TMP_DIR\n"; exit 5; }

echo -e "[INFO]: Cloning the zchunk git repository to $TMP_CLONE_DIR\n"
echo -e "[COMMAND]: git clone https://github.com/zchunk/zchunk\n"
git clone https://github.com/zchunk/zchunk || { echo "[ERROR]: Failed to clone the zchunk git repository\n"; exit 6; }
echo -e "[SUCCESS]: Cloned repo to $TMP_CLONE_DIR\n"

echo -e "[INFO]: Navigating to $TMP_CLONE_DIR"
cd "$TMP_CLONE_DIR" || { echo -e "[ERROR]: Unable to navigate to $TMP_CLONE_DIR\n"; exit 5; }
echo -e "[SUCCESS]: Navigated to $TMP_CLONE_DIR"

echo -e "[INFO]: Starting build process for zchunk\n"
echo -e "[COMMAND]: CFLAGS=-I/usr/local/include CXXFLAGS=-I/usr/local/include LDFLAGS=-L/usr/local/lib meson build --default-library=static \n"
meson setup build --default-library=static -Dbuildtype=release || {  echo -e "[ERROR]: Failed to start build process.\n"; exit 7; }
echo -e "[SUCCESS]: Successfully built meson setup files\n"

echo -e "[INFO]: Starting static library compilation\n"
echo -e "[COMMAND]: cd build && ninja\n"
cd build && ninja || { echo -e "[ERROR]: Unable to compile a static library for zchunk\n"; exit 8; }
echo -e "[SUCCESS]: Static library compilation complete\n"

echo -e "[INFO]: Running build tests\n"
echo -e "[COMMAND]: ninja test\n"
ninja test || { echo -e "[ERROR]: Unable to run build tests\n"; exit 9; }
echo -e "[SUCCESS]: All build tests passed\n"

echo -e "[INFO]: Removing build dependencies\n"
echo -e "[COMMAND]: sudo apt purge -y meson ninja-build pkg-config libcurl4-openssl-dev libzstd-dev libssl-dev zlib1g-dev"
sudo apt purge -y meson ninja-build pkg-config libcurl4-openssl-dev libzstd-dev libssl-dev zlib1g-dev ||  {  echo -e "[ERROR]: Failed to remove build dependencies\n"; exit 10; }
echo -e "[SUCCESS]: Removed build dependencies\n"