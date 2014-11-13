using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GrappleGame
{
    /// <summary>
    /// This class contains all the textures, methods, attributes, and data pertaining to a tile in a map
    /// </summary>
    public class tileClass
    {
        /// <summary>
        ///The texture associated with the tile
        /// </summary>
        public Texture2D tile;
        /// <summary>
        /// The texture associated with an object on the tile
        /// </summary>
        public Texture2D entity;
        /// <summary>
        /// The position of a tile within a map based on the tile count
        /// </summary>
        public Vector2 tilePosition;
        /// <summary>
        /// The drawing level of the object. Between 0 and 1, a higher number is drawn earlier (layered in the back)
        /// </summary>
        public float entitydepth;
        /// <summary>
        /// indicates what is on top of the tile. -1 = nothing, 0 = shadow, 1 = object, >1 = overhanging object
        /// </summary>
        public float coverage;
        Constants Constants = new Constants();
        /// <summary>
        /// Indicates whether characters can transerve this tile (independent of any objects)
        /// </summary>
        public bool impassible;
        /// <summary>
        /// Indicates whether the Dude's grapple can attach to this tile (takes into account objects)
        /// </summary>
        public bool grappable;
        /// <summary>
        /// Indicates the frame of the object for animated tiles
        /// </summary>
        public int frame;
        /// <summary>
        /// indicates the height of the tile in the map
        /// </summary>
        public float height;

        public tileClass(int X, int Y, Texture2D texture, float heightValue, int tileValue, int impassibleValue, int grappableValue, Texture2D Entity, float depthValue, float coverageValue)
        {
            tilePosition = new Vector2(X, Y);
            tile = texture;
            entity = Entity;
            height = heightValue;
            grappable = grappableValue == 1 ? true : false;
            impassible = impassibleValue == 1 ? true : false;
            entitydepth = depthValue;
            coverage = coverageValue;
            frame = 0;
        }
        /// <summary>
        /// Draws the tile and the object on the tile if it exists
        /// </summary>
        /// <param name="spritebatch">XNA tool for drawing</param>
        /// <param name="filter">A white texture. Combined with alpha levels to create light/dark tints on the tiles</param>
        /// <param name="dudeHeight">The current height of the dude. The tint of the tile is dependent on the comparison with the tile height and the dude height.</param>
        public void Draw(SpriteBatch spritebatch, Texture2D heightTint, float dudeHeight)
        {
            spritebatch.Draw(tile, tilePosition * Constants.tilesize, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
            if (entity != null)
                spritebatch.Draw(entity, tilePosition * Constants.tilesize, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, entitydepth);
            if (height - dudeHeight > 0)
                spritebatch.Draw(heightTint, tilePosition * Constants.tilesize, null, Color.Lerp(Color.White, Color.Transparent, 1 - ((float)(height - dudeHeight) / 50)), 0f, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            else if (height - dudeHeight < 0)
                spritebatch.Draw(heightTint, tilePosition * Constants.tilesize, null, Color.Lerp(Color.Black, Color.Transparent, 1 + ((float)(height - dudeHeight) / 25)), 0f, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
        }
        /// <summary>
        /// Draws the height level number of the tile on top of the tile in editor mode
        /// </summary>
        /// <param name="spritebatch">XNA tool for drawing</param>
        /// <param name="font">XNA font</param>
        public void HeightDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            spritebatch.DrawString(font, height.ToString(), new Vector2(tilePosition.X * Constants.tilesize + (Constants.tilesize / 3), tilePosition.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
    }
}
