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
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _connManager.AddSocket(webSocket);
            _logger.LogInformation("Accept socket");
           
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}