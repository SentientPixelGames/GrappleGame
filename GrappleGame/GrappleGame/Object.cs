using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrappleGame
{
    public class Object
    {
        /// <summary>
        /// Object Texture
        /// </summary>
        public Texture2D basicTexture;
        /// <summary>
        /// Contains additonal shadowing, stuff hidden by the basicTexture;
        /// </summary>
        public Texture2D advancedTexture;
        /// <summary>
        /// indicates the shadow grid, which affects how the dude, npc's, enemies are drawn on this tile.
        /// </summary>
        public float[,] shadow;
        /// <summary>
        /// indicates height grid of the object. 0 = nothing, 1 = object, >1 = overhanging object
        /// </summary>
        public float[,] height;
        /// <summary>
        /// number associated to object in code, is used to reference this object
        /// </summary>
        public int number;
        /// <summary>
        /// indicates solidity grid of the object. 0 = not solid, 1 = solid at height 1. for example, tree trunk is solid, tree branches are not solid
        /// </summary>
        public float[,] solid;
        Constants Constants = new Constants();

        /// <summary>
        /// Contains all the attributes, data, information pertaining to an object
        /// </summary>
        /// <param name="name">name of the object</param>
        /// <param name="number">number associated to object in code, is used to reference this object. Is the array value of the object</param>
        /// <param name="basicTexture">Object Texture</param>
        /// <param name="advancedTexture">indicates the shadow grid, which affects how the dude, npc's, enemies are drawn on this tile.</param>
        /// <param name="shadowing">indicates the shadow grid, which affects how the dude, npc's, enemies are drawn on this tile.</param>
        /// <param name="height">indicates height grid of the object. 0 = nothing, 1 = object, >1 = overhanging object</param>
        /// <param name="solid">indicates solidity grid of the object. 0 = not solid, 1 = solid at height 1. for example, tree trunk is solid, tree branches are not solid</param>
        public Object(int number, Texture2D basicTexture, Texture2D advancedTexture, float[,] height, float[,] solid, float[,] shadow)
        {
            this.number = number;
            this.basicTexture = basicTexture;
            this.advancedTexture = advancedTexture;
            this.shadow = shadow;
            this.height = height;
            this.solid = solid;
        }
    }
}
