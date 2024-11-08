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
            var geBuff = _entityManager.GetBuffer<GameplayEffectBufferElement>(asc);
            for (var i = geBuff.Length - 1; i >= 0; i--)
            {
                var ge = geBuff[i].GameplayEffect;
                if (SystemAPI.IsComponentEnabled<ComInUsage>(ge)) continue;
                geBuff.RemoveAt(i);
                // 含有子实例的组件也要清理
                if (_entityManager.HasComponent<ComPeriod>(ge))
                {
                    var period = _entityManager.GetComponentData<ComPeriod>(ge);
                    foreach (var sonGe in period.GameplayEffects)
                        _entityManager.DestroyEntity(sonGe);
                }
                _entityManager.DestroyEntity(ge);
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