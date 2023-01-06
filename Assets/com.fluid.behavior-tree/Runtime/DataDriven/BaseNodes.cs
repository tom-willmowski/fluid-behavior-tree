using System;
using System.Collections.Generic;
using CleverCrow.Fluid.BTs.Trees;
using Newtonsoft.Json;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.DataDriven
{
    public class Blackboard
    {
        public Dictionary<string, int> intValues = new();
        public Dictionary<string, string> stringValues = new();
        public Dictionary<string, Vector3> vectorValues = new();
        public Dictionary<string, bool> boolValues = new();
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
}
