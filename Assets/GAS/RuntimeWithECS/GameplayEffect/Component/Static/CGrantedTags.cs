using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CGrantedTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}