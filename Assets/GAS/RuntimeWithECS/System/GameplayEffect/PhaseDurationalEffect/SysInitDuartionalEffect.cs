﻿using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup( typeof( SysGroupDurationalEffect ) )]
    public partial struct SysInitDuartionalEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInUsage>();
            state.RequireForUpdate<CInApplicationProgress>();
            state.RequireForUpdate<CValidEffect>();
            state.RequireForUpdate<CDuration>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (inUsage,_,_,_,ge) in 
                     SystemAPI.Query<RefRW<CInUsage>,RefRO<CInApplicationProgress>,RefRO<CValidEffect>,RefRO<CDuration>>().WithEntityAccess())
            {
                var owner = inUsage.ValueRO.Target;
                // TODO 初始化，设置Level
                
                // 加入GE Container列表
                var geContainer = SystemAPI.GetBuffer<BuffEleGameplayEffect>(owner);
                geContainer.Add(new BuffEleGameplayEffect { GameplayEffect = ge });
                
                ecb.AddComponent<EffectContainerDirty>(owner);
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}