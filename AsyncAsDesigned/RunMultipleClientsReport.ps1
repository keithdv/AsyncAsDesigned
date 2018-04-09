
#Set the working directory to the location of the script
$path = $MyInvocation.MyCommand.Path
$dir = Split-Path $path
Push-Location $dir
[Environment]::CurrentDirectory = $PWD


#dotnet build --project .\AsyncAsDesigned.PerfClient\AsyncAsDesigned.PerfClient.csproj --"configuration Debug"


Stop-Process -Name "dotnet"
    

for($num = 1; $num -le 10; $num++){


    
    Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfDataServer -ArgumentList "run", "--configuration Release", "--no-build"

    For($i=1; $i -le $num; $i++)
    {
        Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "run", "--configuration Release", "--no-build", "25", "$i"
    }


    Start-Process -Wait -WindowStyle Normal  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfAppServer -ArgumentList "run", "--configuration Release", "--no-build", "sync", "$num"

    Start-Sleep -Seconds 1

    Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfDataServer -ArgumentList "run", "--configuration Release", "--no-build"

    For($i=1; $i -le $num; $i++)
    {
        Start-Process -WindowStyle Hidden  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfClient -ArgumentList "run", "--configuration Release", "--no-build", "25", "$i"
    }


    Start-Process -Wait -WindowStyle Normal  "dotnet" -WorkingDirectory .\AsyncAsDesigned.PerfAppServer -ArgumentList "run", "--configuration Release", "--no-build", "async", "$num"

    Start-Sleep -Seconds 1

    Write-Output "$num is done"

}

