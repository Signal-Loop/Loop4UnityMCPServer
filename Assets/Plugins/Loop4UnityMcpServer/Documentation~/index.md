# Loop MCP Server for Unity

A Model Context Protocol (MCP) server implementation for Unity Editor that enables AI assistants to interact with Unity directly.

## Architecture

```
┌─────────────┐     STDIO      ┌─────────────────┐      TCP      ┌─────────────────┐
│  MCP Client │ ◄────────────► │  STDIO Bridge   │ ◄───────────► │  Unity Server   │
│             │                │    (Python)     │               │  (This Package) │
└─────────────┘                └─────────────────┘               └─────────────────┘
```

## Features

- **Auto-start**: Server starts automatically with Unity Editor
- **Domain reload safe**: Handles Unity domain reloads gracefully
- **Extensible**: Easy to add new tools, prompts, and resources
- **Async support**: Supports both sync and async tools via UniTask
- **Testable**: Includes unit tests for all components

## Installation

https://github.com/Signal-Loop/Loop4UnityMCPServer.git?path=Assets/Plugins/Loop4UnityMcpServer

## MCP Configuration

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

## Configuration

1. Create settings asset: **Assets > Create > LoopMcpServer > Server Settings**
2. Place in `Resources/` folder for auto-loading
3. Configure port (default: 21088) and other settings


## Components

### Protocol (`Protocol/`)
- `McpProtocol.cs` - MCP protocol constants and version
- `McpMessages.cs` - JSON-RPC message types

### Interfaces (`Interfaces/`)
- `ITool` - Interface for synchronous tools
- `IToolAsync` - Interface for asynchronous tools (using UniTask)
- `IPrompt` - Interface for prompt templates
- `IResource` - Interface for resources

### Registry (`Registry/`)
- `McpRegistry.cs` - Auto-discovers and registers tools/prompts/resources via reflection

### Handlers (`Handlers/`)
- `McpMessageHandler.cs` - Handles JSON-RPC message dispatching

### Server (`Server/`)
- `LoopMcpTcpServer.cs` - TCP server with auto-start and domain reload handling

### Settings (`Settings/`)
- `LoopMcpServerSettings.cs` - ScriptableObject for server configuration

### Sample Tools (`Samples~/`)
- Example implementations demonstrating how to create tools, prompts, and resources

## Creating Custom Tools

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

- **LoopMcpServer > Refresh Registry** - Re-scan for new tools/prompts/resources
- **LoopMcpServer > Restart Server** - Restart the TCP server

## STDIO Bridge

See `stdio.md` for setting up the Python STDIO bridge for MCP clients.

## Testing

Unity tests are in `Tests/`:
- `McpMessageHandlerTests.cs` - Tests for message handling
- `McpRegistryTests.cs` - Tests for registry and tool execution

Run via Unity Test Runner.

## License

MIT
