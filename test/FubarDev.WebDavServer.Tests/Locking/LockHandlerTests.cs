﻿// <copyright file="LockHandlerTests.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using DecaTec.WebDav;
using DecaTec.WebDav.WebDavArtifacts;

using Xunit;

namespace FubarDev.WebDavServer.Tests.Locking
{
    public class LockHandlerTests : ServerTestsBase
    {
        [Fact]
        public async Task AddLockToRootRecursiveMinimumTest()
        {
            var response = await Client.LockAsync(
                "/",
                WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                WebDavDepthHeaderValue.Infinity,
                new LockInfo()
                {
                    LockScope = LockScope.CreateExclusiveLockScope(),
                    LockType = LockType.CreateWriteLockType(),
                }).ConfigureAwait(false);
            var prop = await response.EnsureSuccessStatusCode()
                .Content.ParsePropResponseContentAsync().ConfigureAwait(false);
            Assert.NotNull(prop.LockDiscovery);
            Assert.Collection(
                prop.LockDiscovery.ActiveLock,
                activeLock =>
                {
                    Assert.Equal("/", activeLock.LockRoot.Href);
                    Assert.Equal(WebDavDepthHeaderValue.Infinity.ToString(), activeLock.Depth, StringComparer.OrdinalIgnoreCase);
                    Assert.IsType<Exclusive>(activeLock.LockScope.Item);
                    Assert.Null(activeLock.Owner);
                    Assert.Equal(WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout().ToString(), activeLock.Timeout, StringComparer.OrdinalIgnoreCase);
                    Assert.NotNull(activeLock.LockToken?.Href);
                    Assert.True(Uri.IsWellFormedUriString(activeLock.LockToken.Href, UriKind.RelativeOrAbsolute));
                });
        }

