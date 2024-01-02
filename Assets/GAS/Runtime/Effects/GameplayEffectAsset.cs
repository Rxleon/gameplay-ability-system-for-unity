﻿using GAS.Runtime.Cue;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Effects
{
    [CreateAssetMenu(fileName = "GameplayEffect", menuName = "GAS/GameplayEffect")]
    public class GameplayEffectAsset : ScriptableObject
    {
        public string Name;
        public string Description;
        public EffectsDurationPolicy DurationPolicy;
        public float Duration;
        public float Period;
        public GameplayEffectAsset PeriodExecution;

        // Tag Container
        public GameplayTag[] AssetTags;
        public GameplayTag[] GrantedTags;
        public GameplayTag[] ApplicationRequiredTags;
        public GameplayTag[] OngoingRequiredTags;
        public GameplayTag[] RemoveGameplayEffectsWithTags;

        // Cues
        public GameplayCue[] CueOnExecute;
        public GameplayCue[] CueOnRemove;
        public GameplayCue[] CueOnAdd;

        public GameplayEffectModifier[] Modifiers;


        // TODO
        public ExecutionCalculation[] Executions;
    }
}