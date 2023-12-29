﻿using System;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    /// <summary>
    /// https://github.com/BillEliot/GASDocumentation_Chinese?tab=readme-ov-file#4610-gameplay-ability-spec
    /// goto 4.6.9 Ability Tag
    /// </summary>
    [Serializable]
    public struct AbilityTagContainer
    {
        public GameplayTagSet AssetTag;

        public GameplayTagSet CancelAbilityTags;
        public GameplayTagSet BlockAbilityTags;

        public GameplayTagSet ActivationOwnedTag;
        
        public GameplayTagSet ActivationRequiredTags;
        public GameplayTagSet ActivationBlockedTags;

        public GameplayTagSet SourceRequiredTags;
        public GameplayTagSet SourceBlockedTags;
        
        public GameplayTagSet TargetRequiredTags;
        public GameplayTagSet TargetBlockedTags;

        public AbilityTagContainer(
            GameplayTag[] assetTags, 
            GameplayTag[] cancelAbilityTags,
            GameplayTag[] blockAbilityTags, 
            GameplayTag[] activationOwnedTag, 
            GameplayTag[] activationRequiredTags,
            GameplayTag[] activationBlockedTags, 
            GameplayTag[] sourceRequiredTags, 
            GameplayTag[] sourceBlockedTags,
            GameplayTag[] targetRequiredTags, 
            GameplayTag[] targetBlockedTags)
        {
            AssetTag = new GameplayTagSet(assetTags);
            CancelAbilityTags = new GameplayTagSet(cancelAbilityTags);
            BlockAbilityTags = new GameplayTagSet(blockAbilityTags);
            ActivationOwnedTag = new GameplayTagSet(activationOwnedTag);
            ActivationRequiredTags = new GameplayTagSet(activationRequiredTags);
            ActivationBlockedTags = new GameplayTagSet(activationBlockedTags);
            SourceRequiredTags = new GameplayTagSet(sourceRequiredTags);
            SourceBlockedTags = new GameplayTagSet(sourceBlockedTags);
            TargetRequiredTags = new GameplayTagSet(targetRequiredTags);
            TargetBlockedTags = new GameplayTagSet(targetBlockedTags);
        }

    }
}