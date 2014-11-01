using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectCamera
{
    class Personagem : IObject
    {
        private bool right = false;
        private bool left = false;
        private Skeleton skeleton;
        private Inventory inventory;
        private Vector2 currentFrame;
        private Vector2 frameSize;
        private Objeto lastPickedItem;
        private TimeSpan pickedItemSpan;
        private bool enterRoom = false;
        private bool pickItem = false;
        private bool showMessage = false;
        private String messageToShow = "";

        public bool ShowMessage
        {
            get { return this.showMessage; }
            set { this.showMessage = value; }
        }

        public String MessageToShow
        {
            get { return this.messageToShow; }
            set { this.messageToShow = value; }
        }

        public bool PickItem
        {
            get { return this.pickItem; }
            set { this.pickItem = value; }
        }

        public bool EnterRoom
        {
            get { return this.enterRoom; }
            set { enterRoom = value; }
        }

        public Personagem(Vector2 position, Texture2D texture, string nome, Vector2 frameSize, Vector2 f)
        {
            this.currentFrame = f;
            this.frameSize = frameSize;
            this.Texture = texture;
            this.Position = position;
            this.Name = nome;
            OriginalTexture = texture;
            inventory = new Inventory();
            this.bitmask = new Color[texture.Width * texture.Height];
            texture.GetData(bitmask);
        }

        public override void Draw(SpriteBatch renderer, Vector2 drawPosition, bool resolutionIndependent, GraphicsDevice device, Vector2 baseScreenSize)
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
            /*if (Velocity.X == 0 && Velocity.Y == 0)
            {

            }*/
            Color[] spriteImageData = new Color[(int)(frameSize.X * frameSize.Y)];

            OriginalTexture.GetData(0,  
                new Rectangle((int)(frameSize.X * currentFrame.X), (int)(frameSize.Y * currentFrame.Y), (int)frameSize.X, (int)frameSize.Y),
                spriteImageData,
                (int)0,
                (int)(frameSize.X * frameSize.Y));
            Texture2D tex = new Texture2D(device, (int)frameSize.X, (int)frameSize.Y);
            tex.SetData(spriteImageData);
            Texture = tex;
            renderer.Draw(tex, drawPosition, null ,Color.White);
        }

        public Vector2 CurrentFrame
        {
            get { return currentFrame; }
        }

        public Vector2 FrameSize
        {
            get { return frameSize; }
        }

        public List<Objeto> Inventory
        {
            get { return this.inventory.Slots; }
        }

        public void pickObject(Objeto objeto)
        {
            inventory.addObject(objeto);

            lastPickedItem = objeto;
            pickedItemSpan = new TimeSpan();
            pickedItemSpan += TimeSpan.FromMilliseconds(1);
        }

        public void changeFrame(Vector2 f)
        {
            this.currentFrame = f;
        }

        public Objeto dropObject(Objeto objeto)
        {
            /*GameTime gt = new GameTime();
            Random randomizer = new Random();
            Objeto obj = inventory.removeObject(objeto);
            obj.Position = new Vector2(Position.X, Position.Y- 50);
            obj.applyForce(new Vector2(randomizer.Next(50), -randomizer.Next(90)),gt);*/
            return inventory.removeObject(objeto);
        }

        public Skeleton Skeleton
        {
            get { return this.skeleton; }
            set { this.skeleton = value; }
        }

        public void updatePosition(Vector2 moveVector)      //faz o update da posição do jogador.
        {
            this.Position += moveVector;
        }

        public bool Right
        {
            get { return this.right; }
            set { this.right = value; }
        }

        public bool Left
        {
            get { return this.left; }
            set { this.left = value; }
        }



        public Objeto LastPickedItem
        {
            get { return lastPickedItem; }
            set { lastPickedItem = value; }
        }
        public TimeSpan PickedItemGameTime
        {
            get { return this.pickedItemSpan; }
            set { this.pickedItemSpan = value; }
        }
    }
}
