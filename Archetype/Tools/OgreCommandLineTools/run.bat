@echo off
for /r A:\Desktop\output\ %%i in (*.xml) do OgreXMLConverter.exe %%i
pause