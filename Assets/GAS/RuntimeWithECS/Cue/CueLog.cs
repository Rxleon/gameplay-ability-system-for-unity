using UnityEngine;

namespace GAS.RuntimeWithECS.Cue
{
    public class CueLog : CueInstant
    {
        CueLogParameters Parameters;

        protected override void Trigger()
        {
            Debug.Log(
                $"SourceType:{Parameters.SourceType.ToString()}, Entity:{Parameters.entity} ,Msg:{Parameters.Message}");
        }

        public CueLog(CueLogParameters p) : base(p)
        {
            Parameters = p;
        }

        public void SetMessage(string message)
        {
            Parameters.Message = message;
        }
    }
}