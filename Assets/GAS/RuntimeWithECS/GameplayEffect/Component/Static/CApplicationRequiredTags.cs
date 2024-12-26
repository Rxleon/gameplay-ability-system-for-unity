using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CApplicationRequiredTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfApplicationRequiredTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.AddComponentData(ge, new CApplicationRequiredTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}