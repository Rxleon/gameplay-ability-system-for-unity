using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SysGroupInstantEffect))]
    public partial class SysGroupDurationalEffect : ComponentSystemGroup
    {
    }
}