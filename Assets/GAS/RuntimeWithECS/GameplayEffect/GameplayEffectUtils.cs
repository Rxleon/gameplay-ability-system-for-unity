﻿using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public static class GameplayEffectUtils
    {
        private static EntityManager GasEntityManager => GASManager.EntityManager;

        public static DynamicBuffer<GameplayEffectBufferElement> GameplayEffectsOf(Entity asc)
        {
            return GasEntityManager.GetBuffer<GameplayEffectBufferElement>(asc);
        }

        public static bool CheckAscAttributeDirty(DynamicBuffer<AttributeSetBufferElement> attrSets,DynamicBuffer<BuffEleModifier> modifiers)
        {
            var isDirty = false;
            foreach (var modifier in modifiers)
            {
                int attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                if(attrSetIndex==-1) continue;
                    
                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                int attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                if(attrIndex==-1) continue;
                
                isDirty = true;
                
                var attr = attributes[attrIndex];
                if(attr.Dirty) continue;
                
                attr.Dirty = true;
                attrSet.Attributes[attrIndex] = attr;
                attrSets[attrSetIndex] = attrSet;
            }
            return isDirty;
        }
        

    }
}