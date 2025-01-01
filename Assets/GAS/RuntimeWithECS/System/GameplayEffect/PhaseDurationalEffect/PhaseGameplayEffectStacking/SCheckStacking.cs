using GAS.RuntimeWithECS.Common.Component;
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
            state.RequireForUpdate<CInApplicationProgress>();
            state.RequireForUpdate<CStacking>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (inUsage, _, stacking, ge) in SystemAPI
                         .Query<RefRO<CInUsage>, RefRO<CInActivationProgress>, RefRO<CStacking>>()
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
                    var stackCountChanged =
                        TryChangeStackCount(state.EntityManager, stackingGe, 0); // stacking.ValueRO.StackCount);

                    // 3.如果层数改变，额外触发OnGameplayEffectContainerIsDirty
                    if (stackCountChanged)
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

        private bool TryChangeStackCount(EntityManager entityManager, Entity ge, int stackCount)
        {
            // var stackCountComp = entityManager.GetComponentData<CStackCount>(ge);
            // if (stackCountComp.StackCount != stackCount)
            // {
            //     stackCountComp.StackCount = stackCount;
            //     entityManager.SetComponentData(ge, stackCountComp);
            // }
            return true;
        }
    }
}