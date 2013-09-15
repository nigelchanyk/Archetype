@echo off
for /r A:\Desktop\FPS %%i in (*.xml) do OgreXMLConverter.exe %%i
pause