using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComCueOnExecution : IComponentData
    {
        public NativeArray<Entity> cues;
    }
}