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
                if (Map.mapObjects[(int)fireball.Position.X][(int)fireball.Position.Y] != 0)
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
            var fireball = new Fireball(fireballSprite, Position + Direction);
            fireball.Direction = Direction;
            fireballs.Add(fireball);
        }

        public IEnumerable<Fireball> GetFireballs()
        {
            foreach (var fireball in fireballs)
                yield return fireball;
        }

        private Texture fireballTexture;
        private List<Fireball> fireballs;
    }
}
