using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupTryApplyEffect))]
    [UpdateAfter(typeof(SysCheckApplicationCondition))]
    public partial struct SysCheckImmunityTag : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}