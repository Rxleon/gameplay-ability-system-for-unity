﻿using System;
using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AbilitySystemCell
{
    public static class ASCUtil
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        public static void RemoveGameplayEffectWithAnyTags(this Entity asc, Entity gameplayEffect)
        {
            if (!_entityManager.HasComponent<ComRemoveEffectWithTags>(gameplayEffect)) return;

            var comRemoveEffectWithTags = _entityManager.GetComponentData<ComRemoveEffectWithTags>(gameplayEffect);
            var removeEffectWithTags = comRemoveEffectWithTags.tags;
            if (removeEffectWithTags.Length == 0) return;

            var geBuff = _entityManager.GetBuffer<GameplayEffectBufferElement>(asc);
            for (var i = geBuff.Length - 1; i >= 0; i--)
            {
                var ge = geBuff[i].GameplayEffect;
                var hasRemoveTag = ge.CheckEffectHasAnyTags(removeEffectWithTags);
                if (!hasRemoveTag) continue;
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
        }

        public static void ApplyModFromInstantGameplayEffect(this Entity asc, Entity gameplayEffect)
        {
            var attrSets = _entityManager.GetBuffer<AttributeSetBufferElement>(asc);
            var modifiers = _entityManager.GetBuffer<BuffEleModifier>(gameplayEffect);
            foreach (var mod in modifiers)
            {
                var magnitude = MmcHub.Calculate(gameplayEffect, mod);

                int attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
                if (attrSetIndex == -1) continue;

                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                int attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
                if (attrIndex == -1) continue;

                var attr = attributes[attrIndex];
                var oldValue = attr.BaseValue;
                var newValue = oldValue;
                switch (mod.Operation)
                {
                    case GEOperation.Add:
                        newValue += magnitude;
                        break;
                    case GEOperation.Minus:
                        newValue -= magnitude;
                        break;
                    case GEOperation.Multiply:
                        newValue *= magnitude;
                        break;
                    case GEOperation.Divide:
                        newValue /= magnitude;
                        break;
                    case GEOperation.Override:
                        newValue = magnitude;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                // OnChangeBefore
                // BaseValue 不做钳制，因为Max，Min是只针对Current Value
                newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(asc,mod.AttrSetCode,mod.AttrCode,newValue);
                
                attr.BaseValue = newValue;
                
                // OnChangeAfter
                if (newValue != oldValue)
                {
                    // BaseValue 改变，需要标记Dirty
                    attr.Dirty = true;
                    GASManager.EntityManager.AddComponent<AttributeIsDirty>(asc);
                    GASEventCenter.InvokeOnBaseValueChangeAfter(asc,mod.AttrSetCode,mod.AttrCode,oldValue,newValue);
                }
                
                attrSet.Attributes[attrIndex] = attr;
                attrSets[attrSetIndex] = attrSet;
            }
        }
        
        public static bool CheckAscHasAllTags(this Entity asc, NativeArray<int> tags)
        {
            var fixedTags = _entityManager.GetBuffer<BuffElemFixedTag>(asc);
            var tempTags = _entityManager.GetBuffer<BuffElemTemporaryTag>(asc);

            foreach (var tag in tags)
            {
                var hasTag = false;

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
        
        public static bool CheckAscHasAnyTags(this Entity asc, NativeArray<int> tags)
        {
            var fixedTags = _entityManager.GetBuffer<BuffElemFixedTag>(asc);
            var tempTags = _entityManager.GetBuffer<BuffElemTemporaryTag>(asc);

            foreach (var tag in tags)
            {
                foreach (var fixedTag in fixedTags)
                    if (GameplayTagHub.HasTag(fixedTag.tag, tag))
                        return true;

                foreach (var tempTag in tempTags)
                    if (GameplayTagHub.HasTag(tempTag.tag, tag))
                        return true;
            }

            return false;
        }
    }
}