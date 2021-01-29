using LTI.RobotSimulator.Core;
using SFML.Graphics;
using SFML.System;
using System;
using System.Windows;
using System.Windows.Media;

namespace LTI.RobotSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RenderWindow surface;
        private readonly Clock clock;
        private float time;

        // Drawables
        private readonly Robot robot;
        private readonly Grid simulationGrid;

        public MainWindow()
        {
            InitializeComponent();
            renderControl.MouseMove += RenderControl_MouseMove;
            renderControl.MouseWheel += RenderControl_MouseWheel;

            surface = new RenderWindow(renderControl.Handle);
            clock = new Clock();

            robot = new Robot();
            simulationGrid = new Grid(10);

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public float Zoom { get; set; } = 2;

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            float deltaTime = clock.Restart().AsSeconds();
            time += deltaTime;
            if (time >= 1)
            {
                Title = (1 / deltaTime).ToString();
                time = 0;
            }

            surface.DispatchEvents();

            robot.Update(deltaTime);

            var updateView = new View(robot.Position, (Vector2f)surface.Size);
            updateView.Zoom(Zoom);
            surface.SetView(updateView);

            surface.Clear();
            surface.Draw(simulationGrid);
            surface.Draw(robot);
            surface.Display();
        }

        private void RenderControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
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

        private void RenderControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            surface.Close();
        }
    }
}
