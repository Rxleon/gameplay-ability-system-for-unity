﻿using GAS.EditorForECS.GameplayEffect;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Profiling;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public static class GEUtil
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        /// <summary>
        /// 立即应用GameplayEffect
        /// 所有派生的GameplayEffect，以及自定义逻辑中，都使用该接口来实现GameplayEffect的应用
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void ApplyGameplayEffectImmediate(Entity gameplayEffect, Entity target, Entity source)
        {
           
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
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
            _entityManager.AddComponent<CInApplicationProgress>(gameplayEffect);
            _entityManager.AddComponent<CInUsage>(gameplayEffect);
            _entityManager.AddComponent<CValidEffect>(gameplayEffect);

            var comInUsage = _entityManager.GetComponentData<CInUsage>(gameplayEffect);
            comInUsage.Source = source;
            comInUsage.Target = target;
            _entityManager.SetComponentData(gameplayEffect, comInUsage);
            
            // var geBuffers = GameplayEffectUtils.GameplayEffectsOf(target);
            // geBuffers.Add(new BuffEleGameplayEffect { GameplayEffect = gameplayEffect });
        }

        /// <summary>
        ///     检测应用标签
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool CheckApplicationRequiredTags(this Entity gameplayEffect, Entity asc)
        {
            if (!_entityManager.HasComponent<CApplicationRequiredTags>(gameplayEffect)) return true;
            var requiredTags = _entityManager.GetComponentData<CApplicationRequiredTags>(gameplayEffect);
            return asc.CheckAscHasAllTags(requiredTags.tags);
        }

        /// <summary>
        ///     检测激活标签
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool CheckOngoingRequiredTags(this Entity gameplayEffect, Entity asc)
        {
            if (!_entityManager.HasComponent<COngoingRequiredTags>(gameplayEffect)) return true;
            var requiredTags = _entityManager.GetComponentData<COngoingRequiredTags>(gameplayEffect);
            return asc.CheckAscHasAllTags(requiredTags.tags);

        }

        /// <summary>
        ///     检测免疫标签
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool CheckImmunityTags(this Entity gameplayEffect, Entity asc)
        {
            if (!_entityManager.HasComponent<CImmunityTags>(gameplayEffect)) return false;
            var immunityTags = _entityManager.GetComponentData<CImmunityTags>(gameplayEffect);
            return asc.CheckAscHasAnyTags(immunityTags.tags);
        }

        public static void InitGameplayEffect(this Entity gameplayEffect, Entity source, Entity target, int level)
        {
            if (!_entityManager.HasComponent<CInUsage>(gameplayEffect)) return;

            _entityManager.SetComponentData(gameplayEffect,
                new CInUsage { Source = source, Target = target, Level = level });

            if (_entityManager.HasComponent<CDuration>(gameplayEffect))
                if (_entityManager.HasComponent<CPeriod>(gameplayEffect))
                {
                    var period = _entityManager.GetComponentData<CPeriod>(gameplayEffect);
                    var periodGEs = period.GameplayEffects;
                    foreach (var ge in periodGEs)
                        ge.InitGameplayEffect(source, target, level);
                }
            
            // TODO 
            // SetGrantedAbility(GameplayEffect.GrantedAbilities);
        }

        public static void TriggerOnExecute(this Entity gameplayEffect)
        {
            if (!_entityManager.HasComponent<CInUsage>(gameplayEffect)) return;

            var inUsage = _entityManager.GetComponentData<CInUsage>(gameplayEffect);
            var owner = inUsage.Target;
            // 1.移除GameplayEffectWithAnyTags
            owner.RemoveGameplayEffectWithAnyTags(gameplayEffect);

            // 2。应用Modifiers
            owner.ApplyModFromInstantGameplayEffect(gameplayEffect);

            // TODO
            // 3.触发Cue
            // TriggerCueOnExecute();
        }

        public static bool CheckEffectHasAnyTags(this Entity gameplayEffect, NativeArray<int> tags)
        {
            // 1.判断AssetTags
            if (_entityManager.HasComponent<CAssetTags>(gameplayEffect))
            {
                var assetTags = _entityManager.GetComponentData<CAssetTags>(gameplayEffect).tags;

                foreach (var assetTag in assetTags)
                foreach (var tag in tags)
                    if (GameplayTagHub.HasTag(assetTag, tag))
                        return true;
            }

            //2.判断GrantedTags
            if (_entityManager.HasComponent<CGrantedTags>(gameplayEffect))
            {
                var grantedTags = _entityManager.GetComponentData<CGrantedTags>(gameplayEffect).tags;
                foreach (var grantedTag in grantedTags)
                foreach (var tag in tags)
                    if (GameplayTagHub.HasTag(grantedTag, tag))
                        return true;
            }

            return false;
        }

        public static bool CheckEffectHasAnyTags(this Entity gameplayEffect,  SingletonGameplayTagMap singletonGameplayTagMap ,EntityManager entityManager ,NativeArray<int> tags)
        {
            // 1.判断AssetTags
            if (entityManager.HasComponent<CAssetTags>(gameplayEffect))
            {
                var assetTags = entityManager.GetComponentData<CAssetTags>(gameplayEffect).tags;

                foreach (var assetTag in assetTags)
                foreach (var tag in tags)
                    if (singletonGameplayTagMap.IsTagAIncludeTagB(assetTag, tag))
                        return true;
            }

            //2.判断GrantedTags
            if (entityManager.HasComponent<CGrantedTags>(gameplayEffect))
            {
                var grantedTags = entityManager.GetComponentData<CGrantedTags>(gameplayEffect).tags;
                foreach (var grantedTag in grantedTags)
                foreach (var tag in tags)
                    if (singletonGameplayTagMap.IsTagAIncludeTagB(grantedTag, tag))
                        return true;
            }

            return false;
        }
        
        public static void EffectApply(this Entity gameplayEffect)
        {
            if (_entityManager.HasComponent<CValidEffect>(gameplayEffect)) return;
            _entityManager.AddComponent<CValidEffect>(gameplayEffect);
            
            // 校验是否可激活
            var owner = _entityManager.GetComponentData<CInUsage>(gameplayEffect).Target;
            if (gameplayEffect.CheckOngoingRequiredTags(owner))
                gameplayEffect.EffectActivate();
        }
        
        public static void EffectActivate(this Entity gameplayEffect)
        {
            var comDuration = _entityManager.GetComponentData<CDuration>(gameplayEffect);
            if (comDuration.active) return;
            comDuration.active = true;
            
            // 1. 更新激活时间
            var globalFrameTimer = _entityManager.GetComponentData<GlobalTimer>(GASManager.EntityGlobalTimer);
            var currentFrame = globalFrameTimer.Frame;
            var currentTurn = globalFrameTimer.Turn;
            
            if(comDuration.timeUnit == TimeUnit.Frame)
            {
                if (comDuration.activeTime == 0 || comDuration.ResetStartTimeWhenActivated)
                    comDuration.activeTime = currentFrame;
                    
                comDuration.lastActiveTime = currentFrame;
            }
            else
            {
                if (comDuration.activeTime == 0 || comDuration.ResetStartTimeWhenActivated)
                    comDuration.activeTime = currentTurn;
                    
                comDuration.lastActiveTime = currentTurn;
            }
            
            _entityManager.SetComponentData(gameplayEffect,comDuration);
            
            // TODO 触发OnActivation的Cue
            // TriggerOnActivation();
        }
        
        public static void EffectDeactivate(this Entity gameplayEffect)
        {
            
        }

        #region TriggerCue

        public static void TriggerCueOnAdd(this Entity gameplayEffect)
        {
            
        }

        public static void TriggerCueOnRemove(this Entity gameplayEffect)
        {
            
        }

        public static void TriggerCueOnExecute(this Entity gameplayEffect)
        {
            
        }

        public static void TriggerCueOnActivation(this Entity gameplayEffect)
        {
            
        }
        #endregion
    }
}