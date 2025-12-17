#!/usr/bin/env bash
set -e
ROOT="/Users/maxim/Desktop/labworks_itmo/PhysicsProject"
cd "$ROOT" || exit 1
echo "Heuristic scan — возможные неиспользуемые файлы (могут быть ложные срабатывания)."
# helper: use rg if available, else grep
HAS_RG=0
command -v rg >/dev/null 2>&1 && HAS_RG=1

search() {
  local term="$1"
  if [ $HAS_RG -eq 1 ]; then
    rg -n -F --hidden --glob '!**/bin/**' --glob '!**/obj/**' --glob '!**/.git/**' "$term" || true
  else
    # fallback to grep
    grep -RIn --exclude-dir=bin --exclude-dir=obj --exclude-dir=.git -- "$term" . || true
  fi
}

echo
echo "---- Проверка C# файлов ----"
find . -type f -name '*.cs' -not -path './**/bin/*' -not -path './**/obj/*' | while read -r f; do
  name=$(basename "$f" .cs)
  # пропускаем очевидные точки входа/настройки
  case "$name" in
    Program|Startup|AssemblyInfo|DependencyInjection|GlobalUsings) continue;;
  esac
  # ищем упоминания имени (исключая сам файл)
  if [ $HAS_RG -eq 1 ]; then
    hits=$(rg -n -F --hidden --glob '!**/bin/**' --glob '!**/obj/**' --glob '!**/.git/**' "$name" | rg -v "^$f:" || true)
  else
    hits=$(grep -RIn --exclude-dir=bin --exclude-dir=obj --exclude-dir=.git -- "$name" . | grep -v "^$f:" || true)
  fi
  if [ -z "$hits" ]; then
    echo "POSSIBLY UNUSED: $f (name: $name)"
  fi
done

echo
echo "---- Проверка static/wwwroot ----"
if [ -d "PhysicsProject.Api/wwwroot" ]; then
  find PhysicsProject.Api/wwwroot -type f \( -name '*.js' -o -name '*.css' -o -name '*.html' -o -name '*.png' -o -name '*.svg' \) 2>/dev/null | while read -r f; do
    name=$(basename "$f")
    if [ $HAS_RG -eq 1 ]; then
      hits=$(rg -n -F --hidden --glob '!**/bin/**' --glob '!**/obj/**' --glob '!**/.git/**' "$name" || true)
    else
      hits=$(grep -RIn --exclude-dir=bin --exclude-dir=obj --exclude-dir=.git -- "$name" . || true)
    fi
    if [ -z "$hits" ]; then
      echo "POSSIBLY UNUSED STATIC: $f"
    fi
  done
else
  echo "Нет папки PhysicsProject.Api/wwwroot — пропускаю статические файлы."
fi

echo
echo "Готово. Присылайте вывод, я помогу интерпретировать и дам следующий шаг."