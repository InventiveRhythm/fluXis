<h1 align="center">fluXis</h1>
<p align="center"><img src="https://fluXis.flux.moe/src/assets/images/icon.png" width="128"/></p>
<p align="center">A community-driven rhythm game with a focus on creativity and expression.</p>

<p align="center">
<a href="https://github.com/TeamFluXis/fluXis/releases"><img src="https://img.shields.io/github/v/release/TeamFluXis/fluXis"></a>
<a href="https://crowdin.com/project/fluxis"><img src="https://badges.crowdin.net/fluxis/localized.svg"/></a>
<a href="https://github.com/TeamFluXis/fluXis/blob/main/LICENSE.md"><img src="https://img.shields.io/github/license/TeamFluXis/fluXis"></a>

</p>

## Downloading and playing the game
### Windows 8.1+
1. Download the installer from [here](https://dl.flux.moe/fluXis/updater.zip) or the download button on the website.
2. Extract the zip file.
3. Run the installer (`fluXis.Updater.exe`).
4. Wait for the installer to finish.
    - This also creates a shortcut in the start menu.
5. The game should now be automatically launched.

*Make sure to report any issues with the installer to the [discord server](https://discord.gg/29hMftpNq9).*

### Linux
Linux releases soon™

### macOS
macOS releases soon™

## Building and Developing
### Requirements
* A desktop computer running Windows, macOS, or Linux with the [.NET 7.0 SDK](https://dotnet.microsoft.com/download) installed.
* An IDE of your choice, for example [JetBrains Rider](https://www.jetbrains.com/rider/), [Visual Studio](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/).

### Downloading the source code
You can download the source code by cloning the repository using git:
```shell
git clone https://github.com/TeamFluXis/fluXis
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
