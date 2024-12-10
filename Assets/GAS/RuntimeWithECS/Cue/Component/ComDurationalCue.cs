using Unity.Entities;

namespace GAS.RuntimeWithECS.Cue.Component
{
    public class ComDurationalCue : IComponentData
    {
        public CueDurational cue;
        
        public ComDurationalCue()
        {
        }
        
        public ComDurationalCue(CueDurational cue)
        {
            this.cue = cue;
        }
    }
}