using SFML.Graphics;
using SFML.System;

namespace LTI.RobotSimulator.Core
{
    public class Robot : CircleShape, IUpdatable
    {
        public Robot() : base(50)
        {
            Origin = new Vector2f(50, 50);
        }

        public void Update(float deltaTime)
        {
            Position += new Vector2f(50, 0) * deltaTime;
        }
    }
}
