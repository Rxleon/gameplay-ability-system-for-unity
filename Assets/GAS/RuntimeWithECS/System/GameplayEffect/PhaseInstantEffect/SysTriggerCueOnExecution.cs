using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    [UpdateAfter(typeof(SysInstantEffectModifyBaseValue))]
    public partial struct SysTriggerCueOnExecution : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComInApplicationProgress>();
            state.RequireForUpdate<ComValidEffect>();
            state.RequireForUpdate<ComInUsage>();
            state.RequireForUpdate<ComCueOnExecution>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var aspect in SystemAPI.Query<AspCueOnExecution>())
            {
                aspect.Trigger();
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    public readonly partial struct AspCueOnExecution : IAspect
    {
        public readonly Entity self;
        private readonly RefRO<ComInUsage> _inUsage;
        private readonly RefRO<ComValidEffect> _comValidEffect;
        private readonly RefRO<ComInApplicationProgress> _inApplicationProgress;
        private readonly RefRO<ComCueOnExecution> _cueOnExecution;

        public void Trigger()
        {
        }
    }
}