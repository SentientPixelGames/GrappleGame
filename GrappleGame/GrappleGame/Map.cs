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
        List<float[,]> objectList = new List<float[,]>();
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

        public Map(/*int MapSizeX, int MapSizeY, */Editor editor, Dude theDude, ContentManager Content, String MapName, Texture2D[] tile, Texture2D[] entity)
        {
            //object coverage layouts
            objectList.Add(new float[,] { { 0 } });//0, only a placeholder
            objectList.Add(new float[,] { { 2, 2, 2 }, { 2, 2, 2 }, { 0, 1, 0 }, { -1, -1, -1 } });//1
            objectList.Add(new float[,] { { 1, 1 }, { 1, 1 } });//2
            objectList.Add(new float[,] { { 1, 1 }, { 1, 1 } });//3
            objectList.Add(new float[,] { { 1, 1 }, { 1, 1 } });//4
            objectList.Add(new float[,] { { 1 } });//5
            objectList.Add(new float[,] { { 0, 0 } , { 1, 1 }});//6
            objectList.Add(new float[,] { { 1, 1, 1, 1, 1, -1 }, { 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1 }, { -1, 0, 1, 1, 0, 0 }, { 0, 0, 1, 1, 0, 0 }, { -1, -1, -1, 0, -1, -1 } });//7
            this.MapName = MapName;
            calculateSize();
            this.theDude = theDude;
            minimapSet = new int[this.sizeX, this.sizeY];
            this.editor = editor;
            Load(Content, tile, entity);
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
                    tileData[x, y].Draw(spriteBatch, map, tileData[(int)theDude.tilePosition.X, (int)theDude.tilePosition.Y].height);
                    if (editor.editorOn == true && editor.currentEditorState == Editor.EditorState.Edit)
                    {
                        if(editor.currentEdittingState == Editor.Edit.Height)
                            tileData[x, y].HeightDraw(spriteBatch, font);//draws heights on top of tiles
                        else if (editor.currentEdittingState == Editor.Edit.Impassible)
                        {
                            int impassible = TileData[x, y].impassible ? 1 : 0;
                            spriteBatch.DrawString(font, impassible.ToString(), new Vector2(x * 32 + (32 / 3), y * 32), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                        }
                        else if (editor.currentEdittingState == Editor.Edit.Grappable)
                        {
                            int grappable = TileData[x, y].grappable ? 1 : 0;
                            spriteBatch.DrawString(font, grappable.ToString(), new Vector2(x * 32 + (32 / 3), y * 32), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
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
            //tilset 4 is not included yet
            {
                int width = graphics.Viewport.Width/sizeX;
                int height = graphics.Viewport.Height/sizeY;
                int z;
                int x1 = 0;
                int y1 = 0;
                for (int x = 0; x < width*sizeX; x += width)
                {
                    y1 = 0;
                    for (int y = 0; y < height*sizeY; y += height)
                    {
                        z = this.textfiles.tileSet[x1, y1];
                        if (this.MiniMapSet[x1, y1] == 0)
                            spriteBatch.Draw(map, new Rectangle((int)x /** theDude.pixelPosition.X*/, (int)y, width, height), Color.Black);
                        else
                        {
                            if (z == 0 || z == 5 || z == 6 || z == 12 || z == 13 || z == 17 || z == 23 || z == 31
                                || z == 32 || z == 33 || z == 37 || z == 38 || z == 39 || z == 40 || z == 41 || z == 42) //grass tiles
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Green);

                            else if (z == 1 || z == 14 || z == 15 || z == 16 || z == 24 || z == 51 || z == 52) // forest tiles
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.DarkGreen);

                            else if (z == 3 || z == 18 || z == 97 || z == 98 || z == 99 || z == 100 || z == 101 || z == 102
                                || z == 103 || z == 104 || z == 105 || z == 106 || z == 107 || z == 108 || z == 109) //castle tiles
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Gray);

                            else if (z == 2 || z == 8 || z == 9 || z == 10 || z == 11 || z == 43 || z == 44 || z == 45 || //water tiles
                                z == 46 || z == 47 || z == 48 || z == 49 || z == 50 || z == 110 || z == 111 || z == 112 || z == 113 || z == 114
                                || z == 115 || z == 116 || z == 117 || z == 118 || z == 119 || z == 120 || z == 121)
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Blue);

                            else if (z == 7) // sand tiles
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Yellow);

                            else if (z == 19 || z == 20 || z == 21 || z == 22 || z == 25 || z == 26 || z == 27 || z == 28 ||
                               z == 29 || z == 30 || z == 34 || z == 35 || z == 36 || z == 96) //path tiles
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.White);

                            else if (z == 53 || z == 54 || z == 55 || z == 56 || z == 57 || z == 58 || z == 59 // house tiles
                                || z == 60 || z == 61 || z == 62 || z == 63 || z == 64)
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Purple);

                            else if (z == 65 || z == 66 || z == 67 || z == 68 || z == 72 || z == 73 || z == 74 || z == 75 || z == 76 ||
                                z == 77 || z == 78 || z == 79 || z == 80 || z == 81 || z == 82 || z == 83 || z == 84 || z == 85 || z == 86 || z == 87
                                || z == 88 || z == 89 || z == 90 || z == 91 || z == 92 || z == 93 || z == 94 || z == 95) // mountain tiles
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Brown);

                            else if (z == 69 || z == 70 || z == 71) //dock tiles
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.White);
                            else if (z == 122)
                                spriteBatch.Draw(map, new Rectangle(x, y, width, height), Color.Black);
                            
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
        public void objectSorter()
        {
            //add complete sorting capabilities, including height level, tiles, etc
            for (int x = 0; x < this.sizeX; x++)
            {
                for (int y = 0; y < this.SizeY; y++)
                {
                    TileData[x, y].coverage = -1;
                }
            }
            for (int x = 0; x < this.sizeX; x++)
            {
                for (int y = 0; y < this.SizeY; y++)
                {
                    if (TileData[x, y].entity != null)
                    {
                        Rectangle temp = TileData[x, y].entity.Bounds;
                        int tilesizeX = temp.Width / 32;
                        int tilesizeY = temp.Height / 32;
                        CalculateMapCoverage(new Rectangle(x, y, tilesizeX, tilesizeY), textFile.entitySet[x, y]);
                    }
                }
            }
            List<Vector2> Sorted_Objects = new List<Vector2>();
            for (int y = 0; y < this.sizeY; y++)
            {
                for (int x = 0; x < this.SizeX; x++)
                {
                    if (TileData[x, y].entity != null)
                    {
                        bool test = false;
                        foreach (Vector2 sorted_object_positions in Sorted_Objects)
                        {
                            if (TileData[x, y].tilePosition == sorted_object_positions)
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
                                    if (TileData[z, w].entity != null)
                                    {
                                        if (z == x && w == y)
                                            continue;
                                        if (new Rectangle(x, y, TileData[x, y].entity.Width / 32, TileData[x, y].entity.Height / 32).Intersects(new Rectangle(z, w, TileData[z, w].entity.Width / 32, TileData[z, w].entity.Height / 32)))
                                        {
                                            test = false;
                                        }
                                    }
                                }
                            }
                            if (!test)
                            {
                                Touching_Objects.Add(TileData[x, y].tilePosition);
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
                                                if (TileData[z, w].entity != null)
                                                {
                                                    if (new Rectangle((int)T_Object.X, (int)T_Object.Y, TileData[(int)T_Object.X, (int)T_Object.Y].entity.Width / 32, TileData[(int)T_Object.X, (int)T_Object.Y].entity.Height / 32).Intersects(new Rectangle(z, w, TileData[z, w].entity.Width / 32, TileData[z, w].entity.Height / 32)))
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
                                                            Objects_To_Add.Add(TileData[z, w].tilePosition);
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
                                        int direction = Sort(new Rectangle((int)list[counter].X, (int)list[counter].Y, TileData[(int)list[counter].X, (int)list[counter].Y].entity.Width / 32, TileData[(int)list[counter].X, (int)list[counter].Y].entity.Height / 32),
                                            new Rectangle((int)list[counter - 1].X, (int)list[counter - 1].Y, TileData[(int)list[counter - 1].X, (int)list[counter - 1].Y].entity.Width / 32, TileData[(int)list[counter - 1].X, (int)list[counter - 1].Y].entity.Height / 32),
                                            TileData[(int)list[counter].X, (int)list[counter].Y].entitydepth, TileData[(int)list[counter - 1].X, (int)list[counter - 1].Y].entitydepth,
                                            textFile.entitySet[(int)list[counter].X, (int)list[counter].Y], textFile.entitySet[(int)list[counter - 1].X, (int)list[counter - 1].Y]);
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
                                    TileData[(int)list[counter].X, (int)list[counter].Y].entitydepth = 0.7f + 0.0001f * counter;
                                    Sorted_Objects.Add(TileData[(int)list[counter].X, (int)list[counter].Y].tilePosition);
                                }
                            }
                            else
                            {
                                Sorted_Objects.Add(TileData[x, y].tilePosition);
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < this.sizeX; x++)
            {
                for (int y = 0; y < this.SizeY; y++)
                {
                    textFile.depthSet[x, y] = TileData[x, y].entitydepth;
                    textFile.coverageSet[x, y] = TileData[x, y].coverage;
                }
            }
        }
        private void Load(ContentManager Content, Texture2D[] tile, Texture2D[] entity)
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
                    TileData[x, y] = new tileClass(x, y, tile[textFile.tileSet[x, y]], textFile.heightSet[x, y], textFile.tileSet[x, y], textFile.impassSet[x, y], textFile.grappleSet[x, y], entity[textFile.entitySet[x, y]], textfiles.depthSet[x, y], textfiles.coverageSet[x, y]);
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
        private void CalculateMapCoverage(Rectangle imageDimensions, int entityNum)
        {
            //calculates what parts of the map is covered by objects. Only one coverage type exists per tile, even if objects overlap, so coverage must be selected with priorities.
            //the priority is as follows: impassible object (1) (tree trunk), overhanging object (>1) (tree branches), shadows (0) (tree shadow), nothing (-1) (no objects/shadows).
            float[,] coverage = objectList.ElementAt(entityNum);
            for (int i = 0; i < imageDimensions.Width; i++)
            {
                for (int j = 0; j < imageDimensions.Height; j++)
                {
                    if (TileData[imageDimensions.X + i, imageDimensions.Y + j].coverage == 1)
                        continue;
                    else if (TileData[imageDimensions.X + i, imageDimensions.Y + j].coverage > 1)
                    {
                        if (coverage[j, i] == 1)
                            TileData[imageDimensions.X + i, imageDimensions.Y + j].coverage = coverage[j, i];
                        else continue;
                    }
                    else if (TileData[imageDimensions.X + i, imageDimensions.Y + j].coverage == 0)
                    {
                        if (coverage[j, i] > 0)
                            TileData[imageDimensions.X + i, imageDimensions.Y + j].coverage = coverage[j, i];
                        else continue;
                    }
                    else if (TileData[imageDimensions.X + i, imageDimensions.Y + j].coverage == -1)
                    {
                        if (coverage[j, i] > -1)
                            TileData[imageDimensions.X + i, imageDimensions.Y + j].coverage = coverage[j, i];
                        else continue;
                    }
                }
            }
        }
        private int Sort(Rectangle baseObject, Rectangle otherObject, float baseDepth, float otherDepth, int baseEntity, int otherEntity)
        {
            float[,] BASE = objectList.ElementAt(baseEntity);
            float[,] other = objectList.ElementAt(otherEntity);
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
