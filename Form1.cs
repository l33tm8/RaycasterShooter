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
            KeyUp += Form1_KeyUp;
            t.Start();
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.S)
                camera.Velocity = 0;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.D)
                camera.RotationSpeed = 0;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                camera.Velocity = 0.1;
            if (e.KeyCode == Keys.S)
                camera.Velocity = -0.1;
            if (e.KeyCode == Keys.A)
                camera.RotationSpeed = -1;
            if (e.KeyCode == Keys.D)
                camera.RotationSpeed = 1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (var x = 0; x < this.Width; x++)
            {
                var lineHeight = camera.CastWall(x, Width, Height, out int side);
                var drawStart = -lineHeight / 2 + Height / 2;

                if (drawStart < 0) drawStart = 0;
                var drawEnd = lineHeight / 2 + Height / 2;
                if (drawEnd > Height) drawEnd = Height - 1;
                var color = side == 1 ? Color.Black : Color.Gray;
                e.Graphics.DrawLine(new Pen(color), x, drawStart, x, drawEnd);

            }
            
        }

        private void TimerLoop(object sender, EventArgs e)
        {
            tickCount++;
            camera.Move();
            camera.Rotate(0.05);
            Invalidate();
        }
    }
}
