using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace ProjectCamera
{
    public static class Tools
    {

        public static Vector2 ConvertKinectSkeletonPosition(float x, float y, Vector2 Resolution)
        {
            Vector2 positionR = new Vector2((((0.5f * x) + 0.5f) * (Resolution.X)), (((-0.5f * y) + 0.5f) * (Resolution.Y)));
            return new Vector2(positionR.X, positionR.Y);
        }

        public static float getModule(float numero)
        {
            if (numero > 0)
                return numero;
            else
                return -numero;
        }
    }
}
