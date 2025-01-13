using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect;
using GAS.RuntimeWithECS.System.SystemGroup.LogicTick;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseLogicTick
{
    [UpdateInGroup(typeof(SysGroupTickGameplayEffect))]
    [UpdateAfter(typeof(SEffectPeriodTick))]
    public partial struct SEffectStackingTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            state.RequireForUpdate<GlobalTimer>();
            state.RequireForUpdate<CValidEffect>();
            state.RequireForUpdate<CInUsage>();
            state.RequireForUpdate<CStacking>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (duration, stacking,_, _, geEntity) in SystemAPI
                         .Query<RefRW<CDuration>,RefRW<CStacking>, RefRO<CValidEffect>, RefRO<CInUsage>>()
                         .WithEntityAccess())
            {
                // 过滤：
                // 1.持续时间无限的GE
                // 2.未激活的GE
                if (duration.ValueRO.duration <= 0 || !duration.ValueRO.active) continue;

                var durRO = duration.ValueRO;
                var countTime = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                bool expired;
                if (duration.ValueRO.StopTickWhenDeactivated)
                    expired = countTime - durRO.lastActiveTime >= durRO.remianTime;
                else
                    expired = countTime - durRO.activeTime >= durRO.duration;

                if (expired)
                {
                    // 根据Stacking的配置类型，决定过期逻辑
                    if(stacking.ValueRO.expirationPolicy == ExpirationPolicy.ClearEntireStack)
                    {   // 清除整个Stack，相当于直接销毁
                        ecb.RemoveComponent<CValidEffect>(geEntity);
                        ecb.AddComponent<CDestroy>(geEntity);
                    }
                    else if(stacking.ValueRO.expirationPolicy == ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                    {
                        // 1.移除一层stack
                        SCheckStacking.TryChangeStackCount(
                            state.EntityManager,
                            geEntity,
                            stacking.ValueRO,
                            stacking.ValueRO.StackCount - 1,
                            duration,
                            globalFrameTimer.ValueRO);
                        // 2.刷新持续时间
                        SActivateEffect.UpdateActiveTime(ref duration.ValueRW, globalFrameTimer.ValueRO);
                    }
                    else if(stacking.ValueRO.expirationPolicy == ExpirationPolicy.RefreshDuration)
                    {
                        // 刷新持续时间
                        SActivateEffect.UpdateActiveTime(ref duration.ValueRW, globalFrameTimer.ValueRO);
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
    }
}