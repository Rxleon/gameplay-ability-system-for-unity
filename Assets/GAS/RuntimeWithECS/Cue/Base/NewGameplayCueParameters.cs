using Unity.Entities;

namespace GAS.RuntimeWithECS.Cue
{
    public enum CueSourceType
    {
        None,
        AbilitySystemCell,
        GameplayEffect
    }
    
    public class NewGameplayCueParametersBase
    {
        public CueSourceType SourceType;
        public Entity entity;

        public float commonFloat;
        public int commonInt;
        public string commonString;
    }
}