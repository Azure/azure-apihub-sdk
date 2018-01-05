using Microsoft.Azure.ApiHub.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub.Extensions
{

    public static class JsonExtensions
    {
        public static readonly JsonSerializerSettings ObjectSerializationSettings = new JsonSerializerSettings
        {
            MaxDepth = new int?(512),
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
            MaxDepth = new int?(512),
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
        public const int JsonSerializationMaxDepth = 512;

        //public static readonly MediaTypeFormatter JsonObjectTypeFormatter;
        //public static readonly MediaTypeFormatter[] JsonObjectTypeFormatters;
        //public static readonly MediaTypeFormatter JsonMediaTypeFormatter;
        //public static readonly MediaTypeFormatter[] JsonMediaTypeFormatters;

        static JsonExtensions()
        {
            //System.Runtime.Serialization.Formatter
            //System.Net.Http.Formatting.JsonMediaTypeFormatter mediaTypeFormatter1 = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            //mediaTypeFormatter1.SerializerSettings = ObjectSerializationSettings;
            //mediaTypeFormatter1.UseDataContractJsonSerializer = false;
            //JsonObjectTypeFormatter = (MediaTypeFormatter)mediaTypeFormatter1;
            //JsonObjectTypeFormatters = new MediaTypeFormatter[1]
            //{
            //    JsonObjectTypeFormatter
            //};
            //System.Net.Http.Formatting.JsonMediaTypeFormatter mediaTypeFormatter2 = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            //mediaTypeFormatter2.SerializerSettings = MediaTypeFormatterSettings;
            //mediaTypeFormatter2.UseDataContractJsonSerializer = false;
            //JsonMediaTypeFormatter = (MediaTypeFormatter)mediaTypeFormatter2;
            //JsonMediaTypeFormatters = new MediaTypeFormatter[1]
            //{
            //    JsonMediaTypeFormatter
            //};
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
            try
            {
                if (jtoken != null)
                    return FromJToken<T>(jtoken);
            }
            catch (FormatException)
            {
            }
            catch (JsonException)
            {
            }
            return default(T);
        }

        public static JToken GetProperty(this JToken entity, string propertyName)
        {
            JObject jobject = entity as JObject;
            JToken jtoken;
            if (jobject == null || !jobject.TryGetValue(propertyName, StringComparison.InvariantCultureIgnoreCase, out jtoken))
                jtoken = null;
            return jtoken;
        }

        public static JToken GetProperty(this IDictionary<string, JToken> container, string propertyName)
        {
            JToken jtoken;
            if (container == null || !container.TryGetValue(propertyName, out jtoken))
                jtoken = null;
            return jtoken;
        }

        public static T GetProperty<T>(this JToken entity, string propertyName)
        {
            JToken property = GetProperty(entity, propertyName);
            if (property == null)
                return default(T);
            return FromJToken<T>(property);
        }

        public static T GetProperty<T>(this IDictionary<string, JToken> container, string propertyName)
        {
            JToken property = GetProperty(container, propertyName);
            if (property == null)
                return default(T);
            return FromJToken<T>(property);
        }
    }
}
