using System;

namespace GLShooter.Geometry
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector operator -(Vector a, Vector b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector operator *(Vector a, int b) => new(a.X * b, a.Y * b);
        public static Vector operator *(Vector a, double b) => new(a.X * b, a.Y * b);
        public double Length() => Math.Sqrt(X * X + Y * Y);

        public void Rotate(double angle)
        {
            var tempX = X;
            var tempY = Y;
            X = tempX * Math.Cos(-angle) - tempY * Math.Sin(-angle);
            Y = tempX * Math.Sin(-angle) + tempY * Math.Cos(-angle);
        }
        public Vector (double x, double y)
        {
            X = x;
            Y = y;
        }     
    }
}
