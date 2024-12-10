using Unity.Entities;

namespace GAS.RuntimeWithECS.Cue.Component
{
    public class ComInstantCue : IComponentData
    {
        public CueInstant cue;
        
        public ComInstantCue()
        {
        }
        
        public ComInstantCue(CueInstant cue)
        {
            this.cue = cue;
        }
    }
}