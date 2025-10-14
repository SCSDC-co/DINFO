@echo off


FOR /F "tokens=* USEBACKQ" %%F IN (`powershell -Command "$pm=[Windows.Management.Deployment.PackageManager,Windows.Management.Deployment,ContentType=WindowsRuntime]::new(); $pkg=$pm.FindPackagesForUser([System.Security.Principal.WindowsIdentity]::GetCurrent().User.Value)|Where-Object{$_.Id.Name -eq '42369Parallexindiedevs.DINFODirectoryInfo'}; if($pkg){$machine='C:\Program Files\WindowsApps\' + $pkg.Id.FullName; if(Test-Path $machine){Write-Output $machine}else{Write-Output 'error'}} else {Write-Output 'error'}"
`) DO (
SET "dinfo_path=%%F"
)

set "USERPATH="
for /f "skip=2 tokens=2,*" %%A in ('reg query "HKCU\Environment" /v Path 2^>nul') do (
    set "USERPATH=%%B"
)

if "%dinfo_path%"=="error" goto error else goto noerror

:noerror
SET "dinfo_path=%dinfo_path%\VFS\ProgramFilesX86\SCSDC DINFO"

echo %PATH% | findstr /i /c:"%dinfo_path%" >nul
if %errorlevel%==0 (
echo DINFO Installation was already completed. Don't run this, run dinfo in a terminal.
goto end
) else (
powershell -noprofile -command "[Environment]::SetEnvironmentVariable('PATH', '%USERPATH%;%dinfo_path%', 'User');"
echo DINFO Finished installing successfully! Open a new terminal and run dinfo!
goto end
)

:error
echo DINFO does not look like it was installed correctly. Try reinstalling it.
goto end



:end
pause

:quit
