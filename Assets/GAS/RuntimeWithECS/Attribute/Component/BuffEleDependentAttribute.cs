using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet.Component
{
    public struct BuffEleDependentAttribute : IBufferElementData
    {
        public Entity SourceGameplayEffect;
        
        public Entity SourceASC;
        public int AttrSetCode;
        public int AttrCode;
    }
}