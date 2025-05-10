# fluXis lua scripting

These are files used for line-completion and type hints in fluXis scripts.
They are intended to be used with sumneko's Lua Language Server.

## Extensions / Plugins

- VSCode: [Lua](https://marketplace.visualstudio.com/items/?itemName=sumneko.lua)
- Rider: [SumnekoLua](https://plugins.jetbrains.com/plugin/22315-sumnekolua)

## Setup

### VSCode

In your project folder, edit the file at `.vscode/settings.json` (create it if it doesn't exist) to contain the following:

```json
{
  "Lua.workspace.library": ["<your-path-to-fluXis>/scripting"]
}
```

So if for example your fluXis is installed at `C:\Program Files (x86)\Steam\steamapps\common\fluXis`, the path would be `C:\Program Files (x86)\Steam\steamapps\common\fluXis\scripting`.
