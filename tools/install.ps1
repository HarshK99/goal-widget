# Copies the built widget to a per-user folder, makes it start at sign-in, and starts it.
# The installed copy is separate from the build output, so a failed build cannot break the widget in use.
param([string]$Source = (Join-Path $PSScriptRoot '..\artifacts\widget-build'))
$ErrorActionPreference = 'Stop'

$target = Join-Path $env:LOCALAPPDATA 'Programs\GoalWidget'
if (-not (Test-Path (Join-Path $Source 'GoalWidget.exe'))) { throw "No GoalWidget.exe in $Source. Build the widget first." }

# The goal and position are saved on every change, so stopping the process loses nothing.
foreach ($running in Get-Process GoalWidget -ErrorAction SilentlyContinue) {
    [void]$running.CloseMainWindow()
    if (-not $running.WaitForExit(5000)) { Stop-Process -Id $running.Id -Force; $running.WaitForExit() }
}

if (Test-Path $target) { Remove-Item -LiteralPath $target -Recurse -Force }
New-Item -ItemType Directory -Force $target | Out-Null
Copy-Item -Path (Join-Path $Source '*') -Destination $target -Recurse

$shortcut = (New-Object -ComObject WScript.Shell).CreateShortcut((Join-Path ([Environment]::GetFolderPath('Startup')) 'Goal Widget.lnk'))
$shortcut.TargetPath = Join-Path $target 'GoalWidget.exe'
$shortcut.WorkingDirectory = $target
$shortcut.Description = 'Goal widget'
$shortcut.Save()

Start-Process -FilePath (Join-Path $target 'GoalWidget.exe') -WorkingDirectory $target
"Installed to $target and set to start at sign-in."
