﻿// Originally from:
// https://github.com/kerryjiang/SuperSocket.ClientEngine
// Modified to support authorization headers and HTTP 1.0 responses.

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Proxy;

namespace Energistics.Etp.WebSocket4Net
{
    public class HttpConnectProxy : ProxyConnectorBase
    {
        class ConnectContext
        {
            public Socket Socket { get; set; }

            public SearchMarkState<byte> SearchState { get; set; }
        }

        private const string m_RequestTemplate = "CONNECT {0}:{1} HTTP/1.1\r\nHost: {0}:{1}\r\nProxy-Connection: Keep-Alive\r\n{2}\r\n";
        private const string m_ResponsePrefix = "HTTP/";
        private const char m_Space = ' ';

        private static byte[] m_LineSeparator;

        static HttpConnectProxy()
        {
            m_LineSeparator = ASCIIEncoding.GetBytes("\r\n\r\n");
        }

        private readonly int m_ReceiveBufferSize;
        private readonly string m_Authorizaton;

#if SILVERLIGHT && !WINDOWS_PHONE
        public HttpConnectProxy(EndPoint proxyEndPoint, SocketClientAccessPolicyProtocol clientAccessPolicyProtocol)
            : this(proxyEndPoint, clientAccessPolicyProtocol, 128)
        {

        }

        public HttpConnectProxy(EndPoint proxyEndPoint, SocketClientAccessPolicyProtocol clientAccessPolicyProtocol, int receiveBufferSize)
            : base(proxyEndPoint, clientAccessPolicyProtocol)
        {
            m_ReceiveBufferSize = receiveBufferSize;
        }
#else
        public HttpConnectProxy(EndPoint proxyEndPoint, string authorization = null)
            : this(proxyEndPoint, 128, null, authorization)
        {

        }

        public HttpConnectProxy(EndPoint proxyEndPoint, string targetHostName, string authorization = null)
            : this(proxyEndPoint, 128, targetHostName, authorization)
        {

        }

        public HttpConnectProxy(EndPoint proxyEndPoint, int receiveBufferSize, string targetHostName, string authorization = null)
            : base(proxyEndPoint, targetHostName)
        {
            m_ReceiveBufferSize = receiveBufferSize;
            m_Authorizaton = authorization;
        }
#endif

        public override void Connect(EndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
                throw new ArgumentNullException("remoteEndPoint");

            if (!(remoteEndPoint is IPEndPoint || remoteEndPoint is DnsEndPoint))
                throw new ArgumentException("remoteEndPoint must be IPEndPoint or DnsEndPoint", "remoteEndPoint");

            try
            {
#if SILVERLIGHT && !WINDOWS_PHONE
                ProxyEndPoint.ConnectAsync(ClientAccessPolicyProtocol, ProcessConnect, remoteEndPoint);
#elif WINDOWS_PHONE
                ProxyEndPoint.ConnectAsync(ProcessConnect, remoteEndPoint);
#else
                ProxyEndPoint.ConnectAsync(null, ProcessConnect, remoteEndPoint);
#endif
            }
            catch (Exception e)
            {
                OnException(new Exception("Failed to connect proxy server", e));
            }
        }

        protected override void ProcessConnect(Socket socket, object targetEndPoint, SocketAsyncEventArgs e, Exception exception)
        {
            if (exception != null)
            {
                OnException(exception);
                return;
            }

            if (e != null)
            {
                if (!ValidateAsyncResult(e))
                    return;
            }

            if (socket == null)
            {
                OnException(new SocketException((int)SocketError.ConnectionAborted));
                return;
            }

            if (e == null)
                e = new SocketAsyncEventArgs();

            string request;
            string authorizationHeader = null;

            if (!string.IsNullOrWhiteSpace(m_Authorizaton))
            {
                authorizationHeader = $"Proxy-Authorization: {m_Authorizaton}\r\n";
            }

            if (targetEndPoint is DnsEndPoint)
            {
                var targetDnsEndPoint = (DnsEndPoint)targetEndPoint;
                request = string.Format(m_RequestTemplate, targetDnsEndPoint.Host, targetDnsEndPoint.Port, authorizationHeader);
            }
            else
            {
                var targetIPEndPoint = (IPEndPoint)targetEndPoint;
                request = string.Format(m_RequestTemplate, targetIPEndPoint.Address, targetIPEndPoint.Port, authorizationHeader);
            }

            var requestData = ASCIIEncoding.GetBytes(request);

            e.Completed += AsyncEventArgsCompleted;
            e.UserToken = new ConnectContext { Socket = socket, SearchState = new SearchMarkState<byte>(m_LineSeparator) };
            e.SetBuffer(requestData, 0, requestData.Length);

            StartSend(socket, e);
        }

        protected override void ProcessSend(SocketAsyncEventArgs e)
        {
            if (!ValidateAsyncResult(e))
                return;

            var context = (ConnectContext)e.UserToken;

            var buffer = new byte[m_ReceiveBufferSize];
            e.SetBuffer(buffer, 0, buffer.Length);

            StartReceive(context.Socket, e);
        }

        protected override void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (!ValidateAsyncResult(e))
                return;

            var context = (ConnectContext)e.UserToken;

            int prevMatched = context.SearchState.Matched;

            int result = e.Buffer.SearchMark(e.Offset, e.BytesTransferred, context.SearchState);

            if (result < 0)
            {
                int total = e.Offset + e.BytesTransferred;

                if(total >= m_ReceiveBufferSize)
                {
                    OnException("receive buffer size has been exceeded");
                    return;
                }

                e.SetBuffer(total, m_ReceiveBufferSize - total);
                StartReceive(context.Socket, e);
                return;
            }

            int responseLength = (prevMatched > 0 && result == e.Offset) ? (e.Offset - prevMatched) : result;

            if (e.Offset + e.BytesTransferred > responseLength + m_LineSeparator.Length)
            {
                OnException("protocol error: more data has been received");
                return;
            }

            var lineReader = new StringReader(ASCIIEncoding.GetString(e.Buffer, 0, responseLength));

            var line = lineReader.ReadLine();

            if (string.IsNullOrEmpty(line))
            {
                OnException("protocol error: invalid response");
                return;
            }

            //HTTP/1.1 2** OK
            var pos = line.IndexOf(m_Space);

            if (pos <= 0 || line.Length <= (pos + 2))
            {
                OnException("protocol error: invalid response");
                return;
            }

            var httpProtocol = line.Substring(0, pos);

            if (!httpProtocol.StartsWith(m_ResponsePrefix))
            {
                OnException("protocol error: invalid protocol");
                return;
            }

            var statusPos = line.IndexOf(m_Space, pos + 1);

            if (statusPos < 0)
            {
                OnException("protocol error: invalid response");
                return;
            }

            int statusCode;
            //Status code should be 2**
            if (!int.TryParse(line.Substring(pos + 1, statusPos - pos - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out statusCode) || (statusCode > 299 || statusCode < 200))
            {
                OnException("the proxy server refused the connection");
                return;
            }

            OnCompleted(new ProxyEventArgs(context.Socket, TargetHostHame));
        }
    }
}