// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Azure.ApiHub.Serialization
{
    #region Converters

    internal class CamelCasePropertyNamesWithOverridesContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            object[] customAttributes = member.GetCustomAttributes(typeof(JsonPropertyAttribute), true);
            if (Enumerable.Any<object>(customAttributes))
            {
                string propertyName = Enumerable.Single(Enumerable.Cast<JsonPropertyAttribute>((IEnumerable)customAttributes)).PropertyName;
                if (!string.IsNullOrEmpty(propertyName))
                {
                    property.PropertyName = propertyName;
                }
            }
            return property;
        }

        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            // Fix PropertyNameResolver : https://github.com/JamesNK/Newtonsoft.Json/commit/0929c2b4c3349ae1fb3d22c522504fbf57c8268e
            JsonDictionaryContract dictionaryContract = base.CreateDictionaryContract(objectType);
            if (Enumerable.Any(objectType.GetCustomAttributes(typeof(JsonPreserveCaseDictionaryAttribute), true)))
            {
                dictionaryContract.DictionaryKeyResolver = propertyName => propertyName;
            }
            return dictionaryContract;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class JsonPreserveCaseDictionaryAttribute : Attribute
    {
    }

    internal class TimeSpanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (object)XmlConvert.ToString((TimeSpan)value));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result = null;
            if (reader.TokenType != JsonToken.Null)
            {
                result = XmlConvert.ToTimeSpan(serializer.Deserialize<string>(reader));
            }
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            if (!(objectType == typeof(TimeSpan)))
            {
                return objectType == typeof(TimeSpan?);
            }
            return true;
        }
    }
    public class JsonLineInfo
    {
        [JsonIgnore]
        public int? LineNumber { get; set; }

        [JsonIgnore]
        public int? LinePosition { get; set; }

        public bool HasLineInfo()
        {
            if (!LineNumber.HasValue)
            {
                return LinePosition.HasValue;
            }
                
            return true;
        }
    }

    internal class LineInfoConverter : JsonConverter
    {
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Converter is not writable. Method should not be invoked");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonLineInfo).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            { 
                return null;
            }
            int num1 = 0;
            int num2 = 0;
            IJsonLineInfo jsonLineInfo1 = reader as IJsonLineInfo;
            if (jsonLineInfo1 != null && jsonLineInfo1.HasLineInfo())
            {
                num1 = jsonLineInfo1.LineNumber;
                num2 = jsonLineInfo1.LinePosition;
            }
            JsonLineInfo jsonLineInfo2 = Activator.CreateInstance(objectType) as JsonLineInfo;
            serializer.Populate(reader, jsonLineInfo2);
            jsonLineInfo2.LineNumber = new int?(num1);
            jsonLineInfo2.LinePosition = new int?(num2);
            return jsonLineInfo2;
        }
    }

    public class AdjustToUniversalIsoDateTimeConverter : IsoDateTimeConverter
    {
        public AdjustToUniversalIsoDateTimeConverter()
        {
            DateTimeStyles = DateTimeStyles.AdjustToUniversal;
            Culture = CultureInfo.InvariantCulture;
        }
    }

    #endregion
}
