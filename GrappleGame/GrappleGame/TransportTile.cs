using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GrappleGame
{
    class TransportTile
    {
        /// <summary>
        /// Contains the position of the wormhole entrance
        /// </summary>
        private Vector2 position;
        /// <summary>
        /// Contains the map the wormhole entrance is located
        /// </summary>
        private Map map;
        /// <summary>
        /// Contains the position of the wormhole exit
        /// </summary>
        private Vector2 transportPosition;
        /// <summary>
        /// Contains the map the wormhole exit is located
        /// </summary>
        private Map transportMap;
        /// <summary>
        /// Contains the walking direction of the dude exiting the wormhole.  0 is up, 1 is down, 2 is right, 3 is left
        /// </summary>
        private int walkDirectionOut;
        /// <summary>
        /// Contains the Texture symbolizing a transport tile. Used exclusively in Editor mode
        /// </summary>
        private Texture2D texture;
        /// <summary>
        /// Contains a texture of an arrow pointing up. Is used to show walking directions on transport tiles in editor mode
        /// </summary>
        private Texture2D arrow;
        /// <summary>
        /// This function returns the position of the wormhole entrance on the tilemap
        /// </summary>
        public Vector2 GetPosition
        {
            get { return position; }
        }
        /// <summary>
        /// This function returns the map the wormhole entrance is located
        /// </summary>
        public Map GetMap
        {
            get { return map; }
        }
        /// <summary>
        /// This function returns the position of the wormhole exit on the tilemap
        /// </summary>
        public Vector2 GetTransportPosition
        {
            get { return transportPosition; }
        }
        /// <summary>
        /// This function returns the map the wormhole exit is located
        /// </summary>
        public Map GetTransportMap
        {
            get { return transportMap; }
        }
        /// <summary>
        /// This function returns the walk direction exiting the wormhole. 0 is up, 1 is down, 2 is right, 3 is left.
        /// </summary>
        public int GetWalkDirectionOut
        {
            get { return walkDirectionOut; }
        }
        /// <summary>
        /// This function creates a wormhole between 2 positions on a single map or on individual maps
        /// </summary>
        /// <param name="position">Contains the position of the wormhole entrance</param>
        /// <param name="transportPosition">Contains the position of the wormhole exit</param>
        /// <param name="map">Contains the map the wormhole entrance is located</param>
        /// <param name="transportMap">Contains the map the wormhole exit is located</param>
        /// <param name="walkdirectionout">Contains the walking direction of the dude exiting the wormhole.  0 is up, 1 is down, 2 is right, 3 is left</param>
        /// <param name="texture">Contains the Texture symbolizing a transport tile. Used exclusively in Editor mode</param>
        /// <param name="arrow">Contains a texture of an arrow pointing up. Is used to show walking directions on transport tiles in editor mode</param>
        public TransportTile(Vector2 position, Vector2 transportPosition, Map map, Map transportMap, int walkdirectionout, Texture2D texture, Texture2D arrow)
        {
            this.position = position;
            this.transportPosition = transportPosition;
            this.map = map;
            this.transportMap = transportMap;
            walkDirectionOut = walkdirectionout;
            this.texture = texture;
            this.arrow = arrow;

        }
        /// <summary>
        /// This function contains the wormhole. If the dude is standing on the entrance, it will transport the dude to the exit of the wormhole and designates which direction the dude should be walking when exiting. Returns bool indicating whether dude enters wormhole
        /// </summary>
        /// <param name="theDude">Player controlled character</param>
        /// <param name="currentMap">Map the dude is currently located</param>
        /// <returns>true if dude enters wormhole, false if dude did not enter wormhole</returns>
        public bool transport(ref Dude theDude, ref Map currentMap)
        {
            if (theDude.tilePosition.Equals(this.position) && currentMap.Equals(this.map))
            {
                theDude.tilePosition = new Vector2(this.transportPosition.X, this.transportPosition.Y);
                theDude.pixelPosition = theDude.tilePosition * 32;
                currentMap = transportMap;
                switch (walkDirectionOut)
                {    //use queue function
                    case 0://walking up
                        theDude.ForceInput("Walk", "Up");
                        break;
                    case 1: //walking down
                        theDude.ForceInput("Walk", "Down");
                        break;
                    case 2: //walking right
                        theDude.ForceInput("Walk", "Right");
                        break;
                    case 3: //walking left
                        theDude.ForceInput("Walk", "Left");
                        break;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Draws the texture and the exit direction after entering the transport tile on the map, making them visible in game. Typically used in editor mode. 
        /// </summary>
        /// <param name="spriteBatch">XNA tool for drawing</param>
        public void Draw(SpriteBatch spriteBatch)
        {
                spriteBatch.Draw(this.texture, this.position*Constants.tilesize, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0.02f);
                switch (walkDirectionOut)
                {    //use queue function
                    case 0://walking up
                        spriteBatch.Draw(this.arrow, new Vector2((this.position.X + 0.5f) * Constants.tilesize, (this.position.Y + 0.5f) * Constants.tilesize), null, Color.White, 0f, new Vector2(Constants.tilesize / 2, Constants.tilesize / 2), 1, SpriteEffects.None, 0.01f);
                        break;
                    case 1: //walking down
                        spriteBatch.Draw(this.arrow, new Vector2((this.position.X + 0.5f) * Constants.tilesize, (this.position.Y + 0.5f) * Constants.tilesize), null, Color.White, (float)(Math.PI), new Vector2(Constants.tilesize / 2, Constants.tilesize / 2), 1, SpriteEffects.None, 0.01f);
                        break;
                    case 2: //walking right
                        spriteBatch.Draw(this.arrow, new Vector2((this.position.X + 0.5f) * Constants.tilesize, (this.position.Y + 0.5f) * Constants.tilesize), null, Color.White, 1.57f, new Vector2(Constants.tilesize / 2, Constants.tilesize / 2), 1, SpriteEffects.None, 0.01f);
                        break;
                    case 3: //walking left
                        spriteBatch.Draw(this.arrow, new Vector2((this.position.X + 0.5f) * Constants.tilesize, (this.position.Y + 0.5f) * Constants.tilesize), null, Color.White, -1.57f, new Vector2(Constants.tilesize / 2, Constants.tilesize / 2), 1, SpriteEffects.None, 0.01f);
                        break;
                }
        }
    }
}
