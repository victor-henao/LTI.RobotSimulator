using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LTI.RobotSimulator.Core
{
    public class Robot : CircleShape, IUpdatable
    {
        private const float BaseSpeed = 50;
        private readonly uint sensorCount;
        private bool allowMove = false;

        public Robot(uint sensorCount) : base(50)
        {
            this.sensorCount = sensorCount;

            Origin = new Vector2f(50, 50);
            Trajectory = new List<TrajectoryCircle>();
            Sensors = new List<CircleShape>();

            for (float angle = 0; angle < Math.PI * 2; angle += (float)Math.PI * 2 / sensorCount)
            {
                var circle = new CircleShape(2)
                {
                    Origin = new Vector2f(2, 2),
                    Position = new Vector2f(
                        (float)Math.Cos(angle) * Radius,
                        (float)Math.Sin(angle) * Radius),
                    FillColor = Color.Blue
                };

                Sensors.Add(circle);
            }
        }

        public List<TrajectoryCircle> Trajectory { get; set; }

        public List<CircleShape> Sensors { get; set; }

        public void SetAllowMove(bool allowMove) => this.allowMove = allowMove;

        public void UpdatePointCloud(List<List<Vertex>> obstacles)
        {
            if (allowMove)
            {
                var trajectoryCircle = new TrajectoryCircle(2)
                {
                    Radius = 2,
                    Origin = new Vector2f(2, 2),
                    Position = Position,
                    Rotation = Rotation,
                    FillColor = Color.Green
                };

                foreach (var sensor in Sensors)
                {
                    var sensorLine = new Ray(Position, sensor.Position);
                    var distances = new Dictionary<CircleShape, float>();

                    foreach (var obstacle in obstacles)
                    {
                        var obstacleLines = new List<LineSegment>();

                        for (int i = 0; i < obstacle.Count - 1; i++)
                        {
                            obstacleLines.Add(new LineSegment(obstacle[i].Position, obstacle[i + 1].Position));
                        }

                        foreach (var obstacleLine in obstacleLines)
                        {
                            var intersectionCoords = sensorLine.IntersectionWith(obstacleLine);

                            if (intersectionCoords != null)
                            {
                                var intersectionCircle = new CircleShape(2)
                                {
                                    Origin = new Vector2f(2, 2),
                                    FillColor = Color.Yellow,
                                    Position = intersectionCoords.Value
                                };

                                distances.Add(intersectionCircle, new LineSegment(sensor.Position, intersectionCoords.Value).Lenght);
                            }
                        }
                    }

                    var nearestPoint = distances.OrderBy(pair => pair.Value).FirstOrDefault().Key;

                    if (nearestPoint != null)
                    {
                        trajectoryCircle.ImpactCircles.Add(nearestPoint);
                    }
                }

                Trajectory.Add(trajectoryCircle);
            }
        }

        public void Update(float deltaTime)
        {
            if (allowMove)
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

                float angleOffset = (float)Math.PI * 2 / sensorCount;
                float angle = 0;

                foreach (var sensor in Sensors)
                {
                    sensor.Position = new Vector2f(
                        Position.X + (float)Math.Cos(angle - Rotation) * Radius,
                        Position.Y + (float)Math.Sin(angle - Rotation) * Radius);

                    angle += angleOffset;
                }
            }
        }
    }
}
