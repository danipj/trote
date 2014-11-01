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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ClassePrincipal : Microsoft.Xna.Framework.Game
    {
        [FlagsAttribute]
        public enum GameState
        {
            MainMenu = 0,
            MainMenuOptions = 1,
            Inventory = 2,
            Game = 3,
            Paused = 4,
            VideoOptions = 5,
            SoundOptions = 6,
            Ajuda = 7,
            Creditos =8
        }
        [FlagsAttribute]
        public enum MenuState
        {
            Jogar = 0,
            Opcoes = 1,
            Sair = 2,
            Creditos = 3
        }
        [FlagsAttribute]
        public enum VideoMenuState
        {
            Resolution800x600 = 0,
            Resolution1024x768 = 1,
            Resolution1440x900 = 2,
            Resolution1680x1050 = 3,
            Fullscreen = 4,
            Sair = 5
        }
        [FlagsAttribute]
        public enum SoundMenuState
        {
            Som = 1,
            Musica = 2,
            Sair = 3
        }
        [FlagsAttribute]
        public enum OptionsMenuState
        {
            VideoOptions = 1,
            SoundOptions = 2,
            Ajuda = 3,
            Sair = 4
        }
        [FlagsAttribute]
        public enum PauseMenuState
        {
            Resume = 0,
            Options = 1,
            MainMenu = 2
        }        

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        KinectSensor sensor;
        private Texture2D texturaPersonagem;
        private Camera2D camera;

        /*FONTS*/
        private SpriteFont font;
        private SpriteFont menuTitle;
        private SpriteFont menuOptions;
        /*FONTS*/

        private IWorld world;
        private Personagem player;
        private Objeto lol;
        private Objeto lol2;
        private Objeto lol3;
        private Objeto lol4;
        private Objeto lol5;
        private Objeto lol6;
        private Objeto floor;
        private Objeto leftLimit;
        private Objeto rightLimit;

        /*STATES*/
        private GameState gameState;
        private MenuState menuState;
        private OptionsMenuState menuOptionsState;
        private PauseMenuState pauseState;
        private SoundMenuState soundState;
        private VideoMenuState videoState;
        /*STATES*/

        private KeyboardState oldState;
        private float _optionTime = 0;
        private Skeleton playerSkeleton;
        private Skeleton[] skeletonData;
        private Vector2 RESOLUTION = new Vector2(1440,900);
        private Vector2 baseScreenSize = new Vector2(1440, 900);
        private int screenWidth, screenHeight;
        

        /*TEXTURAS DE MENUS*/
        private Texture2D mainMenu;
        private Texture2D pause;
        private Texture2D inventory;
        private Texture2D portalTexture;
        private Texture2D options;
        private Texture2D videoOptions;
        private Texture2D soundOptions;
        private Texture2D ajuda;
        private Texture2D creditos;
        private Texture2D colorVideo = null;
        private Texture2D sheetTexture;
        private Texture2D jointTexture;
        private Texture2D limitTexture;
        /*TEXTURAS DE MENUS*/

        BasicEffect effect;

        Objeto obj1;
        Objeto obj2;
        public bool paused = false;

        private float elapsedTime = 0;
        private float frameCounter = 0;
        private float displayableFrame = 0;
        Song s;
        Light2D testLight;
        public ClassePrincipal()
        {
            
            graphics = new GraphicsDeviceManager(this);
            oldState = Keyboard.GetState();                 //aqui pegamos o estado inicial do teclado (nada apertado) isso servirá como comparação mais tarde.
            Content.RootDirectory = "Content";
            
            //var profilerGameComponent = new ProfilerGameComponent(this, "Font"); 
            //ProfilingManager.Run = true; Components.Add(profilerGameComponent);
        }


        protected override void Initialize()
        {
           /* sensor = KinectSensor.KinectSensors[0];
            sensor.Start();
            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            sensor.ElevationAngle = 0;
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);*/

            //this.IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;

            graphics.PreferredBackBufferHeight = 900;           //altura da tela 600
            graphics.PreferredBackBufferWidth = 1440;            //largura da tela 800
            graphics.ApplyChanges();
            world = new IWorld(spriteBatch, 2.3f);  //2.3

            base.Initialize();
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            //Get raw image
            ColorImageFrame colorVideoFrame = e.OpenColorImageFrame();

            if (colorVideoFrame != null)
            {
                //Create array for pixel data and copy it from the image frame
                Byte[] pixelData = new Byte[colorVideoFrame.PixelDataLength];
                colorVideoFrame.CopyPixelDataTo(pixelData);

                //Convert RGBA to BGRA
                Byte[] bgraPixelData = new Byte[colorVideoFrame.PixelDataLength];
                for (int i = 0; i < pixelData.Length; i += 4)
                {
                    bgraPixelData[i] = pixelData[i + 2];
                    bgraPixelData[i + 1] = pixelData[i + 1];
                    bgraPixelData[i + 2] = pixelData[i];
                    bgraPixelData[i + 3] = (Byte)255; //The video comes with 0 alpha so it is transparent
                }

                // Create a texture and assign the realigned pixels
                colorVideo = new Texture2D(graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                colorVideo.SetData(bgraPixelData);

                colorVideoFrame.Dispose();
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (!((KinectSensor)sender).SkeletonStream.IsEnabled)
            {
                return;
            }

            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != frame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[frame.SkeletonArrayLength];
                    }
                    frame.CopySkeletonDataTo(skeletonData);

                    if (skeletonData != null)
                    {
                        foreach (Skeleton skele in skeletonData)
                        {
                            if (skele.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                playerSkeleton = skele;
                                player.Skeleton = skele;
                            }
                        }
                    }
                    frame.Dispose();
                }

            }
        }

        int yCoord;
        public void showInventory()
        {
            gameState = GameState.Inventory;
            yCoord = -400;
        }

        public void resetEverything()
        {
            world = new IWorld(spriteBatch,2.3f);
            LoadContent();
        }

        protected override void LoadContent()        {
            s = Content.Load<Song>("Trote - soundtrack");
            MediaPlayer.Play(s);
            
            testLight = new Light2D(spriteBatch, Color.White, 1, 100, new Vector2(50, 50), GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainMenu = Content.Load<Texture2D>("MENU INICIAL FINAL PORRA com o nome 1440");
            font = Content.Load<SpriteFont>("Font");                            //Uma font pra poder escrever na tela
            menuTitle = Content.Load<SpriteFont>("MainMenuTitle");
            menuOptions = Content.Load<SpriteFont>("MainMenuOptions");
            pause = Content.Load<Texture2D>("pause");
            portalTexture = Content.Load<Texture2D>("portal");
            videoOptions = Content.Load<Texture2D>("imgoptions");
            soundOptions = Content.Load<Texture2D>("audiooptions");
            ajuda = Content.Load<Texture2D>("ajuda_final");
            creditos = Content.Load<Texture2D>("créditosss");
            options = Content.Load<Texture2D>("options");
            //sheetTexture = Content.Load<Texture2D>("walkingSheet250x330");
            sheetTexture = Content.Load<Texture2D>("PORRACANSADA");
            jointTexture = Content.Load<Texture2D>("joint");

            limitTexture = Content.Load<Texture2D>("limitTexture");
    
            player = new Personagem(new Vector2(400,0), sheetTexture, "Jogador",new Vector2(300,625), new Vector2(0,0));//Cria o personagem (posicao, textura, nome)
            
            Map mapa1 = new Map(Content.Load<Texture2D>("corredor1_2_1"), Content.Load<Texture2D>("corredor2_1_2"), "Mapa1");     //
            Map mapa2 = new Map(Content.Load<Texture2D>("corredor1_1_1"), Content.Load<Texture2D>("corredor2_2_2"), "Mapa2");     //Cria os mapas (textura, nome)
            Map labEnf = new Map(Content.Load<Texture2D>("enf1440"), null, "LabEnf");
            Map lapa = new Map(Content.Load<Texture2D>("lapa1440"), null, "Lapa");
            Map moniHum = new Map(Content.Load<Texture2D>("humani1440"),null,"Monitoria");
            Map labBio =  new Map(Content.Load<Texture2D>("bio+nro_14"), Content.Load<Texture2D>("bio+lamp_14"), "LabBio");
            Map exatas = new Map(null, Content.Load<Texture2D>("exatas1440"), "exatas");
            Map claudio = new Map( Content.Load<Texture2D>("claudio"), null,"Claudio");

            leftLimit = new Objeto(new Vector2(0, 0), limitTexture, false, "paredeEsquerda", 0, false, true);
            rightLimit = new Objeto(new Vector2(4096, 0), limitTexture, false, "paredeEsquerda", 0, false, true);


            floor = new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height -2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true);
            inventory = Content.Load<Texture2D>("invent");
        

            //chao
            mapa1.addObject(floor, 1);
            mapa1.addObject(floor, 2);  
            mapa2.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height -2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true), 1);
            mapa2.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height - 2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true),2 );
                //lado1
            lapa.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height - 2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true), 1);
            lapa.addObject(new Objeto(new Vector2(1440, 0), limitTexture, false, "ParedeLapa", 0, false, true), 1);            
            labEnf.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height - 2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true), 1);
            labEnf.addObject(new Objeto(new Vector2(1440, 0), limitTexture, false, "ParedeEnf", 0, false, true), 1);
            moniHum.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height - 2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true), 1);
            moniHum.addObject(new Objeto(new Vector2(1440, 0), limitTexture, false, "ParedeHumani", 0, false, true), 1);
            claudio.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height - 2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true), 1);
            claudio.addObject(new Objeto(new Vector2(1440, 0), limitTexture, false, "ParedeClaudio", 0, false, true), 1);
            
            
                //lado2
            labBio.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height - 2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true), 2);
            labBio.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height - 2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true), 1);
            labBio.addObject(new Objeto(new Vector2(1440, 0), limitTexture, false, "ParedeBio", 0, false, true), 2);
            labBio.addObject(new Objeto(new Vector2(1440, 0), limitTexture, false, "ParedeBio2", 0, false, true), 1);
            exatas.addObject(new Objeto(new Vector2(0, mapa1.CurrentSide.Texture.Height - 2), Content.Load<Texture2D>("floor"), false, "Chão", 0f, false, true), 2);
            exatas.addObject(new Objeto(new Vector2(0-limitTexture.Width, 0), limitTexture, false, "ParedeExatas", 0, false, true), 2);
           

           
            //paredes extremas
            mapa1.addObject(new Objeto(new Vector2(0, 0), limitTexture, false, "paredeEsquerda", 0, false, true), 2);
            mapa1.addObject(new Objeto(new Vector2(4096, 0), limitTexture, false, "paredeEsquerda", 0, false, true), 1);
            mapa2.addObject(new Objeto(new Vector2(0, 0), limitTexture, false, "paredeEsquerda", 0, false, true), 1);
            mapa2.addObject(new Objeto(new Vector2(4096, 0), limitTexture, false, "paredeEsquerda", 0, false, true), 2);

            labEnf.addClickable(new Objeto(new Vector2(500, 450), Content.Load<Texture2D>("luv"), false, "Luva", 0, true, false), portalTexture, 1, new Vector2(500, 450));

            ///Objetos
            List<Objeto> objs = new List<Objeto>();
            objs.Add(new Objeto(new Vector2(1150, 600), Content.Load<Texture2D>("chave"), false, "Chave", 0, true, false));
            labEnf.addEvent(objs, "Luva", portalTexture, 1, new Vector2(1150, 600), "Nao e nada higienico mexer em curativos sem luvas!");
            objs = new List<Objeto>();
            objs.Add(new Objeto(new Vector2(170, 272), Content.Load<Texture2D>("estojo"), false, "Estojo", 0, true, false));
            objs.Add(new Objeto(new Vector2(170, 272), Content.Load<Texture2D>("fenda"), false, "Chave de Fenda", 0, true, false));
            mapa2.addEvent(objs, "Chave", portalTexture, 1, new Vector2(230, 272), "");
            objs = new List<Objeto>();
            objs.Add(new Objeto(new Vector2(792, 900 - player.Texture.Height), Content.Load<Texture2D>("lampada"), false, "Lampada", 0, true, false));
            moniHum.addEvent(objs, "", portalTexture, 1, new Vector2(792, 300), "");
            objs = new List<Objeto>();
            objs.Add(new Objeto(new Vector2(1206, 500 - player.Texture.Height), Content.Load<Texture2D>("cad"), false, "Caderno", 0, true, false));
            lapa.addEvent(objs, "Chave de Fenda", portalTexture, 1, new Vector2(1206, 500), "");

            objs = new List<Objeto>();
            objs.Add(new Objeto(new Vector2(800, 500 - player.Texture.Height), Content.Load<Texture2D>("lumin"), false, "      ", 0, true, false));
            labBio.addEvent(objs, "Lampada", portalTexture, 2, new Vector2(800, 500), "");
            
            //addPortal(mapa1, mapa2,
            //  posMapa1,
            //  posMapa2,
            //  textura do portal,
            //
            //
            //);
            
            mapa2.addPortal(mapa2, mapa1, 
                  new Vector2(mapa1.CurrentSide.Texture.Width, 830 - portalTexture.Height), 
                  new Vector2(-50, 830 - portalTexture.Height),
                  portalTexture, 1, 1);
            mapa1.addPortal(mapa1, mapa2, 
                new Vector2(-50, 830 - portalTexture.Height), 
                new Vector2(mapa2.CurrentSide.Texture.Width, 830 - portalTexture.Height), 
                portalTexture, 1, 1);

            mapa2.addPortal(mapa2, mapa1,
                  new Vector2(-50, 830 - portalTexture.Height),
                  new Vector2(mapa1.CurrentSide.Texture.Width, 830 - portalTexture.Height),                  
                  portalTexture, 2, 2);
            mapa1.addPortal(mapa1, mapa2,                
                new Vector2(mapa2.CurrentSide.Texture.Width, 830 - portalTexture.Height),
                new Vector2(-50, 830 - portalTexture.Height),
                portalTexture, 2, 2);


            //doors
            mapa2.addDoor(mapa2, labEnf,
                  new Vector2(1000, 500),
                  new Vector2(0 - portalTexture.Width, 800),
                  portalTexture, 1, 1);

            mapa2.addDoor(mapa2, moniHum,
                  new Vector2(3719, 450),
                  new Vector2(0 - portalTexture.Width, 800),
                  portalTexture, 1, 1);

            mapa1.addDoor(mapa1, lapa,
                  new Vector2(3780, 450),
                  new Vector2(0 - portalTexture.Width, 800),
                  portalTexture, 1, 1);
            
            mapa1.addDoor(mapa1, claudio,
                  new Vector2(2323, 450),
                  new Vector2(0 - portalTexture.Width, 800),
                  portalTexture, 1, 1);

            mapa1.addDoor(mapa1, labBio,
                 new Vector2(970, 450),
                 new Vector2(0 - portalTexture.Width, 800),
                 portalTexture, 2, 2);

            mapa1.addDoor(mapa1, exatas,
                 new Vector2(2400, 450),
                 new Vector2(0 - portalTexture.Width, 800),
                 portalTexture, 2, 2);
            

            moniHum.addPortal(moniHum, mapa2, new Vector2(0 - portalTexture.Width, 800), new Vector2(3780, 750), portalTexture, 1, 1);

            //mapa1.addEvent(null, "", portalTexture, 1, new Vector2(2323, 450), "A porta esta trancada");
            mapa2.addEvent(null, "", portalTexture, 1, new Vector2(1600, 450), "A porta esta trancada");
            mapa2.addEvent(null, "", portalTexture, 2, new Vector2(1525, 450), "A porta esta trancada");

            lapa.addPortal(lapa, mapa1, new Vector2(0 - portalTexture.Width, 800), new Vector2(3780, 750), portalTexture, 1, 1);
            labEnf.addPortal(labEnf, mapa2, new Vector2(0-portalTexture.Width, 800), new Vector2(1000, 750), portalTexture, 1, 1);
            labBio.addPortal(labBio, mapa1, new Vector2(0 - portalTexture.Width, 800), new Vector2(970, 750), portalTexture, 2, 2);
            labBio.addPortal(labBio, mapa1, new Vector2(0 - portalTexture.Width, 800), new Vector2(970, 750), portalTexture, 1, 1);
            exatas.addPortal(exatas, mapa1, new Vector2(1440+portalTexture.Width, 800), new Vector2(2400, 750), portalTexture, 2, 2);
            claudio.addPortal(lapa, mapa1, new Vector2(0 - portalTexture.Width, 800), new Vector2(2323, 750), portalTexture, 1, 1);


            camera = new Camera2D(spriteBatch, GraphicsDevice, baseScreenSize, true);  //cria uma nova câmera. O spritebatch é passado como parâmetro para que possamos desenhar os objetos. (todos os desenhos são feitos na classe camera)
            camera.followObject(player);

            world.addMap(mapa1);        //
            world.addMap(mapa2);        //adiciona o mapa ao mundo
            world.addMap(labEnf);
            world.addMap(labBio);
            world.addMap(lapa);
            world.addMap(exatas);
            world.addMap(claudio);
            world.addMap(moniHum);
            world.addObject(player);


            world.addCamera(camera);    //adiciona a camera ao mundo

            player.CurrentMap = mapa1.Name;
            world.NomeMapaAtual = mapa1.Name;   //seta o nome do mapa atual para poder desenhar. É NECESSÁRIO!

            gameState = GameState.MainMenu;
            menuState = MenuState.Jogar;
            menuOptionsState = OptionsMenuState.VideoOptions;
            videoState = VideoMenuState.Resolution800x600;
            soundState = SoundMenuState.Musica;
            pauseState = PauseMenuState.Resume;
        }

        protected override void UnloadContent()
        {
            this.Dispose();
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
          //  trataMovimentos(gameTime);
            trataTeclado(gameTime);   //handlers do teclado

            if (gameState == GameState.Game)
            {
                this.world.updateAllObjects(gameTime);
                this.world.updatePhysics(gameTime);
            }

            frameCounter++;
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime >= 1)
            {
                displayableFrame = frameCounter;
                frameCounter = 0;
                elapsedTime = 0;
            }

            //player.Velocity = Vector2.Zero;
            base.Update(gameTime);
        }

        Vector2 lastPositionLeftFoot = Vector2.Zero;
        Vector2 lastPositionRightFoot = Vector2.Zero;
        private float movementElapsedTime = 0;
        private float selectedTime = 0;
        float changeBackgroundTime = 0;
        private void trataMovimentos(GameTime gt){
        
            if (gameState == GameState.Game)
            {

                if (player.Skeleton != null)
                {
                    //abrir inventario
                    if (bracosAbertos(gt))
                    {
                        showInventory();
                    }
                    else if (bracosCruzados())
                    {
                        gameState = GameState.Paused;
                        paused = true;
                    }
                    else if (selecionar(player.Skeleton.Joints[JointType.HandRight], gt))
                    {
                        player.EnterRoom = true;
                        player.PickItem = true;
                    }
                    else
                    {
                        player.EnterRoom = false;
                        player.PickItem = false;
                    }
                    foreach (Joint ER in player.Skeleton.Joints)
                    {
                        if (ER.JointType == JointType.ElbowRight)
                        {
                            foreach (Joint HR in player.Skeleton.Joints)
                            {
                                if (HR.JointType == JointType.HandRight)
                                {
                                    if (Tools.getModule(Tools.ConvertKinectSkeletonPosition(ER.Position.X, ER.Position.Y, RESOLUTION).Y - Tools.ConvertKinectSkeletonPosition(HR.Position.X, HR.Position.Y, RESOLUTION).Y) < 20)
                                    {
                                        if (player.Velocity.X == 0)
                                        {
                                            Vector2 movement = new Vector2(400, 0);
                                            player.applyForce(movement, gt);
                                            if (frameTimer > .2)
                                            {
                                                if (currentFrame == 2)
                                                {
                                                    currentFrame = 0;
                                                }
                                                else
                                                    currentFrame++;

                                                player.changeFrame(new Vector2(currentFrame, 0));
                                                frameTimer = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        player.Velocity = new Vector2(0, player.Velocity.Y);
                                    }
                                }
                            }
                        }
                    }

                    if (player.Velocity.X == 0)
                    {
                        foreach (Joint j in player.Skeleton.Joints)
                        {
                            if (j.JointType == JointType.ElbowLeft)
                            {
                                foreach (Joint o in player.Skeleton.Joints)
                                {
                                    if (o.JointType == JointType.HandLeft)
                                    {
                                        if (Tools.getModule(Tools.ConvertKinectSkeletonPosition(j.Position.X, j.Position.Y, RESOLUTION).Y - Tools.ConvertKinectSkeletonPosition(o.Position.X, o.Position.Y, RESOLUTION).Y) < 20)
                                        {
                                            if (player.Velocity.X == 0)
                                            {
                                                Vector2 movement = new Vector2(-400, 0);
                                                player.applyForce(movement, gt);
                                                if (frameTimer > .2)
                                                {
                                                    if (currentFrame == 0)
                                                    {
                                                        currentFrame = 2;
                                                    }
                                                    else
                                                        currentFrame--;

                                                    player.changeFrame(new Vector2(currentFrame, 1));
                                                    frameTimer = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            player.Velocity = new Vector2(0, player.Velocity.Y);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    foreach (Joint j in player.Skeleton.Joints)
                    {
                        if (j.JointType == JointType.FootLeft)
                        {
                            if (Tools.getModule(lastPositionLeftFoot.Y - Tools.ConvertKinectSkeletonPosition(j.Position.X, j.Position.Y, RESOLUTION).Y) > 30)
                            {
                                if (player.Velocity.Y == 0)
                                {
                                    player.applyForce(new Vector2(0, -100), gt);
                                }
                                if (changeBackgroundTime > 2f)
                                {

                                    this.world.MapaAtual.changeBackground();

                                    this.player.Position =
                                        new Vector2(this.world.MapaAtual.CurrentSide.Texture.Width - this.player.Position.X,
                                            this.player.Position.Y - 2);

                                    this.player.Velocity = Vector2.Zero;
                                    if (this.player.CurrentFrame.Y == 0)
                                        player.changeFrame(new Vector2(currentFrame, 1));
                                    else
                                        player.changeFrame(new Vector2(currentFrame, 0));
                                    changeBackgroundTime = 0;
                                }
                                lastPositionLeftFoot = Tools.ConvertKinectSkeletonPosition(j.Position.X, j.Position.Y, RESOLUTION);
                            }
                        }
                    }


                    

                }
            }
            else if (gameState == GameState.MainMenu)
            {
                if (movementElapsedTime > 1)
                {

                    if (player.Skeleton != null)
                    {
                        foreach (Joint mao in player.Skeleton.Joints)
                        {
                            if (mao.JointType == JointType.HandRight)
                            {
                                //subir mão acima da cabeça
                                if (subir(mao))
                                { //ato de mandar a setinha pra cima
                                    if (menuState == MenuState.Jogar)
                                        menuState = MenuState.Sair;
                                    else if (menuState == MenuState.Opcoes)
                                        menuState = MenuState.Jogar;
                                    else if (menuState == MenuState.Sair)
                                        menuState = MenuState.Opcoes;
                                }
                                //selecionar abaixo da cabeça e acima do espinha
                                if (selecionar(mao, gt))
                                {
                                    if (menuState == MenuState.Jogar)
                                        gameState = GameState.Game;
                                    else if (menuState == MenuState.Opcoes)
                                        gameState = GameState.MainMenuOptions;
                                    else if (menuState == MenuState.Sair)
                                        this.Exit();
                                }
                                //descer abaixo da espinha
                                if (descer(mao))
                                {//ato de descer a setinha
                                    if (menuState == MenuState.Jogar)
                                        menuState = MenuState.Opcoes;
                                    else if (menuState == MenuState.Opcoes)
                                        menuState = MenuState.Sair;
                                    else if (menuState == MenuState.Sair)
                                        menuState = MenuState.Jogar;
                                }
                            }
                        }
                    }
                    movementElapsedTime = 0;
                }
                movementElapsedTime += (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else if (gameState == GameState.MainMenuOptions)
            {
                if (movementElapsedTime > 1)
                    {
                        if (player.Skeleton != null)
                        {
                            foreach (Joint mao in player.Skeleton.Joints)
                            {
                                if (mao.JointType == JointType.HandRight)
                                {
                                    //subir mão acima da cabeça
                                    if (subir(mao))
                                    {
                                        if (menuOptionsState == OptionsMenuState.VideoOptions)
                                            menuOptionsState = OptionsMenuState.Sair;
                                        else if (menuOptionsState == OptionsMenuState.SoundOptions)
                                            menuOptionsState = OptionsMenuState.VideoOptions;
                                        else if (menuOptionsState == OptionsMenuState.Ajuda)
                                            menuOptionsState = OptionsMenuState.SoundOptions;
                                        else if (menuOptionsState == OptionsMenuState.Sair)
                                            menuOptionsState = OptionsMenuState.Ajuda;
                                    }
                                    //selecionar abaixo da cabeça e acima do espinha
                                    if (selecionar(mao, gt))
                                    {
                                        if (menuOptionsState == OptionsMenuState.VideoOptions)
                                            gameState = GameState.VideoOptions;
                                        else if (menuOptionsState == OptionsMenuState.SoundOptions)
                                            gameState = GameState.SoundOptions;
                                        else if (menuOptionsState == OptionsMenuState.Ajuda)
                                            gameState = GameState.Ajuda;
                                        else if (menuOptionsState == OptionsMenuState.Sair)
                                        {
                                            if (paused)
                                            {
                                                gameState = GameState.Game;
                                                paused = false;
                                            }
                                            else
                                                gameState = GameState.MainMenu;
                                        }
                                    }
                                    //descer abaixo da espinha
                                    if (descer(mao))
                                    {
                                        if (menuOptionsState == OptionsMenuState.VideoOptions)
                                            menuOptionsState = OptionsMenuState.SoundOptions;
                                        else if (menuOptionsState == OptionsMenuState.SoundOptions)
                                            menuOptionsState = OptionsMenuState.Ajuda;
                                        else if (menuOptionsState == OptionsMenuState.Ajuda)
                                            menuOptionsState = OptionsMenuState.Sair;
                                        else if (menuOptionsState == OptionsMenuState.Sair)
                                            menuOptionsState = OptionsMenuState.VideoOptions;
                                    }
                                }
                            }
                        }
                        movementElapsedTime = 0;
                    }
                    movementElapsedTime += (float)gt.ElapsedGameTime.TotalSeconds;
                }
            else if (gameState == GameState.Paused)
            {
                if (movementElapsedTime > 1)
                {
                    if (player.Skeleton != null)
                    {
                        foreach (Joint mao in player.Skeleton.Joints)
                        {
                            if (mao.JointType == JointType.HandRight)
                            {
                                if (subir(mao))
                                {
                                    if (pauseState == PauseMenuState.Resume)
                                        pauseState = PauseMenuState.MainMenu;
                                    else if (pauseState == PauseMenuState.Options)
                                        pauseState = PauseMenuState.Resume;
                                    else if (pauseState == PauseMenuState.MainMenu)
                                        pauseState = PauseMenuState.Options;
                                }
                                if (selecionar(mao, gt))
                                {
                                    if (pauseState == PauseMenuState.MainMenu)
                                        gameState = GameState.MainMenu;
                                    else if (pauseState == PauseMenuState.Options)
                                        gameState = GameState.MainMenuOptions;
                                    else if (pauseState == PauseMenuState.Resume)
                                        gameState = GameState.Game;
                                }
                                if (descer(mao))
                                {
                                    if (pauseState == PauseMenuState.Resume)
                                        pauseState = PauseMenuState.Options;
                                    else if (pauseState == PauseMenuState.Options)
                                        pauseState = PauseMenuState.MainMenu;
                                    else if (pauseState == PauseMenuState.MainMenu)
                                        pauseState = PauseMenuState.Resume;
                                }
                            }
                        }
                    }
                    movementElapsedTime = 0;
                }
                movementElapsedTime += (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else if (gameState == GameState.VideoOptions)
            {
                if (movementElapsedTime > 1)
                {
                    if (player.Skeleton != null)
                    {
                        foreach (Joint mao in player.Skeleton.Joints)
                        {
                            if (mao.JointType == JointType.HandRight)
                            {
                                if (subir(mao))
                                {
                                    if (videoState == VideoMenuState.Resolution800x600)
                                        videoState = VideoMenuState.Sair;
                                    else if (videoState == VideoMenuState.Resolution1024x768)
                                        videoState = VideoMenuState.Resolution800x600;
                                    else if (videoState == VideoMenuState.Resolution1440x900)
                                        videoState = VideoMenuState.Resolution1024x768;
                                    else if (videoState == VideoMenuState.Resolution1680x1050)
                                        videoState = VideoMenuState.Resolution1440x900;
                                    else if (videoState == VideoMenuState.Sair)
                                        videoState = VideoMenuState.Fullscreen;
                                    else if (videoState == VideoMenuState.Fullscreen)
                                        videoState = VideoMenuState.Resolution1680x1050;
                                }
                                if (descer(mao))
                                {
                                    if (videoState == VideoMenuState.Resolution800x600)
                                        videoState = VideoMenuState.Resolution1024x768;
                                    else if (videoState == VideoMenuState.Resolution1024x768)
                                        videoState = VideoMenuState.Resolution1440x900;
                                    else if (videoState == VideoMenuState.Resolution1440x900)
                                        videoState = VideoMenuState.Resolution1680x1050;
                                    else if (videoState == VideoMenuState.Resolution1680x1050)
                                        videoState = VideoMenuState.Fullscreen;
                                    else if (videoState == VideoMenuState.Fullscreen)
                                        videoState = VideoMenuState.Sair;
                                    else if (videoState == VideoMenuState.Sair)
                                        videoState = VideoMenuState.Resolution800x600;
                                }
                                if (selecionar(mao, gt))
                                {
                                    if (videoState == VideoMenuState.Resolution800x600)
                                    {
                                        graphics.PreferredBackBufferHeight = 600;
                                        graphics.PreferredBackBufferWidth = 800;
                                    }
                                    else if (videoState == VideoMenuState.Resolution1024x768)
                                    {
                                        graphics.PreferredBackBufferHeight = 768;
                                        graphics.PreferredBackBufferWidth = 1024;
                                    }
                                    else if (videoState == VideoMenuState.Resolution1440x900)
                                    {
                                        graphics.PreferredBackBufferHeight = 900;
                                        graphics.PreferredBackBufferWidth = 1440;
                                    }
                                    else if (videoState == VideoMenuState.Resolution1680x1050)
                                    {
                                        graphics.PreferredBackBufferHeight = 1050;
                                        graphics.PreferredBackBufferWidth = 1680;
                                    }
                                    else if (videoState == VideoMenuState.Fullscreen)
                                    {
                                        graphics.ToggleFullScreen();
                                    }
                                    else if (videoState == VideoMenuState.Sair)
                                    {
                                        gameState = GameState.MainMenuOptions;
                                    }
                                    graphics.ApplyChanges();
                                }
                            }
                        }
                    }
                    movementElapsedTime = 0;
                }
                movementElapsedTime += (float)gt.ElapsedGameTime.TotalSeconds;

            }
            else if (gameState == GameState.SoundOptions)
            {
                if (movementElapsedTime > 1)
                {
                    if (player.Skeleton != null)
                    {
                        foreach (Joint mao in player.Skeleton.Joints)
                        {
                            if (mao.JointType == JointType.HandRight)
                            {
                                if (descer(mao))
                                {
                                    if (soundState == SoundMenuState.Musica)
                                        soundState = SoundMenuState.Som;
                                    else if (soundState == SoundMenuState.Som)
                                        soundState = SoundMenuState.Sair;
                                    else if (soundState == SoundMenuState.Sair)
                                        soundState = SoundMenuState.Musica;
                                }

                                if (subir(mao))
                                {
                                    if (soundState == SoundMenuState.Musica)
                                        soundState = SoundMenuState.Sair;
                                    else if (soundState == SoundMenuState.Som)
                                        soundState = SoundMenuState.Musica;
                                    else if (soundState == SoundMenuState.Sair)
                                        soundState = SoundMenuState.Som;
                                }

                                if (selecionar(mao, gt))
                                {
                                    if (soundState == SoundMenuState.Musica)
                                    {

                                    }
                                    else if (soundState == SoundMenuState.Som)
                                    {

                                    }
                                    else if (soundState == SoundMenuState.Sair)
                                    {
                                        gameState = GameState.MainMenuOptions;
                                    }
                                }
                            }
                        }
                    }
                    movementElapsedTime = 0;
                }
                movementElapsedTime += (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else if (gameState == GameState.Inventory)
            {
                if (bracosAbertos(gt))
                {
                    gameState = GameState.Game;
                }
            }
            frameTimer += (float)gt.ElapsedGameTime.TotalSeconds;
            changeBackgroundTime += (float)gt.ElapsedGameTime.TotalSeconds;
        }

        private bool descer(Joint mao)
        {
            foreach (Joint cabeca in player.Skeleton.Joints)
            {
                if (cabeca.JointType == JointType.Head)
                {
                    foreach (Joint meioColuna in player.Skeleton.Joints)
                    {
                        if (meioColuna.JointType == JointType.Spine)
                        {
                            Vector2 auxColuna = Tools.ConvertKinectSkeletonPosition(meioColuna.Position.X, meioColuna.Position.Y, RESOLUTION),
                                auxCabeca = Tools.ConvertKinectSkeletonPosition(cabeca.Position.X, cabeca.Position.Y, RESOLUTION),
                                auxMao = Tools.ConvertKinectSkeletonPosition(mao.Position.X, mao.Position.Y, RESOLUTION);
                            if (auxMao.Y > auxColuna.Y)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool bracosAbertos(GameTime gt)
        {
            foreach (Joint centroOmbros in player.Skeleton.Joints)
            {
                if (centroOmbros.JointType == JointType.ShoulderCenter)
                {
                    foreach (Joint maoE in player.Skeleton.Joints)
                    {
                        if (maoE.JointType == JointType.HandLeft)
                        {
                            foreach (Joint maoD in player.Skeleton.Joints)
                            {
                                if (maoD.JointType == JointType.HandRight)
                                {
                                    Vector2 auxMaoE = Tools.ConvertKinectSkeletonPosition(maoE.Position.X, maoE.Position.Y, RESOLUTION),
                                            auxMaoD = Tools.ConvertKinectSkeletonPosition(maoD.Position.X, maoE.Position.Y, RESOLUTION),
                                            auxOmbros = Tools.ConvertKinectSkeletonPosition(centroOmbros.Position.X, centroOmbros.Position.Y, RESOLUTION);
                                    if ((auxMaoE.Y < auxOmbros.Y) && (auxMaoD.Y < auxOmbros.Y))
                                    {
                                        selectedTime += (float)gt.ElapsedGameTime.TotalSeconds;

                                        if (selectedTime > 1.5f)
                                        {
                                            selectedTime = 0;
                                            return true;
                                        }


                                    }
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool bracosCruzados()
        {
            foreach (Joint maoE in player.Skeleton.Joints)
            {
                if (maoE.JointType == JointType.HandLeft)
                {
                    foreach (Joint maoD in player.Skeleton.Joints)
                    {
                        if (maoD.JointType == JointType.HandRight)
                        {
                            Vector2 auxMaoE = Tools.ConvertKinectSkeletonPosition(maoE.Position.X, maoE.Position.Y, RESOLUTION),
                                auxMaoD = Tools.ConvertKinectSkeletonPosition(maoD.Position.X, maoE.Position.Y, RESOLUTION);
                            if (auxMaoE.X > auxMaoD.X)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool selecionar(Joint mao, GameTime gt)
        {
            foreach (Joint cabeca in player.Skeleton.Joints)
            {
                if (cabeca.JointType == JointType.Head)
                {
                    foreach (Joint quadril in player.Skeleton.Joints)
                    {
                        if (quadril.JointType == JointType.HipCenter)
                        {
                            Vector2 auxQuadril = Tools.ConvertKinectSkeletonPosition(quadril.Position.X, quadril.Position.Y, RESOLUTION),
                                auxCabeca = Tools.ConvertKinectSkeletonPosition(cabeca.Position.X, cabeca.Position.Y, RESOLUTION),
                                auxMao = Tools.ConvertKinectSkeletonPosition(mao.Position.X, mao.Position.Y, RESOLUTION);
                                
                            if ((auxMao.Y > auxCabeca.Y) && (auxMao.Y < auxQuadril.Y) && Vector2.Distance(auxMao, auxQuadril) < 150)
                            {
                                selectedTime += 0.1f ;
                                if (selectedTime > 0.3)
                                {
                                    selectedTime = 0;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool subir(Joint mao)
        {
            foreach (Joint cabeca in player.Skeleton.Joints)
            {
                if (cabeca.JointType == JointType.Head)
                {
                    Vector2 auxMao = Tools.ConvertKinectSkeletonPosition(mao.Position.X, mao.Position.Y, RESOLUTION),
                        auxCabeca = Tools.ConvertKinectSkeletonPosition(cabeca.Position.X, cabeca.Position.Y, RESOLUTION);
                    if (auxMao.Y < auxCabeca.Y)
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }


        private bool abrirPorta(GameTime gt)
        {
            Vector2 auxQuadril = Tools.ConvertKinectSkeletonPosition(player.Skeleton.Joints[JointType.HipCenter].Position.X, player.Skeleton.Joints[JointType.HipCenter].Position.Y, RESOLUTION),
                                auxCabeca = Tools.ConvertKinectSkeletonPosition(player.Skeleton.Joints[JointType.Head].Position.X, player.Skeleton.Joints[JointType.Head].Position.Y, RESOLUTION),
                                auxMaoEsquerda = Tools.ConvertKinectSkeletonPosition(player.Skeleton.Joints[JointType.HandLeft].Position.X, player.Skeleton.Joints[JointType.HandLeft].Position.Y, RESOLUTION),
                                auxMaoDireita = Tools.ConvertKinectSkeletonPosition(player.Skeleton.Joints[JointType.HandRight].Position.X, player.Skeleton.Joints[JointType.HandRight].Position.Y, RESOLUTION);

            if ((auxMaoEsquerda.Y > auxCabeca.Y) && (auxMaoDireita.Y > auxCabeca.Y) && (auxMaoEsquerda.Y < auxQuadril.Y) && (auxMaoDireita.Y < auxQuadril.Y))
            {
                selectedTime += 0.1f;
                if (selectedTime > 0.3)
                {
                    selectedTime = 0;
                    return true;
                }
            }
            return false;
        }

        bool up,down,enter = false;
        float inputCounter = 0;
        int currentFrame = 1;
        float frameTimer = 0;
        private void trataTeclado(GameTime gt)
        {
            #region movimentacao da camera
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Vector2 movement = new Vector2(2, 0);
                camera.translate(movement);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Vector2 movement = new Vector2(0, 2);
                camera.translate(movement);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Vector2 movement = new Vector2(-2, 0);
                camera.translate(movement);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Vector2 movement = new Vector2(0, -2);
                camera.translate(movement);
            }
            #endregion

            #region movimentacao do player
            if (gameState == GameState.Game)
            {
                #region movimentação do jogo
                if (Keyboard.GetState().IsKeyDown(Keys.C)) //wtf oq C faz
                {
                    if (player.Inventory.Count != 0)
                    {
                        Random random = new Random();
                        Objeto objeto = player.dropObject(player.Inventory[0]);
                        objeto.Position = new Vector2(player.Position.X-20, player.Position.Y-objeto.Texture.Height - 50);
                        objeto.applyForce(new Vector2(random.Next(120), -random.Next(190)), gt);
                        world.MapaAtual.CurrentSide.addObject(objeto);
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.E)) 
                {
                    player.EnterRoom = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.E))
                {
                    player.EnterRoom = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    player.PickItem = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.Q))
                {
                    player.PickItem = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.T))
                {
                    if (inputCounter > 1)
                    {
                        if (!player.CurrentMap.Equals("LabBio") && !player.CurrentMap.Equals("LabEnf") && !player.CurrentMap.Equals("Claudio")
                            && !player.CurrentMap.Equals("Lapa") && !player.CurrentMap.Equals("Monitoria") && !player.CurrentMap.Equals("exatas"))
                        {
                            this.world.MapaAtual.changeBackground();

                            this.player.Position =
                                new Vector2(this.world.MapaAtual.CurrentSide.Texture.Width - this.player.Position.X,
                                    this.player.Position.Y - 2);

                            this.player.Velocity = Vector2.Zero;
                            if (this.player.CurrentFrame.Y == 0)
                                player.changeFrame(new Vector2(currentFrame, 1));
                            else
                                player.changeFrame(new Vector2(currentFrame, 0));

                            inputCounter = 0;

                        }
                    }
                    
                }
                
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (inputCounter > 1)
                    {
                       showInventory();
                       inputCounter = 0;
                    }
                }


                if (Keyboard.GetState().IsKeyDown(Keys.K))
                {
                    if (!(lol.Velocity.X > 200))
                        lol.applyForce(new Vector2(200, 0), gt);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.J))
                {
                    if (!(lol4.Velocity.X < -100))
                        lol4.applyForce(new Vector2(-100, 0), gt);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    if (player.Velocity.Y == 0)
                    {
                        player.applyForce(new Vector2(0, -190), gt);
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    if (player.Velocity.X == 0)
                    {
                        if (player.Left == false)
                        {
                            Vector2 movement = new Vector2(400, 0);
                            player.applyForce(movement, gt);
                            player.Right = true;                         
                        }
                    }
                    if (frameTimer > .2)
                    {
                        if (currentFrame == 2)
                        {
                            currentFrame = 0;
                        }
                        else
                            currentFrame++;
                        player.changeFrame(new Vector2(currentFrame, 0));
                        frameTimer = 0;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    if (player.Velocity.X == 0)
                    {
                        if (player.Right == false)
                        {
                            Vector2 movement = new Vector2(-400, 0);
                            player.applyForce(movement, gt);
                            player.Left = true; 
                        }
                       // currentFrame = 7;
                    }
                    if (frameTimer > .2)
                    {
                        if (currentFrame == 0)
                        {
                            currentFrame = 2;
                        }
                        else
                            currentFrame--;

                        player.changeFrame(new Vector2(currentFrame, 1));
                        frameTimer = 0;
                    }
                }

                if (Keyboard.GetState().IsKeyUp(Keys.Right))
                {
                    if (player.Right)
                    {
                        player.applyForce(new Vector2(-400, 0), gt);
                        player.Right = false;
                    }

                }

                if (Keyboard.GetState().IsKeyUp(Keys.Left))
                {
                    if (player.Left)
                    {
                        player.applyForce(new Vector2(400, 0), gt);
                        player.Left = false;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    if (gameState == GameState.Paused){
                        gameState = GameState.Game;
                        paused = false;
                    }
                    else
                    {
                        gameState = GameState.Paused;
                        paused = true;
                    }
                }
                #endregion
            }
            else  //arrumar aqui
            {
                #region movimentação em menus
                if (gameState == GameState.MainMenu || gameState == GameState.MainMenuOptions || gameState == GameState.Paused || gameState== GameState.SoundOptions || gameState == GameState.Ajuda || gameState == GameState.VideoOptions || gameState == GameState.Creditos)
                {

                    
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {

                        if (!up)
                        {//subindo a setinha no menu
                            if (gameState == GameState.MainMenu)
                            {
                                if (menuState == MenuState.Jogar)
                                    menuState = MenuState.Sair;
                                else if (menuState == MenuState.Opcoes)
                                    menuState = MenuState.Jogar;
                                else if (menuState == MenuState.Sair)
                                    menuState = MenuState.Creditos;
                                else if (menuState == MenuState.Creditos)
                                    menuState = MenuState.Opcoes;
                            }
                            else
                            {
                                if (gameState == GameState.MainMenuOptions)
                                {
                                    if (menuOptionsState == OptionsMenuState.VideoOptions)
                                        menuOptionsState = OptionsMenuState.SoundOptions;
                                    else if (menuOptionsState == OptionsMenuState.SoundOptions)
                                        menuOptionsState = OptionsMenuState.Sair;
                                    else if (menuOptionsState == OptionsMenuState.Ajuda)
                                        menuOptionsState = OptionsMenuState.VideoOptions;
                                    else if (menuOptionsState == OptionsMenuState.Sair)
                                        menuOptionsState = OptionsMenuState.Ajuda;

                                }
                                else
                                {
                                    if (gameState == GameState.Paused)
                                    {
                                        if (pauseState == PauseMenuState.Resume)
                                            pauseState = PauseMenuState.MainMenu;
                                        else if (pauseState == PauseMenuState.Options)
                                            pauseState = PauseMenuState.Resume;
                                        else if (pauseState == PauseMenuState.MainMenu)
                                            pauseState = PauseMenuState.Options;
                                    }
                                    else if (gameState == GameState.VideoOptions)
                                    {
                                        if (videoState == VideoMenuState.Resolution800x600)
                                            videoState = VideoMenuState.Sair;
                                        else if (videoState == VideoMenuState.Resolution1024x768)
                                            videoState = VideoMenuState.Resolution800x600;
                                        else if (videoState == VideoMenuState.Resolution1440x900)
                                            videoState = VideoMenuState.Resolution1024x768;
                                        else if (videoState == VideoMenuState.Resolution1680x1050)
                                            videoState = VideoMenuState.Resolution1440x900;
                                        else if (videoState == VideoMenuState.Sair)
                                            videoState = VideoMenuState.Fullscreen;
                                        else if (videoState == VideoMenuState.Fullscreen)
                                            videoState = VideoMenuState.Resolution1680x1050;
                                    }
                                    else if (gameState == GameState.SoundOptions)
                                    {
                                        if (soundState == SoundMenuState.Musica)
                                            soundState = SoundMenuState.Sair;
                                        else if (soundState == SoundMenuState.Som)
                                            soundState = SoundMenuState.Musica;
                                        else if (soundState == SoundMenuState.Sair)
                                            soundState = SoundMenuState.Som;
                                    }

                                }

                            }
                        }
                        up = true;
                    }


                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {//descendo a setinha no menu
                        if (!down)
                        {
                            if (gameState == GameState.MainMenu)
                            {
                                if (menuState == MenuState.Jogar)
                                    menuState = MenuState.Opcoes;
                                else if (menuState == MenuState.Opcoes)
                                    menuState = MenuState.Creditos;
                                else if (menuState == MenuState.Sair)
                                    menuState = MenuState.Jogar;
                                else if (menuState == MenuState.Creditos)
                                    menuState = MenuState.Sair;

                                _optionTime += 1;
                            }
                            else
                            {
                                if (gameState == GameState.MainMenuOptions)
                                {
                                    if (menuOptionsState == OptionsMenuState.VideoOptions)
                                        menuOptionsState = OptionsMenuState.Ajuda;
                                    else if (menuOptionsState == OptionsMenuState.SoundOptions)
                                        menuOptionsState = OptionsMenuState.VideoOptions;
                                    else if (menuOptionsState == OptionsMenuState.Ajuda)
                                        menuOptionsState = OptionsMenuState.Sair;
                                    else if (menuOptionsState == OptionsMenuState.Sair)
                                        menuOptionsState = OptionsMenuState.SoundOptions;
                                }
                                else
                                {
                                    if (gameState == GameState.Paused)
                                    {
                                        if (pauseState == PauseMenuState.Resume)
                                            pauseState = PauseMenuState.Options;
                                        else if (pauseState == PauseMenuState.Options)
                                            pauseState = PauseMenuState.MainMenu;
                                        else if (pauseState == PauseMenuState.MainMenu)
                                            pauseState = PauseMenuState.Resume;
                                    }
                                    else if (gameState == GameState.VideoOptions)
                                    {
                                        if (videoState == VideoMenuState.Resolution800x600)
                                            videoState = VideoMenuState.Resolution1024x768;
                                        else if (videoState == VideoMenuState.Resolution1024x768)
                                            videoState = VideoMenuState.Resolution1440x900;
                                        else if (videoState == VideoMenuState.Resolution1440x900)
                                            videoState = VideoMenuState.Resolution1680x1050;
                                        else if (videoState == VideoMenuState.Resolution1680x1050)
                                            videoState = VideoMenuState.Fullscreen;
                                        else if (videoState == VideoMenuState.Fullscreen)
                                            videoState = VideoMenuState.Sair;
                                        else if (videoState == VideoMenuState.Sair)
                                            videoState = VideoMenuState.Resolution800x600;
                                    }
                                    else if (gameState == GameState.SoundOptions)
                                    {
                                        if (soundState == SoundMenuState.Musica)
                                            soundState = SoundMenuState.Som;
                                        else if (soundState == SoundMenuState.Som)
                                            soundState = SoundMenuState.Sair;
                                        else if (soundState == SoundMenuState.Sair)
                                            soundState = SoundMenuState.Musica;
                                    }
                                }
                            }
                        }
                        down = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        if (!enter)
                        {
                              
                            if (gameState == GameState.MainMenu)
                            {
                                if (menuState == MenuState.Jogar)
                                {
                                    gameState = GameState.Game;
                                }
                                else if (menuState == MenuState.Opcoes)
                                    gameState = GameState.MainMenuOptions;
                                else if (menuState == MenuState.Sair)
                                    this.Exit();
                                else if (menuState == MenuState.Creditos)
                                    gameState = GameState.Creditos;
                            }
                            else
                            {
                                if (gameState == GameState.MainMenuOptions)
                                {
                                    if (menuOptionsState == OptionsMenuState.VideoOptions)
                                    {
                                        gameState = GameState.VideoOptions;
                                        
                                    }
                                    else if (menuOptionsState == OptionsMenuState.SoundOptions)
                                    {
                                        gameState = GameState.SoundOptions;
                                    }
                                    else if (menuOptionsState == OptionsMenuState.Sair)
                                    {
                                        if (paused)
                                        {
                                            gameState = GameState.Paused;
                                            paused = false;
                                        }
                                        else
                                        {
                                            gameState = GameState.MainMenu;
                                        }
                                    }
                                    else if (menuOptionsState == OptionsMenuState.Ajuda)
                                    {
                                        gameState = GameState.Ajuda;
                                    }

                                }
                                else
                                {
                                    if (gameState == GameState.Paused)
                                    {
                                        if (pauseState == PauseMenuState.Resume)
                                            gameState = GameState.Game;
                                        else if (pauseState == PauseMenuState.Options)
                                            gameState = GameState.MainMenuOptions;
                                        else if (pauseState == PauseMenuState.MainMenu)
                                        {
                                            gameState = GameState.MainMenu;
                                            resetEverything();
                                        }

                                    }
                                    else if (gameState == GameState.VideoOptions)
                                    {
                                        if (videoState == VideoMenuState.Resolution800x600)
                                        {
                                            graphics.PreferredBackBufferHeight = 600;
                                            graphics.PreferredBackBufferWidth = 800;
                                        }
                                        else if (videoState == VideoMenuState.Resolution1024x768)
                                        {
                                            graphics.PreferredBackBufferHeight = 768;
                                            graphics.PreferredBackBufferWidth = 1024;
                                        }
                                        else if (videoState == VideoMenuState.Resolution1440x900)
                                        {
                                            graphics.PreferredBackBufferHeight = 900;
                                            graphics.PreferredBackBufferWidth = 1440;
                                        }
                                        else if (videoState == VideoMenuState.Resolution1680x1050)
                                        {
                                            graphics.PreferredBackBufferHeight = 1050;
                                            graphics.PreferredBackBufferWidth = 1680;
                                        }
                                        else if (videoState == VideoMenuState.Fullscreen)
                                        {
                                            graphics.ToggleFullScreen();
                                        }
                                        else if (videoState == VideoMenuState.Sair)
                                        {
                                            gameState = GameState.MainMenuOptions;
                                        }
                                        graphics.ApplyChanges();
                                    }
                                    else if (gameState == GameState.SoundOptions)
                                    {
                                        if (soundState == SoundMenuState.Musica)
                                        {
                                           
                                        }
                                        else if (soundState == SoundMenuState.Som)
                                        {//tiop o som é a música, vlw celso
                                            if (MediaPlayer.State == MediaState.Playing)
                                                MediaPlayer.Pause();
                                            else
                                                MediaPlayer.Play(s);
                                        }
                                        else if (soundState == SoundMenuState.Sair)
                                        {
                                            gameState = GameState.MainMenuOptions;
                                        }
                                    }

                                    else if (gameState == GameState.Ajuda)
                                    {
                                        gameState = GameState.MainMenuOptions;
                                    }

                                        //////////////////////////////////////////////////////////////////////////vei onde eu coloco isso pra voltar pro menu
                                    else
                                    {
                                        if (gameState == GameState.Creditos)
                                        {
                                            gameState = GameState.MainMenu;
                                        }
                                    }
                                }
                            }
                            
                            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
                            form.Location = new System.Drawing.Point(GraphicsDevice.DisplayMode.Width / 2 - graphics.PreferredBackBufferWidth / 2, GraphicsDevice.DisplayMode.Height / 2 - graphics.PreferredBackBufferHeight / 2);

                            enter = true;
                        }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Up))
                        up = false;
                    if (Keyboard.GetState().IsKeyUp(Keys.Down))
                        down = false;
                    if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                        enter = false;

                }
                else
                {
                    if (gameState == GameState.Inventory)
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            if (inputCounter > 1)
                            {
                                gameState = GameState.Game;
                                inputCounter = 0;
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
             
            #region troca de mapas
            KeyboardState newState = Keyboard.GetState();    
            if (newState.IsKeyDown(Keys.P))
            {
                if (oldState.IsKeyDown(Keys.P) != newState.IsKeyDown(Keys.P))
                {
                    if (world.NomeMapaAtual == "Mapa1")
                        world.NomeMapaAtual = "Mapa2";
                    else
                    {
                        if (world.NomeMapaAtual == "Mapa3")
                            world.NomeMapaAtual = "Mapa1";
                        else
                        {
                            if (world.NomeMapaAtual == "Mapa2")
                                world.NomeMapaAtual = "Mapa3";
                        }
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                if (oldState.IsKeyDown(Keys.O) != newState.IsKeyDown(Keys.O))
                {
                    if (world.NomeMapaAtual == "Mapa1")
                        world.NomeMapaAtual = "Mapa3";
                    else
                    {
                        if (world.NomeMapaAtual == "Mapa3")
                            world.NomeMapaAtual = "Mapa2";
                        else
                        {
                            if (world.NomeMapaAtual == "Mapa2")
                                world.NomeMapaAtual = "Mapa1";
                        }
                    }
                }
            }
            //if (Keyboard.GetState().IsKeyDown(Keys.T))
            oldState = Keyboard.GetState();
            #endregion

            if (newState.IsKeyDown(Keys.NumPad1))
            {
                if (gameState == GameState.Inventory)
                {
                    resetEverything();
                    gameState = GameState.Game;
                }
                else
                {
                    showInventory();
                    gameState = GameState.Inventory;
                }
            }


            inputCounter += (float)gt.ElapsedGameTime.TotalSeconds;
            frameTimer += (float)gt.ElapsedGameTime.TotalSeconds;
        }
 
        protected override void Draw(GameTime gameTime)
        {
            Vector3 screenScalingFactor;
            float horScaling = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / baseScreenSize.X;
            float verScaling = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
            screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, globalTransformation);
            
            if (gameState == GameState.MainMenu)
            {
                GraphicsDevice.Clear(Color.Black);

                /*Vector3 screenScalingFactor;
                float horScaling = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / baseScreenSize.X;
                float verScaling = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
                screenScalingFactor = new Vector3(horScaling, verScaling, 1);
                Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, globalTransformation);*/
                spriteBatch.Draw(mainMenu, new Vector2(0,0), Color.White);

                if (menuState == MenuState.Jogar)
                    spriteBatch.DrawString(menuOptions, "->", new Vector2(850, -30), Color.White);
                else if (menuState == MenuState.Opcoes)
                    spriteBatch.DrawString(menuOptions, "->", new Vector2(850, 40), Color.White);
                else if (menuState == MenuState.Creditos)
                    spriteBatch.DrawString(menuOptions, "->", new Vector2(850, 110), Color.White);
                else if (menuState == MenuState.Sair)
                    spriteBatch.DrawString(menuOptions, "->", new Vector2(880, 180), Color.White);

            }
            else
            {
                if (gameState == GameState.MainMenuOptions)
                {
                    GraphicsDevice.Clear(Color.Black);

                   /* Vector3 screenScalingFactor;
                    float horScaling = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / baseScreenSize.X;
                    float verScaling = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
                    screenScalingFactor = new Vector3(horScaling, verScaling, 1);
                    Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, globalTransformation);*/
                    spriteBatch.Draw(options, Vector2.Zero, Color.White);

                    if (menuOptionsState == OptionsMenuState.VideoOptions)
                        spriteBatch.DrawString(menuOptions, "->", new Vector2(502, 409), Color.White);
                    else if (menuOptionsState == OptionsMenuState.SoundOptions)
                        spriteBatch.DrawString(menuOptions, "->", new Vector2(532, 277), Color.White);
                    else if (menuOptionsState == OptionsMenuState.Ajuda)
                        spriteBatch.DrawString(menuOptions, "->", new Vector2(528, 530), Color.White);
                    else if (menuOptionsState == OptionsMenuState.Sair)
                        spriteBatch.DrawString(menuOptions, "->", new Vector2(1069, 744), Color.White);

                }
                else
                {
                    
                    if (gameState == GameState.Paused)
                    {
                        GraphicsDevice.Clear(Color.Black);
                        spriteBatch.Draw(pause, new Vector2(0, 0), Color.White);

                        if (pauseState == PauseMenuState.Resume)
                            spriteBatch.DrawString(menuOptions, "->", new Vector2(425, 99), Color.White);
                        else if (pauseState == PauseMenuState.Options)
                            spriteBatch.DrawString(menuOptions, "->", new Vector2(425, 330), Color.White);
                        else if (pauseState == PauseMenuState.MainMenu)
                            spriteBatch.DrawString(menuOptions, "->", new Vector2(425, 557), Color.White);

                    }
                    else
                    {
                        if (gameState == GameState.Game)
                        {
                            GraphicsDevice.Clear(Color.Black);
                            //effect.CurrentTechnique.Passes[0].Apply();
                            world.drawAllObjects();
                            testLight.Draw();
                           /* Vector3 screenScalingFactor;
                            float horScaling = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / baseScreenSize.X;
                            float verScaling = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
                            screenScalingFactor = new Vector3(horScaling, verScaling, 1);
                            Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, globalTransformation);*/

                             //spriteBatch.DrawString(font, "Camera X:" + camera.Position.X + " Y:" + camera.Position.Y, new Vector2(10, 10), Color.White);    //Desenha a posição atual da câmera.
                             //spriteBatch.DrawString(font, "Ca X:" + camera.Position.X + " Y:" + camera.Position.Y, new Vector2(10, 10), Color.White);    //Desenha a posição atual da câmera.
                            
                             spriteBatch.DrawString(font, "Player X: " + player.Position.X + " Y:" + player.Position.Y + " |Colidiu:" + player.Colidiu + " |Left: " + player.Left + " |Right: " + player.Right + " |Velocity: " + player.Velocity, new Vector2(10, 90), Color.White);    //Desenha a posição atual da câmera. 
                             spriteBatch.DrawString(font, "Camera Pos "+camera.Position , new Vector2(10, 130), Color.White);    //Desenha a posição atual da câmera. 
                             spriteBatch.DrawString(font, "Mapa atual: " + player.CurrentMap, new Vector2(10, 170), Color.White);
                           /* spriteBatch.DrawString(font, "*", new Vector2(lol.Position.X, lol.Position.Y), Color.White);
                             spriteBatch.DrawString(font, "*", new Vector2(lol4.Position.X, lol4.Position.Y), Color.White);
                             spriteBatch.DrawString(font, "*", new Vector2(player.Position.X, player.Position.Y), Color.White);
                             spriteBatch.DrawString(font, "-", new Vector2(player.Position.X, player.Bounds.Top), Color.White);
                             spriteBatch.DrawString(font, "-", new Vector2(lol4.Position.X, lol4.Bounds.Top), Color.White);
                             spriteBatch.DrawString(font, "*", new Vector2(camera.Position.X, camera.Position.Y), Color.White);*/
                            if (player.LastPickedItem != null)
                            {
                                if (player.PickedItemGameTime != TimeSpan.Zero)
                                {
                                    spriteBatch.Draw(player.LastPickedItem.Texture, new Vector2(baseScreenSize.X / 2 - player.LastPickedItem.Texture.Width / 2, 100), Color.White);
                                    spriteBatch.DrawString(font, player.LastPickedItem.Name, new Vector2(baseScreenSize.X / 2 - font.MeasureString(player.LastPickedItem.Name).X/2, 100 + player.LastPickedItem.Texture.Height + 10), Color.White);
                                    player.PickedItemGameTime += gameTime.ElapsedGameTime;
                                }
                                if (player.PickedItemGameTime.Seconds > 4)
                                {
                                    player.PickedItemGameTime = TimeSpan.Zero;
                                    player.LastPickedItem = null;
                                }
                            }
                            if (player.ShowMessage)
                            {
                                if (player.PickedItemGameTime != TimeSpan.Zero)
                                {
                                    spriteBatch.DrawString(font, player.MessageToShow, new Vector2(baseScreenSize.X / 2 - font.MeasureString(player.MessageToShow).X / 2, 100 + 10), Color.Black);
                                    player.PickedItemGameTime += gameTime.ElapsedGameTime;
                                }
                                if (player.PickedItemGameTime.Seconds > 2)
                                {
                                    player.PickedItemGameTime = TimeSpan.Zero;
                                    player.ShowMessage = false;
                                    player.MessageToShow = "";
                                }
                            }
                            //spriteBatch.DrawString(font, "LastpickedItem:" + player.LastPickedItem.Name , new Vector2(10, 110), Color.White);
                            //spriteBatch.DrawString(font,"Fps: "+ displayableFrame, new Vector2(10, 10), Color.White);
                        }
                        else
                        {
                            GraphicsDevice.Clear(Color.CornflowerBlue);
                            world.drawAllObjects();
                            /*Vector3 screenScalingFactor;
                            float horScaling = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / baseScreenSize.X;
                            float verScaling = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
                            screenScalingFactor = new Vector3(horScaling, verScaling, 1);
                            Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, globalTransformation);*/

                            if (gameState == GameState.Inventory)
                            {
                                if (yCoord != 0)
                                {
                                    spriteBatch.Draw(inventory, new Vector2(0, yCoord), Color.White);
                                    yCoord += 10;
                                }
                                else
                                {
                                    float xItemCoord = 150;
                                    float yItemCoord = 200;
                                    spriteBatch.Draw(inventory, new Vector2(0, yCoord), Color.White);
                                    for (int i = 0; i < player.Inventory.Count; i++)
                                    {
                                        spriteBatch.Draw(player.Inventory[i].Texture, new Vector2(xItemCoord, yItemCoord), Color.White);
                                        spriteBatch.DrawString(font, player.Inventory[i].Name, new Vector2(xItemCoord + player.Inventory[i].Texture.Width / 2 - font.MeasureString(player.Inventory[i].Name).X/2, yItemCoord + player.Inventory[i].Texture.Height + 10), Color.White);
                                        
                                        xItemCoord += 200;
                                        if (xItemCoord >= 676)
                                        {
                                            xItemCoord = 150;
                                            yItemCoord += 200;
                                        }
                                    }
                                }
                            }
                            else if (gameState == GameState.VideoOptions)
                            {
                                spriteBatch.Draw(videoOptions, new Vector2(0, 0), Color.White);
                                if (videoState == VideoMenuState.Resolution800x600)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(482, 241), Color.White);
                                else if (videoState == VideoMenuState.Resolution1024x768)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(450, 371), Color.White);
                                else if (videoState == VideoMenuState.Resolution1440x900)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(448, 491), Color.White);
                                else if (videoState == VideoMenuState.Resolution1680x1050)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(458, 615), Color.White);
                                else if (videoState == VideoMenuState.Fullscreen)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(516, 734), Color.White);
                                else if (videoState == VideoMenuState.Sair)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(1113, 745), Color.White);
                            }
                            else if (gameState == GameState.SoundOptions)
                            {
                                spriteBatch.Draw(soundOptions, new Vector2(0, 0), Color.White);
                                if (soundState == SoundMenuState.Musica)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(337, 380), Color.White);
                                else if (soundState == SoundMenuState.Som)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(349, 502), Color.White);
                                else if (soundState == SoundMenuState.Sair)
                                    spriteBatch.DrawString(menuOptions, "->", new Vector2(1121, 792), Color.White);
                            }
                            else if (gameState == GameState.Ajuda) {
                                spriteBatch.Draw(ajuda, new Vector2(0, 0), Color.White);
                                spriteBatch.DrawString(menuOptions, "->", new Vector2(1121, 792), Color.White);
                            }
                            else if (gameState == GameState.Creditos) {
                                spriteBatch.Draw(creditos, new Vector2(0, 0), Color.White);
                                spriteBatch.DrawString(menuOptions, "->", new Vector2(921, 732), Color.White);
                            }
                        }
                    }
                }
            }
            if (colorVideo != null)
                spriteBatch.Draw(colorVideo, GraphicsDevice.Viewport.Bounds, Color.White*.3f);

            if (playerSkeleton != null)
            {
                foreach (Joint j in playerSkeleton.Joints)
                {
                    if (j.TrackingState == JointTrackingState.Tracked)
                        spriteBatch.Draw(jointTexture, Tools.ConvertKinectSkeletonPosition(j.Position.X, j.Position.Y, RESOLUTION), Color.White);
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }

 }

