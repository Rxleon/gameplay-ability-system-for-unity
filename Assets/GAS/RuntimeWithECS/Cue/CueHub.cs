using System.Collections.Generic;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.RuntimeWithECS.Modifier.CommonUsage;

namespace GAS.RuntimeWithECS.Cue
{
    public static class CueHub
    {
        private static Dictionary<int, CueInstant> _cueInstantMap;
        private static Dictionary<int, CueDurational> _cueDurationalMap;
        
        public static void Init()
        {
            _cueInstantMap = new Dictionary<int, CueInstant>();
            // TODO :初始化项目内所有类型Cue原型实例
            //_cueInstantMap.Add(CueTypeToCode.Map[typeof(CueLog)],new MMCScalableFloat());
        }
    }
}