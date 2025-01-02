using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupApplicationGameplayEffectStacking))]
    public partial struct SCheckStacking : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GlobalTimer>();
            state.RequireForUpdate<CInApplicationProgress>();
            state.RequireForUpdate<CStacking>();
            state.RequireForUpdate<CDuration>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (inUsage, _, stacking,duration, ge) in SystemAPI
                         .Query<RefRO<CInUsage>, RefRO<CInActivationProgress>, RefRO<CStacking>,RefRW<CDuration>>()
                         .WithEntityAccess())
            {
                // 判断是否是新添加的GE
                var stackingGe = GetStackingGameplayEffect(state.EntityManager, 
                    inUsage.ValueRO.Target,
                    inUsage.ValueRO.Source,
                    stacking.ValueRO);
                if (stackingGe != Entity.Null)
                {
                    // 1.叠加的GE不走Activate流程，并且直接销毁
                    ecb.RemoveComponent<CInApplicationProgress>(ge);
                    ecb.RemoveComponent<CValidEffect>(ge);
                    ecb.AddComponent<CDestroy>(ge);

                    // 2.尝试更新Stack层数，触发OnStackCountChanged事件
                    var changed = TryChangeStackCount(state.EntityManager, stackingGe, 0); // stacking.ValueRO.StackCount);

                    // 3.如果层数改变，额外触发OnGameplayEffectContainerIsDirty
                    if (changed)
                    {
                        
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        /// <summary>
        ///     判断是否是新添加的GameplayEffect
        /// </summary>
        private Entity GetStackingGameplayEffect(EntityManager entityManager, Entity owner, Entity source,
            CStacking stacking)
        {
            var effects = entityManager.GetBuffer<BuffEleGameplayEffect>(owner);
            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i].GameplayEffect;
                var hasStacking = entityManager.HasComponent<CStacking>(effect);
                if (hasStacking)
                {
                    var s = entityManager.GetComponentData<CStacking>(effect);
                    if (stacking.StackingCode == s.StackingCode)
                    {
                        if (stacking.StackType == EffectStackType.AggregateByTarget)
                            return effect;
                        if (stacking.StackType == EffectStackType.AggregateBySource)
                        {
                            var inUsage = entityManager.GetComponentData<CInUsage>(effect);
                            if (inUsage.Source == source)
                                return effect;
                        }
                    }
                }
            }

            return Entity.Null;
        }

        private bool TryChangeStackCount(EntityManager entityManager, Entity ge,CStacking stacking, int stackCount,RefRW<CDuration> duration,GlobalTimer globalFrameTimer)
        {
            // 获取旧Stacking数据
            var oldStackCount = entityManager.GetComponentData<CStacking>(ge).StackCount;
            int newStackCount = stackCount;
            if (stackCount <= stacking.LimitCount)
            {
                // 更新栈数
                newStackCount = Math.Max(1,stackCount); // 最小层数为1
                // 是否刷新Duration
                if (stacking.EffectDurationRefreshPolicy == EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    RefreshDuration(duration,globalFrameTimer);
                }
                // 是否重置Period
                if (stacking.EffectPeriodResetPolicy == EffectPeriodResetPolicy.ResetOnSuccessfulApplication)
                {
                    bool hasPeriodTicker = entityManager.HasComponent<CPeriod>(ge);
                    if(hasPeriodTicker)
                        PeriodTicker.ResetPeriod();
                }
            }
            else
            {
                // 溢出GE生效
                foreach (var overflowEffect in stacking.overflowEffects)
                    Owner.ApplyGameplayEffectToSelf(overflowEffect);

                if (stacking.EffectDurationRefreshPolicy == EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    if (stacking.denyOverflowApplication)
                    {
                        //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
                        if (stacking.clearStackOnOverflow)
                        {
                            RemoveSelf();
                        }
                    }
                    else
                    {
                        RefreshDuration();
                    }
                }
            }
            
            RefreshStack(StackCount + 1);
            OnStackCountChange(oldStackCount, newStackCount);
            return oldStackCount != newStackCount;
        }
        
        private void RefreshDuration(RefRW<CDuration> duration,GlobalTimer globalFrameTimer)
        {
            SActivateEffect.UpdateActiveTime(ref duration.ValueRW,globalFrameTimer);
        }
    }
}