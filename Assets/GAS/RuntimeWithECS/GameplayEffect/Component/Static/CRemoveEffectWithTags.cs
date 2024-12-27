using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CRemoveEffectWithTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}