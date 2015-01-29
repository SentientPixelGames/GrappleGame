using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrappleGame
{
    public class Tile
    {
        /// <summary>
        /// tile texture located on this tile
        /// </summary>
        public Texture2D texture;
        /// <summary>
        /// Indicates whether characters can transerve this tile (independent of any objects)
        /// </summary>
        public bool impassible;
        /// <summary>
        /// number associated with the tile
        /// </summary>
        public int number;
        /// <summary>
        /// sets the minimap color of this object
        /// </summary>
        public string minimapColor;
        Constants Constants = new Constants();

        /// <summary>
        /// Contains all the attributes, data, information pertaining to a tile
        /// </summary>
        /// <param name="name">name of tile</param>
        /// <param name="number">associated number of tile for easy reference</param>
        /// <param name="texture">texture associated with this tile</param>
        /// <param name="impassible">the default impassibility of this tile</param>
        public Tile(int number, Texture2D texture, bool impassible, string minimapColor)
        {
            this.texture = texture;
            this.impassible = impassible;
            this.number = number;
            this.minimapColor = minimapColor;
        }

    }
}
