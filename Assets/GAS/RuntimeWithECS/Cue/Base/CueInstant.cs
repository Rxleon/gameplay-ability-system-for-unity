namespace GAS.RuntimeWithECS.Cue
{
    public abstract class CueInstant : NewGameplayCueBase
    {
        public bool TryTrigger()
        {
            var triggerable = Triggerable();
            if (triggerable) Trigger();
            return triggerable;
        }

        protected abstract void Trigger();

        protected CueInstant(NewGameplayCueParametersBase p) : base(p)
        {
        }
    }
}