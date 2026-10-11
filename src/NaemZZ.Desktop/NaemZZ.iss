#define MyAppName "Наеми"
#define MyAppVersion "1.0.0"
#define MyAppExeName "NaemZZ.Desktop.exe"
#define MyAppPublisher "NaemZZ"

[Setup]
AppId={{D7C3F490-5A18-4C29-9C6D-EE2B91540762}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

DefaultDirName={autopf}\NaemZZ
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

OutputDir=installer
OutputBaseFilename=NaemZZ_Setup_{#MyAppVersion}

SetupIconFile=Assets\brovis.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

Compression=lzma2
SolidCompression=yes

PrivilegesRequired=admin

WizardStyle=modern
CloseApplications=yes

[Languages]
Name: "bulgarian"; MessagesFile: "compiler:Languages\Bulgarian.isl"

[Tasks]
Name: "desktopicon"; Description: "Създай икона на работния плот"; GroupDescription: "Допълнителни икони:"; Flags: unchecked

[Files]
Source: "bin\Release\net10.0-windows10.0.17763.0\publish\win-x86\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "Assets\brovis.ico"; DestDir: "{app}\Assets"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\Assets\brovis.ico"

Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\Assets\brovis.ico"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Стартирай {#MyAppName}"; Flags: nowait postinstall skipifsilent