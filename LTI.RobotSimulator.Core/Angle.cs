using System;

namespace LTI.RobotSimulator.Core
{
    static class Angle
    {
        public static float ToRadians(float angle) => angle * (float)Math.PI / 180.0f;

        public static float ToDegrees(float angle) => angle * 180.0f / (float)Math.PI;
    }
}
