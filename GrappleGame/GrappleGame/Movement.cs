using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrappleGame
{
    class Movement
    {
        private const int TileSize = 32;

        public Vector2 Walking(String walkingdirection, Vector2 pixelposition, Vector2 tileposition, int walkingspeed)
        {
            //Walk Up
            if (walkingdirection == "Up")
            {
                if (pixelposition.Y > tileposition.Y * TileSize)
                {
                    pixelposition.Y -= walkingspeed;
                    return pixelposition;
                }
                else if (pixelposition.Y <= tileposition.Y * TileSize)
                {
                    pixelposition = tileposition * TileSize;
                    return pixelposition;
                }
            }
            //Walk Down
            else if (walkingdirection == "Down")
            {
                if (pixelposition.Y < tileposition.Y * TileSize)
                {
                    pixelposition.Y += walkingspeed;
                    return pixelposition;
                }
                else if (pixelposition.Y >= tileposition.Y * TileSize)
                {
                    pixelposition = tileposition * TileSize;
                    return pixelposition;
                }
            }

            //Walk Left
            else if (walkingdirection == "Left")
            {
                if (pixelposition.X > tileposition.X * TileSize)
                {
                    pixelposition.X -= walkingspeed;
                    return pixelposition;
                }
                else if (pixelposition.X <= tileposition.X * TileSize)
                {
                    pixelposition = tileposition * TileSize;
                    return pixelposition;
                }
            }

            //Walk Right
            else if (walkingdirection == "Right")
            {
                if (pixelposition.X < tileposition.X * TileSize)
                {
                    pixelposition.X += walkingspeed;
                    return pixelposition;
                }
                else if (pixelposition.X >= tileposition.X * TileSize)
                {
                    pixelposition = tileposition * TileSize;
                    return pixelposition;
                }
            }
            return pixelposition;
        }
    }
}
