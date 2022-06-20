#Originally created by Elizabeth Clements
#Please read the LICENSE file found in the github or in the root directory of this project (https://github.com/bluedoge/panic-at-the-loadbalancer)
#
# This script assumes it is executing from the root directory, and is on a Windows OS


$SourceRoot = "./src/"
$ReleaseBinPath = "bin/Release/net6.0/"
#$DebugBinPath = "bin/Debug/net6.0/"
$PackagedPath = "./packaged/"
$PackagedExecutableName = "patlb"
$MaximumCompileAttempts = 2


#Resolve the Source Root Directory to a full path
$WorkingDir = (Get-Location).Path + "\"
$SourceRoot = $WorkingDir + $SourceRoot
$PackagedPath = $WorkingDir + $PackagedPath
$ReleaseBinPath = ($SourceRoot + "bin/Release/net6.0/")
#$DebugBinPath = ($SourceRoot + "bin/Debug/net6.0/")

#"SourceRoot = " + $SourceRoot
#"PackagedPath = " + $PackagedPath
#"ReleaseBinPath = " + $ReleaseBinPath

function Run-ForcedCommand()
{
    param([string]$program = (throw "Specify a program")
        ,[string]$argumentString = ""
        ,[string]$workingDirectory = (throw "Specify the working directory")
        ,[switch]$waitForExit)
    $psi = New-Object "Diagnostics.ProcessStartInfo"

    $psi.FileName = $program
    $psi.Arguments = $argumentString
    $psi.WorkingDirectory = $workingDirectory
    $proc = [Diagnostics.Process]::Start($psi)
    if($waitForExit)
    {
        $proc.WaitForExit();
    }
}

function Get-PackageIsCompiled()
{
    Test-Path -Path $ReleaseBinPath -PathType Container
}

function Create-PackagedDirectory()
{
    if(-not(Test-Path -Path $PackagedPath -PathType Container))
    {
        $newitem = New-Item $PackagedPath -ItemType Directory
    }
}

function Copy-PackagedBinaries()
{
    if(Test-Path -Path $ReleaseBinPath -PathType Container)
    {
        try
        {
            Copy-Item ($ReleaseBinPath+"*.exe") -Destination ($PackagedPath) -Force -Recurse
            Copy-Item ($ReleaseBinPath+"*.dll") -Destination ($PackagedPath) -Force -Recurse
            Copy-Item ($ReleaseBinPath+"*.json") -Destination ($PackagedPath) -Force -Recurse
        }
        catch
        {
            throw $_.Exception.Message
        }
    }
    else
    {
        throw "Packer could not access any bins. Please compile the project first."
    }
}

if(-not(Get-PackageIsCompiled))
{
    "Package is not compiled... attempting build..."
}

# A sentinel value to ensure the loop doesn't go on forever, assuming a critical failure
$Steps = 0
while(-not(Get-PackageIsCompiled) -and ($Steps -lt $MaximumCompileAttempts))
{
    # Move out of root, step into src
    Set-Location $SourceRoot
    "Building... Attempt($($Steps + 1) of $($MaximumCompileAttempts))"
    Run-ForcedCommand -program "C:\Windows\System32\cmd.exe" -argumentString "/c `"dotnet build panic-at-the-loadbalancer.sln -c Release`"" -workingDirectory $SourceRoot -waitForExit
    $Steps = $Steps + 1
    # Step back out to root
    Set-Location ".."
}

if(-not(Get-PackageIsCompiled))
{
    throw "Packer couldn't compile project. Critical error!"
}

# Copy those floppies
"Attempting to package the binaries."
Create-PackagedDirectory
Copy-PackagedBinaries
"Binaries packaged."