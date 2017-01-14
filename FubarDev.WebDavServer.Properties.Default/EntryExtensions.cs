﻿using System;

using FubarDev.WebDavServer.FileSystem;

namespace FubarDev.WebDavServer.Properties.Default
{
    public static class EntryExtensions
    {
        public static IProperty GetResourceTypeProperty(this IEntry entry)
        {
            var coll = entry as ICollection;

            if (coll != null)
                return ResourceType.Collection;

            var doc = entry as IDocument;
            if (doc != null)
                return ResourceType.Document;

            throw new NotSupportedException();
        }
    }
}