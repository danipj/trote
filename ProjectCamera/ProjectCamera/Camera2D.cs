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
    class Camera2D
    {
        private SpriteBatch renderer;           //responsável por desenhar tudo
        private Vector2 cameraPosition;         //posição da câmera no mundo
        private bool resolutionIndependent;
        private Vector2 baseScreenSize;
        private GraphicsDevice device;
        private int screenWidth, screenHeight;
        private bool following;
        private IObject followedObject;

        public bool Follow
        {
            get { return following; }
            set
            {
                if (value == false)
                    cameraPosition = Vector2.Zero;
                this.following = value;
            }
        }

        public void followObject(IObject obj)
        {
            if (obj == null)
            {
                following = false;
                followedObject = null;
            }
            else
            {
                //cameraPosition = new Vector2(obj.Position.X - renderer.GraphicsDevice.Viewport.Width / 2, obj.Position.Y - renderer.GraphicsDevice.Viewport.Height / 2);
                
                cameraPosition = new Vector2(obj.Position.X - renderer.GraphicsDevice.Viewport.Width / 2, obj.Position.Y - renderer.GraphicsDevice.Viewport.Height / 2);
                following = true;
                followedObject = obj;
            }
        }

        public Vector2 BaseScreenSize
        {
            get { return this.baseScreenSize; }
            set { this.baseScreenSize = value; }
        }

        public bool ResolutionIndependent
        {
            get { return this.resolutionIndependent; }
            set { resolutionIndependent = value; }
        }

        public Vector2 Position     
        {
            get { return this.cameraPosition;}
            set { this.cameraPosition = value; }
        }

        public SpriteBatch Batch
        {
            get { return this.renderer; }
        }

        public Camera2D(SpriteBatch renderer, GraphicsDevice device, Vector2 baseScreenSize, bool dependent)
        {
            this.renderer = renderer;
            this.device = device;
            this.baseScreenSize = baseScreenSize;
            resolutionIndependent = dependent;
            cameraPosition = new Vector2(0, 0);
            if (resolutionIndependent)
            {
                screenWidth = (int)baseScreenSize.X;
                screenHeight = (int)baseScreenSize.Y;
            }
            else
            {
                screenWidth = device.PresentationParameters.BackBufferWidth;
                screenHeight = device.PresentationParameters.BackBufferHeight;
            }
        }

        public void drawObject(IObject Object, Vector2 limit)
        {
            if (following)
            {
                //pega a posicao do Object;
                //cameraPosition = new Vector2(followedObject.Position.X - baseScreenSize.X / 2, followedObject.Position.Y - baseScreenSize.Y / 2);
                if (Object.Position.X - BaseScreenSize.X / 2 < 0)
                    cameraPosition = new Vector2(0, 0);
                else if ((Object.Position.X + BaseScreenSize.X / 2 > limit.X))
                    cameraPosition = new Vector2(limit.X - BaseScreenSize.X, 0);
                else
                    cameraPosition = new Vector2(followedObject.Position.X - baseScreenSize.X / 2, 0);
                    
                Vector2 drawPosition = applyTransformations(Object.Position);       //Calculamos a posição que o objeto deve ser desenhado com a movimentação da câmera
                Object.Draw(renderer, drawPosition, resolutionIndependent, device,baseScreenSize);                                //Desenhamos o objeto
            }
            else
            {
                //pega a posicao do Object;
                Vector2 drawPosition = applyTransformations(Object.Position);       //Calculamos a posição que o objeto deve ser desenhado com a movimentação da câmera
                Object.Draw(renderer, drawPosition, resolutionIndependent, device, baseScreenSize); 
            }
        }

        public void drawPortal(Portal portal, Map currentMap)
        {
            Vector2 drawPosition1 = applyTransformations(portal.Position1);
            Vector2 drawPosition2 = applyTransformations(portal.Position2);
            portal.Draw(renderer, drawPosition1, drawPosition2, resolutionIndependent, device, baseScreenSize, currentMap);
        }

        public void drawMap(Map mapa)
        {
            Vector2 drawPosition = applyTransformations(mapa.Position);         //Mesma coisa que o método de cima
            mapa.Draw(renderer, drawPosition, resolutionIndependent, device, baseScreenSize);                          
        }

        public void drawClickable(Clickable click, Map currentMap)
        {
            Vector2 drawPosition1 = applyTransformations(click.Position);
            click.Draw(renderer, drawPosition1);
        }

        public void drawEvent(Event click, Map currentMap)
        {
            Vector2 drawPosition1 = applyTransformations(click.Position);
            click.Draw(renderer, drawPosition1);
        }

        private Vector2 applyTransformations(Vector2 position)
        {
            //calcula onde o nó vai ser desenhado
            Vector2 finalPosition = position - cameraPosition;                  //Aqui é onde nós saberemos onde desenhar o objeto com a movimentação da câmera. Sabemos que no xna y aumenta pra baixo e diminui para cima.
            //Aqui vc define rotação, escala...                                 //logo, se a câmera for para baixo, subtraimos a posição da câmera da posição do objeto, oq fará com que ele suba.
            return finalPosition;                                               //O mesmo principio se encaixa para direita e esquerda.
        }

        public void translate(Vector2 moveVector)
        {
            cameraPosition += moveVector;                                       //Move a câmera
        }
    }
}
