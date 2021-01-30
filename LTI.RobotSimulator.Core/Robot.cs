using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace LTI.RobotSimulator.Core
{
    public class Robot : CircleShape, IUpdatable
    {
        private const float BaseSpeed = 50;

        public Robot() : base(50)
        {
            Origin = new Vector2f(50, 50);
            Trajectory = new List<CircleShape>();
        }

        public List<CircleShape> Trajectory { get; set; }

        public void Update(float deltaTime)
        {
            float left = Angle.ToRadians(BaseSpeed * deltaTime) * 2;
            float right = Angle.ToRadians(BaseSpeed * deltaTime);
            Move(left, right);

            void Move(float leftDelta, float rightDelta)
            {
                float thetaDelta = ((50 * rightDelta) - (50 * leftDelta)) / (2 * Radius);

                Position += new Vector2f(
                    (50 * leftDelta + 50 * rightDelta) / 2 * (float)Math.Cos(-Rotation),
                    (50 * leftDelta + 50 * rightDelta) / 2 * (float)Math.Sin(-Rotation));

                Rotation += thetaDelta;
            }

            // Update trajectory
            Trajectory.Add(new CircleShape(2) { Position = Position, Origin = new Vector2f(2, 2), FillColor = Color.Green });
        }
    }
}
