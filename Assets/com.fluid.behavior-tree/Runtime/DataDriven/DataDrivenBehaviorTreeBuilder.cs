using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.DataDriven
{
    public class DataDrivenBehaviorTreeBuilder
    {
        public static BehaviorTree Build(BehaviorTreeNode treeNode, DataDrivenBehaviorTreeRegistry registry, Blackboard blackboard, GameObject owner)
        {
            var nodes = treeNode.nodes;
            var b = new BehaviorTreeBuilder(owner);
            var context = new DataDrivenBehaviorTreeContext(b, registry, blackboard, owner);
            for (int i = 0; i < nodes.Count; i++)
            {
                var command = nodes[i];
                command.Add(context);
            }
            return b.Build();
        }
    }
}
