using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    /// <summary>
    /// Trim value between -180 ~ 180
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float GetTrimmedEular(float value)
    {
        float f = value;
        if (f > 360)
            f %= 360;
        if (f > 180)
            f -= 360;
        else if (f < -180)
            f += 360;

        return f;
    }
}