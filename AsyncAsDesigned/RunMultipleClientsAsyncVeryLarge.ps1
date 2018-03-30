
#Set the working directory to the location of the script
$path = $MyInvocation.MyCommand.Path
$dir = Split-Path $path
Push-Location $dir
[Environment]::CurrentDirectory = $PWD


dotnet build --project .\AsyncAsDesigned.PerfClient\AsyncAsDesigned.PerfClient.csproj --"configuration Debug"

Start-Process -Verb runas "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfAppServer -ArgumentList "run", "--configuration Debug", "async"
Start-Process -Verb runas "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfDataServer -ArgumentList "run", "--configuration Debug", "async"
Start-Process -Verb runas "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "run", "--configuration Debug", "--no-build", "100"
Start-Process -Verb runas "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "run", "--configuration Debug", "--no-build", "100"

