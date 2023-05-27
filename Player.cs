using GLShooter.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLShooter
{
    public class Player : Creature
    {
        public Camera Camera { get; }
        public Player(Vector position, double health, Camera camera) : base(position, health)
        {
            this.Camera = camera;
        }

        public override void Move()
        {
            throw new NotImplementedException();
        }

        public override void TakeDamage(double value)
        {
            throw new NotImplementedException();
        }
    }
}
