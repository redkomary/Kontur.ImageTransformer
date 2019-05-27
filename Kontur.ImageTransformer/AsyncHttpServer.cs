using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer
{
    internal class AsyncHttpServer : IDisposable
    {
        public AsyncHttpServer()
        {
            listener = new HttpListener();
        }
        
        public void Start(string prefix)
        {
            lock (listener)
            {
                if (!isRunning)
                {
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(prefix);
                    listener.Start();

                    listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };
                    listenerThread.Start();
                    
                    isRunning = true;
                }
            }
        }

        public void Stop()
        {
            lock (listener)
            {
                if (!isRunning)
                    return;

                listener.Stop();

                listenerThread.Abort();
                listenerThread.Join();
                
                isRunning = false;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            Stop();

            listener.Close();
        }
        
        private void Listen()
        {
            while (true)
            {
                try
                {
                    if (listener.IsListening)
                    {
                        var context = listener.GetContext();
                        Task.Run(() => HandleContextAsync(context));
                    }
                    else Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception error)
                {
                    // TODO: log errors
                }
            }
        }

		private async Task HandleContextAsync(HttpListenerContext listenerContext)
		{
			// TODO: implement request handling
			var request = listenerContext.Request;
			if (request.HttpMethod != HttpMethod.Post.Method)
			{
				BadResponse(listenerContext.Response);
				return;
			}

			string[] @urlParams = listenerContext.Request.Url
				.Segments
				.Skip(1)
				.Select(s => s.Replace("/", ""))
				.ToArray();

			if (@urlParams.Length < 3)
			{
				BadResponse(listenerContext.Response);
				return;
			}

			if (@urlParams[0] != "process")
			{
				BadResponse(listenerContext.Response);
				return;
			}

			var transformMethod = TryParseTransformMethod(@urlParams[1]);
			if (transformMethod == null)
			{
				BadResponse(listenerContext.Response);
				return;
			}

			int[] coordArgs = TryParseCoords(@urlParams[2]);
			if (coordArgs == null)
			{
				BadResponse(listenerContext.Response);
				return;
			}

			GoodResponse(listenerContext.Response);
		}

		private TransformMethods? TryParseTransformMethod(string transformValue)
		{
			switch (transformValue)
			{
				case "rotate-cw":
					return TransformMethods.RotateCw;
				case "rotate-ccw":
					return TransformMethods.RotateCcw;
				case "flip-v":
					return TransformMethods.FlipV;
				case "flip-h":
					return TransformMethods.FlipH;
				default:
					return null;
			}
		}

		private int[] TryParseCoords(string coordsValue)
		{
			string[] args = coordsValue.Split(',').ToArray();
			if (args.Length != 4)
				return null;

			var result = new int[4];
			for (int i = 0; i < args.Length; i++)
			{
				bool parsed = int.TryParse(args[i], out int value);
				if (!parsed)
					return null;

				result[i] = value;
			}

			return result;
		}

		private void GoodResponse(HttpListenerResponse response)
		{
			response.StatusCode = (int)HttpStatusCode.OK;
			using (var writer = new StreamWriter(response.OutputStream))
				writer.WriteLine("Hello, world!");
		}

		private void BadResponse(HttpListenerResponse response)
		{
			response.StatusCode = (int)HttpStatusCode.BadRequest;
		}

		private void EmptyResponse(HttpListenerResponse response)
		{
			response.StatusCode = (int)HttpStatusCode.NoContent;
		}

		private readonly HttpListener listener;

		private Thread listenerThread;
		private bool disposed;
		private volatile bool isRunning;

		private enum TransformMethods
		{
			RotateCw,
			RotateCcw,
			FlipV,
			FlipH
		}
	}
}
