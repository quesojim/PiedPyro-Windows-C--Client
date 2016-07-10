; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{5AF628B7-E062-4500-B51F-EF078E869590}
AppName=PIroTech
AppVersion=1.0
;AppVerName=PIroTech 1.0
AppPublisher=CJX Development
AppPublisherURL=http://www.example.com/
AppSupportURL=http://www.example.com/
AppUpdatesURL=http://www.example.com/
DefaultDirName={pf}\PIroTech
DisableProgramGroupPage=yes
OutputDir=C:\Users\QuesoJim\Desktop\pi
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\IronPython.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\IronPython.SQLite.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\IronPython.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\IronPython.Wpf.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.application"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.vshost.application"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\PIroTech.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\QuesoJim\Google Drive\Pyrotechnics\PIroTech\PIroTech\bin\Release\Renci.SshNet.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{commonprograms}\PIroTech"; Filename: "{app}\PIroTech.exe"
Name: "{commondesktop}\PIroTech"; Filename: "{app}\PIroTech.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\PIroTech.exe"; Description: "{cm:LaunchProgram,PIroTech}"; Flags: nowait postinstall skipifsilent

