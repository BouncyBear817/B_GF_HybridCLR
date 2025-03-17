@echo off
cd %~dp0
set ROOT_PATH=%~dp0
set WORKSPACE=..
echo =================start gen proto code=================
set pb_path=pb_message
set out_path=scripts
set protoc_path=bin\protoc.exe
del /f /s /q %out_path%\*.*
for /f "delims=" %%i in ('dir /b %pb_path%') do (
echo ------------%%i start gen
%protoc_path% -I=pb_message --csharp_out=%out_path% pb_message\%%i
echo ------------%%i gen success
)
echo =================end gen proto code=================
set destination_folder=%WORKSPACE%\..\Unity\Assets\GameMain\Scripts\Network\Proto
xcopy "%out_path%\*.*" "%destination_folder%\" /E /I /C /Y
pause

