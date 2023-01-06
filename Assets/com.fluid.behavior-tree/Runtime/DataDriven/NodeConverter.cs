using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CleverCrow.Fluid.BTs.DataDriven
{
    public class NodeContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Node).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }

    public class NodeConverter : JsonConverter
    {
        private static Dictionary<string, Type> types;

        public static void Init()
        {
            types = new Dictionary<string, Type>();
            var assembly = typeof(DataDrivenBehaviorTreeBuilder).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                var node = type.GetCustomAttribute<NodeAttribute>();
                if (node != null)
                {
                    types.TryAdd(node.Name, type);
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // won't be called because CanWrite returns false
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var name = jo.Properties().ElementAt(0).Name;
            return jo.GetValue(name)?.ToObject(GetChildType(name), serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Node);
        }

        public override bool CanWrite => false;

        private static Type GetChildType(string name)
        {
            if (types == null)
            {
                Init();
            }
            return types[name];
        }
    }
}
