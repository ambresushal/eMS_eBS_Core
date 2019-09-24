@echo off
net stop EquinoxService_CBC_5998
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil /u bin\Debug\\EquinoxProcessor.exe
