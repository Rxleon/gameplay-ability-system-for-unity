using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupActivateEffect))]
    [UpdateBefore(typeof(SysActivateEnd))]
    public partial struct SysActivateEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<CInApplicationProgress>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;
            //  更新激活时间
            foreach (var (_, duration) in SystemAPI.Query<RefRO<CInActivationProgress>, RefRW<CDuration>>())
            {
                // 过滤已经激活的GE
                if (duration.ValueRO.active) continue;
                
                duration.ValueRW.active = true;
                if (duration.ValueRO.timeUnit == TimeUnit.Frame)
                {
                    if (duration.ValueRO.activeTime == 0 || duration.ValueRO.ResetStartTimeWhenActivated)
                        duration.ValueRW.activeTime = currentFrame;

                    duration.ValueRW.lastActiveTime = currentFrame;
                }
                else
                {
                    if (duration.ValueRO.activeTime == 0 || duration.ValueRO.ResetStartTimeWhenActivated)
                        duration.ValueRW.activeTime = currentTurn;

                    duration.ValueRW.lastActiveTime = currentTurn;
                }}

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}