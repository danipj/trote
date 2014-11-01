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
    class MapSide
    {
        private Texture2D texture;
        private List<Objeto> objetos;
        private int id;
        private List<Portal> portals;
        private List<Clickable> clickables;
        private List<Event> events;
        private String ownerMapName = "";

        public MapSide(Texture2D texture, int name, String mapName){
            this.texture = texture;
            objetos = new List<Objeto>();
            portals = new List<Portal>();
            clickables = new List<Clickable>();
            events = new List<Event>();
            id = name;
            ownerMapName = mapName;
        }

        public List<Clickable> Clickables
        {
            get { return this.clickables; }
        }

        public List<Event> Events
        {
            get { return this.events; }
        }

        public int SideID
        {
            get { return this.id; }
        }

        public String OwnerMapName
        {
            get { return this.ownerMapName; }
        }

        public List<Objeto> Objects
        {
            get { return this.objetos; }
        }

        public Texture2D Texture
        {
            get { return this.texture; }
        }

        public List<Portal> Portals
        {
            get { return this.portals; }
        }


        public void addObject(Objeto objeto)
        {

            if (objetos.Count >= 1)
            {
                Objeto chao = objetos[objetos.Count - 1];
                objetos.RemoveAt(objetos.Count - 1);
                objetos.Add(objeto);      //adiciona algum objeto no mapa
                objetos.Add(chao);
            }
            else
            {
                objetos.Add(objeto);
            }
            objeto.CurrentMap = this.ownerMapName;
        }

        public void addPortal(Map map1, Map map2, Vector2 pos1, Vector2 pos2, Texture2D texture, int side2)
        {
            portals.Add(new Portal(map1, map2, pos1, pos2, texture, id, side2));
        }

        public void addClickable(Objeto obj, Texture2D t, Vector2 pos)
        {
            clickables.Add(new Clickable(obj, t, pos));
        }

        public void addEvent(List<Objeto> objParaDar, String objetoParaVerificar, Texture2D t, Vector2 pos, String mensagem)
        {
            events.Add(new Event(objParaDar, objetoParaVerificar, t, pos, mensagem));
        }

        public void removeObject(Objeto objeto, int side)
        {
            objetos.Remove(objeto);
        }
    }
}
