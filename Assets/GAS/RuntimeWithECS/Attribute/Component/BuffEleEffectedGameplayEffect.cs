using Unity.Entities;

namespace GAS.RuntimeWithECS.Attribute.Component
{
    public struct BuffEleEffectedGameplayEffect : IBufferElementData
    {
        public Entity GameplayEffect;
    }
}