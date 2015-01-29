using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace GrappleGame
{
    class Map
    {
        Texture2D map; // blank image for map
        int sizeX;
        int sizeY;
        String MapName;
        public List<TransportTile> transporttiles = new List<TransportTile>();
        public int Index;
        public bool mapOn;
        textFiles textFile;
        tileClass[,] TileData;
        int[,] minimapSet;
        int maxbufferX;
        int minbufferX;
        int maxbufferY;
        int minbufferY;
        #region getters
        public textFiles textfiles
        {
            get { return textFile; }
        }
        public tileClass[,] tileData
        {
            get { return TileData; }
        }
        public int SizeX
        {
            get { return sizeX; }
        }
        public int SizeY
        {
            get { return sizeY; }
        }
        public int[,] MiniMapSet
        {
            get { return minimapSet; }
        }
        public string GetMapName
        {
            get { return MapName; }
        }
        #endregion

        Editor editor;
        Dude theDude;
        SpriteFont font;
        public int smallmapCount;

        Random rndNUM = new Random();

        public Map(Editor editor, Dude theDude, ContentManager Content, String MapName, Tile[] tiles, Object[] objects)
        {
            this.MapName = MapName;
            calculateSize();
            this.theDude = theDude;
            minimapSet = new int[this.sizeX, this.sizeY];
            this.editor = editor;
            Load(Content, tiles, objects);
        }

        private void calculateSize()
        {
            using (StreamReader reader = new StreamReader(this.MapName))
            {

                int y = 0;
                int x = 0;
                int tempX = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    foreach (char ch in line)
                    {
                        if (ch == ',')
                            tempX++;
                    }
                    if (tempX > x)
                        x = tempX;
                    tempX = 0;
                    y++;
                }
                reader.Close();
                this.sizeX = x;
                this.sizeY = y;
            }
        }
        public void TransportUpdate(ref Map currentMap,ref Dude dude)
        {
            bool isTransported = false;
            if (theDude.tilePosition * 32 == theDude.pixelPosition)
            {
                foreach (TransportTile transTile in transporttiles)
                {
                    isTransported = transTile.transport(ref dude, ref currentMap);
                    if (isTransported)
                        break;//need to clear queue after transport once queue function enabled
                }
            }
        }
        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.M) && editor.editorOn == false)
                mapOn = true; //turns map on when m button is held down
            else mapOn = false;
            TileBuffer();//calculates drawing tile buffer
        }

        public void Draw(SpriteBatch spriteBatch, Dude theDude)
        {
            for (int x = minbufferX; x < maxbufferX; x++)
            {
                for (int y = minbufferY; y < maxbufferY; y++)
                { 
                    tileData[x, y].DrawTile(spriteBatch, map, tileData[(int)theDude.tilePosition.X, (int)theDude.tilePosition.Y].tileData.height);
                    tileData[x, y].DrawObject(spriteBatch, theDude.isHidden());
                    if (editor.editorOn == true && editor.currentEditorState == Editor.EditorState.Edit)
                    {
                        if (editor.currentEdittingState == Editor.Edit.Height)
                            tileData[x, y].HeightDraw(spriteBatch, font);//draws heights on top of tiles
                        else if (editor.currentEdittingState == Editor.Edit.Impassible)
                        {
                            TileData[x, y].ImpassibleDraw(spriteBatch, font);
                            int impassible = TileData[x, y].tileData.impassible ? 1 : 0;
                            spriteBatch.DrawString(font, impassible.ToString(), new Vector2(x * 32 + (32 / 3), y * 32), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                        }
                        else if (editor.currentEdittingState == Editor.Edit.ObjectDepth_Main)
                        {
                            TileData[x, y].MainDepthDraw(spriteBatch, font);
                        }
                        else if (editor.currentEdittingState == Editor.Edit.ObjectDepth_Secondary)
                        {
                            TileData[x, y].SecondaryDepthDraw(spriteBatch, font);
                        }
                        else if (editor.currentEdittingState == Editor.Edit.ObjectHeight)
                        {
                            TileData[x, y].HeightObjectDraw(spriteBatch, font);
                        }
                        else if (editor.currentEdittingState == Editor.Edit.Shadow)
                        {
                            TileData[x, y].ShadowDraw(spriteBatch, font);
                        }
                        else if (editor.currentEdittingState == Editor.Edit.Solid)
                        {
                            TileData[x, y].SolidDraw(spriteBatch, font);
                        }
                        else if (editor.currentEdittingState == Editor.Edit.TileDepth)
                        {
                            TileData[x, y].TileDepthDraw(spriteBatch, font);
                        }

                    }
                    minimapSet[x, y] = 1;//set visible range on minimap visible if not already
                }
            }
            if (editor.editorOn)
            {
                foreach (TransportTile tile in transporttiles)
                {
                    tile.Draw(spriteBatch);
                }
            }
        }

        public void DrawMiniMap(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            #region minimap draw
            if (mapOn == true) //This nested for loop draws a miniature map in the upper left corner for the player to observe when the "M" is clicked
            //tilset 4 is not included yet, objects are not included yet, heights are not included yet.
            {
                int width = graphics.Viewport.Width/sizeX;
                int height = graphics.Viewport.Height/sizeY;
                string z;
                int x1 = 0;
                int y1 = 0;
                for (int x = 0; x < width*sizeX; x += width)
                {
                    y1 = 0;
                    for (int y = 0; y < height*sizeY; y += height)
                    {
                        z = this.TileData[x1,y1].tile.minimapColor;
                        if (this.MiniMapSet[x1, y1] == 0)
                            spriteBatch.Draw(map, new Rectangle((int)x, (int)y, width, height), Color.Black);
                        else
                        {
                            switch (z)
                            {
                                case "green":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Green);
                                    break;
                                case "darkgreen":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.DarkGreen);
                                    break;
                                case "gray":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Gray);
                                    break;
                                case "blue":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Blue);
                                    break;
                                case "yellow":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Yellow);
                                    break;
                                case "white":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.White);
                                    break;
                                case "purple":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Purple);
                                    break;
                                case "brown":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Brown);
                                    break;
                                case "black":
                                    spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Black);
                                    break;
                            }                           
                        }
                        y1++;
                    }
                    x1++;
                }
                spriteBatch.Draw(map, new Rectangle((int)theDude.tilePosition.X*width, (int)theDude.tilePosition.Y*height, width, height), Color.Red); //draws dudes position on minimap
            }
            #endregion
        }

        public void setTheDude(Dude theDude)
        {
            this.theDude = theDude;
        }
        public void setTheEditor(Editor editor)
        {
            this.editor = editor;
        }
        public void objectSorter(Object[] objects)
        {
            //add complete sorting capabilities, including base height level, tiles, etc
            for (int x = 0; x < this.sizeX; x++)
            {
                for (int y = 0; y < this.SizeY; y++)
                {
                    TileData[x, y].objectData.height = 0;
                    TileData[x, y].objectData.solid = false;
                    TileData[x, y].objectData.shadow = 0;//has not been applied yet
                }
            }
            for (int x = 0; x < this.sizeX; x++)
            {
                for (int y = 0; y < this.SizeY; y++)
                {
                    if (TileData[x, y].Object != null)
                    {
                        Rectangle temp = TileData[x, y].Object.basicTexture.Bounds;
                        int tilesizeX = temp.Width / 32;
                        int tilesizeY = temp.Height / 32;
                        temp = new Rectangle(x, y, tilesizeX, tilesizeY);
                        CalculateMapHeights(temp, objects[textFile.objectSet[x, y]]);
                    }
                }
            }
            List<Vector2> Sorted_Objects = new List<Vector2>();
            for (int y = 0; y < this.sizeY; y++)
            {
                for (int x = 0; x < this.SizeX; x++)
                {
                    if (TileData[x, y].Object != null)
                    {
                        bool test = false;
                        foreach (Vector2 sorted_object_positions in Sorted_Objects)
                        {
                            if (TileData[x, y].position == sorted_object_positions)
                            {
                                test = true;
                                break;
                            }
                        }
                        if (test == false)
                        {
                            test = true;
                            List<Vector2> Touching_Objects = new List<Vector2>();
                            for (int w = 0; w < this.sizeY; w++)
                            {
                                for (int z = 0; z < this.SizeX; z++)
                                {
                                    if (TileData[z, w].Object != null)
                                    {
                                        if (z == x && w == y)
                                            continue;
                                        if (new Rectangle(x, y, TileData[x, y].Object.basicTexture.Width / 32, TileData[x, y].Object.basicTexture.Height / 32)
                                            .Intersects(new Rectangle(z, w, TileData[z, w].Object.basicTexture.Width / 32, TileData[z, w].Object.basicTexture.Height / 32)))
                                        {
                                            test = false;
                                        }
                                    }
                                }
                            }
                            if (!test)
                            {
                                Touching_Objects.Add(TileData[x, y].position);
                                while (true) //finds and stores objects that are touching eachother
                                {
                                    List<Vector2> Objects_To_Add = new List<Vector2>();
                                    bool all_objects_found = true;
                                    foreach (Vector2 T_Object in Touching_Objects)
                                    {
                                        for (int w = 0; w < this.sizeY; w++)
                                        {
                                            for (int z = 0; z < this.SizeX; z++)
                                            {
                                                if (z == T_Object.X && w == T_Object.Y)
                                                    continue;
                                                if (TileData[z, w].Object != null)
                                                {
                                                    if (new Rectangle((int)T_Object.X, (int)T_Object.Y, TileData[(int)T_Object.X, (int)T_Object.Y].Object.basicTexture.Width / 32, TileData[(int)T_Object.X, (int)T_Object.Y].Object.basicTexture.Height / 32)
                                                        .Intersects(new Rectangle(z, w, TileData[z, w].Object.basicTexture.Width / 32, TileData[z, w].Object.basicTexture.Height / 32)))
                                                    {
                                                        bool alreadyinlistcheck = false;
                                                        foreach (Vector2 object_check in Touching_Objects)
                                                        {
                                                            if (new Vector2(z, w) == object_check)
                                                            {
                                                                alreadyinlistcheck = true;
                                                            }
                                                        }
                                                        foreach (Vector2 object_check in Objects_To_Add)
                                                        {
                                                            if (new Vector2(z, w) == object_check)
                                                            {
                                                                alreadyinlistcheck = true;
                                                            }
                                                        }
                                                        if (alreadyinlistcheck)
                                                            continue;
                                                        else
                                                        {
                                                            Objects_To_Add.Add(TileData[z, w].position);
                                                            all_objects_found = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    foreach (Vector2 T_Objects in Objects_To_Add)
                                    {
                                        Touching_Objects.Add(T_Objects);
                                    }
                                    if (all_objects_found)
                                        break;
                                }
                                bool sorted = false;
                                Vector2[] list = new Vector2[Touching_Objects.Count];
                                int q = 0;
                                foreach (Vector2 OBJECT in Touching_Objects)
                                {
                                    list[q] = OBJECT;
                                    q++;
                                }
                                while (!sorted)
                                {
                                    sorted = true;
                                    for (int counter = 1; counter < list.Length; counter++)
                                    {
                                        Vector2 temp;
                                        int direction = Sort(new Rectangle((int)list[counter].X, (int)list[counter].Y, TileData[(int)list[counter].X, (int)list[counter].Y].Object.basicTexture.Width / 32, TileData[(int)list[counter].X, (int)list[counter].Y].Object.basicTexture.Height / 32),
                                            new Rectangle((int)list[counter - 1].X, (int)list[counter - 1].Y, TileData[(int)list[counter - 1].X, (int)list[counter - 1].Y].Object.basicTexture.Width / 32, TileData[(int)list[counter - 1].X, (int)list[counter - 1].Y].Object.basicTexture.Height / 32),
                                            TileData[(int)list[counter].X, (int)list[counter].Y].objectData.mainDepth, TileData[(int)list[counter - 1].X, (int)list[counter - 1].Y].objectData.mainDepth,
                                            objects[textFile.objectSet[(int)list[counter].X, (int)list[counter].Y]], objects[textFile.objectSet[(int)list[counter - 1].X, (int)list[counter - 1].Y]]);
                                        if (direction == 0) //the object later in the list should be drawn last
                                        {
                                            temp = list[counter];
                                            list[counter] = list[counter - 1];
                                            list[counter - 1] = temp;
                                            sorted = false;
                                        }
                                    }
                                }
                                for (int counter = 0; counter < list.Length; counter++)
                                {
                                    TileData[(int)list[counter].X, (int)list[counter].Y].objectData.mainDepth = 0.40003f + 0.0001f * counter;
                                    TileData[(int)list[counter].X, (int)list[counter].Y].objectData.secondaryDepth = TileData[(int)list[counter].X, (int)list[counter].Y].objectData.mainDepth + 0.3f;
                                    Sorted_Objects.Add(TileData[(int)list[counter].X, (int)list[counter].Y].position);
                                }
                            }
                            else
                            {
                                Sorted_Objects.Add(TileData[x, y].position);
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < this.sizeX; x++)
            {
                for (int y = 0; y < this.SizeY; y++)
                {
                    textFile.depthSet[x, y] = TileData[x, y].objectData.mainDepth;
                    textFile.objHeightSet[x, y] = TileData[x, y].objectData.height;
                    int solid = TileData[x, y].objectData.solid == true ? 1 : 0;
                    textFile.solidSet[x, y] = solid;
                    textFile.shadowSet[x, y] = TileData[x, y].objectData.shadow;
                }
            }
        }
        private void Load(ContentManager Content, Tile[] tiles, Object[] objects)
        {
            font = Content.Load<SpriteFont>("Fonts/words");
            map = Content.Load<Texture2D>("Other/map");
            TileData = new tileClass[this.sizeX, this.sizeY];
            textFile = new textFiles(this.sizeX, this.sizeY, this.MapName);
            textFile.LoadLevel(this.sizeX);
            for (int x = 0; x < this.sizeX; x++)
            {
                for (int y = 0; y < this.sizeY; y++)
                {
                    TileData[x, y] = new tileClass(new Vector2(x, y), tiles[textFile.tileSet[x, y]], objects[textFile.objectSet[x, y]]);
                    TileData[x, y].loadTileData(1f, textFile.impassSet[x, y], textfiles.heightSet[x, y]);
                    TileData[x, y].loadObjectData(textFile.depthSet[x, y], textFile.depthSet[x, y] + 0.3f, textFile.objHeightSet[x,y], textFile.solidSet[x, y], textfiles.shadowSet[x,y]);
                }
            }
            for (int x1 = 0; x1 < this.sizeX; x1++)
            {
                for (int y1 = 0; y1 < this.sizeY; y1++)
                {
                    minimapSet[x1, y1] = 0;
                    //sets minimap to be completely visible, change to 1 for invisible
                }
            }
        }
        private void CalculateMapHeights(Rectangle imageDimensions, Object Object)
        {
            //calculates what parts of the map is covered by objects. Only one coverage type exists per tile, even if objects overlap, so coverage must be selected with priorities.
            //the priority is as follows: impassible object (1) (tree trunk), overhanging object (>1) (tree branches), nothing (0).
            float[,] heights = Object.height;
            float[,] solids = Object.solid;
            float[,] shadows = Object.shadow;
            for (int i = 0; i < imageDimensions.Width; i++)
            {
                for (int j = 0; j < imageDimensions.Height; j++)
                {
                    if (TileData[imageDimensions.X + i, imageDimensions.Y + j].objectData.height < heights[j,i])
                        TileData[imageDimensions.X + i, imageDimensions.Y + j].objectData.height = heights[j, i];
                    if (solids[j, i] == 1)
                        TileData[imageDimensions.X + i, imageDimensions.Y + j].objectData.solid = true;
                    if (shadows[j, i] == 1)
                        TileData[imageDimensions.X + i, imageDimensions.Y + j].objectData.shadow = shadows[j, i];
                }
            }
        }
        private int Sort(Rectangle baseObject, Rectangle otherObject, float baseDepth, float otherDepth, Object Object1, Object Object2)
        {
            float[,] BASE = Object1.height;
            float[,] other = Object2.height;
            int baseLow = baseObject.Y;
            int otherLow = otherObject.Y;
            int baseRight = baseObject.X;
            int otherright = otherObject.X;
            float baseMax = -1;
            float otherMax = -1;
            int tempLow = 0;
            int tempRight = 0;
            for (int i = 0; i < baseObject.Height; i++)
            {
                for (int j = 0; j < baseObject.Width; j++)
                {
                    if (BASE[i, j] > 0)
                        if (i >= tempLow)
                            tempLow = i;
                    if (BASE[i, j] > 0)
                        if (j >= tempRight)
                            tempRight = j;
                    if (BASE[i, j] > baseMax)
                        baseMax = BASE[i, j];
                }

            }
            baseLow += tempLow;
            baseRight += tempRight;
            tempLow = 0;
            tempRight = 0;
            for (int i = 0; i < otherObject.Height; i++)
            {
                for (int j = 0; j < otherObject.Width; j++)
                {
                    if (other[i, j] > 0)
                        if (i >= tempLow)
                            tempLow = i;
                    if (other[i, j] > 0)
                        if (j >= tempRight)
                            tempRight = j;
                    if (other[i, j] > otherMax)
                        otherMax = other[i, j];
                }
            }
            otherLow += tempLow;
            otherright += tempRight;

            if (otherLow < baseLow)
                return 0;
            else if (otherLow > baseLow)
                return 1;
            else
            {
                if (otherMax > baseMax)
                    return 1;
                else if (otherMax < baseMax)
                    return 0;
                else
                {
                    if (otherright < baseRight)
                        return 0;
                    else if (otherright > baseRight)
                        return 1;
                    else return 0;
                }
            }
        }
        public void TileBuffer()
        {
            maxbufferX = 1 + (int)(theDude.pixelPosition.X + (15 * Constants.tilesize * (1 / theDude.zoom))) / Constants.tilesize; //calculates buffer just outside edge of gamescreen
            minbufferX = -1 + (int)(theDude.pixelPosition.X - (15 * Constants.tilesize * (1 / theDude.zoom))) / Constants.tilesize;
            maxbufferY = 1 + (int)(theDude.pixelPosition.Y + (13 * Constants.tilesize * (1 / theDude.zoom))) / Constants.tilesize;
            minbufferY = -1 + (int)(theDude.pixelPosition.Y - (13 * Constants.tilesize * (1 / theDude.zoom))) / Constants.tilesize;
            if (maxbufferX >= sizeX)
                maxbufferX = sizeX;
            if (maxbufferY >= sizeY)
                maxbufferY = sizeY;
            if (minbufferX < 0)
                minbufferX = 0;
            if (minbufferY < 0)
                minbufferY = 0;
        }
        public void CreateNewMap(int Sx, int Sy, int basetile, string newmapname)
        {
            textFile.SavedSmall(Sx, Sy, smallmapCount, basetile, newmapname);
        }
    }
}
