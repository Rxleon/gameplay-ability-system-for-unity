using GAS.EditorForECS.GameplayEffect;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public static class GEUtil
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        public static Entity CreateGameplayEffectEntity(NewGameplayEffectAsset asset)
        {
            return CreateGameplayEffectEntity(asset.components);
        }

        public static Entity CreateGameplayEffectEntity(GameplayEffectComponentConfig[] componentAssets)
        {
            var entity = _entityManager.CreateEntity();

            foreach (var config in componentAssets) 
                config.LoadToGameplayEffectEntity(entity);
            return entity;
        }

        public static NewGameplayEffectSpec CreateGameplayEffectSpec(NewGameplayEffectAsset asset)
        {
            return CreateGameplayEffectSpec(asset.components);
        }
        
        public static NewGameplayEffectSpec CreateGameplayEffectSpec(GameplayEffectComponentConfig[] componentAssets)
        {
            return new NewGameplayEffectSpec(componentAssets);
        }

        public static void ApplyGameplayEffectTo(Entity gameplayEffect, Entity target, Entity source)
        {
            _entityManager.AddComponent<ComNeedInit>(gameplayEffect);
            _entityManager.AddComponent<ComInUsage>(gameplayEffect);
            _entityManager.AddComponent<NeedCheckEffects>(target);
            var comInUsage = _entityManager.GetComponentData<ComInUsage>(gameplayEffect);
            comInUsage.Source = source;
            comInUsage.Target = target;
            _entityManager.SetComponentEnabled<ComInUsage>(gameplayEffect,true);
            _entityManager.SetComponentData(gameplayEffect,comInUsage);
            
            var geBuffers = GameplayEffectUtils.GameplayEffectsOf(target);
            geBuffers.Add(new GameplayEffectBufferElement { GameplayEffect = gameplayEffect });
        }
        
        /// <summary>
        /// 检测应用标签
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool CheckApplicationRequiredTags(this Entity gameplayEffect, Entity asc)
        {
            if (!_entityManager.HasComponent<ComApplicationRequiredTags>(gameplayEffect)) return true;
            
            var requiredTags = _entityManager.GetComponentData<ComApplicationRequiredTags>(gameplayEffect);

            var fixedTags = _entityManager.GetBuffer<BuffElemFixedTag>(asc);
            var tempTags = _entityManager.GetBuffer<BuffElemTemporaryTag>(asc);
                
            foreach (var tag in requiredTags.tags)
            {
                bool hasTag = false;
                
                foreach (var fixedTag in fixedTags)
                    if (GameplayTagHub.HasTag(fixedTag.tag, tag))
                    {
                        hasTag = true;
                        break;
                    }
                
                if (!hasTag)
                    foreach (var tempTag in tempTags)
                        if (GameplayTagHub.HasTag(tempTag.tag, tag))
                        {
                            hasTag = true;
                            break;
                        }

                if (!hasTag) return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 检测免疫标签
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool CheckImmunityTags(this Entity gameplayEffect, Entity asc)
        {
            if (!_entityManager.HasComponent<ComImmunityTags>(gameplayEffect)) return false;
            
            var immunityTags = _entityManager.GetComponentData<ComImmunityTags>(gameplayEffect);
            var fixedTags = _entityManager.GetBuffer<BuffElemFixedTag>(asc);
            var tempTags = _entityManager.GetBuffer<BuffElemTemporaryTag>(asc);
                
            foreach (var tag in immunityTags.tags)
            {
                foreach (var fixedTag in fixedTags)
                    if (GameplayTagHub.HasTag(fixedTag.tag, tag))
                        return true;
                
                foreach (var tempTag in tempTags)
                    if (GameplayTagHub.HasTag(tempTag.tag, tag))
                        return true;

                return false;
            }
            
            return false;
        }

        public static void InitGameplayEffect(this Entity gameplayEffect ,Entity source, Entity target, int level)
        {
            if (!_entityManager.HasComponent<ComInUsage>(gameplayEffect)) return;
      
            _entityManager.SetComponentData(gameplayEffect, new ComInUsage { Source = source, Target = target ,Level = level});

            if (_entityManager.HasComponent<ComDuration>(gameplayEffect))
            {
                if (_entityManager.HasComponent<ComPeriod>(gameplayEffect))
                {
                    var period = _entityManager.GetComponentData<ComPeriod>(gameplayEffect);
                    var periodGEs = period.GameplayEffects;
                    foreach (var ge in periodGEs)
                        ge.InitGameplayEffect(source, target, level);
                }
                
                // TODO 
                // SetGrantedAbility(GameplayEffect.GrantedAbilities);
            }
        }
        
        public static void TriggerOnExecute(this Entity gameplayEffect)
        {
            if (!_entityManager.HasComponent<ComInUsage>(gameplayEffect)) return;

            var inUsage = _entityManager.GetComponentData<ComInUsage>(gameplayEffect);

            if (_entityManager.HasComponent<ComRemoveEffectWithTags>(gameplayEffect))
            {
                var comRemoveEffectWithTags = _entityManager.GetComponentData<ComRemoveEffectWithTags>(gameplayEffect);
                var tags = comRemoveEffectWithTags.tags;
                inUsage.Target.RemoveGameplayEffectWithAnyTags(tags);
                // Owner.GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(GameplayEffect.TagContainer
                //     .RemoveGameplayEffectsWithTags);
            }
           
            
            Owner.ApplyModFromInstantGameplayEffect(this);
            
            // TODO
            // TriggerCueOnExecute();
        }
    }
}