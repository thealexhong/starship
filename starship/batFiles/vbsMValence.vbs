Set WshShell = CreateObject("WScript.Shell")
'Nolan workstation
'WshShell.Run "%comspec% /K ""C:\Users\Calvin\Box Sync\NL UToronto\NAO Robot\NAO Programs\starship\starship\batFiles\Mvalence.bat""", 0, False
'Alex workstation
WshShell.Run "%comspec% /K ""C:\Users\Alex\Desktop\starship\starship\batFiles\Mvalence.bat""", 0, False
Set WshShell = Nothing