using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    [UpdateAfter(typeof(SysInitEffect))]
    public partial struct SysRemoveEffectWithTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComRemoveEffectWithTags>();
            state.RequireForUpdate<ComInApplicationProgress>();
            state.RequireForUpdate<ComValidEffect>();
            state.RequireForUpdate<ComInUsage>();
        }

        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (removeEffectWithTags,inUsage,_,_) in 
                     SystemAPI.Query<RefRO<ComRemoveEffectWithTags>,RefRO<ComInUsage>,RefRO<ComInApplicationProgress>,RefRO<ComValidEffect>>())
            {
                var tags = removeEffectWithTags.ValueRO.tags;
                if (tags.Length == 0) continue;

                var asc = inUsage.ValueRO.Target;
                var geBuff = SystemAPI.GetBuffer<BuffEleGameplayEffect>(asc);
                for (var i = geBuff.Length - 1; i >= 0; i--)
                {
                    var ge = geBuff[i].GameplayEffect;
                    var hasRemoveTag = ge.CheckEffectHasAnyTags(tags);
                    if (!hasRemoveTag) continue;
                    geBuff.RemoveAt(i);
                    // 含有子实例的组件也要清理
                    if (SystemAPI.HasComponent<ComPeriod>(ge))
                    {
                        var period = SystemAPI.GetComponentRO<ComPeriod>(ge);
                        foreach (var sonGe in period.ValueRO.GameplayEffects)
                            ecb.AddComponent<ComDestroy>(sonGe);
                    }
                    ecb.AddComponent<ComDestroy>(ge);
                }
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        // public static void RemoveGameplayEffectWithAnyTags(this Entity asc, Entity gameplayEffect)
        // {
        //     if (!_entityManager.HasComponent<ComRemoveEffectWithTags>(gameplayEffect)) return;
        //
        //     var comRemoveEffectWithTags = _entityManager.GetComponentData<ComRemoveEffectWithTags>(gameplayEffect);
        //     var removeEffectWithTags = comRemoveEffectWithTags.tags;
        //     if (removeEffectWithTags.Length == 0) return;
        //
        //     var geBuff = _entityManager.GetBuffer<GameplayEffectBufferElement>(asc);
        //     for (var i = geBuff.Length - 1; i >= 0; i--)
        //     {
        //         var ge = geBuff[i].GameplayEffect;
        //         var hasRemoveTag = ge.CheckEffectHasAnyTags(removeEffectWithTags);
        //         if (!hasRemoveTag) continue;
        //         geBuff.RemoveAt(i);
        //         // 含有子实例的组件也要清理
        //         if (_entityManager.HasComponent<ComPeriod>(ge))
        //         {
        //             var period = _entityManager.GetComponentData<ComPeriod>(ge);
        //             foreach (var sonGe in period.GameplayEffects)
        //                 _entityManager.DestroyEntity(sonGe);
        //         }
        //
        //         _entityManager.DestroyEntity(ge);
        //     }
        // }
    }
}