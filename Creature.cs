using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLShooter.Geometry;

namespace GLShooter
{
    public abstract class Creature
    {
        public Sprite Sprite { get => sprite; }
        public Vector Position { get => sprite.position; set => sprite.position = value; }
        public double Health { get; set; }
        public Vector Direction { get; set; }

        public Creature(Sprite sprite, Vector position, double health)
        {
            this.sprite = sprite;
            this.Direction = new Vector(0.0, 0.0);
            this.Health = health;
        }

        public Creature(Vector position, double health)
        {
            Position = position;
            Health = health;
            Direction = new Vector(0.0, 0.0);
        }

        public abstract void Move();
        public abstract void TakeDamage(double value);

        public bool IsCollidedWith(Creature other)
        {
            var distance = (other.Position - Position).Length();
            return distance < 0.4;
        }

        public bool IsCollidedWith(Sprite other)
        {
            var distance = (other.position - Position).Length();
            return distance < MaximumCollideDistance;
        }

        private const double MaximumCollideDistance = 0.4;
        private Sprite sprite;

    }
}
