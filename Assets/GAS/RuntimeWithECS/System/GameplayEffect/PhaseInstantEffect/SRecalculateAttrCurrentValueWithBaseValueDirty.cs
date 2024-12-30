using GAS.RuntimeWithECS.Attribute;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    [UpdateAfter(typeof(SysInstantEffectModifyBaseValue))]
    [UpdateBefore(typeof(SysInstantEffectOver))]
    public partial struct SRecalculateAttrCurrentValueWithBaseValueDirty : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CAttributeIsDirty>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, attrSets, asc) in SystemAPI
                         .Query<RefRO<CAttributeIsDirty>, DynamicBuffer<AttributeSetBufferElement>>()
                         .WithEntityAccess())
                for (var index = 0; index < attrSets.Length; index++)
                {
                    var attrSet = attrSets[index];
                    for (var i = 0; i < attrSet.Attributes.Length; i++)
                    {
                        var attr = attrSet.Attributes[i];
                        if (!attr.Dirty) continue;

                        attr.Dirty = false;

                        var oldValue = attr.CurrentValue;
                        var newValue = AttributeTool.RecalculateCurrentValue(asc, attrSet.Code, attr.Code);
                        attr.CurrentValue = newValue;

                        // OnChangeAfter
                        if (newValue != oldValue)
                            GASEventCenter.InvokeOnCurrentValueChangeAfter(asc, attrSet.Code, attr.Code, oldValue,
                                newValue);
                    }
                }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}