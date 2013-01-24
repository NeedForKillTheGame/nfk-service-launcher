@echo off

:: set current path
cd /D %~dp0

IF (%1)==(start) goto START
IF (%1)==(stop) goto STOP
IF (%1)==(query) goto QUERY
goto END


:START
%comspec% /c sc start NFK_%2
goto END

:STOP
%comspec% /c sc stop NFK_%2
goto END

:QUERY
%comspec% /c sc query NFK_%2
goto END

:END
