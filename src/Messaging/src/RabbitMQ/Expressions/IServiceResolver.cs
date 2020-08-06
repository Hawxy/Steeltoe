﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using Steeltoe.Common.Expression;

namespace Steeltoe.Messaging.RabbitMQ.Expressions
{
    public interface IServiceResolver
    {
        object Resolve(IEvaluationContext context, string serviceName);
    }
}