using System;
using System.IO;
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

	        bool parsed = ImageTransformRequest.TryParse(request.Url, out ImageTransformRequest requestParams);
	        if (!parsed)
	        {
		        BadResponse(listenerContext.Response);
		        return;
			}

			GoodResponse(listenerContext.Response);
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
	}
}
