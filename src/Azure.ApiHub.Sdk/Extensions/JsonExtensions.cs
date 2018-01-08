// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Microsoft.Azure.ApiHub.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Extensions
{
    public static class JsonExtensions
    {
        public const int JsonSerializationMaxDepth = 512;

        public static readonly JsonSerializerSettings ObjectSerializationSettings = new JsonSerializerSettings
        {
            MaxDepth = new int?(JsonSerializationMaxDepth),
            TypeNameHandling = TypeNameHandling.None,
            DateParseHandling = DateParseHandling.None,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesWithOverridesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new LineInfoConverter(),
                new TimeSpanConverter(),
                new StringEnumConverter()
                {
                    CamelCaseText = false
                },
                new AdjustToUniversalIsoDateTimeConverter()
            }
        };

        public static readonly JsonSerializerSettings MediaTypeFormatterSettings = new JsonSerializerSettings
        {
            MaxDepth = new int?(JsonSerializationMaxDepth),
            TypeNameHandling = TypeNameHandling.None,
            DateParseHandling = DateParseHandling.None,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error,
            ContractResolver = new CamelCasePropertyNamesWithOverridesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new LineInfoConverter(),
                new TimeSpanConverter(),
                new StringEnumConverter()
                {
                    CamelCaseText = false
                },
                new AdjustToUniversalIsoDateTimeConverter()
            }
        };

        public static readonly JsonSerializer JsonObjectTypeSerializer = JsonSerializer.Create(ObjectSerializationSettings);
        public static readonly JsonSerializer JsonMediaTypeSerializer = JsonSerializer.Create(MediaTypeFormatterSettings);

        public static readonly MediaTypeFormatter JsonObjectTypeFormatter;
        public static readonly MediaTypeFormatter[] JsonObjectTypeFormatters;
        public static readonly MediaTypeFormatter JsonMediaTypeFormatter;
        public static readonly MediaTypeFormatter[] JsonMediaTypeFormatters;

        static JsonExtensions()
        {
            JsonObjectTypeFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = ObjectSerializationSettings,
                UseDataContractJsonSerializer = false,
            };

            JsonObjectTypeFormatters = new[] { JsonObjectTypeFormatter };

            JsonMediaTypeFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = MediaTypeFormatterSettings,
                UseDataContractJsonSerializer = false
            };

            JsonMediaTypeFormatters = new[] { JsonMediaTypeFormatter };
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, ObjectSerializationSettings);
        }

        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, ObjectSerializationSettings);
        }

        public static T FromJson<T>(this string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static object FromJson(this string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, ObjectSerializationSettings);
        }

        public static JToken ToJToken(this object obj)
        {
            return JToken.FromObject(obj, JsonObjectTypeSerializer);
        }

        public static T FromJToken<T>(this JToken jtoken)
        {
            return jtoken.ToObject<T>(JsonObjectTypeSerializer);
        }

        public static object FromJToken(this JToken jtoken, Type type)
        {
            return jtoken.ToObject(type, JsonObjectTypeSerializer);
        }

        public static T TryFromJToken<T>(this JToken jtoken)
        {
            T result = default(T);
            try
            {
                if (jtoken != null)
                {
                    result = FromJToken<T>(jtoken);
                }
            }
            catch (FormatException)
            {
            }
            catch (JsonException)
            {
            }

            return result;
        }

        public static JToken GetProperty(this JToken entity, string propertyName)
        {
            JObject jobject = entity as JObject;
            JToken jtoken;
            if (jobject == null || !jobject.TryGetValue(propertyName, StringComparison.InvariantCultureIgnoreCase, out jtoken))
            {
                jtoken = null;
            }

            return jtoken;
        }

        public static JToken GetProperty(this IDictionary<string, JToken> container, string propertyName)
        {
            JToken jtoken;
            if (container == null || !container.TryGetValue(propertyName, out jtoken))
            {
                jtoken = null;
            }
            return jtoken;
        }

        public static T GetProperty<T>(this JToken entity, string propertyName)
        {
            JToken property = GetProperty(entity, propertyName);
            T result = default(T);
            if (property != null)
            {
                result = FromJToken<T>(property);
            }

            return result;
        }

        public static T GetProperty<T>(this IDictionary<string, JToken> container, string propertyName)
        {
            JToken property = GetProperty(container, propertyName);

            T result = default(T);
            if (property != null)
            {
                result = FromJToken<T>(property);
            }

            return result;
        }
    }
}
