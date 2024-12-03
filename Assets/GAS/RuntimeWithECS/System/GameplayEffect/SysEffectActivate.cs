using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateBefore(typeof(SysEffectDurationTicker))]
    [UpdateAfter(typeof(SysCheckEffectOngoingActive))]
    public partial struct SysEffectActivate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComNeedActivate>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            //
            // var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            // var currentFrame = globalFrameTimer.ValueRO.Frame;
            // var currentTurn = globalFrameTimer.ValueRO.Turn;
            //
            // foreach (var (_,duration,ge) in SystemAPI.Query<RefRO<ComNeedActivate>,RefRW<ComDuration>>().WithEntityAccess())
            // {
            //     // TODO 激活GE的对应逻辑
            //     
            //     // 1. 更新激活时间
            //     if(duration.ValueRO.timeUnit == TimeUnit.Frame)
            //     {
            //         if (duration.ValueRO.activeTime == 0 || duration.ValueRO.ResetStartTimeWhenActivated)
            //             duration.ValueRW.activeTime = currentFrame;
            //         
            //         duration.ValueRW.lastActiveTime = currentFrame;
            //     }
            //     else
            //     {
            //         if (duration.ValueRO.activeTime == 0 || duration.ValueRO.ResetStartTimeWhenActivated)
            //             duration.ValueRW.activeTime = currentTurn;
            //         
            //         duration.ValueRW.lastActiveTime = currentTurn;
            //     }
            //
            //     // 2. 触发 激活Cue
            //     // TriggerOnActivation();
            //     
            //     // 3. 标记所有受影响的Attribute为dirty
            //     var inUsage = SystemAPI.GetComponentRO<ComInUsage>(ge);
            //     var asc = inUsage.ValueRO.Target;
            //     var isDirty = GameplayEffectUtils.CheckAscAttributeDirty(
            //         SystemAPI.GetBuffer<AttributeSetBufferElement>(asc),
            //         SystemAPI.GetBuffer<BuffEleModifier>(ge));
            //     if (isDirty) ecb.AddComponent<AttributeIsDirty>(asc);
            //     
            //     // 完成任务后删除执行标签
            //     ecb.RemoveComponent<ComNeedActivate>(ge);
            // }
            //
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}