using LTI.RobotSimulator.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Grid = LTI.RobotSimulator.Core.Grid;
using Window = System.Windows.Window;

namespace LTI.RobotSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string initialTitle;
        private readonly RenderWindow surface;
        private readonly List<List<Vertex>> obstacles;
        private readonly Clock clock;
        private bool firstObstacleClick = true;
        private bool obstacleBeingDrawn = false;
        private bool simulationRunning = false;
        private float time;

        // Drawables
        private readonly Robot robot;
        private readonly Grid simulationGrid;

        public MainWindow()
        {
            InitializeComponent();

            Height = SystemParameters.WorkArea.Height * 0.9;
            Width = Height * 4 / 3;

            initialTitle = Title;

            surface = new RenderWindow(renderControl.Handle, new ContextSettings() { AntialiasingLevel = 16 });
            clock = new Clock();

            robot = new Robot(4);
            simulationGrid = new Grid(40);
            obstacles = new List<List<Vertex>>();

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public float Zoom { get; set; } = 1;

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            float deltaTime = clock.Restart().AsSeconds();
            time += deltaTime;

            if (time >= 1)
            {
                Title = initialTitle + " " + ((uint)(1 / deltaTime)).ToString() + " FPS";
                time = 0;
            }

            surface.DispatchEvents();

            robot.UpdatePointCloud(obstacles);
            robot.Update(deltaTime);

            var updateView = new SFML.Graphics.View(robot.Position, (Vector2f)surface.Size);
            updateView.Zoom(Zoom);
            surface.SetView(updateView);

            surface.Clear();
            surface.Draw(simulationGrid);

            foreach (var obstacle in obstacles)
            {
                surface.Draw(obstacle.ToArray(), PrimitiveType.LineStrip);
            }

            foreach (var trajectoryCirlce in robot.Trajectory)
            {
                surface.Draw(trajectoryCirlce);

                foreach (var impactCircle in trajectoryCirlce.ImpactCircles)
                {
                    surface.Draw(impactCircle);
                }
            }

            foreach (var sensor in robot.Sensors)
            {
                surface.Draw(sensor);
            }

            surface.Draw(robot);
            surface.Display();
        }

        private void RenderControl_MouseWheel(object sender, MouseEventArgs e)
        {
            switch (e.Delta)
            {
                case var delta when delta < 0:
                    Zoom += 0.25f;
                    break;
                case var delta when delta > 0:
                    Zoom -= 0.25f;
                    break;
            }

            if (Zoom < 1)
            {
                Zoom = 1;
            }
            else if (Zoom > 4)
            {
                Zoom = 4;
            }
        }

        private void RenderControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (obstacleBeingDrawn)
            {
                var currentObstacle = obstacles[obstacles.Count - 1];

                if (currentObstacle.Count > 1)
                {
                    currentObstacle[currentObstacle.Count - 1] = new Vertex(surface.MapPixelToCoords(new Vector2i(e.X, e.Y)));
                }
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            robot.SetAllowMove(false);

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "XML simulation files (*.xml)|*.xml|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                }
            }

            robot.SetAllowMove(true);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool simulationWasRunning = simulationRunning;
            SetSimulationRunning(false);

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = ".";
                saveFileDialog.Filter = "XML simulation files (*.xml)|*.xml|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (var writer = XmlWriter.Create(saveFileDialog.FileName, new XmlWriterSettings() { Indent = true }))
                    {
                        writer.WriteStartElement("simulation"); // <simulation>
                        writer.WriteStartElement("trajectory"); // <trajectory>

                        foreach (var point in robot.Trajectory)
                        {
                            writer.WriteStartElement("point"); // <point>
                            writer.WriteElementString("position", $"{point.Position.X} {point.Position.Y}");
                            writer.WriteElementString("rotation", $"{point.Rotation}");

                            writer.WriteStartElement("impactPoints"); // <impactPoints>

                            foreach (var impactPoint in point.ImpactCircles)
                            {
                                writer.WriteStartElement("impactPoint"); // <impactPoint>
                                writer.WriteElementString("position", $"{impactPoint.Position.X} {impactPoint.Position.Y}");
                                writer.WriteEndElement(); // </impactPoint>
                            }

                            writer.WriteEndElement(); // </impactPoints>
                            writer.WriteEndElement(); // </point>
                        }

                        writer.WriteEndElement(); // </trajectory>
                        writer.WriteStartElement("obstacles"); // <obstacles>

                        foreach (var obstacle in obstacles)
                        {
                            writer.WriteStartElement("obstacle"); // <obstacle>

                            foreach (var vertex in obstacle)
                            {
                                writer.WriteElementString("vertexPosition", $"{vertex.Position.X} {vertex.Position.Y}");
                            }

                            writer.WriteEndElement(); // </obstacle>
                        }

                        writer.WriteEndElement(); // </obstacles>
                        writer.WriteEndElement(); // </simulation>
                        writer.Flush();
                    }
                }
            }

            if (simulationWasRunning)
            {
                SetSimulationRunning(true);
            }
        }

        private void RunPauseButton_Click(object sender, RoutedEventArgs e)
        {
            SetSimulationRunning(!simulationRunning);
        }

        private void SetSimulationRunning(bool simulationRunning)
        {
            robot.SetAllowMove(simulationRunning);

            if (simulationRunning)
            {
                ((TextBlock)runPauseStackPanel.Children[1]).Text = "Pause";
                ((System.Windows.Controls.Image)runPauseStackPanel.Children[0]).Source = new BitmapImage(new Uri("Icons/Pause_16x.png", UriKind.Relative));
            }
            else
            {
                ((TextBlock)runPauseStackPanel.Children[1]).Text = "Run";
                ((System.Windows.Controls.Image)runPauseStackPanel.Children[0]).Source = new BitmapImage(new Uri("Icons/Run_16x.png", UriKind.Relative));
            }

            robot.SetAllowMove(simulationRunning);
            this.simulationRunning = simulationRunning;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            surface.Close();
        }

        private void RenderControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (obstacles.Count == 0)
            {
                obstacles.Add(new List<Vertex>());
            }

            var currentObstacle = obstacles[obstacles.Count - 1];

            if (e.Button == MouseButtons.Left)
            {
                SetSimulationRunning(false);

                if (firstObstacleClick)
                {
                    obstacleBeingDrawn = true;
                    currentObstacle.Add(new Vertex(surface.MapPixelToCoords(new Vector2i(e.X, e.Y))));
                    currentObstacle.Add(new Vertex(surface.MapPixelToCoords(new Vector2i(e.X, e.Y))));
                    firstObstacleClick = false;
                }
                else
                {
                    currentObstacle.Add(new Vertex(surface.MapPixelToCoords(new Vector2i(e.X, e.Y))));
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                switch (currentObstacle.Count)
                {
                    case var count when count >= 2:
                        if (currentObstacle.Count >= 4)
                        {
                            currentObstacle[currentObstacle.Count - 1] = currentObstacle[0];
                        }
                        else if (currentObstacle.Count == 3)
                        {
                            currentObstacle.RemoveAt(currentObstacle.Count - 1);
                        }
                        else if (currentObstacle.Count == 2)
                        {
                            currentObstacle.Clear();
                        }

                        obstacleBeingDrawn = false;
                        firstObstacleClick = true;

                        obstacles.Add(new List<Vertex>());

                        break;
                }
            }
        }
    }
}
