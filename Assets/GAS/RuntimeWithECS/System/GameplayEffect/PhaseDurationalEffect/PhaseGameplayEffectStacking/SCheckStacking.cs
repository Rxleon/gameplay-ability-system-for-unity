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
                bool isNewGe = IsNewAddedGameplayEffect(state.EntityManager, inUsage.ValueRO.Target, ge);
                if (!isNewGe)
                {
                    // 1.叠加的GE不走Activate流程，并且直接销毁
                    ecb.RemoveComponent<CInApplicationProgress>(ge);
                    ecb.RemoveComponent<CValidEffect>(ge);
                    ecb.AddComponent<CDestroy>(ge);
                    
                    // 2.尝试更新Stack层数，触发OnStackCountChanged事件
                    
                    // 3.如果层数改变，额外触发OnGameplayEffectContainerIsDirty
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
        /// 判断是否是新添加的GameplayEffect
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="owner"></param>
        /// <param name="ge"></param>
        /// <returns></returns>
        private bool IsNewAddedGameplayEffect(EntityManager entityManager, Entity owner, Entity ge)
        {
            // var effects = entityManager.GetBuffer<BuffEleGameplayEffect>(owner);
            // for (var i = 0; i < effects.Length; i++)
            // {
            //     if (effects[i].GameplayEffect == ge)
            //     {
            //         return false;
            //     }
            // }

            return true;
        }
    }
}