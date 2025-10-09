Name "DINFO"

# define installer name
OutFile "win-installer.exe"
 
# set desktop as install directory
InstallDir $APPDATA/SCSDC/DINFO
 
# default section start
Section
 
# define output path
SetOutPath $INSTDIR
 
# specify file to go in output path
File dinfo.exe
 
# define uninstaller name
WriteUninstaller $INSTDIR\uninstaller.exe

EnVar::AddValue "PATH" "$INSTDIR"
 
#-------
# default section end
SectionEnd
 
# create a section to define what the uninstaller does.
# the section will always be named "Uninstall"
Section "Uninstall"
 
# Delete installed file
Delete $INSTDIR\dinfo.exe
 
# Delete the uninstaller
Delete $INSTDIR\uninstaller.exe
 
# Delete the directory
RMDir $INSTDIR
SectionEnd
