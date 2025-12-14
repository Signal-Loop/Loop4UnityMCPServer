# Loop MCP Server for Unity

A Model Context Protocol (MCP) server implementation for Unity Editor that enables AI assistants to perform **almost** anything in Unity Editor using c# scripts and Unity Engine/Unity Editor scripting API. That includes, but is not limited to, scene manipulation, asset management, configuration, and more.

## Features

- **Access to Unity Editor/Unity Engine API**: Perform any task available through public API or reflection
- **Auto-start**: Server starts automatically with Unity Editor
- **STDIO transport**: No need to start separate server processes
- **Domain reload safe**: Handles Unity domain reloads gracefully
- **Extensible**: Add new tools, async tools, resources or prompts by implementing interfaces anywhere in the codebase

## Security considerations


## Available assemblies

## Architecture

```
┌─────────────────┐      TCP      ┌─────────────────┐     STDIO      ┌─────────────┐
│  Unity Server   │ ◄───────────► │  STDIO Bridge   │ ◄────────────► │  MCP Client │
│  (This Package) │               │ (Python script) │                │             │
└─────────────────┘               └─────────────────┘                └─────────────┘
```

## Requirements

- Tested with Unity 2022.3 LTS
- UniTask:  Efficient allocation free async/await integration for Unity. Refer to [UniTask UPM Package](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package) for installation instructions
- UV: Python package and project manager, allows to run python scripts out of the box, without manual installation. Refer to [uv installation](https://github.com/astral-sh/uv?tab=readme-ov-file#installation) for instructions.

## Installation

Install via git URL:
https://github.com/Signal-Loop/Loop4UnityMCPServer.git?path=Assets/Plugins/Loop4UnityMcpServer

## MCP Configuration

Run menu item: **Tools/LoopMcpServer/Log MCP Configuration** to log configuration for your project to console.

### MCP Client Configuration
```json
{
  "mcpServers": {
    "unity": {
      "command": "uv",
      "args": [
        "run",
        "--directory",
        "C:/Users/YOUR_USERNAME/path/to/Assets/Plugins/Loop4UnityMcpServer/Editor/STDIO~",
        "loop-mcp-stdio",
        "--host",
        "localhost",
        "--port",
        "21088"
      ]
    }
  }
}
```

> **Note:** Replace `C:/Users/YOUR_USERNAME/path/to/...` with the actual path to your Unity project's STDIO folder.

## Server Configuration

1. Navigate to `Resources/` folder
2. Create settings asset: **Right Click > Create > LoopMcpServer > Server Settings**
3. Configure port (default: 21088) and other options as needed

## Adding Tools

### Synchronous Tool

```csharp
using LoopMcpServer.Interfaces;
using LoopMcpServer.Protocol;
using Newtonsoft.Json.Linq;

public class MyTool : ITool
{
    public string Name => "my_tool";
    public string Description => "Description of my tool";
    public JObject InputSchema => JObject.Parse(@"{
        ""type"": ""object"",
        ""properties"": {
            ""param1"": { ""type"": ""string"" }
        },
        ""required"": [""param1""]
    }");

    public ToolsCallResult Execute(JObject arguments)
    {
        var param1 = arguments["param1"]?.ToString();
        return new ToolsCallResult
        {
            IsError = false,
            Content = new List<ContentItem> { ContentItem.TextContent($"Result: {param1}") }
        };
    }
}
```

### Asynchronous Tool

```csharp
using Cysharp.Threading.Tasks;
using LoopMcpServer.Interfaces;

public class MyAsyncTool : IToolAsync
{
    public string Name => "my_async_tool";
    public string Description => "An async tool";
    public JObject InputSchema => new JObject { ["type"] = "object" };

    public async UniTask<ToolsCallResult> ExecuteAsync(JObject arguments)
    {
        await UniTask.Delay(1000);
        return new ToolsCallResult { /* ... */ };
    }
}
```

## Menu Commands

- **Tool > LoopMcpServer > Refresh Registry** - Re-scan for new tools/prompts/resources
- **Tool > LoopMcpServer > Restart Server** - Restart the TCP server
- **Tool > LoopMcpServer > Log MCP Configuration** - Log MCP client configuration to console

## STDIO Bridge

See [stdio.md](stdio.md) for setting up the Python STDIO bridge for MCP clients.

## Testing

Unity tests are in `Tests/`:
- `McpMessageHandlerTests.cs` - Tests for message handling
- `McpRegistryTests.cs` - Tests for registry and tool execution

Run via Unity Test Runner.

## License

MIT
