@ECHO OFF
REM VS2015
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.shader" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.cginc" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.compute" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.glslinc" /f

REM VS2013
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\12.0_Config\Languages\File Extensions\.shader" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\12.0_Config\Languages\File Extensions\.cginc" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\12.0_Config\Languages\File Extensions\.compute" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\12.0_Config\Languages\File Extensions\.glslinc" /f

REM VS2012
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\12.0_Config\Languages\File Extensions\.shader" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\12.0_Config\Languages\File Extensions\.cginc" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\12.0_Config\Languages\File Extensions\.compute" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\12.0_Config\Languages\File Extensions\.glslinc" /f

pause
