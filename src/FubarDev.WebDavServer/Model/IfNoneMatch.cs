﻿// <copyright file="IfNoneMatch.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using FubarDev.WebDavServer.FileSystem;

using JetBrains.Annotations;

namespace FubarDev.WebDavServer.Model
{
    public class IfNoneMatch : IIfMatcher
    {
        [CanBeNull]
        private readonly ISet<EntityTag> _etags;

        private IfNoneMatch([NotNull] IEnumerable<EntityTag> etags)
        {
            _etags = new HashSet<EntityTag>(etags, EntityTagComparer.Default);
        }

        private IfNoneMatch()
        {
            _etags = null;
        }

        [NotNull]
        public static IfNoneMatch Parse([CanBeNull] string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s == "*")
                return new IfNoneMatch();

            return new IfNoneMatch(EntityTag.Parse(s));
        }

        [NotNull]
        public static IfNoneMatch Parse([NotNull][ItemNotNull] IEnumerable<string> s)
        {
            var result = new List<EntityTag>();
            foreach (var etag in s)
            {
                if (etag == "*")
                    return new IfNoneMatch();

                result.AddRange(EntityTag.Parse(etag));
            }

            if (result.Count == 0)
                return new IfNoneMatch();

            return new IfNoneMatch(result);
        }

        public bool IsMatch(IEntry entry, EntityTag etag, IReadOnlyCollection<Uri> stateTokens)
        {
            if (_etags == null)
                return false;
            return !_etags.Contains(etag);
        }
    }
}