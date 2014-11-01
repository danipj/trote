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
using Microsoft.Kinect;


namespace ProjectCamera
{
    class Inventory
    {
        private List<Objeto> objetos;

        public Inventory()
        {
            objetos = new List<Objeto>();
        }

        public void addObject(Objeto obj)
        {
            if (objetos.Count < 10)
                objetos.Add(obj);
        }

        public Objeto removeObject(Objeto obj)
        {
            Objeto objetoRemovido = null;
            foreach (Objeto o in objetos)
            {
                if (o == obj)
                {
                    objetoRemovido = o;
                }
            }
            objetos.Remove(obj);
            return objetoRemovido;
        }

        public List<Objeto> Slots
        {
            get { return this.objetos; }
        }

    }
}
