using System.Threading.Tasks;
using CleverCrow.Fluid.BTs.DataDriven;
using CleverCrow.Fluid.BTs.Trees;
using Newtonsoft.Json;
using UnityEngine;

public class DataDrivenTester : MonoBehaviour
{
    [SerializeField] private BehaviorTree tree;
    [SerializeField] private TextAsset behaviorJson;
    [SerializeField] private TextAsset dependentBehaviorJson;

    private JsonSerializerSettings settings = new() { ContractResolver = new NodeContractResolver() };

    private Blackboard blackboard;

    private async void Awake()
    {
        blackboard = new Blackboard();

        var registry = new DataDrivenBehaviorTreeRegistry();
        var behavior = JsonConvert.DeserializeObject<BehaviorTreeNode>(behaviorJson.text, settings);
        var dependentBehavior = JsonConvert.DeserializeObject<BehaviorTreeNode>(dependentBehaviorJson.text, settings);

        registry.Add(behavior);
        registry.Add(dependentBehavior);

        tree = DataDrivenBehaviorTreeBuilder.Build(dependentBehavior, registry, blackboard, gameObject);
        blackboard.Add("phase", 0);
        await Task.Delay(3000);
        blackboard.Change("phase", 1);
    }

    private void Update()
    {
        tree?.Tick();
    }
}
