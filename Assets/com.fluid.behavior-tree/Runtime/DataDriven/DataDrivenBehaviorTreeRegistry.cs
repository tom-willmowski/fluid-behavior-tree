using System.Collections.Generic;

namespace CleverCrow.Fluid.BTs.DataDriven
{
    public class DataDrivenBehaviorTreeRegistry
    {
        private Dictionary<string, BehaviorTreeNode> registry = new();

        public BehaviorTreeNode Get(string id)
        {
            return registry[id];
        }

        public void Add(BehaviorTreeNode tree)
        {
            registry.Add(tree.id, tree);
        }

        public void Remove(string id)
        {
            registry.Remove(id);
        }
    }
}
