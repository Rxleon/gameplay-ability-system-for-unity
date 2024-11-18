using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet.Component
{
    public struct BuffEleEffectingAttribute : IBufferElementData
    {
        public Entity TargetASC;
        public int AttrSetCode;
        public int AttrCode;
    }
}