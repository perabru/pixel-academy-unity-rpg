param([Parameter(Mandatory=$true)][string]$UnityEditor)
$ErrorActionPreference = 'Stop'
if (!(Test-Path -LiteralPath $UnityEditor)) { throw 'Editor Unity não encontrado.' }
$ProjectRoot = $PSScriptRoot
New-Item -ItemType Directory -Force -Path "$ProjectRoot/Builds" | Out-Null
$BuildLog = "$ProjectRoot/Builds/windows-build.log"
$Process = Start-Process -FilePath $UnityEditor -ArgumentList @('-batchmode','-nographics','-quit','-projectPath',('"'+$ProjectRoot+'"'),'-executeMethod','Aurora.EditorTools.ProjectBuilder.BuildWindows','-logFile',('"'+$BuildLog+'"')) -Wait -PassThru
if ($Process.ExitCode -ne 0) { throw "Unity terminou com erro. Consulte $BuildLog" }
$Executable = "$ProjectRoot/Builds/Windows/EcosDeAurora.exe"
if (!(Test-Path -LiteralPath $Executable)) { throw 'O executável não foi gerado. Não será criado um ZIP de jogo vazio.' }
$OutputZip = "$ProjectRoot/Builds/EcosDeAurora-Windows.zip"
Compress-Archive -Path "$ProjectRoot/Builds/Windows/*" -DestinationPath $OutputZip -Force
Write-Host "Build Windows criado: $OutputZip"
