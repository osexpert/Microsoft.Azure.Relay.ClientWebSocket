// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Relay
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IClientWebSocket : IDisposable
    {
        IClientWebSocketOptions Options { get; }

        HttpResponseMessage Response { get; }

        WebSocket WebSocket { get; }

        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);
    }

    public interface IClientWebSocketOptions
    {
        IWebProxy Proxy { get; set; }

        TimeSpan KeepAliveInterval { get; set; }

        void AddSubProtocol(string subProtocol);

        void SetBuffer(int receiveBufferSize, int sendBufferSize);

        void SetRequestHeader(string name, string value);
    }

    public static class ClientWebSocketFactory
    {
        public static IClientWebSocket Create(bool tryNativeFirst)
        {
//#if NETSTANDARD
            //if (!useBuiltInWebSocket)
            //{
            //    //if (Microsoft.Azure.Relay.WebSockets.NetCore21.ClientWebSocket.IsSupported())
            //    //{
            //    //    return new Microsoft.Azure.Relay.WebSockets.NetCore21.ClientWebSocket();
            //    //}

            //    return new Microsoft.Azure.Relay.WebSockets.NetStandard20.ClientWebSocket();
            //}
//#endif // NETSTANDARD

            if (tryNativeFirst)
			{
				try
				{
                    return new FrameworkClientWebSocket(new System.Net.WebSockets.ClientWebSocket());
                }
                catch (PlatformNotSupportedException)
				{

				}
			}

            return new Microsoft.Azure.Relay.WebSockets.NetStandard20.ClientWebSocket();
        }

        class FrameworkClientWebSocket : IClientWebSocket
        {
            readonly System.Net.WebSockets.ClientWebSocket client;

            public FrameworkClientWebSocket(System.Net.WebSockets.ClientWebSocket client)
            {
                this.client = client;
                this.Options = new FrameworkClientWebSocketOptions(this.client.Options);
            }

            public IClientWebSocketOptions Options { get; }

            public HttpResponseMessage Response { get { return null; } }
            
            public WebSocket WebSocket { get { return this.client; } }

            public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
            {
                return this.client.ConnectAsync(uri, cancellationToken);
            }

			public void Dispose()
			{
                WebSocket?.Dispose();
			}

			class FrameworkClientWebSocketOptions : IClientWebSocketOptions
            {
                readonly System.Net.WebSockets.ClientWebSocketOptions options;
                public FrameworkClientWebSocketOptions(System.Net.WebSockets.ClientWebSocketOptions options)
                {
                    this.options = options;
                }

                public IWebProxy Proxy
                {
                    get { return this.options.Proxy; }
                    set { this.options.Proxy = value; }
                }

                public TimeSpan KeepAliveInterval
                {
                    get { return this.options.KeepAliveInterval; }
                    set { this.options.KeepAliveInterval = value; }
                }

                public void AddSubProtocol(string subProtocol)
                {
                    this.options.AddSubProtocol(subProtocol);
                }
               
                public void SetBuffer(int receiveBufferSize, int sendBufferSize)
                {
                    this.options.SetBuffer(receiveBufferSize, sendBufferSize);
                }

                public void SetRequestHeader(string name, string value)
                {
                    this.options.SetRequestHeader(name, value);
                }
            }
        }
    }
}
