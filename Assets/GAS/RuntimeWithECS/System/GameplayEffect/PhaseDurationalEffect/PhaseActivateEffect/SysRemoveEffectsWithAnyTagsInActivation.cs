using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupDurationalEffect))]
    [UpdateBefore(typeof(SysActivateEnd))]
    public partial struct SysRemoveEffectsWithAnyTagsInActivation : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInActivationProgress>();
            state.RequireForUpdate<CRemoveEffectWithTags>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}