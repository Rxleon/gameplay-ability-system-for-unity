using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CImmunityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}