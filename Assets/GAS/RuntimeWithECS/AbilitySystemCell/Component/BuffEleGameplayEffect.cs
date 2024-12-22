using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    [InternalBufferCapacity(500)]
    public struct BuffEleGameplayEffect : IBufferElementData
    {
        public Entity GameplayEffect;
    }
}