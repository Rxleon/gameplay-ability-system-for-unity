using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct COngoingRequiredTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}