using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GrappleGame
{
    /// <summary>
    /// contains all object data unique to this tile
    /// </summary>
    public struct objectData
    {
        /// <summary>
        /// The drawing level of the object. Between 0 and 1, a higher number is drawn earlier (layered in the back)
        /// </summary>
        public float mainDepth;
        /// <summary>
        /// drawing depth of the secondary object texture
        /// </summary>
        public float secondaryDepth;
        /// <summary>
        /// indicates the largest height level of any objuect on top of the tile. 0 = nothing, 1 = object, >1 = overhanging object
        /// </summary>
        public float height;
        /// <summary>
        /// indicates whether any objects over this tile are solid
        /// </summary>
        public bool solid;
        /// <summary>
        /// indicates the shadow type on this tile, which affects how the dude, npc's, enemies are drawn on this tile.
        /// </summary>
        public float shadow;
    }

    /// <summary>
    /// contains all tile data unique to this tile
    /// </summary>
    public struct tileData
    {
        /// <summary>
        /// The drawing level of the tile. Between 0 and 1, a higher number is drawn earlier (layered in the back)
        /// </summary>
        public float depth;
        /// <summary>
        /// Indicates whether characters can transerve this tile (independent of any objects)
        /// </summary>
        public bool impassible;
        /// <summary>
        /// indicates the height of the tile in the map
        /// </summary>
        public float height;
        /// <summary>
        /// Is there a character on this tile
        /// If not then param should equal -1
        /// </summary>
        public int characterOnTile;
        /// <summary>
        /// Is there an enemy on this tile
        /// If not then param should equal -1
        /// </summary>
        public int enemyOnTile;
    }

    /// <summary>
    /// This class contains all the textures, methods, attributes, and data pertaining to a tile in a map
    /// </summary>
    public class tileClass
    {
        /// <summary>
        /// object located on this tile
        /// </summary>
        public Object Object;
        /// <summary>
        /// tile texture located on this tile
        /// </summary>
        public Tile tile;
        /// <summary>
        /// position in map
        /// </summary>
        public Vector2 position;
        Constants Constants = new Constants();
        /// <summary>
        /// Indicates the frame of the object for animated tiles
        /// </summary>
        public int frame;
        public objectData objectData;
        public tileData tileData;

        /// <summary>
        /// contains all data pertaining to a specific tile position on a specific map
        /// </summary>
        /// <param name="position">the position of the tile. often matches the array value</param>
        /// <param name="tile">The tile that is located here</param>
        /// <param name="Object">the object that is located here, if any</param>
        public tileClass(Vector2 position, Tile tile, Object Object)
        {
            this.position = position;
            this.tile = tile;
            this.Object = Object;
            frame = 0;
        }
        /// <summary>
        /// loads in all of object data specific to this tile
        /// </summary>
        /// <param name="mainDepth">drawing depth of main object texture</param>
        /// <param name="secondaryDepth">drawing depth of secondary drawing texture, if any</param>
        /// <param name="height">height of most prominent object on tile</param>
        /// <param name="solid">the solidity of the object</param>
        /// <param name="shadow">any shadows present in the texure</param>
        public void loadObjectData(float mainDepth, float secondaryDepth, float height, int solid, float shadow)
        {
            objectData.mainDepth = mainDepth;
            objectData.secondaryDepth = secondaryDepth;
            objectData.height = height;
            objectData.solid = solid == 1 ? true : false;
            objectData.shadow = shadow;
        }
        /// <summary>
        /// loads in all tile data specific to this tile
        /// </summary>
        /// <param name="depth">drawing depth of the tile</param>
        /// <param name="impassible">whether tile can be walked on under normal circumstances</param>
        /// <param name="height">the height level of the tile</param>
        public void loadTileData(float depth, int impassible, float height)
        {
            tileData.depth = depth;
            tileData.height = height;
            tileData.impassible = impassible == 1 ? true : false;
            tileData.characterOnTile = -1;
            tileData.enemyOnTile = -1;
        }

        /// <summary>
        /// Draws the tile 
        /// </summary>
        /// <param name="spritebatch">XNA tool for drawing</param>
        /// <param name="filter">A white texture. Combined with alpha levels to create light/dark tints on the tiles</param>
        /// <param name="dudeHeight">The current height of the dude. The tint of the tile is dependent on the comparison with the tile height and the dude height.</param>
        public void DrawTile(SpriteBatch spritebatch, Texture2D heightTint, float dudeHeight)
        {
            spritebatch.Draw(tile.texture, position * Constants.tilesize, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, tileData.depth);
            //if (height - dudeHeight > 0)
            //    spritebatch.Draw(heightTint, tilePosition * Constants.tilesize, null, Color.Lerp(Color.White, Color.Transparent, 1 - ((float)(height - dudeHeight) / 50)), 0f, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            //else if (height - dudeHeight < 0)
            //    spritebatch.Draw(heightTint, tilePosition * Constants.tilesize, null, Color.Lerp(Color.Black, Color.Transparent, 1 + ((float)(height - dudeHeight) / 25)), 0f, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
        }
        /// <summary>
        /// Draws the main object texture and the secondary object texture if it exists
        /// </summary>
        /// <param name="spritebatch">XNA tool for drawing</param>
        /// <param name="dudeHeight">The current height of the dude. The tint of the object is dependent on the comparison with the tile height and the dude height.</param>
        public void DrawObject(SpriteBatch spritebatch, bool hidden, Vector2 dudePosition)
        {
            if (Object != null)
            {
                if (Object.advancedTexture != null)
                {
                    if (hidden && position.Y - dudePosition.Y > (-2 - Object.basicTexture.Width / 32) && position.Y - dudePosition.Y < 3 && position.X - dudePosition.X > (-3 - Object.basicTexture.Width / 32) && position.X - dudePosition.X < 4)
                        spritebatch.Draw(Object.basicTexture, position * Constants.tilesize, null, Color.Lerp(Color.White, Color.Transparent, 0.5f), 0f, Vector2.Zero, 1, SpriteEffects.None, objectData.mainDepth);
                    else spritebatch.Draw(Object.basicTexture, position * Constants.tilesize, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, objectData.mainDepth);
                    spritebatch.Draw(Object.advancedTexture, position * Constants.tilesize, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, objectData.secondaryDepth);
                }
                else spritebatch.Draw(Object.basicTexture, position * Constants.tilesize, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, objectData.mainDepth);
            }
        }
        /// <summary>
        /// Draws the height level number of the tile on top of the tile in editor mode
        /// </summary>
        /// <param name="spritebatch">XNA tool for drawing</param>
        /// <param name="font">XNA font</param>
        public void HeightDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            spritebatch.DrawString(font, tileData.height.ToString(), new Vector2(position.X * Constants.tilesize + (Constants.tilesize / 3), position.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        public void MainDepthDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            int mainDepth = (int)((objectData.mainDepth - 0.4f) * 10000);
            spritebatch.DrawString(font, mainDepth.ToString(), new Vector2(position.X * Constants.tilesize + (Constants.tilesize / 3), position.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        public void SecondaryDepthDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            int secondaryDepth = (int)((objectData.secondaryDepth - 0.7f) * 10000);
            spritebatch.DrawString(font, secondaryDepth.ToString(), new Vector2(position.X * Constants.tilesize + (Constants.tilesize / 3), position.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        public void SolidDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            int solid = objectData.solid ? 1 : 0;
            spritebatch.DrawString(font, solid.ToString(), new Vector2(position.X * Constants.tilesize + (Constants.tilesize / 3), position.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        public void HeightObjectDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            spritebatch.DrawString(font, objectData.height.ToString(), new Vector2(position.X * Constants.tilesize + (Constants.tilesize / 3), position.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        public void ShadowDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            spritebatch.DrawString(font, objectData.shadow.ToString(), new Vector2(position.X * Constants.tilesize + (Constants.tilesize / 3), position.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        public void ImpassibleDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            int impassible = tileData.impassible ? 1 : 0;
            spritebatch.DrawString(font, impassible.ToString(), new Vector2(position.X * Constants.tilesize + (Constants.tilesize / 3), position.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        public void TileDepthDraw(SpriteBatch spritebatch, SpriteFont font)
        {
            spritebatch.DrawString(font, tileData.depth.ToString(), new Vector2(position.X * Constants.tilesize + (Constants.tilesize / 3), position.Y * Constants.tilesize), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
    }
}
