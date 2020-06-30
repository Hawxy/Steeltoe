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

using Steeltoe.Messaging.Rabbit.Config;
using System;

namespace Steeltoe.Messaging.Rabbit.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    public class DeclareExchangeAttribute : Attribute
    {
        public DeclareExchangeAttribute()
        {
        }

        public string Name { get; set; }

        public string Type { get; set; } = ExchangeType.DIRECT;

        public string Durable { get; set; } = "True";

        public string AutoDelete { get; set; } = "False";

        public string Internal { get; set; } = "False";

        public string IgnoreDeclarationExceptions { get; set; } = "False";

        public string Delayed { get; set; } = "False";

        public string Declare { get; set; } = "True";

        public string Admin
        {
            get
            {
                if (Admins.Length == 0)
                {
                    return null;
                }

                return Admins[0];
            }

            set
            {
                Admins = new string[] { value };
            }
        }

        public string[] Admins { get; set; } = new string[0];
    }
}
