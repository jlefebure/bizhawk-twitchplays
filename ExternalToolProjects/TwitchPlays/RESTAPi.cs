using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class RESTApi
{
    private readonly HttpListener _listener = new HttpListener();
    private readonly Dictionary<string, Func<HttpListenerRequest, Task<string>>> _routes =
        new Dictionary<string, Func<HttpListenerRequest, Task<string>>>();

    private bool _isRunning = true;

    public RESTApi(string url)
    {
        _listener.Prefixes.Add(url);
    }

    // Permet d'enregistrer des endpoints dynamiquement
    public void RegisterEndpoint(string path, Func<HttpListenerRequest, Task<string>> handler)
    {
        _routes[path] = handler;
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("Server started on " + string.Join(", ", _listener.Prefixes));

        while (_isRunning)
        {
            var context = await _listener.GetContextAsync();
            _ = Task.Run(() => HandleRequest(context)); // Gérer la requête en asynchrone
        }
    }

    private async void HandleRequest(HttpListenerContext context)
    {
        string path = context.Request.Url.AbsolutePath;
        Console.WriteLine($"Incoming request: {path}");

        if (_routes.TryGetValue(path, out var handler))
        {
            string responseText = await handler(context.Request);
            await SendResponse(context, responseText);
        }
        else
        {
            await SendResponse(context, "{\"error\": \"Endpoint not found\"}", 404);
        }
    }

    private async Task SendResponse(HttpListenerContext context, string responseText, int statusCode = 200)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(responseText);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
    }
}
