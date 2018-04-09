
#Set the working directory to the location of the script
$path = $MyInvocation.MyCommand.Path
$dir = Split-Path $path
Push-Location $dir
[Environment]::CurrentDirectory = $PWD


#dotnet build --project .\AsyncAsDesigned.PerfClient\AsyncAsDesigned.PerfClient.csproj --"configuration Debug"

$numToRun = 1, 5, 10, 25, 50, 100, 500, 1000, 5000, 10000

foreach($num in $numToRun){

Start-Process -WindowStyle Hidden "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfAppServer -ArgumentList "run", "--configuration Release", "--no-build", "sync"
Start-Process -WindowStyle Hidden "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfDataServer -ArgumentList "run", "--configuration Release", "--no-build"
Start-Process -WindowStyle Hidden -Wait "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "run", "--configuration Release", "--no-build", "$num"

Start-Sleep -Seconds 1
Stop-Process -Name "dotnet"
Start-Sleep -Seconds 1


Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfAppServer -ArgumentList "run", "--configuration Release", "--no-build", "async"
Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfDataServer -ArgumentList "run", "--configuration Release", "--no-build"
Start-Process -WindowStyle Hidden  -Wait "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "run", "--configuration Release", "--no-build", "$num"

Start-Sleep -Seconds 1
Stop-Process -Verb runas -Name "dotnet"
Start-Sleep -Seconds 1

Write-Output "$num is done"

}


