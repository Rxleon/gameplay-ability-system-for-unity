﻿using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class TimelineInspector
    {
        private VisualElement _root;
        private VisualElement _clipInspector;
        public object CurrentInspectorObject;
        public TimelineInspector(VisualElement root)
        {
            _root = root;
            _clipInspector = _root.Q<VisualElement>("ClipInspector");
            SetInspector();
        }

        public void SetInspector(object target=null)
        {
            UpdateInspector(false,target);
        }

        private void UpdateInspector(bool force = false, object target = null)
        {
            if (CurrentInspectorObject == target && !force) return;

            if (CurrentInspectorObject != null && !force)
            {
                if (CurrentInspectorObject is TrackClipBase oldTrackItem) oldTrackItem.Ve.OnUnSelect();
                if (CurrentInspectorObject is TrackBase oldTrack) oldTrack.OnUnSelect();
                if (CurrentInspectorObject is TrackMarkBase oldMark) oldMark.OnUnSelect();
            }

            CurrentInspectorObject = target;
            _clipInspector.Clear();
            switch (CurrentInspectorObject)
            {
                case null:
                    return;
                case TrackClipBase trackClip:
                    _clipInspector.Add(trackClip.Inspector());
                    break;
                case TrackBase track:
                    _clipInspector.Add(track.Inspector());
                    break;
                case TrackMarkBase mark:
                    _clipInspector.Add(mark.Inspector());
                    break;
            }
        }
        
        public void RefreshInspector()
        {
            UpdateInspector(true,CurrentInspectorObject);
        }
    }
}