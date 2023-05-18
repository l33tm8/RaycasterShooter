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
        public double Velocity { get; set; }
        public double RotationSpeed { get; set; }

        private Vector position;
        private Vector direction;

        private Vector plane;

        public Camera(Vector position, Vector direction)
        {
            this.position = position;
            this.direction = direction;
            plane = new Vector(0, 0.66);
            Velocity = 0;
        }


        public int CastWall(int x, int width, int height, out int side, out int mapX, out int mapY, out int texX, out double perpWallDist)
        {
            var cameraX = 2 * x / (double)width - 1;
            var rayDir = Direction + Plane * cameraX;
            var deltaDist = new Vector(Math.Abs(1 / rayDir.X), Math.Abs(1 / rayDir.Y));

            perpWallDist = 0;
            mapX = (int)Position.X;
            mapY = (int)Position.Y;
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
            texX = 0;
            if (!isHitted)
                return 0;
            if (side == 0)
                perpWallDist = sideDist.X - deltaDist.X;
            else
                perpWallDist = sideDist.Y - deltaDist.Y;
            var hitX = side == 0 ? (Position.Y + perpWallDist * rayDir.Y) : (Position.X + perpWallDist * rayDir.X);
            texX = (int)((hitX - Math.Floor(hitX)) * 64);
            
            
            if (side == 0 && rayDir.X > 0) hitX = 64 - hitX - 1;
            if (side == 1 && rayDir.Y < 0) hitX = 64 - hitX - 1;
            return (int)(height / perpWallDist);

        }

        public void Rotate(double angle)
        {
            Plane.Rotate(angle * RotationSpeed);
            Direction.Rotate(angle * RotationSpeed);
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

        public void Move()
        {
            var newPosition = Position + Direction * Velocity;
            if (Map.mapObjects[(int)newPosition.X][(int)newPosition.Y] == 0)
                Position = newPosition;
        }
    }
}
