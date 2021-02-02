using SFML.System;

namespace LTI.RobotSimulator.Core
{
    class Ray : Line
    {
        public Ray(Vector2f a, Vector2f b) : base(a, b)
        {
        }

        public Vector2f? IntersectionWith(LineSegment lineSegment)
        {
            var intersectionCoords = SolveSystem(this, lineSegment);

            if (intersectionCoords == null)
            {
                return null;
            }

            if ((B.X > A.X && intersectionCoords.Value.X > B.X) ||
                (B.X < A.X && intersectionCoords.Value.X < B.X))
            {
                if ((intersectionCoords.Value.X >= lineSegment.A.X && intersectionCoords.Value.X <= lineSegment.B.X) ||
                    (intersectionCoords.Value.X <= lineSegment.A.X && intersectionCoords.Value.X >= lineSegment.B.X))
                {
                    return intersectionCoords;
                }
            }

            return null;
        }
    }
}
