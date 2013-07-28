@echo off
for /r C:\Users\User\Desktop\out\ %%i in (*.xml) do OgreXMLConverter.exe %%i
pause