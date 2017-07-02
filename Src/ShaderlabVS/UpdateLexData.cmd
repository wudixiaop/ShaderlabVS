@ECHO OFF

SET FILE=1.cs
SET FILE2=2.cs
SET FILE3=3.cs
SET LEX=3.lex
SET LEX_LIST=3.lst
SET DATA_APP=data.exe


SET VS_DEV_BAT="%VS140COMNTOOLS%VsDevCmd.bat"

IF NOT EXIST %VS_DEV_BAT% (
    SET VS_DEV_BAT="%VS150COMNTOOLS%VsDevCmd.bat"
    IF NOT EXIST %VS_DEV_BAT% (
    	SET VS_DEV_BAT="%VS120COMNTOOLS%VsDevCmd.bat"
    )
) ELSE (
    GOTO :MAIN
)

IF NOT EXIST %VS_DEV_BAT% (
    ECHO "There are no supported Visual Studio version installed on your machine!!!"
    GOTO :PAUSE
) ELSE (
	GOTO :MAIN
)

GOTO :PAUSE

GOTO :EOF

REM --------------MAIN START---------
:MAIN
ECHO Found %VS_DEV_BAT%
CALL :CLEAN

ShaderlabVS.LexTools.exe %LEX%
gplex %LEX%

CALL :WRITE_CS_ONE
CALL :WRITE_CS_TWO

CALL %VS_DEV_BAT%
ECHO Building
csc /target:exe /out:%DATA_APP% %FILE% %FILE2% %FILE3%

IF EXIST %DATA_APP% (
    %DATA_APP%   
) ELSE (
    ECHO no data exe generated!!!
)

GOTO :CLEAN
GOTO :PAUSE

REM --------------MAIN END--------

:WRITE_CS_ONE

IF EXIST %FILE% (
    DEL /q %FILE%
)

echo namespace ShaderlabVS              >> %FILE%
echo {                                  >> %FILE%
echo    internal enum ShaderlabToken    >> %FILE%
echo    {                               >> %FILE%
echo       TEXT,                        >> %FILE%
echo        COMMENT,                    >> %FILE%
echo        STRING_LITERAL,             >> %FILE%
echo        HLSLCGKEYWORD,              >> %FILE%
echo        HLSLCGKEYWORDSPECIAL,       >> %FILE%
echo        HLSLCGDATATYPE,             >> %FILE%
echo        HLSLCGFUNCTION,             >> %FILE%
echo        UNITYKEYWORD,               >> %FILE%
echo        UNITYKEYWORD_PARA,          >> %FILE%
echo        UNITYVALUES,                >> %FILE%
echo        UNITYDATATYPE,              >> %FILE%
echo        UNITYFUNCTION,              >> %FILE%
echo        UNITYMACROS,                >> %FILE%
echo        UNDEFINED,                  >> %FILE%
echo    }                               >> %FILE%
echo }                                  >> %FILE%

GOTO :EOF

:WRITE_CS_TWO

IF EXIST %FILE2% (
    DEL /q %FILE2%
)

echo using ShaderlabVS.Lexer;                   >> %FILE2%
echo namespace ShaderlabVS.LexData              >> %FILE2%
echo {                                          >> %FILE2%
echo     class Program                          >> %FILE2%
echo     {                                      >> %FILE2%
echo         static void Main(string[] args)    >> %FILE2%
echo         {                                  >> %FILE2%
echo             Scanner.GenerateTableData();   >> %FILE2%
echo         }                                  >> %FILE2%
echo     }                                      >> %FILE2%
echo }                                          >> %FILE2%

GOTO :EOF

:CLEAN
DEL /q %FILE%
DEL /q %FILE2%
DEL /q %LEX%
DEL /q %LEX_LIST%
DEL /q %FILE3%
DEL /q %DATA_APP%
GOTO :EOF

:PAUSE
pause

:EOF
