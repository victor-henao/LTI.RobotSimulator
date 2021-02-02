using SFML.System;
using System;

namespace LTI.RobotSimulator.Core
{
    class LineSegment : Line
    {
        public LineSegment(Vector2f a, Vector2f b) : base(a, b)
        {
        }

        public float Lenght => (float)Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
    }
}
