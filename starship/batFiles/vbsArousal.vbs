Set WshShell = CreateObject("WScript.Shell")
'Nolans workstation
'WshShell.Run "%comspec% /K ""C:\Users\Calvin\Box Sync\NL UToronto\NAO Robot\NAO Programs\starship\starship\batFiles\blarousal.bat""", 0, False
'Alex workstation
'WshShell.Run "%comspec% /K ""C:\Users\Alex\Desktop\starship\starship\batFiles\blarousal.bat""", 0, False
'Yuma workstation
WshShell.Run "%comspec% /K ""C:\Users\Yuma\Desktop\starship\starship\batFiles\blarousal.bat""", 0, False
Set WshShell = Nothing