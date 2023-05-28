using GLShooter.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLShooter
{
    public class Enemy : Creature
    {
        public Enemy(Sprite sprite, Vector position, double health) : base(sprite, position, health)
        {
            this.Direction.X = 0.1;
            fireballTexture = new Texture(new Bitmap("Images/fire.png"));
            fireballTexture.InitializeColorArray();
            fireballs = new();
        }

        public override void Move()
        {
            var newPosition = Position + Direction;
            if (Map.mapObjects[(int)newPosition.X][(int)newPosition.Y] == 0)
                Position = newPosition;
            foreach (var fireball in fireballs.ToList())
            {
                fireball.Position += fireball.Direction;
                if (!Map.InBounds(fireball.Position))
                    fireballs.Remove(fireball);
                else if (Map.mapObjects[(int)fireball.Position.X][(int)fireball.Position.Y] != 0)
                    fireballs.Remove(fireball);
            }
        }

        public override void TakeDamage(double value)
        {
            Health -= value;
        }

        public void Fire()
        {
            var fireballSprite = new Sprite(Position, fireballTexture);
            if (Direction.Length() > 0)
            {
                var fireball = new Fireball(fireballSprite, Position + Direction)
                {
                    Direction = Direction * 2
                };
                fireballs.Add(fireball);
            }
        }

        public IEnumerable<Fireball> GetFireballs()
        {
            foreach (var fireball in fireballs.ToList())
                yield return fireball;
        }

        public void RemoveFireball(Fireball fireball)
        {
            fireballs.Remove(fireball);
        }

        private Texture fireballTexture;
        private List<Fireball> fireballs;
    }
}
