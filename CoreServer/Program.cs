using System;
using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace CoreServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initing Server . . .");

            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            var trd = new Thread(StartServer);
            trd.Start();
            
            while (true)
            {
                var l = Console.ReadLine();
                if(l == "exit")
                    break;
            }
        }

        private static async void StartServer()
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://127.0.0.1:5000/");
            httpListener.Start();

            HttpListenerContext context = await httpListener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                WebSocket webSocket = webSocketContext.WebSocket;
                while (webSocket.State == WebSocketState.Open)
                {
                    Console.WriteLine("Open");
                    //await webSocket.SendAsync();
                }
            }
            
        }
    }
}