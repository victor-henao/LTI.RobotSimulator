using SFML.Graphics;
using System.Collections.Generic;

namespace LTI.RobotSimulator.Core
{
    public class TrajectoryCircle : CircleShape
    {
        public TrajectoryCircle(float radius) : base(radius) =>
            ImpactCircles = new List<CircleShape>();

        public List<CircleShape> ImpactCircles { get; set; }
    }
}
