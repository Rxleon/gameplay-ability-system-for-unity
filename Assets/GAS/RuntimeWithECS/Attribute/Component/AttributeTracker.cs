using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet.Component
{
    public struct AttributeTracker : IComponentData
    {
        public int AttrSetCode;
        public int AttrCode;
        public Entity BasedASC;
        
    }
}