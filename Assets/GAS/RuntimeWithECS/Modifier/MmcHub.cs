﻿using System;
using System.Collections.Generic;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Modifier
{
    public static class MmcHub
    {
        private static Dictionary<int, ModMagnitudeCalculation> _magnitudeCalculations;

        public static void Init()
        {
            _magnitudeCalculations = new Dictionary<int, ModMagnitudeCalculation>();
            // TODO :初始化项目内所有类型MMC实例
            _magnitudeCalculations.Add(MMCTypeToCode.Map[typeof(MMCScalableFloat)],new MMCScalableFloat());
        }

        public static float Calculate(Entity ge, BuffEleModifier modifier)
        {
            var setting = modifier.MMC;
            float result;
#if UNITY_EDITOR
            try
            {
#endif
                var mmc = _magnitudeCalculations[setting.TypeCode];
                mmc.InitParameters(setting.floatParams, setting.intParams, setting.stringParams);
                result = mmc.CalculateMagnitude(ge, modifier.Magnitude);
#if UNITY_EDITOR
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
#endif
            return result;
        }
        
        public static float Calculate(Entity ge, BuffEleModifier modifier, float sourceValue)
        {
            var result = sourceValue;
            var magnitude = Calculate(ge, modifier);
            switch (modifier.Operation)
            {
                case GEOperation.Add:
                    result += magnitude;
                    break;
                case GEOperation.Minus:
                    result -= magnitude;
                    break;
                case GEOperation.Multiply:
                    result *= magnitude;
                    break;
                case GEOperation.Divide:
                    result /= magnitude;
                    break;
                case GEOperation.Override:
                    result = magnitude;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }
    }
}