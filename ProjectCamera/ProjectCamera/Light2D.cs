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
    class Light2D
    {
        Color lightColor;
        float lightIntensity = 0;
        Vector2 position;
        float radius;
        SpriteBatch renderer;
        Texture2D lightTexture;

        public Light2D(SpriteBatch renderer,Color color, float intensity, float radius,Vector2 position, GraphicsDevice device)
        {
            this.renderer = renderer;
            lightColor = color;
            lightIntensity = intensity;
            this.position = position;
            Texture2D text = new Texture2D(device, (int)(intensity*radius*2), 700);
            Color[] colorData = new Color[text.Width * text.Height];
            for (int i = 0; i < colorData.Length; i++){
                colorData[i] = Color.White;
            }
            text.SetData(colorData);
            lightTexture = text;

        }

        public void Draw(){
          //  renderer.Draw(lightTexture, Vector2.Zero, Color.White*lightIntensity);
        }
    }
}
