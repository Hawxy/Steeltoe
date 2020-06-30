﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Common.Contexts;
using Steeltoe.Messaging;
using Steeltoe.Messaging.Support;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Steeltoe.Integration.Channel.Test.DirectChannelTest;

namespace Steeltoe.Integration.Channel.Test
{
    public class DirectChannelWriterTest
    {
        private ServiceCollection services;

        public DirectChannelWriterTest()
        {
            services = new ServiceCollection();
            services.AddSingleton<IIntegrationServices, IntegrationServices>();
            var config = new ConfigurationBuilder().Build();
            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IApplicationContext, GenericApplicationContext>();
        }

        [Fact]
        public async Task TestWriteAsync()
        {
            var provider = services.BuildServiceProvider();
            var target = new ThreadNameExtractingTestTarget();
            var channel = new DirectChannel(provider.GetService<IApplicationContext>());
            channel.Subscribe(target);
            var message = Message.Create("test");
            var currentId = Task.CurrentId;
            var curThreadId = Thread.CurrentThread.ManagedThreadId;
            await channel.Writer.WriteAsync(message);
            Assert.Equal(currentId, target.TaskId);
            Assert.Equal(curThreadId, target.ThreadId);
        }

        [Fact]
        public void TestTryWrite()
        {
            var provider = services.BuildServiceProvider();
            var target = new ThreadNameExtractingTestTarget();
            var channel = new DirectChannel(provider.GetService<IApplicationContext>());
            channel.Subscribe(target);
            var message = Message.Create("test");
            var currentId = Task.CurrentId;
            var curThreadId = Thread.CurrentThread.ManagedThreadId;
            Assert.True(channel.Writer.TryWrite(message));
            Assert.Equal(currentId, target.TaskId);
            Assert.Equal(curThreadId, target.ThreadId);
        }

        [Fact]
        public async Task TestWaitToWriteAsync()
        {
            var provider = services.BuildServiceProvider();
            var target = new ThreadNameExtractingTestTarget();
            var channel = new DirectChannel(provider.GetService<IApplicationContext>());
            channel.Subscribe(target);
            Assert.True(await channel.Writer.WaitToWriteAsync());
            channel.Unsubscribe(target);
            Assert.False(await channel.Writer.WaitToWriteAsync());
        }

        [Fact]
        public void TestTryComplete()
        {
            var provider = services.BuildServiceProvider();
            var channel = new DirectChannel(provider.GetService<IApplicationContext>());
            Assert.False(channel.Writer.TryComplete());
        }
    }
}
