namespace Scylla
{
    using System.Collections;
    using System.Collections.Generic;

    public static class MathUtils
    {
        public static float ScaleInRange(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        public static float Normalize(float value)
        {
            return ScaleInRange(value, 0f, 1f);
        }
    }
}

