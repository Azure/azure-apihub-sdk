using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Microfoft.Azure.ApiHub.Sdk.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Microfoft.Azure.ApiHub.Sdk.Extensions
{

    public static class JsonExtensions
    {
        public static readonly JsonSerializerSettings ObjectSerializationSettings = new JsonSerializerSettings()
        {
            MaxDepth = new int?(512),
            TypeNameHandling = TypeNameHandling.None,
            DateParseHandling = DateParseHandling.None,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = (IContractResolver)new CamelCasePropertyNamesWithOverridesContractResolver(),
            Converters = (IList<JsonConverter>)new List<JsonConverter>()
      {
        (JsonConverter) new LineInfoConverter(),
        (JsonConverter) new TimeSpanConverter(),
        (JsonConverter) new StringEnumConverter()
        {
          CamelCaseText = false
        },
        (JsonConverter) new AdjustToUniversalIsoDateTimeConverter()
      }
        };
        public static readonly JsonSerializerSettings MediaTypeFormatterSettings = new JsonSerializerSettings()
        {
            MaxDepth = new int?(512),
            TypeNameHandling = TypeNameHandling.None,
            DateParseHandling = DateParseHandling.None,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error,
            ContractResolver = (IContractResolver)new CamelCasePropertyNamesWithOverridesContractResolver(),
            Converters = (IList<JsonConverter>)new List<JsonConverter>()
      {
        (JsonConverter) new LineInfoConverter(),
        (JsonConverter) new TimeSpanConverter(),
        (JsonConverter) new StringEnumConverter()
        {
          CamelCaseText = false
        },
        (JsonConverter) new AdjustToUniversalIsoDateTimeConverter()
      }
        };
        public static readonly JsonSerializer JsonObjectTypeSerializer = JsonSerializer.Create(JsonExtensions.ObjectSerializationSettings);
        public static readonly JsonSerializer JsonMediaTypeSerializer = JsonSerializer.Create(JsonExtensions.MediaTypeFormatterSettings);
        public const int JsonSerializationMaxDepth = 512;
        public static readonly MediaTypeFormatter JsonObjectTypeFormatter;
        public static readonly MediaTypeFormatter[] JsonObjectTypeFormatters;
        public static readonly MediaTypeFormatter JsonMediaTypeFormatter;
        public static readonly MediaTypeFormatter[] JsonMediaTypeFormatters;

        static JsonExtensions()
        {
            System.Net.Http.Formatting.JsonMediaTypeFormatter mediaTypeFormatter1 = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            mediaTypeFormatter1.SerializerSettings = JsonExtensions.ObjectSerializationSettings;
            mediaTypeFormatter1.UseDataContractJsonSerializer = false;
            JsonExtensions.JsonObjectTypeFormatter = (MediaTypeFormatter)mediaTypeFormatter1;
            JsonExtensions.JsonObjectTypeFormatters = new MediaTypeFormatter[1]
            {
        JsonExtensions.JsonObjectTypeFormatter
            };
            System.Net.Http.Formatting.JsonMediaTypeFormatter mediaTypeFormatter2 = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            mediaTypeFormatter2.SerializerSettings = JsonExtensions.MediaTypeFormatterSettings;
            mediaTypeFormatter2.UseDataContractJsonSerializer = false;
            JsonExtensions.JsonMediaTypeFormatter = (MediaTypeFormatter)mediaTypeFormatter2;
            JsonExtensions.JsonMediaTypeFormatters = new MediaTypeFormatter[1]
            {
        JsonExtensions.JsonMediaTypeFormatter
            };
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, JsonExtensions.ObjectSerializationSettings);
        }

        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonExtensions.ObjectSerializationSettings);
        }

        public static T FromJson<T>(this string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static object FromJson(this string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, JsonExtensions.ObjectSerializationSettings);
        }

        public static JToken ToJToken(this object obj)
        {
            return JToken.FromObject(obj, JsonExtensions.JsonObjectTypeSerializer);
        }

        public static T FromJToken<T>(this JToken jtoken)
        {
            return jtoken.ToObject<T>(JsonExtensions.JsonObjectTypeSerializer);
        }

        public static object FromJToken(this JToken jtoken, Type type)
        {
            return jtoken.ToObject(type, JsonExtensions.JsonObjectTypeSerializer);
        }

        public static T TryFromJToken<T>(this JToken jtoken)
        {
            try
            {
                if (jtoken != null)
                    return JsonExtensions.FromJToken<T>(jtoken);
            }
            catch (FormatException ex)
            {
            }
            catch (JsonException ex)
            {
            }
            return default(T);
        }

        public static JToken GetProperty(this JToken entity, string propertyName)
        {
            JObject jobject = entity as JObject;
            JToken jtoken;
            if (jobject == null || !jobject.TryGetValue(propertyName, StringComparison.InvariantCultureIgnoreCase, out jtoken))
                return (JToken)null;
            return jtoken;
        }

        public static JToken GetProperty(this IDictionary<string, JToken> container, string propertyName)
        {
            JToken jtoken;
            if (container == null || !container.TryGetValue(propertyName, out jtoken))
                return (JToken)null;
            return jtoken;
        }

        public static T GetProperty<T>(this JToken entity, string propertyName)
        {
            JToken property = JsonExtensions.GetProperty(entity, propertyName);
            if (property == null)
                return default(T);
            return JsonExtensions.FromJToken<T>(property);
        }

        public static T GetProperty<T>(this IDictionary<string, JToken> container, string propertyName)
        {
            JToken property = JsonExtensions.GetProperty(container, propertyName);
            if (property == null)
                return default(T);
            return JsonExtensions.FromJToken<T>(property);
        }
    }
}
