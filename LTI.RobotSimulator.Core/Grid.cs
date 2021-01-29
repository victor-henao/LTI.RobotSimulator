using SFML.Graphics;
using SFML.System;
using System;

namespace LTI.RobotSimulator.Core
{
    public class Grid : IDrawable, IDisposable
    {
        private readonly VertexArray lines;
        private readonly Font font;
        private readonly Text xText, yText;

        public Grid(int squareCount)
        {
            lines = new VertexArray(PrimitiveType.Lines);
            font = new Font("segoeui.ttf");
            xText = new Text("100", font) { FillColor = Colors.TransparentRed, Position = new Vector2f(100, 0) };
            yText = new Text("100", font) { FillColor = Colors.TransparentGreen, Position = new Vector2f(0, 100) };

            int width = squareCount * 100 / 2;

            lines.Append(new Vertex(new Vector2f(-width, 0), Colors.TransparentRed));
            lines.Append(new Vertex(new Vector2f(width, 0), Colors.TransparentRed));

            lines.Append(new Vertex(new Vector2f(0, -width), Colors.TransparentGreen));
            lines.Append(new Vertex(new Vector2f(0, width), Colors.TransparentGreen));

            int offset = -squareCount * 100 / 2;
            for (int i = 0; i < squareCount + 1; i++)
            {
                if (offset != 0)
                {
                    lines.Append(new Vertex(new Vector2f(-width, offset), Colors.Transparent));
                    lines.Append(new Vertex(new Vector2f(width, offset), Colors.Transparent));

                    lines.Append(new Vertex(new Vector2f(offset, -width), Colors.Transparent));
                    lines.Append(new Vertex(new Vector2f(offset, width), Colors.Transparent));
                }
                offset += 100;
            }
        }

        public void Draw(IRenderTarget target, RenderStates states)
        {
            target.Draw(lines);
            target.Draw(xText);
            target.Draw(yText);
        }

        public void Dispose()
        {
            lines.Dispose();
            font.Dispose();
            xText.Dispose();
            yText.Dispose();
        }
    }
}
