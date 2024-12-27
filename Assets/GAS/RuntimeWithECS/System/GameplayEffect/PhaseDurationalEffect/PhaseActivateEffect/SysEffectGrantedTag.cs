using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    public partial struct SysEffectGrantedTag : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInUsage>();
            state.RequireForUpdate<CInActivationProgress>();
            state.RequireForUpdate<CGrantedTags>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (inUsage, _, grantedTags, ge) in SystemAPI
                         .Query<RefRO<CInUsage>, RefRO<CInActivationProgress>, RefRO<CGrantedTags>>()
                         .WithEntityAccess())
            {
                var owner = inUsage.ValueRO.Target;
                var tags = grantedTags.ValueRO.tags;
                var buff = state.EntityManager.GetBuffer<BTemporaryTag>(owner);
                foreach (var tag in tags)
                {
                    var temTag = new BTemporaryTag
                        { tag = tag, source = ge, sourceType = GameplayTagSourceType.GameplayEffect };
                    buff.Add(temTag);
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}