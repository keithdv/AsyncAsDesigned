
#Set the working directory to the location of the script
$path = $MyInvocation.MyCommand.Path
$dir = Split-Path $path
Push-Location $dir
[Environment]::CurrentDirectory = $PWD


#dotnet build --project .\AsyncAsDesigned.PerfClient\AsyncAsDesigned.PerfClient.csproj --"configuration Debug"


Stop-Process -Name "dotnet"
   
Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfDataServer -ArgumentList "build", "--configuration Release", "/maxcpucount:1"
Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "build", "--configuration Release"
Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfAppServer -ArgumentList "build", "--configuration Release"

#Can't wait the above
#https://github.com/Microsoft/msbuild/issues/2269

Start-Sleep 30

for($num = 1; $num -le 15; $num++){


    
    Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfDataServer -ArgumentList "run", "--configuration Release", "--no-build"

    For($i=1; $i -le $num; $i++)
    {
        Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "run", "--configuration Release", "--no-build", "25", "$i"
    }


    Start-Process -Wait -WindowStyle Normal  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfAppServer -ArgumentList "run", "--configuration Release", "--no-build", "sync", "$num"

    Start-Sleep -Seconds 30

    Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfDataServer -ArgumentList "run", "--configuration Release", "--no-build"

    For($i=1; $i -le $num; $i++)
    {
        Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "run", "--configuration Release", "--no-build", "25", "$i"
    }


    Start-Process -Wait -WindowStyle Normal  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfAppServer -ArgumentList "run", "--configuration Release", "--no-build", "async", "$num"

    Write-Output "$num is done"

    Start-Sleep 30

}

