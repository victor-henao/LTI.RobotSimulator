using SFML.System;

namespace LTI.RobotSimulator.Core
{
    class Line
    {
        public Line(Vector2f a, Vector2f b) => (A, B) = (a, b);

        public Vector2f A { get; }

        public Vector2f B { get; }

        public float Slope => (B.Y - A.Y) / (B.X - A.X);

        public float YIntercept => A.Y - (Slope * A.X);

        public bool IsHorizontal => A.Y == B.Y;

        public bool IsVertical => A.X == B.X;

        public static Vector2f? SolveSystem(Line a, Line b)
        {
            if (a.Slope == b.Slope)
            {
                return null;
            }

            float x, y;

            if (a.IsVertical)
            {
                x = a.A.X;
                y = b.Slope * x + b.YIntercept;
            }
            else if (b.IsVertical)
            {
                x = b.A.X;
                y = a.Slope * x + a.YIntercept;
            }
            else
            {
                x = (b.YIntercept - a.YIntercept) / (a.Slope - b.Slope);
                y = (a.Slope * x) + a.YIntercept;
            }

            return new Vector2f(x, y);
        }
    }
}
