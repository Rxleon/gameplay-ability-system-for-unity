using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.GameplayEffect.PhaseLogicTick;
using GAS.RuntimeWithECS.System.SystemGroup.LogicTick;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupTickGameplayEffect))]
    [UpdateBefore(typeof(SEffectDurationTick))]
    public partial struct SEffectPeriodTicker : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CPeriod>();
            state.RequireForUpdate<CValidEffect>();
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<CInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;
            
            foreach (var (duration, inUsage, period,_, geEntity) in SystemAPI
                         .Query<RefRO<CDuration>, RefRO<CInUsage>, RefRW<CPeriod>,RefRO<CValidEffect>>()
                         .WithNone<CInApplicationProgress>()
                         .WithEntityAccess())
            {
                // 过滤未激活的GE
                if (!duration.ValueRO.active) continue;
            
                var time = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                if (period.ValueRO.StartTime == 0) period.ValueRW.StartTime = time - 1 < 0 ? 0 : time;
            
                if (time - period.ValueRO.StartTime >= period.ValueRO.Period)
                {
                    period.ValueRW.StartTime = time;
                    foreach (var ge in period.ValueRO.GameplayEffects)
                    {
                        // 实例化GE
                        var instanceGe = state.EntityManager.Instantiate(ge);
                        
                        // GasQueueCenter.AddEffectWaitingForApplication(instanceGe, inUsage.ValueRO.Target,
                        //     inUsage.ValueRO.Target);
                    }
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}