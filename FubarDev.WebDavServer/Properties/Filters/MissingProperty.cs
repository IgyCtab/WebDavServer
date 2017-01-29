﻿using System.Xml.Linq;

using FubarDev.WebDavServer.Model;

namespace FubarDev.WebDavServer.Properties.Filters
{
    public class MissingProperty
    {
        public MissingProperty(WebDavStatusCode statusCode, XName name)
        {
            StatusCode = statusCode;
            PropertyName = name;
        }

        public WebDavStatusCode StatusCode { get; }

        public XName PropertyName { get; }
    }
}
