using GLShooter.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLShooter
{
    public class Fireball : Creature
    {
        public Fireball(Sprite sprite, Vector position) : base(sprite, position, 0)
        {
         
        }

        public override void Move()
        {
            Position += Direction;
        }

        public override void TakeDamage(double value)
        {
            throw new NotImplementedException();
        }
    }
}
