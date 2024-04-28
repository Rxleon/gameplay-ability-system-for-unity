﻿using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using GAS.Editor;
#endif
using GAS.General;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Demo.Script.GAS.TargetCatcher
{
    [Serializable]
    public class CatchUndefending : CatchAreaBox2D
    {
        public override List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            var targets = CatchDefaultTargets(mainTarget);
            var result = new List<AbilitySystemComponent>();
            foreach (var target in targets)
                if (!IsDefendSuccess(target))
                    result.Add(target);
            return result;
        }

        protected List<AbilitySystemComponent> CatchDefaultTargets(AbilitySystemComponent mainTarget)
        {
            return base.CatchTargets(mainTarget);
        }

        /// <summary>
        /// 没有防御成功的判定：1.没有防御  2.防御了，但是方向错误(丢弃判断)
        /// </summary>
        /// <returns></returns>
        protected bool IsDefendSuccess(AbilitySystemComponent target)
        {
            return target.HasTag(GTagLib.Event_Defending);
            // if (!target.HasTag(GTagLib.Event_Defending)) return false;
            // return target.transform.localScale.x * Owner.transform.localScale.x < 0;
        }
        
#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject obj)
        {
            // 使用Debug 绘制box预览
            float showTime = 1;
            var color = Color.green;
            var relativeTransform = AbilityTimelineEditorWindow.Instance.PreviewObject.transform;
            var center = offset;
            var angle = rotation + relativeTransform.eulerAngles.z;
            switch (centerType)
            {
                case EffectCenterType.SelfOffset:
                    center = relativeTransform.position;
                    center.y += relativeTransform.lossyScale.y > 0 ? offset.y : -offset.y;
                    center.x += relativeTransform.lossyScale.x > 0 ? offset.x : -offset.x;
                    break;
                case EffectCenterType.WorldSpace:
                    center = offset;
                    break;
                case EffectCenterType.TargetOffset:
                    //center = _spec.Target.transform.position + (Vector3)_task.Offset;
                    break;
            }

            DebugExtension.DebugBox(center, size, angle, color, showTime);
        }
#endif
    }
#if UNITY_EDITOR
    public class CatchUndefendingInspector : TargetCatcherInspector<CatchUndefending>
    {
        [BoxGroup] [Delayed] [OnValueChanged("OnCatcherChanged")]
        public Vector2 Offset;
        
        [BoxGroup] [Delayed] [OnValueChanged("OnCatcherChanged")]
        public Vector2 Size;
        
        [BoxGroup] [Delayed] [LabelText("Rotation")] [OnValueChanged("OnCatcherChanged")]
        public float Rotation;
        
        [BoxGroup] [Delayed] [LabelText("Detect Layer")] [OnValueChanged("OnCatcherChanged")]
        public LayerMask Layer;
        
        [BoxGroup] [Delayed] [LabelText("Center Type")] [OnValueChanged("OnCatcherChanged")]
        public EffectCenterType CenterType;
        
        public CatchUndefendingInspector(CatchUndefending targetCatcherBase) : base(targetCatcherBase)
        {
            Offset = targetCatcherBase.offset;
            Size = targetCatcherBase.size;
            Rotation = targetCatcherBase.rotation;
            Layer = targetCatcherBase.checkLayer;
            CenterType = targetCatcherBase.centerType;
        }
        
        public void OnCatcherChanged()
        {
            _targetCatcher.offset = Offset;
            _targetCatcher.size = Size;
            _targetCatcher.rotation = Rotation;
            _targetCatcher.checkLayer = Layer;
            _targetCatcher.centerType = CenterType;
            Save();
        }
    }
#endif
}