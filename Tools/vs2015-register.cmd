@ECHO OFF
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.shader" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.cginc" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.compute" /f
reg delete "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.glslinc" /f

pause