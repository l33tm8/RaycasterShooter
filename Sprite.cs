using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLShooter.Geometry;

namespace GLShooter
{
    public struct Sprite
    {
        public Vector position;
        public Texture texture;

        public Sprite(Vector position, Texture texture)
        {
            this.texture = texture;
            this.position = position;
        }
    }
}
