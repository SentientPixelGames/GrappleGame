using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GrappleGame
{
    class Editor
    {
        public bool editorOn;
        public bool canSwitchEditor;

        public bool CreatingTransportTile;

        public bool CreateNewMap = false;
        public bool RedoCurrentMap = false;
        public bool InputtingSizeX = false;
        public bool InputtingSizeY = false;
        NumericalInput typing = new NumericalInput();
        string SizeX = "", SizeY = "";

        private Texture2D tileH, tileup, tiledown, canvas, chartab, maptab, tiletab, enemytab, transporttab, arrow;
        private int pagenumber = 1;
        private bool mouseisclicked;
        public float brushers = 0;
        public int charactercount = 0;
        private int i;
        List<Map> Maps;
        private int MouseOnMapX;
        private int MouseOnMapY;
        public bool DrawingAble;
        int topLevelbuttonOn = 0;
        public int WindowX;
        public int WindowY;
        private float Egt;
        SpriteFont font;

        private int[] impassableTiles = new int[70] {2, 1, 8, 9, 10, 11, 14, 15, 16, 18, 24, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52,
                                             53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 66, 68
                                            , 70, 72, 74, 78, 79, 81, 82, 83, 84, 97, 98, 99, 100, 101, 102, 103, 104, 105, 
                                            106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122};
        private int[] grappableTiles = new int[44] {1, 14, 15, 16, 18, 24, 51, 52, 53, 54, 55, 56, 57, 58, 59,
                                            60, 61, 62, 63, 64, 66, 68, 74, 78, 79, 81, 82, 83, 84, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106,107, 108, 109, 122};

        public enum EditorState
        {
            Tiles,
            Enemies,
            Characters,
            Maps,
            Edit,
            Transport,
            entities,
        }
        public enum Edit
        {
            Grappable,
            Height,
            Impassible,
            Off,
        }
        public EditorState currentEditorState = EditorState.Tiles;
        public Edit currentEdittingState = Edit.Off;

        public Editor(int newcharactercount)
        {
            charactercount = newcharactercount;
        }
        public void Load(ContentManager Content, SpriteFont font)
        {
            tileH = Content.Load<Texture2D>("Editor/htile");
            tileup = Content.Load<Texture2D>("Editor/arrowup");
            tiledown = Content.Load<Texture2D>("Editor/arrowdown");
            canvas = Content.Load<Texture2D>("Editor/canvas");
            chartab = Content.Load<Texture2D>("Editor/chartab");
            maptab = Content.Load<Texture2D>("Editor/maptab");
            tiletab = Content.Load<Texture2D>("Editor/tiletab");
            enemytab = Content.Load<Texture2D>("Editor/enemytab");
            transporttab = Content.Load<Texture2D>("Editor/transporttab");
            arrow = Content.Load<Texture2D>("Editor/arrow");
            this.font = font;
        }
        public void mapListLoader(List<Map> Maps)
        {
            this.Maps = Maps;
        }
        public Texture2D GetTransportTile
        {
            get { return transporttab; }
        }
        public Texture2D GetArrow
        {
            get { return arrow; }
        }
        public bool FindingSizeX()
        {
            bool EnteringSizes;
            SizeX = typing.text;
            EnteringSizes = typing.userinput();
            return EnteringSizes;
        }
        public bool FindingSizeY()
        {
            bool EnteringSizes;
            SizeY = typing.text;
            EnteringSizes = typing.userinput();            
            return EnteringSizes;
        }
        public Map createMap(Map currentMap, Editor editor, Dude theDude, ContentManager Content, Texture2D[] tiles, Texture2D[] entity)
        {
            NumericalInput typing = new NumericalInput();
            Map Mappy = new Map(editor, theDude, Content, currentMap.GetMapName, tiles, entity);
            if (CreateNewMap)
            {
                string newmapname = currentMap.GetMapName;
                newmapname = newmapname.Remove(newmapname.Length - 3, 3);
                newmapname = newmapname + currentMap.smallmapCount.ToString() + ".txt";
                int sizeX, sizeY;
                sizeX = Convert.ToInt32(SizeX);
                sizeY = Convert.ToInt32(SizeY);
                currentMap.CreateNewMap(sizeX, sizeY, 0, newmapname);
                currentMap.smallmapCount++;
                Mappy = new Map(editor, theDude, Content, newmapname, tiles, entity);
            }
            if (RedoCurrentMap)
            {
                int sizeX, sizeY;
                sizeX = Convert.ToInt32(SizeX);
                sizeY = Convert.ToInt32(SizeY);
                currentMap.CreateNewMap(sizeX, sizeY, 0, currentMap.GetMapName);
                Mappy = new Map(editor, theDude, Content, currentMap.GetMapName, tiles, entity);
            }
            return Mappy;
        }
        public Map mapClickCheck(Map currentMap, MouseState mousey, GraphicsDevice graphics, MouseState oldmouse)
        {
                int[] maps = new int[Maps.Count];
                int i = 0;
                foreach (Map map in Maps)
                {
                    string pattern = "";
                    if (topLevelbuttonOn == 0)
                        pattern = "^" + currentMap.GetMapName.Remove(currentMap.GetMapName.Length - 3, 3) + "\\d" + "\\.txt";
                    else pattern = "\\.\\d\\.";
                    if (topLevelbuttonOn == 0)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(map.GetMapName, pattern))
                        {
                            maps[i] = map.Index;
                            i++;
                        }
                    }
                    else if (topLevelbuttonOn == 1)
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(map.GetMapName, pattern))
                        {
                            maps[i] = map.Index;
                            i++;
                        }
                    }
                    else
                    {
                        maps[i] = map.Index;
                        i++;
                    }

                }
            int p = 0;
            int j = 195;
            for (int k = 100; k <= 610; k = k + 40)
            {
                if (mousey.LeftButton == ButtonState.Pressed && oldmouse.LeftButton == ButtonState.Released && mousey.X > graphics.Viewport.Width - j && mousey.X < graphics.Viewport.Width - j + 190 && mousey.Y > k && mousey.Y < k + 25)
                {

                    if (p < i)
                    {
                        currentMap = Maps[maps[p]];
                    }
                }
                p++;
            }
            return currentMap;
        }




        public void EditorMouseControls(MouseState mousey, GraphicsDevice graphics, int tilesize, float zoom, GameTime gametime, Vector2 tilePosition)
        {

            if (currentEditorState != EditorState.Edit)
            {
                if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 195 && mousey.X < graphics.Viewport.Width - 175 && mousey.Y > 650 && mousey.Y < 670)
                {
                    pagenumber++;
                    mouseisclicked = true;
                }
                else if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 155 && mousey.X < graphics.Viewport.Width - 135 && mousey.Y > 650 && mousey.Y < 670)
                {
                    pagenumber--;
                    if (pagenumber < 1)
                    {
                        pagenumber = 1;
                    }
                    mouseisclicked = true;
                }
            }
            if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 195 && mousey.X < graphics.Viewport.Width - 163 && mousey.Y > 10 && mousey.Y < 40)
            {
                currentEditorState = EditorState.Characters;
                pagenumber = 1;
                mouseisclicked = true;
            }
            if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 155 && mousey.X < graphics.Viewport.Width - 123 && mousey.Y > 10 && mousey.Y < 40)
            {
                currentEditorState = EditorState.Enemies;
                pagenumber = 1;
                mouseisclicked = true;
            }
            if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 115 && mousey.X < graphics.Viewport.Width - 83 && mousey.Y > 10 && mousey.Y < 40)
            {
                currentEditorState = EditorState.Tiles;
                pagenumber = 1;
                mouseisclicked = true;
            }
            if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 75 && mousey.X < graphics.Viewport.Width - 43 && mousey.Y > 10 && mousey.Y < 40)
            {
                currentEditorState = EditorState.Maps;
                pagenumber = 1;
                mouseisclicked = true;
            }
            if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 35 && mousey.X < graphics.Viewport.Width - 3 && mousey.Y > 10 && mousey.Y < 40)
            {
                currentEditorState = EditorState.Transport;
                pagenumber = 1;
                mouseisclicked = true;
            }
            if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 60 && mousey.X < graphics.Viewport.Width - 5 && mousey.Y > 650 && mousey.Y < 670)
            {
                    brushers = 0;
                    currentEditorState = EditorState.Edit;
                    pagenumber = 1;
                    mouseisclicked = true;
            }
            if (mouseisclicked == true && mousey.LeftButton == ButtonState.Released)
                mouseisclicked = false;
            MouseOnMapX = (int)Math.Floor((decimal)(mousey.X / (tilesize * zoom)));
            MouseOnMapY = (int)Math.Floor((decimal)(mousey.Y / (tilesize * zoom)));
            if (mousey.LeftButton == ButtonState.Pressed)
            {
                if (currentEditorState == EditorState.Tiles)
                {
                    if (mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 95 && mousey.X < graphics.Viewport.Width - 5 && mousey.Y > 50 && mousey.Y < 82)
                    {
                        currentEditorState = EditorState.entities;
                    }
                    if (pagenumber == 1)
                    {
                        int p = 0;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 90; k <= 610; k = k + 40)
                            {
                                if (mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - j && mousey.X < graphics.Viewport.Width - j + 32 && mousey.Y > k && mousey.Y < k + 40)
                                {
                                    i = 1;
                                    DrawingAble = true;
                                    brushers = p;
                                }
                                p++;
                            }
                        }
                    }
                    else if (pagenumber == 2)
                    {
                        int p = 70;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 90; k <= 610; k = k + 40)
                            {
                                if (p < 124 && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - j && mousey.X < graphics.Viewport.Width - j + 32 && mousey.Y > k && mousey.Y < k + 40)
                                {
                                    i = 1;
                                    DrawingAble = true;
                                    brushers = p;
                                }
                                p++;
                            }
                        }
                    }
                    else if (pagenumber == 3)
                    {
                        int p = 42;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 90; k <= 610; k = k + 40)
                            {
                                if (p < 123 && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - j && mousey.X < graphics.Viewport.Width - j + 32 && mousey.Y > k && mousey.Y < k + 40)
                                {
                                    i = 1;
                                    DrawingAble = true;
                                    brushers = p;
                                }
                                p++;
                            }
                        }
                    }
                }
                else if (currentEditorState == EditorState.entities)
                {
                    if (mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 195 && mousey.X < graphics.Viewport.Width - 105 && mousey.Y > 50 && mousey.Y < 82)
                    {
                        currentEditorState = EditorState.Tiles;
                    }
                    if (pagenumber == 1)
                    {
                        int p = 1;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 90; k <= 610; k = k + 40)
                            {
                                if (p < 8 && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - j && mousey.X < graphics.Viewport.Width - j + 32 && mousey.Y > k && mousey.Y < k + 40)
                                {
                                    i = 1;
                                    DrawingAble = true;
                                    brushers = p;
                                }
                                p++;
                            }
                        }
                    }
                }
                else if (currentEditorState == EditorState.Edit)
                {
                    if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 190 && mousey.X < graphics.Viewport.Width - 65 && mousey.Y > 200 && mousey.Y < 225)
                        currentEdittingState = Edit.Height;
                    if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 190 && mousey.X < graphics.Viewport.Width - 65 && mousey.Y > 250 && mousey.Y < 275)
                        currentEdittingState = Edit.Impassible;
                    if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 190 && mousey.X < graphics.Viewport.Width - 65 && mousey.Y > 300 && mousey.Y < 325)
                        currentEdittingState = Edit.Grappable;
                    if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 175 && mousey.X < graphics.Viewport.Width - 143 && mousey.Y > 50 && mousey.Y < 82)
                    {
                        DrawingAble = true;
                        if (currentEdittingState == Edit.Height)
                            brushers+= 0.5f;
                        else brushers = 1;
                        mouseisclicked = true;
                    }
                    if (mouseisclicked == false && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 175 && mousey.X < graphics.Viewport.Width - 143 && mousey.Y > 100 && mousey.Y < 132)
                    {
                        DrawingAble = true;
                        if (currentEdittingState == Edit.Height)
                            brushers-= 0.5f;
                        else brushers = 0;
                        mouseisclicked = true;
                    }

                }
                else if (currentEditorState == EditorState.Characters)
                {
                    if (pagenumber == 1)
                    {
                        int p = 0;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 50; k <= 610; k = k + 40)
                            {
                                if (p < 3 && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - j && mousey.X < graphics.Viewport.Width - j + 32 && mousey.Y > k && mousey.Y < k + 40)
                                {
                                    i = 1;
                                    DrawingAble = true;
                                    brushers = p;
                                }
                                p++;
                            }
                        }
                    }
                }
                else if (currentEditorState == EditorState.Maps)
                {
                    if (!mouseisclicked && mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 100 && mousey.X < graphics.Viewport.Width - 5 && mousey.Y > 50 && mousey.Y < 82)
                    {
                        mouseisclicked = true;
                        if (topLevelbuttonOn == 0)
                            topLevelbuttonOn = 1;
                        else if (topLevelbuttonOn == 1)
                            topLevelbuttonOn = 2;
                        else topLevelbuttonOn = 0;
                    }

                }
                else if (currentEditorState == EditorState.Transport)
                {
                    if (mousey.LeftButton == ButtonState.Pressed && mousey.X > graphics.Viewport.Width - 195 && mousey.X < graphics.Viewport.Width - 45 && mousey.Y > 50 && mousey.Y < 82)
                    {
                        CreatingTransportTile = true;
                    }
                }
               
            }
            if (i == 1)
            {
                i = 2;
                Egt = (float)gametime.TotalGameTime.TotalMilliseconds;
            }
            if (Egt + 5 < gametime.TotalGameTime.TotalMilliseconds)
            {
                DrawingAble = true;
                i = 1;
            }
            WindowX = (int)tilePosition.X - (int)Math.Floor((decimal)(12 * (1 / zoom))) + MouseOnMapX;
            WindowY = (int)tilePosition.Y - (int)Math.Floor((decimal)(10 * (1 / zoom))) + MouseOnMapY;
            
        }
        //public void CreateCharacter(List<Character> characterlist, Texture2D[] charactersprites)
        //{
        //    //characterlist.Add(new Character(charactercount, "Hello", new Vector2(WindowX, WindowY), new Vector2(32, 32), charactersprites[brushers], new List<Vector2>()));
        //}
        //public void ChangeTile(int tileValue, int heightValue, MouseState mouse, GraphicsDevice GraphicsDevice, Texture2D newTile, Texture2D oldTile, int heightTile, List<Character> characterlist, Texture2D[] charactersprites)
        //{
        //    if (editorOn == true)
        //    {
        //        if (DrawingAble == true)
        //        {
        //            if (ShowHeightData == false && WindowX >= 0 && WindowY >= 0 && WindowX <= 201 && WindowY <= 201 && !chardraw)
        //            {
        //                tileValue = brushers;
        //                oldTile = newTile;
        //                DrawingAble = false;
        //            }
        //            if (ShowHeightData == true && WindowX >= 0 && WindowY >= 0 && WindowX <= 201 && WindowY <= 201 && !chardraw)
        //            {
        //                heightValue = brushers;
        //                heightTile = brushers;
        //                DrawingAble = false;
        //            }
        //            if (ShowHeightData == false && WindowX >= 0 && WindowY >= 0 && WindowX <= 201 && WindowY <= 201 && chardraw)
        //            {
        //                charactercount++;
                        
        //            }
        //        }
        //    }
        //}
        public void DrawMouseTile(SpriteBatch spritebatch, MouseState mousey, GraphicsDevice graphics, int TileSize)
        {
            if (editorOn == true)
            {
                if (mousey.X < graphics.Viewport.Width - 200 && mousey.Y < graphics.Viewport.Height)
                {
                    spritebatch.Draw(tileH, new Rectangle(WindowX * TileSize, WindowY * TileSize, TileSize, TileSize), Color.Lerp(Color.White, Color.Transparent, 0.4f));
                }
            }
        }
        public void DrawEditorMode(SpriteBatch spriteBatch, Texture2D[] tile, GraphicsDevice GraphicsDevice, int TileSize, SpriteFont font, Texture2D whiteBox, Texture2D[] charactersprites, Map currentMap, Texture2D[] entities)
        {
            if (editorOn == true)
            {
                spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 200, 0, 200, GraphicsDevice.Viewport.Height), Color.LightPink);
                spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 200, 0, 200, 47), Color.Firebrick);

                if (currentEditorState == EditorState.Tiles)
                {
                    spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 195, 55, 90, 32), Color.White);
                    spriteBatch.DrawString(font, "Tiles", new Vector2(GraphicsDevice.Viewport.Width - 180, 55), Color.Black);
                    spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 95, 55, 90, 32), Color.White);
                    spriteBatch.DrawString(font, "Objects", new Vector2(GraphicsDevice.Viewport.Width - 90, 55), Color.Black);
                    if (pagenumber == 1)
                    {
                        int p = 0;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 90; k <= 610; k = k + 40)
                            {
                                spriteBatch.Draw(tile[p], new Rectangle(GraphicsDevice.Viewport.Width - j, k, TileSize, TileSize), Color.White);
                                p++;
                            }
                        }
                    }
                    else if (pagenumber == 2)
                    {
                        int p = 70;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 90; k <= 610; k = k + 40)
                            {
                                if (p < 124)
                                    spriteBatch.Draw(tile[p], new Rectangle(GraphicsDevice.Viewport.Width - j, k, TileSize, TileSize), Color.White);
                                p++;
                            }
                        }
                    }
                }
                else if (currentEditorState == EditorState.entities)
                {
                    spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 195, 55, 90, 32), Color.White);
                    spriteBatch.DrawString(font, "Tiles", new Vector2(GraphicsDevice.Viewport.Width - 180, 55), Color.Black);
                    spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 95, 55, 90, 32), Color.White);
                    spriteBatch.DrawString(font, "Objects", new Vector2(GraphicsDevice.Viewport.Width - 90, 55), Color.Black);
                    if (pagenumber == 1)
                    {
                        int p = 1;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 90; k <= 610; k = k + 40)
                            {
                                if(p < entities.Length)
                                    spriteBatch.Draw(entities[p], new Rectangle(GraphicsDevice.Viewport.Width - j, k, TileSize, TileSize), Color.White);
                                p++;
                            }
                        }
                    }
                }
                else if (currentEditorState == EditorState.Edit)
                {
                    spriteBatch.Draw(tileup, new Rectangle(GraphicsDevice.Viewport.Width - 175, 50, TileSize, TileSize), Color.White);
                    spriteBatch.Draw(tiledown, new Rectangle(GraphicsDevice.Viewport.Width - 175, 100, TileSize, TileSize), Color.White);
                    spriteBatch.DrawString(font, brushers.ToString(), new Vector2(GraphicsDevice.Viewport.Width - 140, 75), Color.Black);

                    if (currentEdittingState == Edit.Height)
                    {
                        spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 190, 200, 125, 25), Color.Black);
                        spriteBatch.DrawString(font, "Height", new Vector2(GraphicsDevice.Viewport.Width - 190, 200), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 190, 200, 125, 25), Color.White);
                        spriteBatch.DrawString(font, "Height", new Vector2(GraphicsDevice.Viewport.Width - 190, 200), Color.Black);
                    }
                    if (currentEdittingState == Edit.Impassible)
                    {
                        spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 190, 250, 125, 25), Color.Black);
                        spriteBatch.DrawString(font, "Impassible", new Vector2(GraphicsDevice.Viewport.Width - 190, 250), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 190, 250, 125, 25), Color.White);
                        spriteBatch.DrawString(font, "Impassible", new Vector2(GraphicsDevice.Viewport.Width - 190, 250), Color.Black);
                    }
                    if (currentEdittingState == Edit.Grappable)
                    {
                        spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 190, 300, 125, 25), Color.Black);
                        spriteBatch.DrawString(font, "Grappable", new Vector2(GraphicsDevice.Viewport.Width - 190, 300), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 190, 300, 125, 25), Color.White);
                        spriteBatch.DrawString(font, "Grappable", new Vector2(GraphicsDevice.Viewport.Width - 190, 300), Color.Black);
                    }
                }
                else if (currentEditorState == EditorState.Characters)
                {
                    if (pagenumber == 1)
                    {
                        int p = 0;
                        for (int j = 195; j >= 35; j = j - 40)
                        {
                            for (int k = 50; k <= 610; k = k + 40)
                            {
                                if (p < charactersprites.Length)
                                    spriteBatch.Draw(charactersprites[p], new Rectangle(GraphicsDevice.Viewport.Width - j, k, TileSize, TileSize), new Rectangle(0, 0, 32, 32), Color.White);
                                p++;
                            }
                        }
                    }
                }
                else if (currentEditorState == EditorState.Maps)
                {
                    int[] maps = new int[Maps.Count];
                    int i = 0;
                    foreach (Map map in Maps)
                    {
                        string pattern = "";
                        if (topLevelbuttonOn == 0)
                            pattern = "^" + currentMap.GetMapName.Remove(currentMap.GetMapName.Length - 3, 3) + "\\d" + "\\.txt";
                        else pattern = "\\.\\d\\.";
                        if (topLevelbuttonOn == 0)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(map.GetMapName, pattern))
                            {
                                maps[i] = map.Index;
                                i++;
                            }
                        }
                        else if (topLevelbuttonOn == 1)
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(map.GetMapName, pattern))
                            {
                                maps[i] = map.Index;
                                i++;
                            }
                        }
                        else
                        {
                            maps[i] = map.Index;
                            i++;
                        }

                    }
                    spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 195, 50, 80, 32), Color.White);
                    spriteBatch.DrawString(font, "New Map", new Vector2(GraphicsDevice.Viewport.Width - 194, 50), Color.Black);
                    spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 100, 50, 95, 32), Color.White);
                    if (topLevelbuttonOn == 1)
                        spriteBatch.DrawString(font, "Top Maps", new Vector2(GraphicsDevice.Viewport.Width - 96, 50), Color.Black);
                    if (topLevelbuttonOn == 2)
                        spriteBatch.DrawString(font, "All Maps", new Vector2(GraphicsDevice.Viewport.Width - 96, 50), Color.Black);
                    if (topLevelbuttonOn == 0)
                        spriteBatch.DrawString(font, "Map Tree", new Vector2(GraphicsDevice.Viewport.Width - 96, 50), Color.Black);
                    spriteBatch.DrawString(font, "Press Delete to Redo and Resize Map", new Vector2(5, 30), Color.White);
                    if (CreateNewMap || RedoCurrentMap)
                    {
                        if (CreateNewMap)
                            spriteBatch.DrawString(font, "Creating New Map...", new Vector2(5, 50), Color.White);
                        if (RedoCurrentMap)
                            spriteBatch.DrawString(font, "Redo Current Map...", new Vector2(5, 50), Color.White);
                        spriteBatch.DrawString(font, "Enter X size of Map and Press Enter: " + SizeX, new Vector2(5, 70), Color.White);
                        if (InputtingSizeY)
                            spriteBatch.DrawString(font, "Enter Y size of Map and Press Enter: " + SizeY, new Vector2(5, 90), Color.White);

                    }
                    int p = 0;
                    int j = 195;
                    for (int k = 100; k <= 610; k = k + 40)
                    {
                        if (p < i)
                        {
                            spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - j, k, 190, 25), Color.White);
                            spriteBatch.DrawString(font, Maps[maps[p]].GetMapName.Remove(Maps[maps[p]].GetMapName.Length - 4, 4), new Vector2(GraphicsDevice.Viewport.Width - j + 5, k), Color.Black);
                        }
                        p++;
                    }



                }
                else if (currentEditorState == EditorState.Transport)
                {
                    spriteBatch.Draw(canvas, new Rectangle(GraphicsDevice.Viewport.Width - 195, 50, 150, 32), Color.White);
                    spriteBatch.DrawString(font, "New Transport", new Vector2(GraphicsDevice.Viewport.Width - 190, 50), Color.Black);
                    if (CreatingTransportTile)
                    {
                        spriteBatch.DrawString(font, "Creating New Transport Tile Instructions", new Vector2(5, 60), Color.White);
                        spriteBatch.DrawString(font, "1.Hit arrow for exit direction from Transport", new Vector2(15, 90), Color.White);
                        spriteBatch.DrawString(font, "2.middle click with mouse over desired tile position", new Vector2(15, 110), Color.White);
                        spriteBatch.DrawString(font, "3.repeat for second transport on desired map", new Vector2(15, 130), Color.White);
                    }
                    spriteBatch.DrawString(font, "Right Click on Transport to delete, including its pair", new Vector2(5, 30), Color.White);
                }
                spriteBatch.Draw(chartab, new Rectangle(GraphicsDevice.Viewport.Width - 195, 10, 32, 32), Color.White);
                spriteBatch.Draw(enemytab, new Rectangle(GraphicsDevice.Viewport.Width - 155, 10, 32, 32), Color.White);
                spriteBatch.Draw(tiletab, new Rectangle(GraphicsDevice.Viewport.Width - 115, 10, 32, 32), Color.White);
                spriteBatch.Draw(maptab, new Rectangle(GraphicsDevice.Viewport.Width - 75, 10, 32, 32), Color.White);
                spriteBatch.Draw(transporttab, new Rectangle(GraphicsDevice.Viewport.Width - 35, 10, 32, 32), Color.White);
                spriteBatch.Draw(tileup, new Rectangle(GraphicsDevice.Viewport.Width - 195, 650, 20, 20), Color.White);
                spriteBatch.Draw(tiledown, new Rectangle(GraphicsDevice.Viewport.Width - 155, 650, 20, 20), Color.White);
                spriteBatch.DrawString(font, "Page " + pagenumber.ToString(), new Vector2(GraphicsDevice.Viewport.Width - 135, 645), Color.Black);
                spriteBatch.Draw(whiteBox, new Rectangle(GraphicsDevice.Viewport.Width - 60, 650, 55, 20), Color.White);
                spriteBatch.DrawString(font, "Edit", new Vector2(GraphicsDevice.Viewport.Width - 55, 645), Color.Black);

            }


        }
        public int impassibleCheck(int tilenumber)
        {
            foreach (int impassibleNumber in impassableTiles)
            {
                if (tilenumber == impassibleNumber)
                    return 1;
            }
            return 0;
        }
        public int grappableCheck(int tilenumber)
        {
            foreach (int grappableNumber in grappableTiles)
            {
                if (tilenumber == grappableNumber)
                    return 1;
            }
            return 0;
        }
        

    }
    class NumericalInput
    {
        public string text = "";
        KeyboardState currentkeyboard;
        KeyboardState oldkeyboard;
        public bool userinput()
        {
            oldkeyboard = currentkeyboard;
            currentkeyboard = Keyboard.GetState();
            bool NotDone = true;
            Keys[] pressedkeys;

            pressedkeys = currentkeyboard.GetPressedKeys();
            foreach (Keys keys in pressedkeys)
            {
                if (oldkeyboard.IsKeyUp(keys))
                {
                    if (keys == Keys.Back && text.Length > 0)
                    {
                        text = text.Remove(text.Length - 1, 1);
                    }
                    else if (keys == Keys.D1)
                        text = text.Insert(text.Length, "1");
                    else if (keys == Keys.D2)
                        text = text.Insert(text.Length, "2");
                    else if (keys == Keys.D3)
                        text = text.Insert(text.Length, "3");
                    else if (keys == Keys.D4)
                        text = text.Insert(text.Length, "4");
                    else if (keys == Keys.D5)
                        text = text.Insert(text.Length, "5");
                    else if (keys == Keys.D6)
                        text = text.Insert(text.Length, "6");
                    else if (keys == Keys.D7)
                        text = text.Insert(text.Length, "7");
                    else if (keys == Keys.D8)
                        text = text.Insert(text.Length, "8");
                    else if (keys == Keys.D9)
                        text = text.Insert(text.Length, "9");
                    else if (keys == Keys.D0)
                        text = text.Insert(text.Length, "0");
                    else if (keys == Keys.Back && text.Length > 0)
                         text = text.Remove(text.Length - 1, 1);
                    else if (keys == Keys.Enter)
                    {
                        text = "";
                        NotDone = false;
                    }
                    else text += keys.ToString();
                }
            }
            return NotDone;
        }
    }
}
