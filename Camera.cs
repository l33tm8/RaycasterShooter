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

        public int CastWall(int x, int width, int height, out int side)
        {
            var cameraX = 2 * x / (double)width - 1;
            var rayDir = Direction + Plane * cameraX;
            var deltaDist = new Vector(Math.Abs(1 / rayDir.X), Math.Abs(1 / rayDir.Y));

            double perpWallDist;
            var mapX = (int)Position.X;
            var mapY = (int)Position.Y;
            var stepX = rayDir.X < 0 ? -1 : 1;
            var stepY = rayDir.Y < 0 ? -1 : 1;
            var isHitted = false;
            side = 0;
            var sideDist = GetSideDist(rayDir, mapX, mapY, deltaDist);

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

            return (int)(height / perpWallDist);

        }

        public void Rotate(double angle)
        {
            Plane.Rotate(angle);
            Direction.Rotate(angle);
        }

        private Vector GetSideDist(Vector rayDir, double mapX, double mapY, Vector deltaDist)
        {
            var sideDist = new Vector(0, 0);
            if (rayDir.X < 0)
                sideDist.X = (Position.X - mapX) * deltaDist.X;
            else
                sideDist.X = (mapX + 1.0 - Position.X) * deltaDist.X;

            if (rayDir.Y < 0)
                sideDist.Y = (Position.Y - mapY) * deltaDist.Y;
            else
                sideDist.Y = (mapY + 1.0 - Position.Y) * deltaDist.Y;
            return sideDist;
        }
    }
}
