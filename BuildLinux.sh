#!/usr/bin/env bash
set -euo pipefail
unity_editor="${1:?Informe o caminho do Editor Unity ativado}"
project_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
mkdir -p "$project_root/Builds"
"$unity_editor" -batchmode -nographics -quit -projectPath "$project_root" -executeMethod Aurora.EditorTools.ProjectBuilder.BuildLinux -logFile "$project_root/Builds/linux-build.log"
test -f "$project_root/Builds/Linux/EcosDeAurora.x86_64"
tar -czf "$project_root/Builds/EcosDeAurora-Linux.tar.gz" -C "$project_root/Builds/Linux" .
printf '%s\n' "Build Linux criado em Builds/EcosDeAurora-Linux.tar.gz"
