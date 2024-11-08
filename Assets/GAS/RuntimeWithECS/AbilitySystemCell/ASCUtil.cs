using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AbilitySystemCell
{
    public static class ASCUtil
    {
        private static EntityManager _entityManager => GASManager.EntityManager;
        
        public static void RemoveGameplayEffectWithAnyTags(this Entity asc, NativeArray<int> tags)
        {
            if (tags.Length==0) return;

            // TODO
            var gameplayEffects = _entityManager.GetBuffer<GameplayEffectBufferElement>(asc);
            for (int i = 0; i < gameplayEffects.Length; i++)
            {
                gameplayEffects.RemoveAt();
            }
            
            // var removeList = new List<GameplayEffectSpec>();
            // foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
            // {
            //     var assetTags = gameplayEffectSpec.GameplayEffect.TagContainer.AssetTags;
            //     if (!assetTags.Empty && assetTags.HasAnyTags(tags))
            //     {
            //         removeList.Add(gameplayEffectSpec);
            //         continue;
            //     }
            //
            //     var grantedTags = gameplayEffectSpec.GameplayEffect.TagContainer.GrantedTags;
            //     if (!grantedTags.Empty && grantedTags.HasAnyTags(tags)) removeList.Add(gameplayEffectSpec);
            // }
            //
            // foreach (var gameplayEffectSpec in removeList) RemoveGameplayEffectSpec(gameplayEffectSpec);
        }
    }
}