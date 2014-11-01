using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectCamera
{
    class Clickable
    {
        private Objeto obj;
        private Texture2D texture;
        private Vector2 position;
        public Clickable(Objeto obj, Texture2D tex, Vector2 position)
        {
            this.obj = obj;
            texture = tex;
            this.position = position;
        }

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public Objeto Objeto
        {
            get { return this.obj; }
            set { this.obj = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
        }

        public void Draw(SpriteBatch renderer, Vector2 drawPosition)
        {

           renderer.Draw(this.texture, drawPosition, Color.White);

        }
    }
}
