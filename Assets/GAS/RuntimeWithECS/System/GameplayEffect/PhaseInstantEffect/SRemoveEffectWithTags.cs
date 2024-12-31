﻿using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    [UpdateAfter(typeof(SInitEffect))]
    public partial struct SRemoveEffectWithTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CRemoveEffectWithTags>();
            state.RequireForUpdate<CInApplicationProgress>();
            state.RequireForUpdate<CValidEffect>();
            state.RequireForUpdate<CInUsage>();
        }

        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (removeEffectWithTags,inUsage,_,_) in 
                     SystemAPI.Query<RefRO<CRemoveEffectWithTags>,RefRO<CInUsage>,RefRO<CInApplicationProgress>,RefRO<CValidEffect>>())
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
                    if (SystemAPI.HasComponent<CPeriod>(ge))
                    {
                        var period = SystemAPI.GetComponentRO<CPeriod>(ge);
                        foreach (var sonGe in period.ValueRO.GameplayEffects)
                            ecb.AddComponent<CDestroy>(sonGe);
                    }
                    ecb.AddComponent<CDestroy>(ge);
                }
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}