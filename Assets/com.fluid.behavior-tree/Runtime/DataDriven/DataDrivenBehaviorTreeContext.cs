using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.DataDriven
{
    public class DataDrivenBehaviorTreeContext
    {
        public DataDrivenBehaviorTreeRegistry Registry { get; }
        public Blackboard Blackboard { get; }
        public GameObject Owner { get; }
        public BehaviorTreeBuilder Builder { get; }


        public DataDrivenBehaviorTreeContext(BehaviorTreeBuilder builder, DataDrivenBehaviorTreeRegistry registry, Blackboard blackboard, GameObject owner)
        {
            Builder = builder;
            Registry = registry;
            Blackboard = blackboard;
            Owner = owner;
        }
    }
}
