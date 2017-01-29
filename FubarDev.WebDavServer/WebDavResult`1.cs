﻿using System.Threading;
using System.Threading.Tasks;

using FubarDev.WebDavServer.Model;

namespace FubarDev.WebDavServer
{
    public class WebDavResult<T> : WebDavResult
    {
        public WebDavResult(WebDavStatusCode statusCode, T data)
            : base(statusCode)
        {
            Data = data;
        }

        public T Data { get; }

        public override Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            var formatter = response.Dispatcher.Formatter;
            response.ContentType = formatter.ContentType;
            formatter.Serialize(response.Body, Data);
            return Task.FromResult(0);
        }
    }
}
