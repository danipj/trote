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
    public class IObject
    {
        private Texture2D texture;
        private Vector2 worldPosition;
        private string name;
        private bool estatico;
        private bool colidiu = false;
        private Vector2 velocity;
        protected Color[] bitmask;
        private String currentMap;
        private Texture2D originalTexture;
        private bool debugDraw = true;

        public Texture2D OriginalTexture
        {
            get { return originalTexture; }
            set { this.originalTexture = value; }
        }
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public bool DebugDraw
        {
            set { debugDraw = value; }
            get { return debugDraw; }
        }

        public String CurrentMap
        {
            get { return this.currentMap; }
            set { this.currentMap = value; }
        }

        public bool Colidiu
        {
            get { return this.colidiu; }
            set { this.colidiu = value; }
        }

        public Vector2 Position
        {

            get { return this.worldPosition; }
            set { this.worldPosition = value; }
        }

        public Texture2D Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }

        public bool Estatico
        {
            get { return this.estatico; }
            set { this.estatico = value; }
            
        }

        public bool CollidesWith(IObject other, bool porPixel)
        {
            int mWidth = this.Texture.Width;
            int mHeight = this.Texture.Height;
            int oWidth = other.Texture.Width;
            int oHeight = other.Texture.Height;

            if (porPixel && (Math.Max(oWidth, oHeight) > 10 || Math.Min(mWidth, mHeight) > 10))
            {
                if (this.Bounds.Intersects(other.Bounds))
                {
                    return PerPixelCollision(this, other);
                   
                }
            }

            return this.Bounds.Intersects(other.Bounds);
        }

        
        private bool PerPixelCollision(IObject a, IObject b)
        { 

            // Calculate the intersecting rectangle
            int x1 = Math.Max(a.Bounds.X, b.Bounds.X);
            int x2 = Math.Min(a.Bounds.X + a.Bounds.Width, b.Bounds.X + b.Bounds.Width);

            int y1 = Math.Max(a.Bounds.Y, b.Bounds.Y);
            int y2 = Math.Min(a.Bounds.Y + a.Bounds.Height, b.Bounds.Y + b.Bounds.Height);

            Color c;
            Color d;
            // For each single pixel in the intersecting rectangle
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the color from each texture
                    c = a.bitmask[(x - a.Bounds.X) + (y - a.Bounds.Y) * a.Texture.Width];
                    d = b.bitmask[(x - b.Bounds.X) + (y - b.Bounds.Y) * b.Texture.Width];

                    if (c.A != 0 && d.A != 0) // If both colors are not transparent (the alpha channel is not 0), then there is a collision
                    {
                        return true;
                    }
                }
            }
            // If no collision occurred by now, we're clear.
            return false;
        }

        /*DrawPosition: posição da janela
         worldPosition: posição no mundo*/

        public void applyForce(Vector2 force, GameTime gt)
        {
            //TimeSpan tempo = gt.TotalGameTime.Subtract(gt.ElapsedGameTime);F
            //this.Position = new Vector2(this.Position.X * (Velocity.X), this.Position.Y * (Velocity.Y));
            this.velocity += force;
            //this.Position = new Vector2(this.Position.X + (Velocity.X * (float)tempo2.TotalSeconds), this.Position.Y + (Velocity.Y * (float)tempo2.TotalSeconds));
        }

        public Vector2 Velocity
        {
            get { return this.velocity; }
            set { this.velocity = value; }
        }

        public void Update(GameTime gt)
        {
            TimeSpan tempo2 = gt.ElapsedGameTime;
            this.Position = new Vector2(this.Position.X + (Velocity.X * (float)tempo2.TotalSeconds), this.Position.Y + (Velocity.Y * (float)tempo2.TotalSeconds));
        }

        public virtual void Draw(SpriteBatch renderer, Vector2 drawPosition, bool resolutionIndependent, GraphicsDevice device, Vector2 baseScreenSize)
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
            renderer.Draw(this.texture, drawPosition, Color.White);

            

        }


        public Rectangle Bounds
        {
            get { return new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Texture.Width, this.Texture.Height);}
        }
    }
}