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
using Indiefreaks.AOP.Profiler;
using Indiefreaks.Xna.Profiler;
namespace ProjectCamera
{
    class Portal
    {
        private Map map1;
        private Map map2;
        private int sideMap1;
        private int sideMap2;
        private Vector2 positionMap1;
        private Vector2 positionMap2;
        private Texture2D texture;
        private bool door;

        public bool isDoor
        {
            get { return this.door; }
        }

        public Vector2 Position1
        {
            get { return this.positionMap1; }
            set { this.positionMap1 = value; }
        }

        public Vector2 Position2
        {
            get { return this.positionMap2; }
            set { this.positionMap2 = value; }
        }

        public Map Map1
        {
            get { return this.map1; }
        }

        public Map Map2
        {
            get { return this.map2; }
        }

        public int SideMap1
        {
            get { return this.sideMap1; }
        }

        public int SideMap2
        {
            get { return this.sideMap2; }
        }

        public Portal(Map map1, Map map2, Vector2 positionMap1, Vector2 positionMap2, Texture2D texture, int sideMap1, int sideMap2, bool door = false)
        {
            this.map1 = map1;
            this.map2 = map2;
            this.positionMap1 = positionMap1;
            this.positionMap2 = positionMap2;
            this.sideMap1 = sideMap1;
            this.sideMap2 = sideMap2;
            this.texture = texture;
            this.door = door;
        }

        public void Draw(SpriteBatch renderer, Vector2 drawPosition1, Vector2 drawPosition2, bool resolutionIndependent, GraphicsDevice device, Vector2 baseScreenSize, Map currentMap)
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

            if (currentMap.Equals(map1))
            {
                if (map1.CurrentSide.SideID == sideMap1)
                    renderer.Draw(this.texture, drawPosition1, Color.White);
            }
            else if (currentMap.Equals(map2))
            {
                if (map1.CurrentSide.SideID == sideMap2)
                    renderer.Draw(this.texture, drawPosition2, Color.White);
            }

        }

        public Rectangle Bounds1
        {
            get { return new Rectangle((int)this.Position1.X, (int)this.Position1.Y, this.texture.Width, this.texture.Height); }
        }

        public Rectangle Bounds2
        {
            get { return new Rectangle((int)this.Position2.X, (int)this.Position2.Y, this.texture.Width, this.texture.Height); }
        }
    }
}
