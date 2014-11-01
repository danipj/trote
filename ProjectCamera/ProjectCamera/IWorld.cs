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
    class IWorld
    {
        private List<IObject> Objects;
        private Camera2D cam;
        private List<Map> maps;
        private SpriteBatch renderer;
        private string nomeMapaAtual = "";
        private float gravity;
        private float collisionConstant = 1f; //0.9
        private float maxSlope = 30f;

        public IWorld(SpriteBatch renderer, float gravity)
        {
            Objects = new List<IObject>();
            maps = new List<Map>();
            this.gravity = gravity;
            this.renderer = renderer;
        }

        public void addObject(IObject n)
        {
            Objects.Add(n);                             //adiciona um objeto no mundo.
            n.CurrentMap = NomeMapaAtual;
        }

        public void addCamera(Camera2D cam)
        {
            this.cam = cam;                             //adiciona uma camera no mundo.
        }

        public void addMap(Map map)
        {
            maps.Add(map);                              //adiciona um mapa no mundo.
        }

        public string NomeMapaAtual
        {
            get { return this.nomeMapaAtual; }
            set { this.nomeMapaAtual = value; }
        }

        public Map MapaAtual
        {
            get
            {
                foreach (Map map in maps)
                {
                    if (map.Name.Equals(nomeMapaAtual))
                    {
                        return map;
                    }
                }
                return null;
            }
        }

        public float Gravity
        {
            get { return gravity; }
            set { this.gravity = value; }
        }

        public void updateAllObjects(GameTime gt)
        {
            foreach (IObject obj in Objects)
            {
                obj.Update(gt);
            }

            for (int i = 0; i < maps.Count; i++)
            {
                for (int o = 0; o < maps[i].CurrentSide.Objects.Count; o++)
                {
                    maps[i].CurrentSide.Objects[o].Update(gt);
                }
            }
        }

        private float mb = 0;
        private float ma = 0;
        private float va = 0;
        private float vb = 0;
        private float velocityA = 0;
        private float velocityB = 0;
        public void updatePhysics(GameTime gt)
        {

            #region objetos
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].Name.Equals(NomeMapaAtual))
                {
                    for (int o = 0; o < maps[i].CurrentSide.Objects.Count; o++)//lento
                    {
                        
                        bool aplicado = false;
                        if (!maps[i].CurrentSide.Objects[o].Estatico)
                        {
                            if (!maps[i].CurrentSide.Objects[o].Colidiu)
                            {
                                for (int x = 0; x < maps[i].CurrentSide.Objects.Count; x++)
                                {
                                    if (o != x)
                                    {
                                        if (maps[i].CurrentSide.Objects[o].CollidesWith(maps[i].CurrentSide.Objects[x], true))
                                        {
                                            if (maps[i].CurrentSide.Objects[o].Velocity.Y > 0)
                                            {
                                                if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                {
                                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 2, maps[i].CurrentSide.Objects[o].Position.Y - 6);
                                                }
                                                else
                                                {
                                                    if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                    {
                                                        maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + 2, maps[i].CurrentSide.Objects[o].Position.Y - 6);
                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.X == 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - maps[i].CurrentSide.Objects[o].Velocity.Y / 40);
                                                        }
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                if (maps[i].CurrentSide.Objects[o].Velocity.Y < 0)
                                                {
                                                    if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                    {
                                                        maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 2, maps[i].CurrentSide.Objects[o].Position.Y + 6);
                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + 2, maps[i].CurrentSide.Objects[o].Position.Y + 6);
                                                        }
                                                        else
                                                        {
                                                            if (maps[i].CurrentSide.Objects[o].Velocity.X == 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y + maps[i].CurrentSide.Objects[o].Velocity.Y / 40);
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            maps[i].CurrentSide.Objects[o].Colidiu = true;
                                            //maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X/2, 0);
                                            maps[i].CurrentSide.Objects[o].applyForce(-maps[i].CurrentSide.Objects[o].Velocity, gt);
                                            break;
                                        }
                                        else
                                        {
                                            
                                            maps[i].CurrentSide.Objects[o].Colidiu = false;                          //aqui
                                            if (!aplicado)
                                            {
                                                maps[i].CurrentSide.Objects[o].applyForce(new Vector2(0, Gravity*2), gt);
                                                aplicado = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int z = 0; z < maps[i].CurrentSide.Objects.Count; z++)  //circulamos por todos os outros objetos
                                {
                                    if (o != z) //indice o não pode ser igual a z pois se tratariam do mesmo obj
                                    {
                                        maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y + 2);
                                        if (maps[i].CurrentSide.Objects[o].CollidesWith(maps[i].CurrentSide.Objects[z], true))
                                        {

                                            if (maps[i].CurrentSide.Objects[o].Name != "Chão" && maps[i].CurrentSide.Objects[z].Name != "Chão" && !maps[i].CurrentSide.Objects[o].Estatico && !maps[i].CurrentSide.Objects[z].Estatico)// se nenhum dos objetos for o chao
                                            {
                                                mb = maps[i].CurrentSide.Objects[z].Mass;
                                                ma = maps[i].CurrentSide.Objects[o].Mass;
                                                va = maps[i].CurrentSide.Objects[o].Velocity.X;
                                                vb = maps[i].CurrentSide.Objects[z].Velocity.X;
                                                velocityA = (((ma / mb) * va + (vb * (1 + collisionConstant))) - collisionConstant * va) / (1 + (ma / mb));
                                                velocityB = collisionConstant * (va - vb) + velocityA;

                                                
                                                if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                {
                                                    maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + 5, maps[i].CurrentSide.Objects[z].Position.Y);
                                                    for (int outrosObjetos = 0; outrosObjetos < maps[i].CurrentSide.Objects.Count; outrosObjetos++)
                                                    {
                                                        if (outrosObjetos != z)
                                                        {
                                                            if (maps[i].CurrentSide.Objects[z].CollidesWith(maps[i].CurrentSide.Objects[outrosObjetos], true))
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 5, maps[i].CurrentSide.Objects[o].Position.Y);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                    {
                                                        maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X - 5, maps[i].CurrentSide.Objects[z].Position.Y);
                                                        for (int outrosObjetos = 0; outrosObjetos < maps[i].CurrentSide.Objects.Count; outrosObjetos++)
                                                        {
                                                            if (outrosObjetos != z)
                                                            {
                                                                if (maps[i].CurrentSide.Objects[z].CollidesWith(maps[i].CurrentSide.Objects[outrosObjetos], true))
                                                                {
                                                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + 5, maps[i].CurrentSide.Objects[o].Position.Y);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                               /* if (maps[i].CurrentSide.Objects[z].Velocity.X > 0)
                                                {
                                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + 2, maps[i].CurrentSide.Objects[o].Position.Y);
                                                }
                                                else
                                                {
                                                    if (maps[i].CurrentSide.Objects[z].Velocity.X < 0)
                                                    {
                                                        maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 2, maps[i].CurrentSide.Objects[o].Position.Y);
                                                    }
                                                }*/


                                                if (maps[i].CurrentSide.Objects[o].Velocity.X != 0)
                                                {
                                                    maps[i].CurrentSide.Objects[o].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[o].Velocity.Y);
                                                }
                                                if (maps[i].CurrentSide.Objects[z].Velocity.X != 0)
                                                {
                                                    maps[i].CurrentSide.Objects[z].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[z].Velocity.Y);
                                                }
                                                maps[i].CurrentSide.Objects[z].applyForce(new Vector2(velocityB, maps[i].CurrentSide.Objects[z].Velocity.Y), gt);
                                                maps[i].CurrentSide.Objects[o].applyForce(new Vector2(velocityA, maps[i].CurrentSide.Objects[o].Velocity.Y), gt);

                                                if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                {
                                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + maps[i].CurrentSide.Objects[o].Velocity.X / 25, maps[i].CurrentSide.Objects[o].Position.Y-2);
                                                }
                                                else
                                                {
                                                    if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                    {
                                                        maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + maps[i].CurrentSide.Objects[o].Velocity.X / 25, maps[i].CurrentSide.Objects[o].Position.Y-2);
                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X > 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + maps[i].CurrentSide.Objects[o].Velocity.X / 25, maps[i].CurrentSide.Objects[z].Position.Y-2);
                                                        }
                                                        else
                                                        {
                                                            if (maps[i].CurrentSide.Objects[z].Velocity.X < 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + maps[i].CurrentSide.Objects[o].Velocity.X / 25, maps[i].CurrentSide.Objects[z].Position.Y-2);
                                                            }
                                                        }
                                                    }
                                                }
                                                //maps[i].CurrentSide.Objects[o].Colidiu = true;
                                                //break;
                                            }
                                            else
                                            {
                                                if (!maps[i].CurrentSide.Objects[o].Name.Equals("Chão"))
                                                {

                                                    //maps[i].CurrentSide.Objects[o].Colidiu = true;//aqui
                                                    if (maps[i].CurrentSide.Objects[o].Velocity.X != 0)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                        {
                                                            if (maps[i].CurrentSide.Objects[o].Velocity.X > 0 && maps[i].CurrentSide.Objects[o].Velocity.X < 3)
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[o].Velocity.Y);
                                                            }
                                                            else
                                                            {
                                                                if ((maps[i].CurrentSide.Objects[o].Velocity.X - (float)(100f * gt.ElapsedGameTime.TotalSeconds)) < 0)
                                                                {
                                                                    maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X, 0);
                                                                }
                                                                else
                                                                {
                                                                    maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X - (float)(100f * gt.ElapsedGameTime.TotalSeconds), 0);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (maps[i].CurrentSide.Objects[o].Velocity.X < 0 && maps[i].CurrentSide.Objects[o].Velocity.X > -3)
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[o].Velocity.Y);
                                                            }
                                                            else
                                                            {
                                                                if ((maps[i].CurrentSide.Objects[o].Velocity.X + (float)(100f * gt.ElapsedGameTime.TotalSeconds)) > 0)
                                                                {
                                                                    maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X, 0);
                                                                }
                                                                else
                                                                {
                                                                    maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X + (float)(100f * gt.ElapsedGameTime.TotalSeconds), 0);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X, 0);

                                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 2);
                                                    if (maps[i].CurrentSide.Objects[o].CollidesWith(maps[i].CurrentSide.Objects[z], true))
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 2, maps[i].CurrentSide.Objects[o].Position.Y);
                                                        }
                                                        else
                                                        {
                                                            if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + 2, maps[i].CurrentSide.Objects[o].Position.Y);
                                                            }
                                                        }

                                                        if (maps[i].CurrentSide.Objects[o].Velocity.Y > 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 2);
                                                        }
                                                    }

                                                    //maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 3);
                                                    //break;
                                                }
                                                else
                                                {
                                                    //maps[i].CurrentSide.Objects[z].Colidiu = true;//aqui
                                                    if (maps[i].CurrentSide.Objects[z].Velocity.X != 0)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X > 0)
                                                        {
                                                            if (maps[i].CurrentSide.Objects[z].Velocity.X > 0 && maps[i].CurrentSide.Objects[z].Velocity.X < 1)
                                                            {
                                                                maps[i].CurrentSide.Objects[z].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[z].Velocity.Y);
                                                            }
                                                            else
                                                            {
                                                                if ((maps[i].CurrentSide.Objects[z].Velocity.X - (float)(150f * gt.ElapsedGameTime.TotalSeconds)) < 0)
                                                                {
                                                                    maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X, 0);
                                                                }
                                                                else
                                                                {
                                                                    maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X - (float)(150f * gt.ElapsedGameTime.TotalSeconds), 0);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (maps[i].CurrentSide.Objects[z].Velocity.X < 0 && maps[i].CurrentSide.Objects[z].Velocity.X > -1)
                                                            {
                                                                maps[i].CurrentSide.Objects[z].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[z].Velocity.Y);
                                                            }
                                                            else
                                                            {
                                                                if ((maps[i].CurrentSide.Objects[z].Velocity.X + (float)(150f * gt.ElapsedGameTime.TotalSeconds)) > 0)
                                                                {
                                                                    maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X, 0);
                                                                }
                                                                else
                                                                {
                                                                    maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X + (float)(150f * gt.ElapsedGameTime.TotalSeconds), 0);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X, 0);

                                                    maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X, maps[i].CurrentSide.Objects[z].Position.Y - 2);
                                                    if (maps[i].CurrentSide.Objects[z].CollidesWith(maps[i].CurrentSide.Objects[o], true))
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X > 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X - 2, maps[i].CurrentSide.Objects[z].Position.Y);
                                                        }
                                                        else
                                                        {
                                                            if (maps[i].CurrentSide.Objects[z].Velocity.X < 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + 2, maps[i].CurrentSide.Objects[z].Position.Y);
                                                            }
                                                        }
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.Y > 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 2);
                                                        }
                                                    }

                                                    // maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 3);
                                                    //break;
                                                }
                                            }
                                            
                                            maps[i].CurrentSide.Objects[o].Colidiu = true;
                                            maps[i].CurrentSide.Objects[z].Colidiu = true;
                                            break;
                                        }
                                        else
                                        {
                                            maps[i].CurrentSide.Objects[o].Colidiu = false;//aqui
                                        }
                                        maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 2);
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            }
            #endregion

            #region huasdas
            /*for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].Name.Equals(NomeMapaAtual))
                {
                    for (int o = 0; o < maps[i].CurrentSide.Objects.Count; o++)//lento
                    {
                        if (!maps[i].CurrentSide.Objects[o].Estatico)
                        {
                                for (int x = 0; x < maps[i].CurrentSide.Objects.Count; x++)//rápido
                                {
                                    if (!maps[i].CurrentSide.Objects[o].Estatico)
                                    {
                                        if (o != x)
                                        {
                                            if (!maps[i].CurrentSide.Objects[o].Colidiu)
                                            {
                                                if (maps[i].CurrentSide.Objects[o].CollidesWith(maps[i].CurrentSide.Objects[x], true))
                                                {
                                                    if (maps[i].CurrentSide.Objects[o].Velocity.Y > 0)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 2, maps[i].CurrentSide.Objects[o].Position.Y + 1);
                                                        }
                                                        else
                                                        {
                                                            if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + 2, maps[i].CurrentSide.Objects[o].Position.Y + 1);
                                                            }
                                                            else
                                                            {
                                                                if (maps[i].CurrentSide.Objects[o].Velocity.X == 0)
                                                                {
                                                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y + 1);
                                                                }
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.Y < 0)
                                                        {
                                                            if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 2, maps[i].CurrentSide.Objects[o].Position.Y - 1);
                                                            }
                                                            else
                                                            {
                                                                if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                                {
                                                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + 2, maps[i].CurrentSide.Objects[o].Position.Y - 1);
                                                                }
                                                                else
                                                                {
                                                                    if (maps[i].CurrentSide.Objects[o].Velocity.X == 0)
                                                                    {
                                                                        maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 1);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    maps[i].CurrentSide.Objects[o].Colidiu = true;
                                                    maps[i].CurrentSide.Objects[o].applyForce(-maps[i].CurrentSide.Objects[o].Velocity, gt);
                                                    break;
                                                }
                                                else
                                                {
                                                    maps[i].CurrentSide.Objects[o].Colidiu = false;                          //aqui
                                                    maps[i].CurrentSide.Objects[o].applyForce(new Vector2(0, Gravity), gt);
                                                }
                                            }
                                        }
                                    }
                                }
                            
                        }
                        else
                        {
                            for (int z = 0; z < maps[i].CurrentSide.Objects.Count; z++)  //circulamos por todos os outros objetos
                            {
                                if (o != z) //indice o não pode ser igual a z pois se tratariam do mesmo obj
                                {
                                    //Rectangle bounds = new Rectangle((int)maps[i].CurrentSide.Objects[o].Position.X, (int)maps[i].CurrentSide.Objects[o].Position.Y + 1, maps[i].CurrentSide.Objects[o].Bounds.Width, maps[i].CurrentSide.Objects[o].Bounds.Height + 2); //retangulo com 1 a mais de altura p prever colisoes com o chao     
                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y + 5);
                                    if (maps[i].CurrentSide.Objects[o].CollidesWith(maps[i].CurrentSide.Objects[z], true))
                                    {
                                        if (maps[i].CurrentSide.Objects[o].Name != "Chão" && maps[i].CurrentSide.Objects[z].Name != "Chão" && !maps[i].CurrentSide.Objects[o].Estatico && !maps[i].CurrentSide.Objects[z].Estatico)// se nenhum dos objetos for o chao
                                        {
                                            float mb = maps[i].CurrentSide.Objects[z].Mass;
                                            float ma = maps[i].CurrentSide.Objects[o].Mass;
                                            float va = maps[i].CurrentSide.Objects[o].Velocity.X;
                                            float vb = maps[i].CurrentSide.Objects[z].Velocity.X;
                                            float velocityA = (((ma / mb) * va + (vb * (1 + collisionConstant))) - collisionConstant * va) / (1 + (ma / mb));
                                            float velocityB = collisionConstant * (va - vb) + velocityA;
                                            if (maps[i].CurrentSide.Objects[o].Velocity.X != 0)
                                            {
                                                maps[i].CurrentSide.Objects[o].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[o].Velocity.Y);
                                            }
                                            if (maps[i].CurrentSide.Objects[z].Velocity.X != 0)
                                            {
                                                maps[i].CurrentSide.Objects[z].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[z].Velocity.Y);
                                            }
                                            maps[i].CurrentSide.Objects[z].applyForce(new Vector2(velocityB, maps[i].CurrentSide.Objects[z].Velocity.Y), gt);
                                            maps[i].CurrentSide.Objects[o].applyForce(new Vector2(velocityA, maps[i].CurrentSide.Objects[o].Velocity.Y), gt);
                                            if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                            {
                                                maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + maps[i].CurrentSide.Objects[o].Velocity.X / 25, maps[i].CurrentSide.Objects[o].Position.Y);
                                            }
                                            else
                                            {
                                                if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                {
                                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + maps[i].CurrentSide.Objects[o].Velocity.X / 25, maps[i].CurrentSide.Objects[o].Position.Y);
                                                }
                                                else
                                                {
                                                    if (maps[i].CurrentSide.Objects[z].Velocity.X > 0)
                                                    {
                                                        maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + maps[i].CurrentSide.Objects[o].Velocity.X / 25, maps[i].CurrentSide.Objects[z].Position.Y);
                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X < 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + maps[i].CurrentSide.Objects[o].Velocity.X / 25, maps[i].CurrentSide.Objects[z].Position.Y);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!maps[i].CurrentSide.Objects[o].Name.Equals("Chão"))
                                            {
                                                maps[i].CurrentSide.Objects[o].Colidiu = true;//aqui
                                                //maps[i].CurrentSide.Objects[z].Colidiu = true;
                                                if (maps[i].CurrentSide.Objects[o].Velocity.X != 0)
                                                {
                                                    if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.X > 0 && maps[i].CurrentSide.Objects[o].Velocity.X < 1)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[o].Velocity.Y);
                                                        }
                                                        else
                                                        {
                                                            if ((maps[i].CurrentSide.Objects[o].Velocity.X - (float)(55f * gt.ElapsedGameTime.TotalSeconds)) < 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X, 0);
                                                            }
                                                            else
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X - (float)(55f * gt.ElapsedGameTime.TotalSeconds), 0);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.X < 0 && maps[i].CurrentSide.Objects[o].Velocity.X > -1)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[o].Velocity.Y);
                                                        }
                                                        else
                                                        {
                                                            if ((maps[i].CurrentSide.Objects[o].Velocity.X + (float)(55f * gt.ElapsedGameTime.TotalSeconds)) > 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X, 0);
                                                            }
                                                            else
                                                            {
                                                                maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X + (float)(55f * gt.ElapsedGameTime.TotalSeconds), 0);
                                                            }
                                                        }
                                                    }
                                                }
                                                maps[i].CurrentSide.Objects[o].Velocity = new Vector2(maps[i].CurrentSide.Objects[o].Velocity.X, 0);

                                                maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 5);
                                                if (maps[i].CurrentSide.Objects[o].CollidesWith(maps[i].CurrentSide.Objects[z], true))
                                                {
                                                    if (maps[i].CurrentSide.Objects[o].Velocity.X > 0)
                                                    {
                                                        maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 2, maps[i].CurrentSide.Objects[o].Position.Y);
                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[o].Velocity.X < 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X + 2, maps[i].CurrentSide.Objects[o].Position.Y);
                                                        }
                                                    }
                                                }
                                                maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y + 5);
                                                break;
                                            }
                                            else
                                            {
                                                maps[i].CurrentSide.Objects[z].Colidiu = true;//aqui
                                                if (maps[i].CurrentSide.Objects[z].Velocity.X != 0)
                                                {
                                                    if (maps[i].CurrentSide.Objects[z].Velocity.X > 0)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X > 0 && maps[i].CurrentSide.Objects[z].Velocity.X < 1)
                                                        {
                                                            maps[i].CurrentSide.Objects[z].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[z].Velocity.Y);
                                                        }
                                                        else
                                                        {
                                                            if ((maps[i].CurrentSide.Objects[z].Velocity.X - (float)(55f * gt.ElapsedGameTime.TotalSeconds)) < 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X, 0);
                                                            }
                                                            else
                                                            {
                                                                maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X - (float)(55f * gt.ElapsedGameTime.TotalSeconds), 0);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X < 0 && maps[i].CurrentSide.Objects[z].Velocity.X > -1)
                                                        {
                                                            maps[i].CurrentSide.Objects[z].Velocity = new Vector2(0, maps[i].CurrentSide.Objects[z].Velocity.Y);
                                                        }
                                                        else
                                                        {
                                                            if ((maps[i].CurrentSide.Objects[z].Velocity.X + (float)(55f * gt.ElapsedGameTime.TotalSeconds)) > 0)
                                                            {
                                                                maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X, 0);
                                                            }
                                                            else
                                                            {
                                                                maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X + (float)(55f * gt.ElapsedGameTime.TotalSeconds), 0);
                                                            }
                                                        }
                                                    }
                                                }
                                                maps[i].CurrentSide.Objects[z].Velocity = new Vector2(maps[i].CurrentSide.Objects[z].Velocity.X, 0);
                                                //maps[i].CurrentSide.Objects[z].Velocity = Vector2.Zero;
                                                maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X, maps[i].CurrentSide.Objects[z].Position.Y - 5);
                                                if (maps[i].CurrentSide.Objects[z].CollidesWith(maps[i].CurrentSide.Objects[z], true))
                                                {
                                                    if (maps[i].CurrentSide.Objects[z].Velocity.X > 0)
                                                    {
                                                        maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X - 2, maps[i].CurrentSide.Objects[z].Position.Y);
                                                    }
                                                    else
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X < 0)
                                                        {
                                                            maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + 2, maps[i].CurrentSide.Objects[z].Position.Y);
                                                        }
                                                    }
                                                }
                                                maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X, maps[i].CurrentSide.Objects[z].Position.Y + 5);
                                                break;
                                            }

                                           
                                        }
                                    }
                                    else
                                    {
                                        maps[i].CurrentSide.Objects[o].Colidiu = false;//aqui
                                    }

                                    maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 5);
                                }
                                //maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X, maps[i].CurrentSide.Objects[o].Position.Y - 3);
                            }
                        }
                    }
                }
                break;
            }
            */
            #endregion

            #region Player
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].Name.Equals(NomeMapaAtual))
                {
                    for (int o = 0; o < Objects.Count; o++)
                    {
                        bool aplicado = false;
                        if (!Objects[o].Colidiu)
                        {
                            for (int x = 0; x < maps[i].CurrentSide.Objects.Count; x++)
                            {
                                Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y + 3);
                                if (Objects[o].CollidesWith(maps[i].CurrentSide.Objects[x], true))
                                {
                                    //Quando o objeto colide com algo, uma certa parte do objeto 'Entra'
                                    //No objeto colidido como resultado da força alta da gravidade. Calculamos o quanto o objeto entrou e usamos para dizer o quando o objeto precisa 
                                    //subir ou descer


                                    if (maps[i].CurrentSide.Objects[x].Name.Equals("Chão"))
                                    {
                                        if (Objects[o].Velocity.Y > 0)
                                        {
                                            Objects[o].Velocity = Vector2.Zero;
                                            Objects[o].Colidiu = true;
                                            //Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y-1);
                                        }
                                    }
                                    else
                                    {
                                        //Está no ar e colidiu indo pra direita
                                        if (Objects[o].Velocity.X > 0 && Objects[o].Velocity.Y < 0)
                                        {
                                            int diferenca = Objects[o].Bounds.Right - maps[i].CurrentSide.Objects[x].Bounds.Left;
                                            Objects[o].Position = new Vector2(Objects[o].Position.X - diferenca, Objects[o].Position.Y);
                                            //maps[i].CurrentSide.Objects[x].Velocity = new Vector2(maps[i].CurrentSide.Objects[x].Velocity.X, 0);

                                            if (!maps[i].CurrentSide.Objects[x].Estatico)
                                                maps[i].CurrentSide.Objects[x].applyForce(new Vector2(Objects[o].Velocity.X / 2, maps[i].CurrentSide.Objects[x].Velocity.Y), gt);
                                        }
                                        else
                                        {
                                            if (Objects[o].Velocity.X < 0 && Objects[o].Velocity.Y < 0)
                                            {
                                                //Objects[o].Velocity = Vector2.Zero;
                                                int diferenca = Objects[o].Bounds.Left - maps[i].CurrentSide.Objects[x].Bounds.Right;
                                                if (diferenca < 0)
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X + (-diferenca), Objects[o].Position.Y);
                                                else
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X + (diferenca), Objects[o].Position.Y);

                                                //maps[i].CurrentSide.Objects[x].Velocity = new Vector2(maps[i].CurrentSide.Objects[x].Velocity.X, 0);

                                                if (!maps[i].CurrentSide.Objects[x].Estatico)
                                                    maps[i].CurrentSide.Objects[x].applyForce(new Vector2(Objects[o].Velocity.X / 2, maps[i].CurrentSide.Objects[x].Velocity.Y), gt);
                                                // Objects[o].Colidiu = true;
                                            }
                                            else
                                            {
                                                //Está no ar e colidiu indo pra direita
                                                if (Objects[o].Velocity.X > 0 && Objects[o].Velocity.Y > 0)
                                                {
                                                    if (Objects[o].Bounds.Bottom - 10 < maps[i].CurrentSide.Objects[x].Bounds.Top)
                                                    {
                                                        int diferenca2 = Objects[o].Bounds.Bottom - maps[i].CurrentSide.Objects[x].Bounds.Top;
                                                        Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - diferenca2);
                                                        Objects[o].Velocity = new Vector2(Objects[o].Velocity.X, 0);
                                                    }
                                                    else
                                                    {
                                                        int diferenca = Objects[o].Bounds.Right - maps[i].CurrentSide.Objects[x].Bounds.Left;
                                                        Objects[o].Position = new Vector2(Objects[o].Position.X - diferenca, Objects[o].Position.Y);

                                                        if (!maps[i].CurrentSide.Objects[x].Estatico)
                                                            maps[i].CurrentSide.Objects[x].applyForce(new Vector2(Objects[o].Velocity.X / 2, maps[i].CurrentSide.Objects[x].Velocity.Y), gt);
                                                    }

                                                }
                                                else
                                                {
                                                    if (Objects[o].Velocity.X < 0 && Objects[o].Velocity.Y > 0)
                                                    {
                                                        if (Objects[o].Bounds.Bottom - 10 < maps[i].CurrentSide.Objects[x].Bounds.Top)
                                                        {
                                                            int diferenca2 = Objects[o].Bounds.Bottom - maps[i].CurrentSide.Objects[x].Bounds.Top;
                                                            Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - diferenca2);
                                                            Objects[o].Velocity = new Vector2(Objects[o].Velocity.X, 0);
                                                        }
                                                        else
                                                        {
                                                            int diferenca = Objects[o].Bounds.Left - maps[i].CurrentSide.Objects[x].Bounds.Right;
                                                            if (diferenca < 0)
                                                                Objects[o].Position = new Vector2(Objects[o].Position.X + (-diferenca), Objects[o].Position.Y);
                                                            else
                                                                Objects[o].Position = new Vector2(Objects[o].Position.X + (diferenca), Objects[o].Position.Y);

                                                            if (!maps[i].CurrentSide.Objects[x].Estatico)
                                                                maps[i].CurrentSide.Objects[x].applyForce(new Vector2(Objects[o].Velocity.X / 2, maps[i].CurrentSide.Objects[x].Velocity.Y), gt);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Objects[o].Velocity.Y > 0)
                                                        {
                                                            Objects[o].Velocity = Vector2.Zero;
                                                            Objects[o].Colidiu = true;
                                                            int diferenca = Objects[o].Bounds.Bottom - maps[i].CurrentSide.Objects[x].Bounds.Top;
                                                            Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - diferenca);

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (maps[i].CurrentSide.Objects[x].Pickable)
                                    {
                                        (Objects[o] as Personagem).pickObject(maps[i].CurrentSide.Objects[x]);
                                        maps[i].CurrentSide.Objects.Remove(maps[i].CurrentSide.Objects[x]);
                                    }

                                    Objects[o].Colidiu = true;                       //Dizemos que ele colidiu com algo
                                    /*Objects[o].Velocity = Vector2.Zero; //(zeramos as forças)*/
                                    Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - 3);
                                    break;
                                }
                                else
                                {
                                    Objects[o].Colidiu = false;                         //Se ele não colidiu com nada
                                    if (!aplicado)
                                    {
                                        Objects[o].applyForce(new Vector2(0, Gravity*2), gt); //Aplicamos a força da gravidade
                                        aplicado = true;
                                    }
                                }
                                Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y-3);
                            }


                        }
                        else
                        {
                            for (int z = 0; z < maps[i].CurrentSide.Objects.Count; z++)  //Caso ele já tenho colidido com alguma coisa antes
                            {
                                Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y + 5);
                                if (Objects[o].CollidesWith(maps[i].CurrentSide.Objects[z], true))
                                {
                                    if (!maps[i].CurrentSide.Objects[z].Name.Equals("Chão"))
                                    {
                                        if (Objects[o].Velocity.X < 0)
                                        {
                                            if (Objects[o].Velocity.Y < 0)
                                            {
                                                if (!(Objects[o].Position.Y < maps[i].CurrentSide.Objects[z].Bounds.Top))
                                                {
                                                    int diferenca = Objects[o].Bounds.Left - maps[i].CurrentSide.Objects[z].Bounds.Right;
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X - Objects[o].Velocity.X / 50, Objects[o].Position.Y);
                                                    //Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + maps[i].CurrentSide.Objects[z].Bounds.Width - 1, Objects[o].Position.Y);
                                                    if (!maps[i].CurrentSide.Objects[z].Estatico)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X > -90)
                                                        {
                                                            bool colidiu = false;
                                                            maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X - 5, maps[i].CurrentSide.Objects[z].Position.Y);
                                                            for (int outrosObjetos = 0; outrosObjetos < maps[i].CurrentSide.Objects.Count; outrosObjetos++)
                                                            {
                                                                if (outrosObjetos != z)
                                                                {
                                                                    if (maps[i].CurrentSide.Objects[z].CollidesWith(maps[i].CurrentSide.Objects[outrosObjetos], true))
                                                                    {
                                                                        colidiu = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            //maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + 5, maps[i].CurrentSide.Objects[z].Position.Y);
                                                            if (!colidiu)
                                                                maps[i].CurrentSide.Objects[z].applyForce(new Vector2(-50, 0), gt);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((Objects[o].Position.Y + Objects[o].Texture.Height > maps[i].CurrentSide.Objects[z].Bounds.Top) )
                                                {
                                                    //&& (Objects[o].Position.Y > maps[i].CurrentSide.Objects[z].Bounds.Top - (Objects[o].Bounds.Height + 10))
                                                    int diferenca = Objects[o].Bounds.Left - maps[i].CurrentSide.Objects[z].Bounds.Right;
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X  - Objects[o].Velocity.X / 50, Objects[o].Position.Y);
                                                    //Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + maps[i].CurrentSide.Objects[z].Bounds.Width - 1, Objects[o].Position.Y);
                                                    if (!maps[i].CurrentSide.Objects[z].Estatico)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X > -90)
                                                            maps[i].CurrentSide.Objects[z].applyForce(new Vector2(-50, 0), gt);
                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (Objects[o].Velocity.Y < 0)
                                            {
                                                if (!(Objects[o].Position.Y < maps[i].CurrentSide.Objects[z].Bounds.Top))
                                                {
                                                    int diferenca = Objects[o].Bounds.Right - maps[i].CurrentSide.Objects[z].Bounds.Left;
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X - Objects[o].Velocity.X / 50, Objects[o].Position.Y);
                                                    //Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + maps[i].CurrentSide.Objects[z].Bounds.Width - 1, Objects[o].Position.Y);
                                                    if (!maps[i].CurrentSide.Objects[z].Estatico)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X < 90)
                                                        {
                                                            bool colidiu = false;
                                                            maps[i].CurrentSide.Objects[z].Position = new Vector2(maps[i].CurrentSide.Objects[z].Position.X + 5, maps[i].CurrentSide.Objects[z].Position.Y);
                                                            for (int outrosObjetos = 0; outrosObjetos < maps[i].CurrentSide.Objects.Count; outrosObjetos++)
                                                            {
                                                                if (outrosObjetos != z)
                                                                {
                                                                    if (maps[i].CurrentSide.Objects[z].CollidesWith(maps[i].CurrentSide.Objects[outrosObjetos], true))
                                                                    {
                                                                        colidiu = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            //maps[i].CurrentSide.Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[o].Position.X - 5, maps[i].CurrentSide.Objects[o].Position.Y);
                                                            if (!colidiu)
                                                                maps[i].CurrentSide.Objects[z].applyForce(new Vector2(50, 0), gt);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!(Objects[o].Position.Y < maps[i].CurrentSide.Objects[z].Bounds.Top) && (Objects[o].Position.Y > maps[i].CurrentSide.Objects[z].Bounds.Top - (Objects[o].Bounds.Height + 10)))
                                                {
                                                    //  Objects[o].Position = new Vector2(maps[i].CurrentSide.Objects[z].Bounds.Left - Objects[o].Bounds.Width + 1, Objects[o].Position.Y);
                                                    int diferenca = Objects[o].Bounds.Right - maps[i].CurrentSide.Objects[z].Bounds.Left;
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X - Objects[o].Velocity.X / 50, Objects[o].Position.Y);
                                                    if (!maps[i].CurrentSide.Objects[z].Estatico)
                                                    {
                                                        if (maps[i].CurrentSide.Objects[z].Velocity.X < 90)
                                                            maps[i].CurrentSide.Objects[z].applyForce(new Vector2(50, 0), gt);
                                                    }
                                                }

                                            }

                                        }
                                        if (maps[i].CurrentSide.Objects[z].Pickable)
                                        {
                                            (Objects[o] as Personagem).pickObject(maps[i].CurrentSide.Objects[z]);
                                            maps[i].CurrentSide.Objects.Remove(maps[i].CurrentSide.Objects[z]);
                                            (Objects[o] as Personagem).PickedItemGameTime = gt.ElapsedGameTime;
                                        }
                                    }
                                    else
                                    {
                                        bool positivo = false;
                                        Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y -5);
                                        if (Objects[o].CollidesWith(maps[i].CurrentSide.Objects[z], true))
                                        {
                                            if (Objects[o].Velocity.X > 0)
                                            {
                                                Objects[o].Position = new Vector2(Objects[o].Position.X - 2, Objects[o].Position.Y);
                                            }
                                            else if (Objects[o].Velocity.X < 0)
                                            {
                                                Objects[o].Position = new Vector2(Objects[o].Position.X + 2, Objects[o].Position.Y);
                                            }

                                            /*SLOPE -----  O OBJETIVO É DEFINIR O QUE O JOGADOR PODE ESCALAR E OQ NÃO PODE*/
                                            Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y + 5);
                                            Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - maxSlope);
                                            if (Objects[o].Velocity.X < 0)
                                            {
                                                Objects[o].Position = new Vector2(Objects[o].Position.X - 2, Objects[o].Position.Y);
                                                positivo = false;
                                            }
                                            else
                                            {
                                                Objects[o].Position = new Vector2(Objects[o].Position.X + 2, Objects[o].Position.Y);
                                                positivo = true;
                                            }

                                            if (!Objects[o].CollidesWith(maps[i].CurrentSide.Objects[z], true))
                                            {
                                                if (positivo)
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X - 2, Objects[o].Position.Y);
                                                else
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X + 2, Objects[o].Position.Y);

                                                Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y + maxSlope);
                                                Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - 2);
                                            }
                                            else
                                            {
                                                if (positivo)
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X - 2, Objects[o].Position.Y);
                                                else
                                                    Objects[o].Position = new Vector2(Objects[o].Position.X + 2, Objects[o].Position.Y);

                                                Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y + maxSlope);
                                            }

                                            Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - 5);
                                            /*SLOPE -----  O OBJETIVO É DEFINIR O QUE O JOGADOR PODE ESCALAR E OQ NÃO PODE*/

                                        }
                                        Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y + 5);
                                    }

                                    Objects[o].Colidiu = true;
                                    Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - 5);
                                    if (maps[i].CurrentSide.Objects[z].Name != "Chão")
                                         break;
                                }
                                else
                                {

                                    Objects[o].Colidiu = false;
                                    Objects[o].Position = new Vector2(Objects[o].Position.X, Objects[o].Position.Y - 5);
                                    //break;
                                }
                            }
                        }
                    }
                    break;
                }
                
            }
            #endregion

            #region portal
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].Name.Equals(NomeMapaAtual))
                {
                    for (int o = 0; o < Objects.Count; o++)
                    {
                        for (int z = 0; z < maps[i].CurrentSide.Portals.Count; z++)
                        {
                            if (maps[i].CurrentSide.Portals[z].Map1.Name == NomeMapaAtual)
                            {
                                if (Objects[o].Bounds.Intersects(maps[i].CurrentSide.Portals[z].Bounds1))
                                {
                                    //portais
                                    if (!maps[i].CurrentSide.Portals[z].isDoor)
                                    {
                                        this.NomeMapaAtual = maps[i].CurrentSide.Portals[z].Map2.Name;
                                        this.MapaAtual.changeBackground(maps[i].CurrentSide.Portals[z].SideMap2);

                                        bool colidiu = false;
                                        //Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position2.X - 100, maps[i].CurrentSide.Portals[z].Position2.Y);
                                        Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position2.X - Objects[o].Texture.Width - 10, maps[i].CurrentSide.Portals[z].Position2.Y);
                                        Objects[o].CurrentMap = this.NomeMapaAtual;
                                        foreach (Objeto obj in maps[i].CurrentSide.Portals[z].Map2.CurrentSide.Objects)
                                        {
                                            if (Objects[o].Bounds.Intersects(obj.Bounds))
                                            {
                                                colidiu = true;
                                            }
                                        }
                                        if (!colidiu)
                                        {
                                            Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position2.X + Objects[o].Texture.Width + 10,
                                                maps[i].CurrentSide.Portals[z].Position2.Y - Objects[o].Texture.Height + 105);
                                        }
                                        else
                                        {
                                            Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position2.X - Objects[o].Texture.Width - 10,
                                                maps[i].CurrentSide.Portals[z].Position2.Y - Objects[o].Texture.Height + 105);
                                        }
                                        Objects[o].Velocity = Vector2.Zero;
                                        Objects[o].Colidiu = false;
                                    }
                                    // portas
                                    else
                                    {
                                        if ((Objects[o] as Personagem).EnterRoom)
                                        {
                                            this.NomeMapaAtual = maps[i].CurrentSide.Portals[z].Map2.Name;
                                            this.MapaAtual.changeBackground(maps[i].CurrentSide.Portals[z].SideMap2);

                                            bool colidiu = false;
                                            //Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position2.X - 100, maps[i].CurrentSide.Portals[z].Position2.Y);
                                            Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position2.X - Objects[o].Texture.Width - 10, maps[i].CurrentSide.Portals[z].Position2.Y);
                                            Objects[o].CurrentMap = this.NomeMapaAtual;
                                            foreach (Objeto obj in maps[i].CurrentSide.Portals[z].Map2.CurrentSide.Objects)
                                            {
                                                if (Objects[o].Bounds.Intersects(obj.Bounds))
                                                {
                                                    colidiu = true;
                                                }
                                            }
                                            if (!colidiu)
                                            {
                                                Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position2.X + Objects[o].Texture.Width + 10,
                                                    maps[i].CurrentSide.Portals[z].Position2.Y - Objects[o].Texture.Height + 105);
                                            }
                                            else
                                            {
                                                Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position2.X - Objects[o].Texture.Width - 10,
                                                    maps[i].CurrentSide.Portals[z].Position2.Y - Objects[o].Texture.Height + 105);
                                            }
                                            Objects[o].Velocity = Vector2.Zero;
                                            Objects[o].Colidiu = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Objects[o].Bounds.Intersects(maps[i].CurrentSide.Portals[z].Bounds2))
                                {
                                    this.NomeMapaAtual = maps[i].CurrentSide.Portals[z].Map2.Name;
                                    this.MapaAtual.changeBackground(maps[i].CurrentSide.Portals[z].SideMap2);
                                    bool colidiu = false;
                                    Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position1.X - Objects[o].Texture.Width - 10,
                                        maps[i].CurrentSide.Portals[z].Position1.Y);
                                    //Objects[o].Position = new Vector2(0, 0);
                                    Objects[o].CurrentMap = this.NomeMapaAtual;
                                    foreach (Objeto obj in maps[i].CurrentSide.Portals[z].Map1.CurrentSide.Objects)
                                    {
                                        if (Objects[o].Bounds.Intersects(obj.Bounds))
                                        {
                                            colidiu = true;
                                        }
                                    }
                                    if (!colidiu)
                                    {
                                        Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position1.X + Objects[o].Texture.Width + 10,
                                            maps[i].CurrentSide.Portals[z].Position1.Y - Objects[o].Texture.Height + 110);
                                    }
                                    else
                                    {
                                        Objects[o].Position = new Vector2(maps[i].CurrentSide.Portals[z].Position1.X - Objects[o].Texture.Width + 10,
                                            maps[i].CurrentSide.Portals[z].Position1.Y - Objects[o].Texture.Height + 110);
                                    }
                                    Objects[o].Velocity = Vector2.Zero;
                                    Objects[o].Colidiu = false;
                                }
                            }
                        }
                        //if (Objects[o].Bounds.Intersects(maps))
                    }
                }
            }
            #endregion

            #region clickables
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].Name.Equals(NomeMapaAtual))
                {
                    for (int o = 0; o < Objects.Count; o++)
                    {
                        for (int z = 0; z < maps[i].CurrentSide.Clickables.Count; z++ )
                        {
                            Rectangle r = maps[i].CurrentSide.Clickables[z].Objeto.Texture.Bounds;
                            bool l = true;
                            if (Objects[o].Bounds.Intersects(maps[i].CurrentSide.Clickables[z].Objeto.Bounds))
                            {
                                if ((Objects[o] as Personagem).PickItem)
                                {
                                    (Objects[o] as Personagem).pickObject(maps[i].CurrentSide.Clickables[z].Objeto);
                                    maps[i].CurrentSide.Clickables.Remove(maps[i].CurrentSide.Clickables[z]);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region events
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].Name.Equals(NomeMapaAtual))
                {
                    for (int o = 0; o < Objects.Count; o++)
                    {
                        for (int z = 0; z < maps[i].CurrentSide.Events.Count; z++)
                        {
                            if (Objects[o].Bounds.Intersects(maps[i].CurrentSide.Events[z].Bounds))
                            {
                                if ((Objects[o] as Personagem).PickItem)
                                {
                                    if (maps[i].CurrentSide.Events[z].eventReturn((Objects[o] as Personagem)))
                                    {
                                        if (maps[i].CurrentSide.Events[z].ObjetoParaVerificar.Equals("Lampada"))
                                        {
                                            maps[i].changeBackground();
                                            maps[i].getSide(2).Events.Remove(maps[i].getSide(2).Events[z]);
                                        }
                                        else
                                            maps[i].CurrentSide.Events.Remove(maps[i].CurrentSide.Events[z]);
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }

        public void drawAllObjects()
        {
            for (int i = 0; i < maps.Count; i++)
            {
                if (this.NomeMapaAtual.Equals(maps[i].Name))        //SÓ desenhamos o mapa atual e seu conteúdo.
                {
                    cam.drawMap(maps[i]);                           //Desenha o mapa primeiro e só depois seus objetos, afinal, queremos que os objetos fiquem em uma posição Z maior (que os objetos sobreponham o mapa).
                    for (int z = 0; z < maps[i].CurrentSide.Portals.Count; z++)
                    {
                        cam.drawPortal(maps[i].CurrentSide.Portals[z], MapaAtual);
                    }
                    for (int z = 0; z < maps[i].CurrentSide.Clickables.Count; z++)
                    {
                        cam.drawClickable(maps[i].CurrentSide.Clickables[z], MapaAtual);
                    }
                    for (int z = 0; z < maps[i].CurrentSide.Events.Count; z++)
                    {
                        cam.drawEvent(maps[i].CurrentSide.Events[z], MapaAtual);
                    }
                    for (int o = 0; o < maps[i].CurrentSide.Objects.Count; o++)
                    {
                        if (!maps[i].CurrentSide.Objects[o].PickedUp)        //Se o item não tiver sido pego pelo jogador
                        {
                            cam.drawObject(maps[i].CurrentSide.Objects[o], new Vector2(maps[i].CurrentSide.Texture.Width, maps[i].CurrentSide.Texture.Height));  //desenha
                        }
                    }
                    
                    break;
                }
            }
            foreach (IObject objeto in Objects)  //Eu fiquei na dúvida se o jogo teria NPCs então coloquei esse foreach. Não tenho certeza de como farei ainda, tenho que ver com a maria.
            {
                foreach (Map m in maps)
                {
                    if (m.Name.Equals(objeto.CurrentMap))
                    {
                        Vector2 limit = new Vector2(m.CurrentSide.Texture.Width, m.CurrentSide.Texture.Height);
                        cam.drawObject(objeto, limit);
                    }
                }
           
                //cam.drawObject(objeto, );
            }
        }

        public Rectangle getBoundingRectangle(IObject a, IObject b)
        {
            Color[] bitsA = new Color[a.Texture.Width * a.Texture.Height];
            a.Texture.GetData(bitsA);
            Color[] bitsB = new Color[b.Texture.Width * b.Texture.Height];
            b.Texture.GetData(bitsB);

            // Calculate the intersecting rectangle
            int x1 = Math.Max(a.Bounds.X, b.Bounds.X);
            int x2 = Math.Min(a.Bounds.X + a.Bounds.Width, b.Bounds.X + b.Bounds.Width);

            int y1 = Math.Max(a.Bounds.Y, b.Bounds.Y);
            int y2 = Math.Min(a.Bounds.Y + a.Bounds.Height, b.Bounds.Y + b.Bounds.Height);

            return new Rectangle(x1, y1, x1 - x2, y2 - y1);
        }
    }
}
