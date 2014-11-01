using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProjectCamera
{
    class Objeto : IObject
    {
        private bool pickedUp = false;
        private float mass;
        private bool pickable;

        public Objeto(Vector2 posicao, Texture2D textura, bool pickedUp, string nome, float mass, bool pickable, bool estatico = true)
        {
            this.Position = posicao;
            this.Texture = textura;
            this.pickedUp = pickedUp;
            this.Name = nome;
            this.Estatico = estatico;
            this.mass = mass;
            this.pickable = pickable;
            this.bitmask = new Color[textura.Width * textura.Height];
            textura.GetData(bitmask);
        }

        public bool Pickable
        {
            get { return this.pickable; }
        }

        public bool PickedUp            //variável que vai definir se o objeto será desenhado ou não.
        {
            set { this.pickedUp = value; }
            get { return this.pickedUp; }
        }

        public float Mass
        {
            get { return this.mass; }
            set { this.mass = value; }
        }

    }
}
