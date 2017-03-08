﻿// <copyright file="DeadPropertyFactory.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;
using System.Xml.Linq;

using FubarDev.WebDavServer.FileSystem;
using FubarDev.WebDavServer.Props.Store;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace FubarDev.WebDavServer.Props.Dead
{
    /// <summary>
    /// A factory for the creation of dead properties
    /// </summary>
    /// <remarks>
    /// Some dead properties are special (like the <see cref="GetETagProperty"/>), because they have custom implementations.
    /// </remarks>
    public class DeadPropertyFactory : IDeadPropertyFactory
    {
        [NotNull]
        private readonly Lazy<IWebDavDispatcher> _webDavDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadPropertyFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to query the <see cref="Dispatchers.IWebDavClass"/> implementations</param>
        public DeadPropertyFactory([NotNull] IServiceProvider serviceProvider)
        {
            _webDavDispatcher = new Lazy<IWebDavDispatcher>(serviceProvider.GetRequiredService<IWebDavDispatcher>);
        }

        /// <inheritdoc />
        public virtual IDeadProperty Create(IPropertyStore store, IEntry entry, XName name, string language)
        {
            foreach (var webDavClass in _webDavDispatcher.Value.SupportedClasses)
            {
                IDeadProperty deadProp;
                if (webDavClass.TryCreateDeadProperty(store, entry, name, language, out deadProp))
                    return deadProp;
            }

            return new DeadProperty(store, entry, name, language);
        }

        /// <inheritdoc />
        public IDeadProperty Create(IPropertyStore store, IEntry entry, XElement element)
        {
            var result = Create(store, entry, element.Name, PropertyKey.NoLanguage);
            result.Init(element);
            return result;
        }
    }
}
