using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SysGroupTryApplyEffect))]
    public partial class SysGroupInstantEffect : ComponentSystemGroup
    {
    }
}