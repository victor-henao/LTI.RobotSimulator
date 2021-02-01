using SFML.System;

namespace LTI.RobotSimulator.Core
{
    struct Line
    {
        public Vector2f A;
        public Vector2f B;

        public Line(Vector2f a, Vector2f b) => (A, B) = (a, b);

        public float Slope => (B.Y - A.Y) / (B.X - A.X);

        public float YIntercept => A.Y - (Slope * A.X);

        public static Vector2f SolveSystem(Line a, Line b)
        {
            float x, y;

            var a1 = new Vector2f(a.A.X, a.A.Y);
            var b1 = new Vector2f(a.B.X, a.B.Y);

            var a2 = new Vector2f(b.A.X, b.A.Y);
            var b2 = new Vector2f(b.B.X, b.B.Y);

            if (a1.X == b1.X)
            {
                x = a1.X;
                y = b.Slope * x + b.YIntercept;
            }
            else if (a2.X == b2.X)
            {
                x = a2.X;
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
