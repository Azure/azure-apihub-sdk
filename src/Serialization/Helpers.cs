using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Microfoft.Azure.ApiHub.Sdk.Serialization
{
    #region Converters
    internal class CamelCasePropertyNamesWithOverridesContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            object[] customAttributes = member.GetCustomAttributes(typeof(JsonPropertyAttribute), true);
            if (Enumerable.Any<object>((IEnumerable<object>)customAttributes))
            {
                string propertyName = Enumerable.Single<JsonPropertyAttribute>(Enumerable.Cast<JsonPropertyAttribute>((IEnumerable)customAttributes)).PropertyName;
                if (!string.IsNullOrEmpty(propertyName))
                    property.PropertyName = propertyName;
            }
            return property;
        }

        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract dictionaryContract = base.CreateDictionaryContract(objectType);
            if (Enumerable.Any<object>((IEnumerable<object>)objectType.GetCustomAttributes(typeof(JsonPreserveCaseDictionaryAttribute), true)))
                dictionaryContract.PropertyNameResolver = (Func<string, string>)(propertyName => propertyName);
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
            if (reader.TokenType == JsonToken.Null)
                return (object)null;
            return (object)XmlConvert.ToTimeSpan(serializer.Deserialize<string>(reader));
        }

        public override bool CanConvert(Type objectType)
        {
            if (!(objectType == typeof(TimeSpan)))
                return objectType == typeof(TimeSpan?);
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
            if (!this.LineNumber.HasValue)
                return this.LinePosition.HasValue;
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
                return (object)null;
            int num1 = 0;
            int num2 = 0;
            IJsonLineInfo jsonLineInfo1 = reader as IJsonLineInfo;
            if (jsonLineInfo1 != null && jsonLineInfo1.HasLineInfo())
            {
                num1 = jsonLineInfo1.LineNumber;
                num2 = jsonLineInfo1.LinePosition;
            }
            JsonLineInfo jsonLineInfo2 = Activator.CreateInstance(objectType) as JsonLineInfo;
            serializer.Populate(reader, (object)jsonLineInfo2);
            jsonLineInfo2.LineNumber = new int?(num1);
            jsonLineInfo2.LinePosition = new int?(num2);
            return (object)jsonLineInfo2;
        }
    }
    public class AdjustToUniversalIsoDateTimeConverter : IsoDateTimeConverter
    {
        public AdjustToUniversalIsoDateTimeConverter()
        {
            this.DateTimeStyles = DateTimeStyles.AdjustToUniversal;
            this.Culture = CultureInfo.InvariantCulture;
        }
    }
    #endregion
}
