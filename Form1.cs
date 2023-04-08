using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GLShooter.Geometry;

namespace GLShooter
{

    internal class Form1 : Form
    {
        private System.Windows.Forms.Timer t;

        private int tickCount = 0;
        private Camera camera;
        
        public Form1()
        {
            this.Width = 600;
            this.Height = 600;
            this.DoubleBuffered = true;
            t = new System.Windows.Forms.Timer();
            t.Interval = 15;
            t.Tick += TimerLoop;
            camera = new Camera(new Vector(12, 12), new Vector(-1.0, 0.0));
            KeyDown += Form1_KeyDown;
            t.Start();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                camera.Position += camera.Direction * 0.05;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (var x = 0; x < this.Width; x++)
            {
                var cameraX = 2 * x / (double)this.Width - 1;
                var rayDir = camera.Direction + camera.Plane * cameraX;
                var deltaDist = new Vector(Math.Abs(1 / rayDir.X), Math.Abs(1 / rayDir.Y));
                var sideDist = new Vector(0, 0);
                var perpWallDist = 0.0;

                var mapX = camera.Position.X;
                var mapY = camera.Position.Y;
                var stepX = 0;
                var stepY = 0;
                var isHitted = false;
                var side = 0;

                if (rayDir.X < 0)
                {
                    stepX = -1;
                    sideDist.X = (camera.Position.X - mapX) * deltaDist.X;
                }
                else
                {
                    stepX = 1;
                    sideDist.X = (mapX + 1.0 - camera.Position.X) * deltaDist.X;
                }
                if (rayDir.Y < 0)
                {
                    stepY = -1;
                    sideDist.Y = (camera.Position.Y - mapY) * deltaDist.Y;
                }
                else
                {
                    stepY = 1;
                    sideDist.Y = (mapY + 1.0 - camera.Position.Y) * deltaDist.Y;
                }

                while (!isHitted)
                {
                    if (sideDist.X < sideDist.Y)
                    {
                        sideDist.X += deltaDist.X;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDist.Y += deltaDist.Y;
                        mapY += stepY;
                        side = 1;
                    }

                    if (Map.mapObjects[(int)mapX][(int)mapY] > 0) isHitted = true;
                }
                if (side == 0)
                    perpWallDist = sideDist.X - deltaDist.X;
                else
                    perpWallDist = sideDist.Y - deltaDist.Y;

                var lineHeight = (int)(this.Height / perpWallDist);
                var drawStart = -lineHeight / 2 + Height / 2;

                if (drawStart < 0) drawStart = 0;
                var drawEnd = lineHeight / 2 + Height / 2;
                if (drawEnd > Height) drawEnd = Height - 1;
                var color = side == 1 ? Color.Black : Color.Gray;
                e.Graphics.DrawLine(new Pen(color), x, drawStart, x, drawEnd);

            }
            camera.Direction.Rotate(0.01);
            camera.Plane.Rotate(0.01);
        }

        private void TimerLoop(object sender, EventArgs e)
        {
            tickCount++;
            Invalidate();
        }
    }
}
