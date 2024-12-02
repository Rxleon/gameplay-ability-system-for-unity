using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    [UpdateAfter(typeof(SysRemoveEffectWithTags))]
    public partial struct SysInstantEffectModifyBaseValue : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuffEleModifier>();
            state.RequireForUpdate<ComInApplicationProgress>();
            state.RequireForUpdate<ComValidEffect>();
            state.RequireForUpdate<ComInUsage>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (modifiers,_,_,_, geEntity) in SystemAPI
                         .Query<DynamicBuffer<BuffEleModifier>,RefRO<ComInApplicationProgress>,RefRO<ComValidEffect>, RefRO<ComInUsage>>()
                         .WithNone<ComDuration>()
                         .WithEntityAccess())
            {
                var asc = SystemAPI.GetComponentRO<ComInUsage>(geEntity).ValueRO.Target;
                var attrSets = SystemAPI.GetBuffer<AttributeSetBufferElement>(asc);
            
                foreach (var mod in modifiers)
                {
                    int attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
                    if(attrSetIndex==-1) continue;
                    
                    var attrSet = attrSets[attrSetIndex];
                    var attributes = attrSet.Attributes;
            
                    int attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
                    if(attrIndex==-1) continue;
                    
                    var data = attributes[attrIndex];
                    var baseValue = MmcHub.Calculate(geEntity, mod, data.BaseValue);
                    // 加入base value 更新队列
                    GasQueueCenter.AddBaseValueUpdateInfo(asc,mod.AttrSetCode,mod.AttrCode,baseValue);
                    
                    
                    // data.TriggerCueEvent = true;
                    // data.Dirty = true;
                    // ecb.AddComponent<ComAttributeDirty>(geEntity);
                    //
                    // attrSet.Attributes[attrIndex] = data;
                    // attrSets[attrSetIndex] = attrSet;
                    
                    // 应用完成的Instant GE，使其不合法
                    ecb.SetComponentEnabled<ComInUsage>(geEntity, false);
                }
            
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        
        // public static void ApplyModFromInstantGameplayEffect(this Entity asc, Entity gameplayEffect)
        // {
        //     var attrSets = _entityManager.GetBuffer<AttributeSetBufferElement>(asc);
        //     var modifiers = _entityManager.GetBuffer<BuffEleModifier>(gameplayEffect);
        //     foreach (var mod in modifiers)
        //     {
        //         var magnitude = MmcHub.Calculate(gameplayEffect, mod);
        //
        //         var attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
        //         if (attrSetIndex == -1) continue;
        //
        //         var attrSet = attrSets[attrSetIndex];
        //         var attributes = attrSet.Attributes;
        //
        //         var attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
        //         if (attrIndex == -1) continue;
        //
        //         var attr = attributes[attrIndex];
        //         var oldValue = attr.BaseValue;
        //         var newValue = oldValue;
        //         switch (mod.Operation)
        //         {
        //             case GEOperation.Add:
        //                 newValue += magnitude;
        //                 break;
        //             case GEOperation.Minus:
        //                 newValue -= magnitude;
        //                 break;
        //             case GEOperation.Multiply:
        //                 newValue *= magnitude;
        //                 break;
        //             case GEOperation.Divide:
        //                 newValue /= magnitude;
        //                 break;
        //             case GEOperation.Override:
        //                 newValue = magnitude;
        //                 break;
        //             default:
        //                 throw new ArgumentOutOfRangeException();
        //         }
        //
        //         // OnChangeBefore
        //         // BaseValue 不做钳制，因为Max，Min是只针对Current Value
        //         newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(asc, mod.AttrSetCode, mod.AttrCode, newValue);
        //
        //         attr.BaseValue = newValue;
        //
        //         // OnChangeAfter
        //         if (newValue != oldValue)
        //         {
        //             // BaseValue 改变，需要标记Dirty
        //             attr.Dirty = true;
        //             GASManager.EntityManager.AddComponent<AttributeIsDirty>(asc);
        //             GASEventCenter.InvokeOnBaseValueChangeAfter(asc, mod.AttrSetCode, mod.AttrCode, oldValue, newValue);
        //         }
        //
        //         attrSet.Attributes[attrIndex] = attr;
        //         attrSets[attrSetIndex] = attrSet;
        //     }
        // }
    }
}