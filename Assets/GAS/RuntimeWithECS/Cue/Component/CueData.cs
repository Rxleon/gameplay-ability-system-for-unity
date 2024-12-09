using Unity.Entities;

namespace GAS.RuntimeWithECS.Cue.Component
{
    public class InstantCueData : IComponentData
    {
        public NewGameplayCueParametersBase parameters;
        public CueInstant cue;
        
        public InstantCueData()
        {
        }
    }
}