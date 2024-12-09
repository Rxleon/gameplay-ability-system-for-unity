namespace GAS.RuntimeWithECS.Cue
{
    public abstract class CueInstant:NewGameplayCueBase
    {
        public bool TryTrigger()
        {
            bool triggerable = Triggerable();
            if (triggerable) Trigger();
            return triggerable;
        }

        protected abstract void Trigger();
    }
}