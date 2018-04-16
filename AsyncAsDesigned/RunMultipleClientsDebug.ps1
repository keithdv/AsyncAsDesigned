
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

For($i=1; $i -le $num; $i++)
{
    $processes += startProcess -dir .\AsyncAsDesigned.PerfClient -cmdArgs "run", "--configuration Debug", "--no-build", "10", "$i", "uniquePipeName"
    Start-Sleep -Milliseconds 25
    $processes += startProcess -dir .\AsyncAsDesigned.PerfDataServer -cmdArgs "run", "--configuration Debug", "--no-build", "$i", "uniquePipeName"
    Start-Sleep -Milliseconds 25
}


