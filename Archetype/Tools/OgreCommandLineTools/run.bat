@echo off
for /r C:\Users\User\Desktop\FPS\ %%i in (*.xml) do OgreXMLConverter.exe %%i
pause