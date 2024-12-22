using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    /// <summary>
    /// 所有GE需要初始化的Runtime数据，组件等，都在这个系统内完成
    /// </summary>
    
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    public partial struct SysInitEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComInApplicationProgress>();
            state.RequireForUpdate<ComValidEffect>();
            state.RequireForUpdate<ComInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (inUsage,_,_,ge) in 
                     SystemAPI.Query<RefRW<ComInUsage>,RefRO<ComInApplicationProgress>,RefRO<ComValidEffect>>().WithNone<ComDuration>().WithEntityAccess())
            {
                // TODO 初始化
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        
        
        
        
        
        // public static void InitGameplayEffect(this Entity gameplayEffect, Entity source, Entity target, int level)
        // {
        //     if (!_entityManager.HasComponent<ComInUsage>(gameplayEffect)) return;
        //
        //     _entityManager.SetComponentData(gameplayEffect,
        //         new ComInUsage { Source = source, Target = target, Level = level });
        //
        //     if (_entityManager.HasComponent<ComDuration>(gameplayEffect))
        //         if (_entityManager.HasComponent<ComPeriod>(gameplayEffect))
        //         {
        //             var period = _entityManager.GetComponentData<ComPeriod>(gameplayEffect);
        //             var periodGEs = period.GameplayEffects;
        //             foreach (var ge in periodGEs)
        //                 ge.InitGameplayEffect(source, target, level);
        //         }
        //     
        //     // TODO 
        //     // SetGrantedAbility(GameplayEffect.GrantedAbilities);
        // }
    }
}