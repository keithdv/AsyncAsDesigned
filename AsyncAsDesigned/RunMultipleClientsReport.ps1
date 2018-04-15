
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


Stop-Process -Name "dotnet"


  
(startProcess -dir "AsyncAsDesigned.PerfDataServer" -cmdArgs "build", "--configuration Release").WaitForExit();
(startProcess -dir "AsyncAsDesigned.PerfClient" -cmdArgs "build", "--configuration Release").WaitForExit();
(startProcess -dir "AsyncAsDesigned.PerfAppServer" -cmdArgs "build", "--configuration Release").WaitForExit();


$count = 0;
$processes = @();
$guid = New-Guid

while($true){


    for($num = 1; $num -le 15; $num++){

        $processes = @();

    
        For($i=1; $i -le $num; $i++)
        {
            $processes += startProcess -dir .\AsyncAsDesigned.PerfClient -cmdArgs "run", "--configuration Release", "--no-build", "25", "$i", "$guid"
            $processes += startProcess -dir .\AsyncAsDesigned.PerfDataServer -cmdArgs "run", "--configuration Release", "--no-build", "$i", "$guid"
        }

        # Wait for everything to startup (Probably unneccessary)
        Start-Sleep 2

        $appProcess = startProcess -dir .\AsyncAsDesigned.PerfAppServer -cmdArgs "run", "--configuration Release", "--no-build", "sync", "$num", "$guid"
        $appProcess.WaitForExit();

        foreach($b in $processes){ $b.WaitForExit(); }

        Write-Output "$num sync is done"

        $processes = @();

        For($i=1; $i -le $num; $i++)
        {
            $processes += startProcess -dir .\AsyncAsDesigned.PerfClient -cmdArgs "run", "--configuration Release", "--no-build", "25", "$i", "$guid"
            $processes += startProcess -dir .\AsyncAsDesigned.PerfDataServer -cmdArgs "run", "--configuration Release", "--no-build", "$i", "$guid"
        }

        # Wait for everything to startup (Probably unneccessary)
        Start-Sleep 2

        $appProcess = startProcess -dir .\AsyncAsDesigned.PerfAppServer -cmdArgs "run", "--configuration Release", "--no-build", "async", "$num", "$guid"
        $appProcess.WaitForExit();


        foreach($b in $processes){ $b.WaitForExit(); }


        Write-Output "$num async is done"
        

    }

    $count++;

        Write-Output "Full Set is done $count"

        Start-Sleep 15

}


