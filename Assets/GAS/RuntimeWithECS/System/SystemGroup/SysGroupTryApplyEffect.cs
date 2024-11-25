using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SysGroupTryApplyEffect:ComponentSystemGroup
    {
    }
}