﻿using System.Collections.Generic;
using GAS.Runtime.Component;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Modifier;

namespace GAS.Runtime.Ability
{
    public abstract class AbilitySpec
    {
        private object[] _abilityArguments;

        public AbilitySpec(AbstractAbility ability, AbilitySystemComponent owner)
        {
            Ability = ability;
            Owner = owner;
        }

        public AbstractAbility Ability { get; }

        public AbilitySystemComponent Owner { get; protected set; }

        public float Level { get; private set;}

        public bool IsActive { get; private set; }

        public int ActiveCount { get; private set; }

        public virtual bool CanActivate()
        {
            return !IsActive
                   && CheckGameplayTags()
                   && CheckCost()
                   && CheckCooldown().TimeRemaining <= 0;
        }

        private bool CheckGameplayTags()
        {
            return Owner.HasAllTags(Ability.Tag.SourceRequiredTags);
        }

        protected virtual CooldownTimer CheckCooldown()
        {
            return Ability.Cooldown.NULL
                ? new CooldownTimer()
                : Owner.CheckCooldownFromTags(Ability.Cooldown.TagContainer.GrantedTags);
        }

        public virtual void TryActivateAbility(params object[] args)
        {
            _abilityArguments = args;

            if (!CanActivate()) return;
            IsActive = true;
            ActiveCount++;
            
            ActivateAbility(_abilityArguments);
        }

        public virtual void TryEndAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            EndAbility();
        }

        private GameplayEffectSpec CostSpec()
        {
            return Owner.ApplyGameplayEffectToSelf(Ability.Cost);
        }


        public virtual bool CheckCost()
        {
            if (Ability.Cost.NULL) return true;
            var costSpec = CostSpec();
            if (costSpec == null) return false;

            if (costSpec.GameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant) return true;

            foreach (var modifier in costSpec.GameplayEffect.Modifiers)
            {
                // Cost can't be multiply or override ,so only care about additive.
                if (modifier.Operation != GEOperation.Add) continue;

                var costValue = modifier.MMC.CalculateMagnitude(modifier.ModiferMagnitude);
                var attributeCurrentValue = Owner.GetAttributeCurrentValue(modifier.AttributeSetName, modifier.AttributeShortName);

                // The total attribute after accounting for cost should be >= 0 for the cost check to succeed
                if (attributeCurrentValue + costValue < 0) return false;
            }

            return true;
        }

        public void Tick()
        {
            if (!IsActive) return;
            //foreach (var task in Ability.OngoingAbilityTasks) task.Execute(_abilityArguments);
        }
        
        public abstract void ActivateAbility(params object[] args);
        
        public abstract void CancelAbility();

        public abstract void EndAbility();
    }
}