using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Aspect
{
    public readonly partial struct AspModifyBaseValue : IAspect
    {
        public readonly Entity self;
        private readonly RefRO<ComInUsage> _inUsage;
        private readonly RefRO<ComValidEffect> _comValidEffect;
        private readonly RefRO<ComInApplicationProgress> _inApplicationProgress;
        private readonly DynamicBuffer<BuffEleModifier> _modifiers;

        public Entity ASC => _inUsage.ValueRO.Target;
        
        public bool ModifyBaseValue()
        {
            // 排除掉Durational的GE类型
            var isDurational = GASManager.EntityManager.HasComponent<ComDuration>(self);
            if (isDurational) return false;

            var asc = _inUsage.ValueRO.Target;
            bool changed = false;
            var attrSets = GASManager.EntityManager.GetBuffer<AttributeSetBufferElement>(asc);
            foreach (var mod in _modifiers)
            {
                var attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
                if (attrSetIndex == -1) continue;

                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                var attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
                if (attrIndex == -1) continue;

                var data = attributes[attrIndex];
                var oldValue = data.BaseValue;
                var newValue = MmcHub.Calculate(self, mod, data.BaseValue);

                // OnChangeBefore
                // BaseValue 不做钳制，因为Max，Min是只针对Current Value
                newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(asc, mod.AttrSetCode, mod.AttrCode, newValue);

                data.BaseValue = newValue;

                // OnChangeAfter
                if (newValue != oldValue)
                {
                    // BaseValue 改变，需要标记Dirty
                    data.Dirty = true;
                    changed = true;
                    GASEventCenter.InvokeOnBaseValueChangeAfter(asc, mod.AttrSetCode, mod.AttrCode, oldValue, newValue);
                }

                attrSet.Attributes[attrIndex] = data;
                attrSets[attrSetIndex] = attrSet;
            }

            return changed;
        }
    }
}