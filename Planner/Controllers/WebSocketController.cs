using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Planner.WebSocketManager;

namespace Planner.Controllers;

public class WebSocketController : ControllerBase
{
    private readonly ILogger<WebSocketController> _logger;
    private readonly ConnectionManager _connManager;

    public WebSocketController(ILogger<WebSocketController> logger, ConnectionManager connManager)
    {
        _logger = logger;
        _connManager = connManager;
    }

    [HttpGet("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _connManager.AddSocket(webSocket);
            _logger.LogInformation("Accept socket");
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        _logger.Log(LogLevel.Information, "Message received from Client");

        while (!result.CloseStatus.HasValue)
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            _logger.Log(LogLevel.Information, "Message received from Client");

        }

        var socketId = _connManager.GetId(webSocket);
        await _connManager.RemoveSocket(socketId);
        _logger.Log(LogLevel.Information, "WebSocket connection closed");
    }
}