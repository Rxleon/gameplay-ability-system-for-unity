using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupTryApplyEffect))]
    public partial struct SysCheckApplicationCondition : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInUsage>();
            state.RequireForUpdate<CValidEffect>();
            state.RequireForUpdate<CApplicationCondition>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // TODO 校验ApplicationCondition
            // foreach (var (_, duration, ge) in SystemAPI.Query<RefRO<ComInUsage>, RefRW<ComValidEffect>>()
            //              .WithEntityAccess())
            // {
            // }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}