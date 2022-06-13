using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Models.RBSS_CS
{
    [DataContract]
    [JsonConverter(typeof(StepConverter))]
    public abstract class AbstractStep
    {
        /// <summary>
        /// Gets or Sets IdFrom
        /// </summary>
        [DataMember(Name="idFrom", EmitDefaultValue=false)]
        public string IdFrom { get; set; }

        /// <summary>
        /// Gets or Sets IdTo
        /// </summary>
        [DataMember(Name="idTo", EmitDefaultValue=false)]
        public string IdTo { get; set; }

    }


    public class StepSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(AbstractStep).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }
    public class StepConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings()
        {
            ContractResolver = new StepSpecifiedConcreteClassConverter()
        };
        public override bool CanWrite
        {
            get { return false; }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (jo.Count)
            {
                case 5:
                    return JsonConvert.DeserializeObject<InsertStep>(jo.ToString(), SpecifiedSubclassConversion);
                case 3:
                    return JsonConvert.DeserializeObject<ValidateStep>(jo.ToString(), SpecifiedSubclassConversion);
                default:
                    throw new Exception();
            }
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(AbstractStep));
        }
    }
}
