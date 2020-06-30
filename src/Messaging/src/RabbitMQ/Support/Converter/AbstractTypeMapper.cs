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

using Steeltoe.Messaging.Converter;
using Steeltoe.Messaging.Rabbit.Extensions;
using System;
using System.Collections.Generic;

namespace Steeltoe.Messaging.Rabbit.Support.Converter
{
    public abstract class AbstractTypeMapper
    {
        public const string DEFAULT_CLASSID_FIELD_NAME = "__TypeId__";
        public const string DEFAULT_CONTENT_CLASSID_FIELD_NAME = "__ContentTypeId__";
        public const string DEFAULT_KEY_CLASSID_FIELD_NAME = "__KeyTypeId__";

        private readonly Dictionary<string, Type> _idClassMapping = new Dictionary<string, Type>();

        private readonly Dictionary<Type, string> _classIdMapping = new Dictionary<Type, string>();

        public Dictionary<string, Type> IdClassMapping => _idClassMapping;

        public string ClassIdFieldName { get; internal set; } = DEFAULT_CLASSID_FIELD_NAME;

        public string ContentClassIdFieldName { get; internal set; } = DEFAULT_CONTENT_CLASSID_FIELD_NAME;

        public string KeyClassIdFieldName { get; internal set; } = DEFAULT_KEY_CLASSID_FIELD_NAME;

        public void SetIdClassMapping(Dictionary<string, Type> mapping)
        {
            foreach (var entry in mapping)
            {
                _idClassMapping[entry.Key] = entry.Value;
            }

            CreateReverseMap();
        }

        protected virtual void AddHeader(IMessageHeaders headers, string headerName, Type clazz)
        {
            var accessor = RabbitHeaderAccessor.GetMutableAccessor(headers);
            if (_classIdMapping.ContainsKey(clazz))
            {
                accessor.SetHeader(headerName, _classIdMapping[clazz]);
            }
            else
            {
                accessor.SetHeader(headerName, GetClassName(clazz));
            }
        }

        protected virtual string RetrieveHeader(IMessageHeaders headers, string headerName)
        {
            var classId = RetrieveHeaderAsString(headers, headerName);
            if (classId == null)
            {
                throw new MessageConversionException(
                        "failed to convert Message content. Could not resolve " + headerName + " in header");
            }

            return classId;
        }

        protected virtual string RetrieveHeaderAsString(IMessageHeaders headers, string headerName)
        {
            var classIdFieldNameValue = headers.Get<object>(headerName);
            string classId = null;
            if (classIdFieldNameValue != null)
            {
                classId = classIdFieldNameValue.ToString();
            }

            return classId;
        }

        protected virtual bool HasInferredTypeHeader(IMessageHeaders headers)
        {
            return headers.InferredArgumentType() != null;
        }

        protected Type FromInferredTypeHeader(IMessageHeaders headers)
        {
            return headers.InferredArgumentType();
        }

        protected virtual Type GetContentType(Type type)
        {
            if (IsContainerType(type))
            {
                var typedef = type.GetGenericTypeDefinition();
                if (typeof(Dictionary<,>) == typedef)
                {
                    return type.GetGenericArguments()[1];
                }
                else
                {
                    return type.GetGenericArguments()[0];
                }
            }

            return null;
        }

        protected virtual bool IsContainerType(Type type)
        {
            if (type.IsGenericType)
            {
                var typedef = type.GetGenericTypeDefinition();
                if (typeof(Dictionary<,>) == typedef
                    || typeof(List<>) == typedef
                    || typeof(HashSet<>) == typedef
                    || typeof(LinkedList<>) == typedef
                    || typeof(Stack<>) == typedef
                    || typeof(Queue<>) == typedef)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual Type GetKeyType(Type type)
        {
            if (IsContainerType(type))
            {
                var typedef = type.GetGenericTypeDefinition();
                if (typeof(Dictionary<,>) == typedef)
                {
                    return type.GetGenericArguments()[0];
                }
            }

            return null;
        }

        protected virtual string GetClassName(Type type)
        {
            if (IsContainerType(type))
            {
                return type.GetGenericTypeDefinition().FullName;
            }

            return type.ToString();
        }

        private void CreateReverseMap()
        {
            _classIdMapping.Clear();
            foreach (var entry in _idClassMapping)
            {
                var id = entry.Key;
                var clazz = entry.Value;
                _classIdMapping[clazz] = id;
            }
        }
    }
}
