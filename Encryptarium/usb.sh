#!/bin/bash
echo "Hello world!"

# $1 — первый аргумент (путь до флешки)
FLASH_PATH="$1"

# Проверка, что аргумент передан
if [ -z "$FLASH_PATH" ]; then
    echo "using: $0 /путь/до/флешки"
    exit 1
fi
echo "Working with flesh: $FLASH_PATH"

# Проверка, что путь существует и это каталог
if [ ! -d "$FLASH_PATH" ]; then
    echo "Error: '$FLASH_PATH' not exists or no directory"
    exit 1
fi
echo "Working with flesh: $FLASH_PATH"

DEVICE=$(findmnt -no SOURCE "$FLASH_PATH")
if [ -z "$DEVICE" ]; then
    echo "No mount path"
    exit 1
fi
echo "Working with flesh: $FLASH_PATH"
echo "Device: $DEVICE"
ls -la "$FLASH_PATH"