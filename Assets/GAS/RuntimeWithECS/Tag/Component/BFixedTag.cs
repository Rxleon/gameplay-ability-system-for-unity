using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag.Component
{
    public struct BFixedTag : IBufferElementData
    {
        public int tag;
    }
}