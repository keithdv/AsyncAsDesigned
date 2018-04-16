
function startProcess ($dir, $cmdArgs) {
 $ProcessInfo = New-Object System.Diagnostics.ProcessStartInfo
 $ProcessInfo.FileName = "dotnet"
 $ProcessInfo.WorkingDirectory = (Resolve-Path -Path $dir)
 $ProcessInfo.Arguments = $cmdArgs
 $ProcessInfo.UseShellExecute = $False
 $ProcessInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Hidden
 $newProcess = [System.Diagnostics.Process]::Start($ProcessInfo)
 $newProcess.PriorityClass = [System.Diagnostics.ProcessPriorityClass]::RealTime
 $newProcess
}

#Set the working directory to the location of the script
$path = $MyInvocation.MyCommand.Path
$dir = Split-Path $path
Push-Location $dir
[Environment]::CurrentDirectory = $PWD


#dotnet build --project .\AsyncAsDesigned.PerfClient\AsyncAsDesigned.PerfClient.csproj --"configuration Debug"


Stop-Process -Name "dotnet" -ErrorAction SilentlyContinue


  
(startProcess -dir "AsyncAsDesigned.PerfDataServer" -cmdArgs "build", "--configuration Release").WaitForExit();
(startProcess -dir "AsyncAsDesigned.PerfClient" -cmdArgs "build", "--configuration Release").WaitForExit();
(startProcess -dir "AsyncAsDesigned.PerfAppServer" -cmdArgs "build", "--configuration Debug").WaitForExit();



$processes = @();


    $syncGuid = New-Guid;
    $asyncGuid = New-Guid;

    for($num = 3; $num -le 3; $num = $num + 1){

        $processes = @();
    
        For($i=1; $i -le $num; $i++)
        {
            $processes += startProcess -dir .\AsyncAsDesigned.PerfClient -cmdArgs "run", "--configuration Release", "--no-build", "10", "$i", "$syncGuid"
            Start-Sleep -Milliseconds 25
            $processes += startProcess -dir .\AsyncAsDesigned.PerfDataServer -cmdArgs "run", "--configuration Release", "--no-build", "$i", "$syncGuid"
            Start-Sleep -Milliseconds 25
        }

        # Wait for everything to startup (Probably unneccessary)
        Start-Sleep 1

        $appProcessSync = startProcess -dir .\AsyncAsDesigned.PerfAppServer -cmdArgs "run", "--configuration Debug", "--no-build", "sync", "$num", "$syncGuid"

        foreach($b in $processes){ $b.WaitForExit(); }

        Write-Output "$num sync is done"

        $processes = @();

        For($i=1; $i -le $num; $i++)
        {
            $processes += startProcess -dir .\AsyncAsDesigned.PerfClient -cmdArgs "run", "--configuration Release", "--no-build", "10", "$i", "$asyncGuid"
            Start-Sleep -Milliseconds 25
            $processes += startProcess -dir .\AsyncAsDesigned.PerfDataServer -cmdArgs "run", "--configuration Release", "--no-build", "$i", "$asyncGuid"
            Start-Sleep -Milliseconds 25
        }

        $appProcessAsync = startProcess -dir .\AsyncAsDesigned.PerfAppServer -cmdArgs "run", "--configuration Debug", "--no-build", "async", "$num", "$asyncGuid"

        foreach($b in $processes){ $b.WaitForExit(); }

        

    }





