using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectCamera
{
    class Event
    {
        private List<Objeto> obj;
        private String obj2;
        private Texture2D texture;
        private Vector2 position;
        private String mensagem;
        public Event(List<Objeto> objParaDar, String objParaVerificar, Texture2D tex, Vector2 position, String mensagem)
        {
            this.mensagem = mensagem;
            this.obj2 = objParaVerificar;
            obj = objParaDar;
            texture = tex;
            this.position = position;
        }

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public List<Objeto> ObjetoParaDar
        {
            get { return this.obj; }
            set { this.obj = value; }
        }

        public String ObjetoParaVerificar
        {
            get { return this.obj2; }
            set { this.obj2 = value; }
        }

        public Texture2D Texture
        {
            get { return this.texture; }
        }

        public String Mensagem
        {
            get { return this.mensagem; }
            set { this.mensagem = value; }
        }

        public bool eventReturn(Personagem per)
        {
            if (obj != null)
            {
                if (obj2.Equals(""))
                {
                    for (int o = 0; o < obj.Count; o++)
                    {
                        per.pickObject(obj[o]);
                    }
                    return true;
                }
                else
                {
                    for (int i = 0; i < per.Inventory.Count; i++)
                    {
                        if (per.Inventory[i].Name.Equals(obj2) || obj2.Equals(""))
                        {
                            for (int o = 0; o < obj.Count; o++)
                            {
                                per.pickObject(obj[o]);
                            }
                            per.Inventory.Remove(per.Inventory[i]);
                            return true;
                        }
                    }
                }
                per.MessageToShow = mensagem;
                per.ShowMessage = true;
                per.PickedItemGameTime = TimeSpan.FromMilliseconds(1);
                return false;
            }
            else
            {
                per.MessageToShow = mensagem;
                per.ShowMessage = true;
                per.PickedItemGameTime = TimeSpan.FromMilliseconds(1);
                return false;
            }
        }

        public void Draw(SpriteBatch renderer, Vector2 drawPosition)
        {
            renderer.Draw(this.texture, drawPosition, Color.White);
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Texture.Width, this.Texture.Height); }
        }
    }
}
