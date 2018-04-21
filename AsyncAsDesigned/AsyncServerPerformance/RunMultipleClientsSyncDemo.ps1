#Run 5 Clients each sending 5 Tokens

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

#Stop-Process -Name "dotnet" -ErrorAction SilentlyContinue

(startProcess -dir "AsyncAsDesigned.PerfDataServer" -cmdArgs "build", "--configuration Release").WaitForExit();
(startProcess -dir "AsyncAsDesigned.PerfClient" -cmdArgs "build", "--configuration Release").WaitForExit();
(startProcess -dir "AsyncAsDesigned.PerfAppServer" -cmdArgs "build", "--configuration Debug").WaitForExit();


$guid = New-Guid;
$numClients = 5;

For($i=1; $i -le $numClients; $i++)
{
    startProcess -dir .\AsyncAsDesigned.PerfClient -cmdArgs "run", "--configuration Release", "--no-build", "5", "$i", "$guid"
    Start-Sleep -Milliseconds 25
    startProcess -dir .\AsyncAsDesigned.PerfDataServer -cmdArgs "run", "--configuration Release", "--no-build", "$i", "$guid"
    Start-Sleep -Milliseconds 25
}


$appProcess = startProcess -dir .\AsyncAsDesigned.PerfAppServer -cmdArgs "run", "--configuration Debug", "--no-build", "sync", "$numClients", "$guid"#, "10" # Run with 10 threads
