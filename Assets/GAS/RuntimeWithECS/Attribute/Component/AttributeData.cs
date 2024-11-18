using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Attribute.Component
{
    public struct AttributeData : IComponentData
    {
        public int Code;
        public float BaseValue;
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;
        public bool Dirty;
        
        /// <summary>
        /// 属性追踪器：用于追踪属性变化的连锁反应
        /// 1.影响该属性的GE
        /// 2.该属性依赖的追踪（Track）属性
        /// </summary>
        public Entity Tracker;
        

        public static readonly AttributeData NULL = new()
        {
            Code = -1
        };
    }

    public static class AttributeDataExtension
    {
        public static int IndexOfAttrCode(this NativeArray<AttributeData> attrs, int attrCode)
        {
            for (var i = 0; i < attrs.Length; i++)
                if (attrs[i].Code == attrCode)
                    return i;
            return -1;
        }
    }
}