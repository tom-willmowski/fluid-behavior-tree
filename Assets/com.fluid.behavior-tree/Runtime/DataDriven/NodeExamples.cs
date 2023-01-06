using System;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.DataDriven
{
    public class NodeExamples
    {
        [Node("log_node")]
        [Serializable]
        public class LogNode : Node
        {
            public string message;

            public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
            {
                return context.Builder.Do(name, () =>
                {
                    Debug.Log(message);
                    return TaskStatus.Success;
                });
            }
        }

        [Node("condition_node")]
        [Serializable]
        public class ConditionNode : Node
        {
            public bool condition;

            public override BehaviorTreeBuilder Add(DataDrivenBehaviorTreeContext context)
            {
                return context.Builder.Condition(name, () => condition);
            }
        }
    }
}
