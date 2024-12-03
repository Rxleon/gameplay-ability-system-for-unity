using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupTryApplyEffect))]
    [UpdateAfter(typeof(SysCheckApplicationCondition))]
    public partial struct SysCheckApplicationRequiredTag : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<ComApplicationRequiredTags>();
            state.RequireForUpdate<ComInUsage>();
            state.RequireForUpdate<ComValidEffect>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (requiredTags, comInUsage, _,ge) in SystemAPI
                         .Query<RefRO<ComApplicationRequiredTags>, RefRW<ComInUsage>,RefRO<ComValidEffect>>().WithEntityAccess())
            {
                var asc = comInUsage.ValueRO.Target;
                var fixedTags = SystemAPI.GetBuffer<BuffElemFixedTag>(asc);
                var tempTags = SystemAPI.GetBuffer<BuffElemTemporaryTag>(asc);

                foreach (var tag in requiredTags.ValueRO.tags)
                {
                    var hasTag = false;
                    // 遍历固有Tag
                    foreach (var fixedTag in fixedTags)
                        if (tagMap.IsTagAIncludeTagB(fixedTag.tag, tag))
                        {
                            hasTag = true;
                            break;
                        }

                    // 遍历临时Tag
                    if (!hasTag)
                        foreach (var tempTag in tempTags)
                            if (tagMap.IsTagAIncludeTagB(tempTag.tag, tag))
                            {
                                hasTag = true;
                                break;
                            }

                    if (!hasTag)
                    {
                        ecb.RemoveComponent<ComValidEffect>(ge);
                        ecb.AddComponent<ComDestroy>(ge);
                        break;
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