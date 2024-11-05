using GAS.Runtime;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public static class GameplayEffectApplicator
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        
        /// <summary>
        /// 1.判断GE Application Condition是否生效
        /// 2.判断ApplicationRequiredTags
        /// 3.判断ImmunityTags【触发Immunity Cue】
        /// </summary>
        public static Entity TryApplyGameplayEffectTo(Entity gameplayEffect,Entity source,Entity target)
        {
            // 1. GE Application Condition
            if (!IsConditionSatisfied()) return Entity.Null;
            
            // 2. ApplicationRequiredTags
            if(!gameplayEffect.CheckApplicationRequiredTags(target)) return Entity.Null;

            // 3. ImmunityTags
            if (gameplayEffect.CheckImmunityTags(target))
            {
                // TODO 免疫Cue触发
                return Entity.Null;
            }
            
            // 4. 设置source，target
            _entityManager.SetComponentData(gameplayEffect, new ComInUsage { Source = source, Target = target });
            
            // 5. 根据ge类型处理
            bool hasDuration = _entityManager.HasComponent<ComDuration>(gameplayEffect);
            if (hasDuration)
            {
                // Durational GE
                //
            }
            else
            {
                // Instant GE
                // effectSpec.Init(source, _owner, level);
                // effectSpec.TriggerOnExecute();
                return Entity.Null;
            }
            // var level = overwriteEffectLevel ? effectLevel : source.Level;
            // if (effectSpec.DurationPolicy == EffectsDurationPolicy.Instant)
            // {
            //     effectSpec.Init(source, _owner, level);
            //     effectSpec.TriggerOnExecute();
            //     return null;
            // }
            //
            // // Check GE Stacking
            // if (effectSpec.Stacking.stackingType == StackingType.None)
            // {
            //     return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
            // }
            //
            // // 处理GE堆叠
            // // 基于Target类型GE堆叠
            // if (effectSpec.Stacking.stackingType == StackingType.AggregateByTarget)
            // {
            //     GetStackingEffectSpecByData(effectSpec.GameplayEffect, out var geSpec);
            //     // 新添加GE
            //     if (geSpec == null)
            //         return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
            //     bool stackCountChange = geSpec.RefreshStack();
            //     if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
            //     return geSpec;
            // }
            //
            // // 基于Source类型GE堆叠
            // if (effectSpec.Stacking.stackingType == StackingType.AggregateBySource)
            // {
            //     GetStackingEffectSpecByDataFrom(effectSpec.GameplayEffect,source, out var geSpec);
            //     if (geSpec == null)
            //         return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
            //     bool stackCountChange = geSpec.RefreshStack();
            //     if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
            //     return geSpec;
            // }

            return Entity.Null;
        }
        
        private static bool IsConditionSatisfied()
        {
            return true;
        }

        
        private static void ApplyInstantGameplayEffect()
        {
            // TODO
        }
        
        private static void ApplyInstantDurationEffect()
        {
            // TODO
        }
    }
}