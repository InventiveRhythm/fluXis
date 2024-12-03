<h1 align="center">fluXis</h1>
<p align="center"><img src="https://github.com/InventiveRhythm/fluXis-web/blob/master/src/assets/images/icon.png?raw=true" width="128" alt="fluXis logo"/></p>
<p align="center">A community-driven rhythm game with a focus on creativity and expression.</p>

<p align="center">
<a href="https://github.com/InventiveRhythm/fluXis/releases"><img src="https://img.shields.io/github/v/release/InventiveRhythm/fluXis" alt="github release badge"></a>
<a href="https://github.com/InventiveRhythm/fluXis/blob/main/LICENSE.md"><img src="https://img.shields.io/github/license/InventiveRhythm/fluXis" alt="license badge"></a>
</p>

## Downloading and playing the game
### Windows 8.1+
1. Download and install the .NET 8.0 desktop runtime from [here](https://dotnet.microsoft.com/download/dotnet/8.0/runtime).
2. Download `install.exe` from [the latest release](https://github.com/InventiveRhythm/fluXis/releases/latest) or the download button on the website.
3. Run the installer.

*Make sure to report any issues with the installer to the [discord server](https://discord.gg/29hMftpNq9).*

### Linux
1. Download and install the .NET 8.0 desktop runtime.
2. Download `fluXis.AppImage` from [the latest release](https://github.com/InventiveRhythm/fluXis/releases/latest).
3. Run the AppImage.

### macOS
macOS releases soon™

## Building and Developing
### Requirements
* A desktop computer running Windows, macOS, or Linux with the [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.
* An IDE of your choice, for example [JetBrains Rider](https://www.jetbrains.com/rider/), [Visual Studio](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/).

### Downloading the source code
You can download the source code by cloning the repository using git:
```shell
git clone https://github.com/InventiveRhythm/fluXis
cd fluXis
```

To update the source code to the latest version, run the following command in the repository directory:
```shell
git pull
```

### Building
To build and run the game, run the following command in the repository directory:
```shell
dotnet run --project fluXis.Desktop
```

## License
fluXis is licensed under the [MIT License](LICENSE). tl;dr: You can do whatever you want with the code, as long as you include the original license.
