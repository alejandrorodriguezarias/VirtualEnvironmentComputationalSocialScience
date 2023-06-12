using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Util
{

    public static float NormalizedMinMax(float normVariable, float minOld, float maxOld, float minNew, float maxNew) {
        float normValue = minNew + ((normVariable - minOld) * (maxNew - minNew) / (maxOld - minOld));
        //in case the original value belongs to a distribution that is difficult to delimit
        if (normValue > maxNew) { normValue = maxNew; }
        if (normValue < minNew) { normValue = minNew; }
        return normValue;
    }

    public static float RandNormal()
    {
        //var rng = new System.Random();
        double num1 = UnityEngine.Random.value;
        double num2 = UnityEngine.Random.value;
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(num1)) * Math.Sin(2.0 * Math.PI * num2); //Box Muller Transform
        return (float)randStdNormal;
    }
}
