using GAS.RuntimeWithECS.Cue.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupDurationalEffect))]
    [UpdateAfter(typeof(SysInitDuartionalEffect))]
    public partial struct SysTriggerCueOnAdd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComInUsage>();
            state.RequireForUpdate<ComInApplicationProgress>();
            state.RequireForUpdate<ComValidEffect>();
            state.RequireForUpdate<ComCueOnAdd>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var aspect in SystemAPI.Query<AspCueOnAdd>())
            {
                aspect.Trigger(state.EntityManager);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    public readonly partial struct AspCueOnAdd : IAspect
    {
        public readonly Entity self;
        private readonly RefRO<ComInUsage> _inUsage;
        private readonly RefRO<ComValidEffect> _comValidEffect;
        private readonly RefRO<ComInApplicationProgress> _inApplicationProgress;
        private readonly RefRO<ComCueOnAdd> _cueOnAdd;

        public void Trigger(EntityManager entityManager)
        {
            foreach (var entity in _cueOnAdd.ValueRO.cues)
            {
                var cue = entityManager.GetComponentData<ComInstantCue>(entity);
                cue.cue.TryTrigger();
            }
        }
    }
}