using LTI.RobotSimulator.Core;
using SFML.Graphics;
using SFML.System;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Grid = LTI.RobotSimulator.Core.Grid;

namespace LTI.RobotSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string initialTitle;
        private readonly RenderWindow surface;
        private readonly Clock clock;
        private bool simulationRunning = false;
        private float time;

        // Drawables
        private readonly Robot robot;
        private readonly Grid simulationGrid;

        public MainWindow()
        {
            InitializeComponent();

            Width = SystemParameters.WorkArea.Width * 0.9f;
            Height = SystemParameters.WorkArea.Height * 0.9f;

            initialTitle = Title;

            surface = new RenderWindow(renderControl.Handle);
            clock = new Clock();

            robot = new Robot();
            simulationGrid = new Grid(20);

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public float Zoom { get; set; } = 2;

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

            robot.Update(deltaTime);

            var updateView = new SFML.Graphics.View(robot.Position, (Vector2f)surface.Size);
            updateView.Zoom(Zoom);
            surface.SetView(updateView);

            surface.Clear();
            surface.Draw(simulationGrid);

            foreach (var point in robot.Trajectory)
            {
                surface.Draw(point);
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
                        writer.WriteStartElement("simulation");
                        writer.WriteStartElement("trajectory");

                        foreach (var point in robot.Trajectory)
                        {
                            writer.WriteStartElement("point");
                            writer.WriteElementString("position", $"{point.Position.X} {point.Position.Y}");
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
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

        private void RenderControl_Click(object sender, EventArgs e)
        {
            SetSimulationRunning(false);
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
    }
}
