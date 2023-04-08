using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GLShooter.Geometry;

namespace GLShooter
{
    internal class Camera
    {
        

        public Vector Direction { get => direction; set => direction = value; }
        public Vector Position { get => position; set => position = value; }
        public Vector Plane { get => plane; }

        private Vector position;
        private Vector direction;

        private Vector plane;

        public Camera(Vector position, Vector direction)
        {
            this.position = position;
            this.direction = direction;
            plane = new Vector(0, 0.66);
        }
    }
}
