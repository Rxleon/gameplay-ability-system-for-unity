using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SysGroupDurationalEffect))]
    public partial class SysGroupApplicationEnd : ComponentSystemGroup
    {
    }
}