        [Fact]
        public async Task AddLockToRootRecursiveWithTimeoutTest()
        {
            var response = await Client.LockAsync(
                "/",
                WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(1)),
                WebDavDepthHeaderValue.Infinity,
                new LockInfo()
                {
                    LockScope = LockScope.CreateExclusiveLockScope(),
                    LockType = LockType.CreateWriteLockType(),
                }).ConfigureAwait(false);
            var prop = await response.EnsureSuccessStatusCode()
                .Content.ParsePropResponseContentAsync().ConfigureAwait(false);
            Assert.Collection(
                prop.LockDiscovery.ActiveLock,
                activeLock =>
                {
                    Assert.Equal("/", activeLock.LockRoot.Href);
                    Assert.Equal(WebDavDepthHeaderValue.Infinity.ToString(), activeLock.Depth, StringComparer.OrdinalIgnoreCase);
                    Assert.IsType<Exclusive>(activeLock.LockScope.Item);
                    Assert.Null(activeLock.Owner);
                    Assert.Equal(WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(1)).ToString(), activeLock.Timeout, StringComparer.OrdinalIgnoreCase);
                    Assert.NotNull(activeLock.LockToken?.Href);
                    Assert.True(Uri.IsWellFormedUriString(activeLock.LockToken.Href, UriKind.RelativeOrAbsolute));
                });
        }

        [Fact]
        public async Task AddLockToRootRecursiveWithPrincipalOwnerTest()
        {
            var response = await Client.LockAsync(
                    "/",
                    WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                    WebDavDepthHeaderValue.Infinity,
                    new LockInfo()
                    {
                        LockScope = LockScope.CreateExclusiveLockScope(),
                        LockType = LockType.CreateWriteLockType(),
                        Owner = new XElement("{DAV:}owner", "principal"),
                    })
                .ConfigureAwait(false);
            var prop = await response.EnsureSuccessStatusCode()
                .Content.ParsePropResponseContentAsync().ConfigureAwait(false);
            Assert.Collection(
                prop.LockDiscovery.ActiveLock,
                activeLock =>
                {
                    Assert.Equal("/", activeLock.LockRoot.Href);
                    Assert.Equal(WebDavDepthHeaderValue.Infinity.ToString(), activeLock.Depth, StringComparer.OrdinalIgnoreCase);
                    Assert.IsType<Exclusive>(activeLock.LockScope.Item);
                    Assert.Equal("<owner xmlns=\"DAV:\">principal</owner>", activeLock.Owner.ToString(SaveOptions.DisableFormatting));
                    Assert.Equal(WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout().ToString(), activeLock.Timeout, StringComparer.OrdinalIgnoreCase);
                    Assert.NotNull(activeLock.LockToken?.Href);
                    Assert.True(Uri.IsWellFormedUriString(activeLock.LockToken.Href, UriKind.RelativeOrAbsolute));
                });
        }

        [Fact]
        public async Task AddLockToRootRecursiveWithUriOwnerTest()
        {
            var response = await Client.LockAsync(
                    "/",
                    WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                    WebDavDepthHeaderValue.Infinity,
                    new LockInfo()
                    {
                        LockScope = LockScope.CreateExclusiveLockScope(),
                        LockType = LockType.CreateWriteLockType(),
                        Owner = new XElement("{DAV:}owner", new XElement("{DAV:}href", "http://localhost/uri-owner")),
                    })
                .ConfigureAwait(false);
            var prop = await response.EnsureSuccessStatusCode()
                .Content.ParsePropResponseContentAsync().ConfigureAwait(false);
            Assert.Collection(
                prop.LockDiscovery.ActiveLock,
                activeLock =>
                {
                    Assert.Equal("/", activeLock.LockRoot.Href);
                    Assert.Equal(WebDavDepthHeaderValue.Infinity.ToString(), activeLock.Depth, StringComparer.OrdinalIgnoreCase);
                    Assert.IsType<Exclusive>(activeLock.LockScope.Item);
                    Assert.Equal("<owner xmlns=\"DAV:\"><href>http://localhost/uri-owner</href></owner>", activeLock.Owner.ToString(SaveOptions.DisableFormatting));
                    Assert.Equal(WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout().ToString(), activeLock.Timeout, StringComparer.OrdinalIgnoreCase);
                    Assert.NotNull(activeLock.LockToken?.Href);
                    Assert.True(Uri.IsWellFormedUriString(activeLock.LockToken.Href, UriKind.RelativeOrAbsolute));
                });
        }

        [Fact]
        public async Task AddLockToRootRecursiveWithAttributeOwnerTest()
        {
            var response = await Client.LockAsync(
                    "/",
                    WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                    WebDavDepthHeaderValue.Infinity,
                    new LockInfo()
                    {
                        LockScope = LockScope.CreateExclusiveLockScope(),
                        LockType = LockType.CreateWriteLockType(),
                        Owner = new XElement("{DAV:}owner", new XAttribute("attr", "attr-value")),
                    })
                .ConfigureAwait(false);
            var prop = await response.EnsureSuccessStatusCode()
                .Content.ParsePropResponseContentAsync().ConfigureAwait(false);
            Assert.NotNull(prop?.LockDiscovery?.ActiveLock);
            Assert.Collection(
                prop.LockDiscovery.ActiveLock,
                activeLock =>
                {
                    Assert.Equal("/", activeLock.LockRoot.Href);
                    Assert.Equal(WebDavDepthHeaderValue.Infinity.ToString(), activeLock.Depth, StringComparer.OrdinalIgnoreCase);
                    Assert.IsType<Exclusive>(activeLock.LockScope.Item);
                    Assert.Equal("<owner attr=\"attr-value\" xmlns=\"DAV:\" />", activeLock.Owner.ToString(SaveOptions.DisableFormatting));
                    Assert.Equal(WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout().ToString(), activeLock.Timeout, StringComparer.OrdinalIgnoreCase);
                    Assert.NotNull(activeLock.LockToken?.Href);
                    Assert.True(Uri.IsWellFormedUriString(activeLock.LockToken.Href, UriKind.RelativeOrAbsolute));
                });
        }

        [Fact]
        public async Task AddLockToRootAndQueryLockDiscoveryTest()
        {
            var lockResponse = await Client.LockAsync(
                    "/",
                    WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                    WebDavDepthHeaderValue.Infinity,
                    new LockInfo()
                    {
                        LockScope = LockScope.CreateExclusiveLockScope(),
                        LockType = LockType.CreateWriteLockType(),
                    })
                .ConfigureAwait(false);
            lockResponse.EnsureSuccessStatusCode();
            var propFindResponse = await Client.PropFindAsync(
                "/",
                WebDavDepthHeaderValue.Zero,
                PropFind.CreatePropFindWithEmptyProperties("lockdiscovery")).ConfigureAwait(false);
            Assert.Equal(WebDavStatusCode.MultiStatus, propFindResponse.StatusCode);
            var multiStatus = await propFindResponse.Content.ParseMultistatusResponseContentAsync().ConfigureAwait(false);
            Assert.Collection(
                multiStatus.Response,
                response =>
                {
                    Assert.Equal("/", response.Href);
                    Assert.Collection(
                        response.ItemsElementName,
                        n => Assert.Equal(ItemsChoiceType.propstat, n));
                    Assert.Collection(
                        response.Items,
                        item =>
                        {
                            var propStat = Assert.IsType<Propstat>(item);
                            var status = Model.Status.Parse(propStat.Status);
                            Assert.Equal(200, status.StatusCode);
                            Assert.NotNull(propStat.Prop?.LockDiscovery?.ActiveLock);
                            Assert.Collection(
                                propStat.Prop.LockDiscovery.ActiveLock,
                                activeLock =>
                                {
                                    Assert.Equal("/", activeLock.LockRoot.Href);
                                    Assert.Equal(WebDavDepthHeaderValue.Infinity.ToString(), activeLock.Depth, StringComparer.OrdinalIgnoreCase);
                                    Assert.IsType<Exclusive>(activeLock.LockScope.Item);
                                    Assert.Null(activeLock.Owner);
                                    Assert.Equal(WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout().ToString(), activeLock.Timeout, StringComparer.OrdinalIgnoreCase);
                                    Assert.NotNull(activeLock.LockToken?.Href);
                                    Assert.True(Uri.IsWellFormedUriString(activeLock.LockToken.Href, UriKind.RelativeOrAbsolute));
                                });
                        });
                });
        }

        [Fact]
        public async Task AddLockToRootAndTryRefreshWrongLockTest()
        {
            var lockResponse = await Client.LockAsync(
                    "/",
                    WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                    WebDavDepthHeaderValue.Infinity,
                    new LockInfo()
                    {
                        LockScope = LockScope.CreateExclusiveLockScope(),
                        LockType = LockType.CreateWriteLockType(),
                    })
                .ConfigureAwait(false);
            lockResponse.EnsureSuccessStatusCode();
            var refreshResult = await Client.RefreshLockAsync(
                "/",
                WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                new LockToken("<asasdasd>")).ConfigureAwait(false);
            Assert.Equal(WebDavStatusCode.PreconditionFailed, refreshResult.StatusCode);
        }

        [Fact]
        public async Task AddLockToRootAndTryRefreshTest()
        {
            var lockResponse = await Client.LockAsync(
                    "/",
                    WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                    WebDavDepthHeaderValue.Infinity,
                    new LockInfo()
                    {
                        LockScope = LockScope.CreateExclusiveLockScope(),
                        LockType = LockType.CreateWriteLockType(),
                    })
                .ConfigureAwait(false);
            lockResponse.EnsureSuccessStatusCode();
            var prop = await lockResponse.Content.ParsePropResponseContentAsync().ConfigureAwait(false);
            var lockToken = prop.LockDiscovery.ActiveLock.Single().LockToken.Href;
            var refreshResult = await Client.RefreshLockAsync(
                "/",
                WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(),
                new LockToken($"<{lockToken}>")).ConfigureAwait(false);
            refreshResult.EnsureSuccessStatusCode();
        }
    }
}
