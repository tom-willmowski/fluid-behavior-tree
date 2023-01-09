using System;
using System.Collections.Generic;
using CleverCrow.Fluid.BTs.Trees;
using Newtonsoft.Json;

namespace CleverCrow.Fluid.BTs.DataDriven
{
    public class Blackboard
    {
        private Dictionary<string, object> values = new();

        public void Add(string key, object value)
        {
            values.TryAdd(key, value);
        }

        public (bool available, T value) Get<T>(string key)
        {
            var available = values.TryGetValue(key, out var value);
            if (!available)
            {
                return (false, default);
            }
            return (available, (T)value);
        }

        public (bool available, object value) Get(string key)
        {
            var available = values.TryGetValue(key, out var value);
            return (available, value);
        }

        public bool Change(string key, object value)
        {
            var contains = Contains(key);
            if (contains)
            {
                values[key] = value;
                return true;
            }
            return false;
        }

        public bool Contains(string key)
        {
            return values.ContainsKey(key);
        }
    }

    public class NodeAttribute : Attribute
    {
        public string Name { get; }

        public NodeAttribute(string name)
        {
            Name = name;
        }
    }

    [Serializable]
    public class BehaviorTreeNode
    {
        public string id;
        public List<Node> nodes;
    }

    [JsonConverter(typeof(NodeConverter))]
    [Serializable]
    public abstract class Node
    {
        public string name;

        public abstract BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context);
    }

    [Node("selector")]
    [Serializable]
    public class Selector : Node
    {
        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.Selector(name);
        }
    }

    [Node("sequence")]
    [Serializable]
    public class Sequence : Node
    {
        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.Sequence(name);
        }
    }

    [Node("parallel")]
    [Serializable]
    public class ParallelNode : Node
    {
        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.Parallel(name);
        }
    }

    [Node("splice")]
    [Serializable]
    public class SpliceNode : Node
    {
        public string id;

        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.Splice(DataDrivenBehaviorTreeBuilder.Build(context.Registry.Get(id), context.Registry, context.Blackboard, context.Owner));
        }
    }

    [Node("end")]
    [Serializable]
    public class End : Node
    {
        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.End();
        }
    }

    [Node(Id)]
    [Serializable]
    public class HasCondition : Node
    {
        public const string Id = "has";

        public string key;

        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.Condition(Id, () => context.Blackboard.Contains(key));
        }
    }

    [Node(Id)]
    [Serializable]
    public class LessCondition : Node
    {
        public const string Id = "less";

        public string key;
        public float value;

        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.Condition(Id, () =>
            {
                var result = context.Blackboard.Get(key);
                return result.available && Convert.ToSingle(result.value) < value;
            });
        }
    }

    [Node(Id)]
    [Serializable]
    public class MoreCondition : Node
    {
        public const string Id = "more";

        public string key;
        public float value;

        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.Condition(Id, () =>
            {
                var result = context.Blackboard.Get(key);
                return result.available && Convert.ToSingle(result.value) > value;
            });
        }
    }

    [Node(Id)]
    [Serializable]
    public class EqualsCondition : Node
    {
        public const string Id = "equals";

        public string key;

        public int? intValue;
        public string stringValue;
        public bool? boolValue;

        public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
        {
            return context.Builder.Condition(Id, () =>
            {
                var result = context.Blackboard.Get(key);
                return result.available && Check(result.value);
            });
        }

        private bool Check(object blackboardValue)
        {
            if (intValue.HasValue)
            {
                return Convert.ToInt32(blackboardValue) == intValue.Value;
            }
            if (stringValue != null)
            {
                return Convert.ToString(blackboardValue) == stringValue;
            }
            if (boolValue.HasValue)
            {
                return Convert.ToBoolean(blackboardValue) == boolValue.Value;
            }
            return false;
        }
    }
}
