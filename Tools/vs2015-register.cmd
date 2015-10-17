@ECHO OFF
reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.shader" /t REG_SZ /d {c702cfb7-573c-45f4-9469-115fcb519ad2} /f  
reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\Languages\File Extensions\.cginc" /t REG_SZ /d {c702cfb7-573c-45f4-9469-115fcb519ad2} /f  

reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0Exp_Config\Languages\File Extensions\.shader" /t REG_SZ /d {c702cfb7-573c-45f4-9469-115fcb519ad2} /f  
reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0Exp_Config\Languages\File Extensions\.cginc" /t REG_SZ /d {c702cfb7-573c-45f4-9469-115fcb519ad2} /f  

pause