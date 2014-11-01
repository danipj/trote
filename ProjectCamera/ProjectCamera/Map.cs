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
    class Map
    {
        private MapSide background1;
        private MapSide background2;
        //private List<Objeto> objetosNoMapa;
        private String name;
        private Vector2 position;      
        private MapSide backgroundAtual;
        

        public Map(Texture2D background1, Texture2D background2, String name)
        {
            this.name = name;
            this.background1 = new MapSide(background1, 1, name);
            this.background2 = new MapSide(background2, 2, name);
            this.position = Vector2.Zero;
            backgroundAtual = this.background1;
        }

        public MapSide CurrentSide
        {
            get { return this.backgroundAtual; }
        }

        public MapSide getSide(int side)
        {
            if (side == 1)
                return background1;
            else if (side == 2)
                return background2;

            return null;
        }

        public void changeBackground(int side = -1)
        {
            if (side != -1)
            {
                if (side == 1)
                {
                    backgroundAtual = background1;
                }
                else if (side == 2)
                {
                    if (background2 != null)
                        backgroundAtual = background2;
                }
            }
            else
            {
                if (backgroundAtual == background1)
                {
                    if (background2 != null)
                        backgroundAtual = background2;
                }
                else if (backgroundAtual == background2)
                {
                    backgroundAtual = background1;
                }
            }

        }

        public void addObject(Objeto objeto, int side)
        {
            if (side == 1)
            {
                if (background1.Objects.Count >= 1)
                {
                    Objeto chao = background1.Objects[background1.Objects.Count - 1];
                    background1.Objects.RemoveAt(background1.Objects.Count - 1);
                    background1.Objects.Add(objeto);      //adiciona algum objeto no mapa
                    background1.Objects.Add(chao);
                }
                else
                {
                    background1.Objects.Add(objeto);
                }
                objeto.CurrentMap = this.name;
            }
            else if (side == 2)
            {
                if (background2.Objects.Count >= 1)
                {
                    Objeto chao = background2.Objects[background2.Objects.Count - 1];
                    background2.Objects.RemoveAt(background2.Objects.Count - 1);
                    background2.Objects.Add(objeto);      //adiciona algum objeto no mapa
                    background2.Objects.Add(chao);
                }
                else
                {
                    background2.Objects.Add(objeto);
                }
                objeto.CurrentMap = this.name;
            }
        }

        public void addPortal(Map map1, Map map2, Vector2 pos1, Vector2 pos2, Texture2D texture, int side, int side2)
        {
            if (side == 1)
            {
                background1.Portals.Add(new Portal(map1, map2, pos1, pos2, texture, side, side2));
            }
            else if (side == 2)
            {
                background2.Portals.Add(new Portal(map1, map2, pos1, pos2, texture, side, side2));
            }
        }

        public void addDoor(Map map1, Map map2, Vector2 pos1, Vector2 pos2, Texture2D texture, int side, int side2)
        {
            if (side == 1)
            {
                background1.Portals.Add(new Portal(map1, map2, pos1, pos2, texture, side, side2, true));
            }
            else if (side == 2)
            {
                background2.Portals.Add(new Portal(map1, map2, pos1, pos2, texture, side, side2, true));
            }
        }

        public void addClickable(Objeto obj, Texture2D tex, int side, Vector2 position )
        {
            if (side == 1)
            {
                background1.addClickable(obj, tex, position);
            }
            else if (side == 2)
            {
                background2.addClickable(obj, tex, position);
            }
        }

        public void addEvent(List<Objeto> objParaDar, String objetoParaVerificar, Texture2D t, int side, Vector2 pos, String mensagem)
        {
            if (side == 1)
            {
                background1.addEvent(objParaDar, objetoParaVerificar, t, pos, mensagem);
            }
            else if (side == 2)
            {
                background2.addEvent(objParaDar, objetoParaVerificar, t, pos, mensagem);
            }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public Vector2 Position
        {
            get { return this.position;}
        }

        public void Draw(SpriteBatch renderer, Vector2 drawPosition, bool resolutionIndependent, GraphicsDevice device, Vector2 baseScreenSize)
        {
            Vector3 screenScalingFactor;
            if (resolutionIndependent)
            {
                float horScaling = (float)device.PresentationParameters.BackBufferWidth / baseScreenSize.X;
                float verScaling = (float)device.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
                screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            }
            else
            {
                screenScalingFactor = new Vector3(1, 1, 1);
            }
            Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);
            
            if (this.backgroundAtual.Texture!=null)
            renderer.Draw(this.backgroundAtual.Texture, drawPosition, Color.White);
        }

    }
}
