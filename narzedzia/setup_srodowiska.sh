#!/usr/bin/env bash
# Stawia środowisko pracy Claude w chmurze (Ubuntu 24.04, root, sieć włączona).
# Uruchom: bash narzedzia/setup_srodowiska.sh   (około 3-5 minut)
# Sprawdzone 8.11.2026: Godot 4.7.2 .NET, .NET SDK 8.0, Blender bpy 5.2.3.
set -e
GODOT_VER=${GODOT_VER:-4.7.2}

# 1. Blender jako moduł Pythona + odczyt EXR (potok grafiki, PIPELINE_GRAFIKI.md)
pip install -q --break-system-packages bpy OpenEXR numpy pillow

# 2. .NET 8 SDK (symulacja Latarnia.Sim i skrypty Godota w C#)
curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
bash /tmp/dotnet-install.sh --channel 8.0 --install-dir "$HOME/.dotnet" >/dev/null
grep -q DOTNET_ROOT ~/.bashrc || echo 'export DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$PATH DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1' >> ~/.bashrc
export DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$PATH

# 3. Godot .NET (GitHub releases nie działa przez proxy, oficjalny serwer pobrań tak)
curl -sSL -o /tmp/godot.zip "https://downloads.godotengine.org/?version=${GODOT_VER}&flavor=stable&slug=mono_linux_x86_64.zip&platform=linux.64"
mkdir -p "$HOME/godot" && unzip -oq /tmp/godot.zip -d "$HOME/godot"
ln -sf "$HOME/godot/Godot_v${GODOT_VER}-stable_mono_linux_x86_64/Godot_v${GODOT_VER}-stable_mono_linux.x86_64" /usr/local/bin/godot
mkdir -p ~/.nuget/local && cp "$HOME"/godot/*/GodotSharp/Tools/nupkgs/*.nupkg ~/.nuget/local/
# W projekcie Godota dodaj nuget.config ze źródłem /root/.nuget/local (pakiety Godot.NET.Sdk).

# 4. Wirtualny ekran do renderu bez monitora: xvfb-run -a godot --rendering-driver opengl3
apt-get update -qq >/dev/null 2>&1 || true
apt-get install -y -qq xvfb libgl1-mesa-dri mesa-vulkan-drivers libfontconfig1 >/dev/null

echo "Gotowe: $(godot --version --headless 2>/dev/null | tail -1), dotnet $(dotnet --version), bpy $(python3 -c 'import bpy;print(bpy.app.version_string)')"
