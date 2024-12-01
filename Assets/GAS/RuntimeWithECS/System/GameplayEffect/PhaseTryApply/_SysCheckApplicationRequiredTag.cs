// using GAS.RuntimeWithECS.GameplayEffect.Component;
// using GAS.RuntimeWithECS.System.SystemGroup;
// using Unity.Burst;
// using Unity.Entities;
// using UnityEngine;
//
// namespace GAS.RuntimeWithECS.System.GameplayEffect
// {
//     [UpdateInGroup(typeof(SysGroupTryApplyEffect))]
//     [UpdateAfter(typeof(SysCheckApplicationCondition))]
//     public partial struct SysCheckApplicationRequiredTag : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<ComInUsage>();
//             state.RequireForUpdate<ComValidEffect>();
//             state.RequireForUpdate<ComApplicationRequiredTags>();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state)
//         {
//
//         }
//     }
// }