using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Aspect;
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
            foreach (var aspect in SystemAPI.Query<AspModifyBaseValue>())
            {
                aspect.ModifyBaseValue();
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}