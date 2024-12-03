using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseApplicationEnd
{
    [UpdateInGroup(typeof(SysGroupApplicationEnd))]
    public partial struct SysRemoveApplicationProgressTag : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComInApplicationProgress>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_,ge) in SystemAPI.Query<RefRO<ComInApplicationProgress>>().WithEntityAccess())
            {
                ecb.RemoveComponent<ComInApplicationProgress>(ge);
                if(SystemAPI.HasComponent<ComValidEffect>(ge))
                    ecb.RemoveComponent<ComValidEffect>(ge);
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}