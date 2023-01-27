# fluXis
A free and open-source vertical scrolling rhythm game.

## Building and Developing
### Requirements
* A desktop computer running Windows, macOS, or Linux with the [.NET 6.0 SDK](https://dotnet.microsoft.com/download) installed.
* An IDE of your choice, for example [JetBrains Rider](https://www.jetbrains.com/rider/), [Visual Studio](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/).

### Downloading the source code
You can download the source code by cloning the repository using git:
```
git clone https://github.com/TeamFluXis/fluXis
cd fluXis
```

To update the source code to the latest version, run the following command in the repository directory:
```
git pull
```

### Building
To build and run the game, run the following command in the repository directory:
```
dotnet run --project fluXis.Desktop
```

If you don't want to Debug the game, you can add the `-c Release` flag to the command above.

## License
fluXis is licensed under the [MIT License](LICENSE). tl;dr: You can do whatever you want with the code, as long as you include the original license.