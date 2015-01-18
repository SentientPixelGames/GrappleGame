using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GrappleGame
{
    public class mainScript: Microsoft.Xna.Framework.Game
    {
        #region Global Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Dude theDude; //stores all variables, data, movement, camerawork associated with the dude
        Editor editor; //storese all calculations and drawings associated with editor mode
        Texture2D[] tile = new Texture2D[124]; //secondary storage of each tile texture
        Texture2D[] entities = new Texture2D[8];
        Texture2D[] charactersprites = new Texture2D[3];
        //string[] characternames = new string[1];
        //List<Character> characterlist = new List<Character>();
        Texture2D map; //blank image for map
        SpriteFont font;
        KeyboardState keys;
        MouseState mouse = Mouse.GetState();
        Constants Constants = new Constants();
        Map currentMap;
        MapNames Load_SaveMapNames = new MapNames();
        List<Map> Maps;
        List<string[]> MapName;
        int[] Editing_MapIndex = {0,0,0,0,0};
        Vector2[] Editing_TransportPositions = new Vector2[2];
        MouseState oldmouse;
        UserInput userinput = new UserInput();
        Random rndNUM = new Random();

        #endregion
        XFiles xsd = new XFiles();
        public mainScript()
        {
            #region Graphics Window
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; // makes mouse visible in game
            graphics.PreferredBackBufferHeight = 672;
            graphics.PreferredBackBufferWidth = 800; //sets game window size
            graphics.PreferMultiSampling = false; //this line is possible graphical error fix from deep space like the spritebatch in the initialize
            Window.AllowUserResizing = true;//this line allows user to adjust window in game, will not be included in real game
            #endregion
        }
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);// put this in initialize, it was one of the errors that possibly caused the graphics problem in deep space on ice dogs computer        
            #region tile textures
            #region Tiles 0- 9
            tile[0] = Content.Load<Texture2D>("Tiles/main/grass");
            tile[1] = Content.Load<Texture2D>("treetile");
            tile[2] = Content.Load<Texture2D>("Tiles/main/watertile");
            tile[3] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile");
            tile[4] = Content.Load<Texture2D>("Tiles/main/cavefloortile");
            tile[5] = Content.Load<Texture2D>("Tiles/grass&sand/gstile");
            tile[6] = Content.Load<Texture2D>("Tiles/main/stile");
            tile[7] = Content.Load<Texture2D>("Tiles/main/dtile");
            tile[8] = Content.Load<Texture2D>("Tiles/water&sand/wstiletrans");
            tile[9] = Content.Load<Texture2D>("Tiles/water&sand/wstiletransbot");
            #endregion
            #region Tiles 10-19
            tile[10] = Content.Load<Texture2D>("Tiles/water&sand/wstiletransright");
            tile[11] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranstop");
            tile[12] = Content.Load<Texture2D>("Tiles/grass&sand/gScornertile");
            tile[13] = Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass");
            tile[14] = Content.Load<Texture2D>("bigtreebottomtile");
            tile[15] = Content.Load<Texture2D>("bigtreetoptile");
            tile[16] = Content.Load<Texture2D>("bushtile");
            tile[17] = Content.Load<Texture2D>("Tiles/main/blueflowertile");
            tile[18] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile2");
            tile[19] = Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1tile");
            #endregion
            #region Tiles 20-29
            tile[20] = Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner2tile");
            tile[21] = Content.Load<Texture2D>("Tiles/main/forestpathtile");
            tile[22] = Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile");
            tile[23] = Content.Load<Texture2D>("Tiles/main/purpleflowertile");
            tile[24] = Content.Load<Texture2D>("rosebushtile");
            tile[25] = Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner2-1tile");
            tile[26] = Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner2-2tile");
            tile[27] = Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner2-3tile");
            tile[28] = Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1-1tile");
            tile[29] = Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1-2tile");
            #endregion
            #region Tiles 30-39
            tile[30] = Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1-3tile");
            tile[31] = Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-1");
            tile[32] = Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-2");
            tile[33] = Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-3");
            tile[34] = Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile-1");
            tile[35] = Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile-2");
            tile[36] = Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile-3");
            tile[37] = Content.Load<Texture2D>("Tiles/grass&sand/gstile-1");
            tile[38] = Content.Load<Texture2D>("Tiles/grass&sand/gstile-2");
            tile[39] = Content.Load<Texture2D>("Tiles/grass&sand/gstile-3");
            #endregion
            #region Tiles 40-49
            tile[40] = Content.Load<Texture2D>("Tiles/grass&sand/gScornertile-1");
            tile[41] = Content.Load<Texture2D>("Tiles/grass&sand/gScornertile-2");
            tile[42] = Content.Load<Texture2D>("Tiles/grass&sand/gScornertile-3");
            tile[43] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner1");
            tile[44] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner11");
            tile[45] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner12");
            tile[46] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner13");
            tile[47] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner2");
            tile[48] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner21");
            tile[49] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner22");
            #endregion
            #region Tiles 50-59
            tile[50] = Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner23");
            tile[51] = Content.Load<Texture2D>("stump");
            tile[52] = Content.Load<Texture2D>("boudler");
            tile[53] = Content.Load<Texture2D>("houseside");
            tile[54] = Content.Load<Texture2D>("housedoor");
            tile[55] = Content.Load<Texture2D>("housewindow");
            tile[56] = Content.Load<Texture2D>("ruralrooftile");
            tile[57] = Content.Load<Texture2D>("ruralrooftiletrans");
            tile[58] = Content.Load<Texture2D>("ruralrooftile2");
            tile[59] = Content.Load<Texture2D>("ruralrooftile3");
            #endregion
            #region Tiles 60-69
            tile[60] = Content.Load<Texture2D>("ruralrooftile4");
            tile[61] = Content.Load<Texture2D>("ruralrooftiletrans2");
            tile[62] = Content.Load<Texture2D>("ruralrooftile5");
            tile[63] = Content.Load<Texture2D>("ruralrooftile6");
            tile[64] = Content.Load<Texture2D>("ruralrooftiletrans3");
            tile[65] = Content.Load<Texture2D>("Tiles/mountain/mountain");
            tile[66] = Content.Load<Texture2D>("Tiles/mountain/mountain3");
            tile[67] = Content.Load<Texture2D>("Tiles/mountain/mountain2");
            tile[68] = Content.Load<Texture2D>("Tiles/mountain/mountainopening");
            tile[69] = Content.Load<Texture2D>("Tiles/Dock/dockhoriz");
            #endregion
            #region Tiles 70-79
            tile[70] = Content.Load<Texture2D>("Tiles/Dock/dockpost");
            tile[71] = Content.Load<Texture2D>("Tiles/Dock/dockvert");
            tile[72] = Content.Load<Texture2D>("Tiles/mountain/mountain4");
            tile[73] = Content.Load<Texture2D>("Tiles/mountain/mountain5");
            tile[74] = Content.Load<Texture2D>("Tiles/mountain/mountain6");
            tile[75] = Content.Load<Texture2D>("Tiles/mountain/mountain7");
            tile[76] = Content.Load<Texture2D>("Tiles/mountain/mountain8");
            tile[77] = Content.Load<Texture2D>("Tiles/mountain/mountain9");
            tile[78] = Content.Load<Texture2D>("Tiles/mountain/mountain10");
            tile[79] = Content.Load<Texture2D>("Tiles/mountain/mountain11");
            #endregion
            #region Tiles 80-89
            tile[80] = Content.Load<Texture2D>("Tiles/mountain/mountain12");
            tile[81] = Content.Load<Texture2D>("Tiles/mountain/mountain13");
            tile[82] = Content.Load<Texture2D>("Tiles/mountain/mountain14");
            tile[83] = Content.Load<Texture2D>("Tiles/mountain/mountain15");
            tile[84] = Content.Load<Texture2D>("Tiles/mountain/mountain16");
            tile[85] = Content.Load<Texture2D>("Tiles/mountain/mountain17");
            tile[86] = Content.Load<Texture2D>("Tiles/mountain/mountain18");
            tile[87] = Content.Load<Texture2D>("Tiles/mountain/mountain19");
            tile[88] = Content.Load<Texture2D>("Tiles/grass&edge/gtileedge1");
            tile[89] = Content.Load<Texture2D>("Tiles/grass&edge/gtileedge2");
            #endregion
            #region Tiles 90-99
            tile[90] = Content.Load<Texture2D>("Tiles/grass&edge/gtileedge3");
            tile[91] = Content.Load<Texture2D>("Tiles/grass&edge/gtileedge4");
            tile[92] = Content.Load<Texture2D>("Tiles/grass&edge/gtileedge5");
            tile[93] = Content.Load<Texture2D>("Tiles/grass&edge/gtileedge6");
            tile[94] = Content.Load<Texture2D>("Tiles/grass&edge/gtileedge7");
            tile[95] = Content.Load<Texture2D>("Tiles/grass&edge/gtileedge8");
            tile[96] = Content.Load<Texture2D>("Tiles/stairs/forestpathstairs1");
            tile[97] = Content.Load<Texture2D>("Tiles/Castle/castlewindow1");
            tile[98] = Content.Load<Texture2D>("Tiles/Castle/castlewindow2");
            tile[99] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile3");
            #endregion
            #region Tiles 100-109
            tile[100] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile4");
            tile[101] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile5");
            tile[102] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile6");
            tile[103] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile7");
            tile[104] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile8");
            tile[105] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile9");
            tile[106] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile10");
            tile[107] = Content.Load<Texture2D>("Tiles/Castle/castlefloortile11");
            tile[108] = Content.Load<Texture2D>("Tiles/Castle/castledoor");
            tile[109] = Content.Load<Texture2D>("Tiles/Castle/castledoor2");
            #endregion
            #region Tiles 110-119
            tile[110] = Content.Load<Texture2D>("Tiles/grass&water/grasswater1");
            tile[111] = Content.Load<Texture2D>("Tiles/grass&water/grasswater11");
            tile[112] = Content.Load<Texture2D>("Tiles/grass&water/grasswater12");
            tile[113] = Content.Load<Texture2D>("Tiles/grass&water/grasswater13");
            tile[114] = Content.Load<Texture2D>("Tiles/grass&water/grasswater14");
            tile[115] = Content.Load<Texture2D>("Tiles/grass&water/grasswater2");
            tile[116] = Content.Load<Texture2D>("Tiles/grass&water/grasswater21");
            tile[117] = Content.Load<Texture2D>("Tiles/grass&water/grasswater22");
            tile[118] = Content.Load<Texture2D>("Tiles/grass&water/grasswater23");
            tile[119] = Content.Load<Texture2D>("Tiles/grass&water/grasswater24");
            #endregion
            #region Tiles 120-129
            tile[120] = Content.Load<Texture2D>("Tiles/grass&water/grasswater3");
            tile[121] = Content.Load<Texture2D>("Tiles/grass&water/grasswater4");
            tile[122] = Content.Load<Texture2D>("Other/Black");
            tile[123] = Content.Load<Texture2D>("Tiles/house/House Flooring");
            #endregion
            #endregion
            #region special tiles
            map = Content.Load<Texture2D>("Other/map");            
            #endregion
            #region Character Textures
            charactersprites[0] = Content.Load<Texture2D>("Characters/NPCs/Knight");
            charactersprites[0].Name = "Knight";
            charactersprites[1] = Content.Load<Texture2D>("Characters/NPCs/townmember1");
            charactersprites[1].Name = "townmember1";
            charactersprites[2] = Content.Load<Texture2D>("Characters/NPCs/townmember2");
            charactersprites[2].Name = "townmember2";
            #endregion
            #region object textures
            entities[0] = null;
            entities[1] = Content.Load<Texture2D>("Objects/practiceTree");
            entities[2] = Content.Load<Texture2D>("Objects/basic_bush");
            entities[3] = Content.Load<Texture2D>("Objects/basic_bush2");
            entities[4] = Content.Load<Texture2D>("Objects/basic_bush3");
            entities[5] = Content.Load<Texture2D>("Objects/stump2");
            entities[6] = Content.Load<Texture2D>("Objects/wild bush 1");
            entities[7] = Content.Load<Texture2D>("Objects/Large Tree");
            #endregion

            font = Content.Load<SpriteFont>("Fonts/words");
            editor = new Editor(0);
            editor.Load(Content, font);
            base.Initialize();
        }
        protected override void LoadContent()
        {
            theDude = new Dude(new Vector2(36, 4), -1, Content);
            MapName = Load_SaveMapNames.LoadMapNames();
            Maps = new List<Map>();
            foreach (string[] temp in MapName)
            {
                Maps.Add(new Map(editor, theDude, Content, temp[0], tile, entities));
                Maps[Maps.Count - 1].smallmapCount = Convert.ToInt32(temp[1]);
                Maps[Maps.Count - 1].Index = Maps.Count - 1;
            }
            currentMap = Maps[0];
            Load_SaveMapNames.LoadTransportTiles(Maps, editor.GetTransportTile, editor.GetArrow);

        }
        protected override void UnloadContent()
        {
            spriteBatch.Dispose();
        }
        protected override void Update(GameTime gameTime)
        {
            currentMap.setTheDude(theDude);
            currentMap.setTheEditor(editor);
            currentMap.Update();
            currentMap.TransportUpdate(ref currentMap, ref theDude);
            int k;
            keys = Keyboard.GetState();
            GamePadState gamepadstate1 = GamePad.GetState(PlayerIndex.One);
            oldmouse = mouse;
            mouse = Mouse.GetState();
            if (keys.IsKeyDown(Keys.Escape) || gamepadstate1.Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }



            #region editor Controls
            if (keys.IsKeyDown(Keys.F5) && editor.canSwitchEditor == true)
            {//turns editor on/off, changes dude walk speed and screensize
                editor.canSwitchEditor = false;
                if (editor.editorOn == false)
                {
                    editor.editorOn = true;
                    theDude.walkSpeed = 32;
                    graphics.PreferredBackBufferWidth = 1000;
                }
                else
                {
                    editor.editorOn = false;
                    theDude.walkSpeed = 2;
                    graphics.PreferredBackBufferWidth = 800;
                }
                graphics.ApplyChanges();
            }

            if (keys.IsKeyUp(Keys.F5))
                editor.canSwitchEditor = true;
            if (editor.editorOn == true)
            {
                if (keys.IsKeyDown(Keys.F11))//save button
                {
                    currentMap.objectSorter();
                    currentMap.textfiles.Saved(currentMap.SizeX, currentMap.SizeY);
                    Load_SaveMapNames.SaveMapNames(Maps);
                    Load_SaveMapNames.SaveTransportTiles(Maps);
                }
                editor.EditorMouseControls(mouse, GraphicsDevice, Constants.tilesize, theDude.zoom, gameTime, theDude.tilePosition);
                //editor.EditorMouseControls tracks all mouse movement, button pressing in editor mode
                if (editor.DrawingAble == true)
                {
                    if (editor.currentEditorState == Editor.EditorState.Tiles && mouse.LeftButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {//replaces tiles with new designated tile, replaces the value in the tileset and the texture in the tiledata
                        currentMap.textfiles.tileSet[editor.WindowX, editor.WindowY] = (int)editor.brushers;
                        currentMap.textfiles.impassSet[editor.WindowX, editor.WindowY] = editor.impassibleCheck((int)editor.brushers);
                        currentMap.textfiles.grappleSet[editor.WindowX, editor.WindowY] = editor.grappableCheck((int)editor.brushers);
                        for (int x = 0; x < currentMap.SizeX; x++)
                            for (int y = 0; y < currentMap.SizeY; y++)
                                currentMap.tileData[x, y] = new tileClass(x, y, tile[currentMap.textfiles.tileSet[x, y]], currentMap.textfiles.heightSet[x, y], currentMap.textfiles.tileSet[x, y], currentMap.textfiles.impassSet[x, y], currentMap.textfiles.grappleSet[x, y], entities[currentMap.textfiles.entitySet[x, y]], currentMap.textfiles.depthSet[x, y], currentMap.textfiles.coverageSet[x, y]);
                        editor.DrawingAble = false;
                    }
                    if (editor.currentEditorState == Editor.EditorState.entities && mouse.LeftButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {
                        currentMap.textfiles.entitySet[editor.WindowX, editor.WindowY] = (int)editor.brushers;
                        for (int x = 0; x < currentMap.SizeX; x++)
                            for (int y = 0; y < currentMap.SizeY; y++)
                                currentMap.tileData[x, y] = new tileClass(x, y, tile[currentMap.textfiles.tileSet[x, y]], currentMap.textfiles.heightSet[x, y], currentMap.textfiles.tileSet[x, y], currentMap.textfiles.impassSet[x, y], currentMap.textfiles.grappleSet[x, y], entities[currentMap.textfiles.entitySet[x, y]], currentMap.textfiles.depthSet[x, y], currentMap.textfiles.coverageSet[x, y]);
                        editor.DrawingAble = false;

                    }
                    if (editor.currentEditorState == Editor.EditorState.entities && mouse.RightButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {
                        currentMap.textfiles.entitySet[editor.WindowX, editor.WindowY] = 0;
                        for (int x = 0; x < currentMap.SizeX; x++)
                            for (int y = 0; y < currentMap.SizeY; y++)
                                currentMap.tileData[x, y] = new tileClass(x, y, tile[currentMap.textfiles.tileSet[x, y]], currentMap.textfiles.heightSet[x, y], currentMap.textfiles.tileSet[x, y], currentMap.textfiles.impassSet[x, y], currentMap.textfiles.grappleSet[x, y], entities[currentMap.textfiles.entitySet[x, y]], currentMap.textfiles.depthSet[x, y], currentMap.textfiles.coverageSet[x, y]);
                        editor.DrawingAble = false;

                    }
                    if (editor.currentEditorState == Editor.EditorState.Edit && mouse.LeftButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {
                        if (editor.currentEdittingState == Editor.Edit.Height)
                            currentMap.textfiles.heightSet[editor.WindowX, editor.WindowY] = editor.brushers;
                        if (editor.currentEdittingState == Editor.Edit.Impassible)
                            currentMap.textfiles.impassSet[editor.WindowX, editor.WindowY] = (int)editor.brushers;
                        if (editor.currentEdittingState == Editor.Edit.Grappable)
                            currentMap.textfiles.grappleSet[editor.WindowX, editor.WindowY] = (int)editor.brushers;
                        for (int x = 0; x < currentMap.SizeX; x++)
                            for (int y = 0; y < currentMap.SizeY; y++)
                                currentMap.tileData[x, y] = new tileClass(x, y, tile[currentMap.textfiles.tileSet[x, y]], currentMap.textfiles.heightSet[x, y], currentMap.textfiles.tileSet[x, y], currentMap.textfiles.impassSet[x, y], currentMap.textfiles.grappleSet[x, y], entities[currentMap.textfiles.entitySet[x, y]], currentMap.textfiles.depthSet[x, y], currentMap.textfiles.coverageSet[x, y]);
                        editor.DrawingAble = false;
                    }
                    if (editor.currentEditorState == Editor.EditorState.Maps)
                    {
                        editor.mapListLoader(Maps);
                        currentMap = editor.mapClickCheck(currentMap, mouse, GraphicsDevice, oldmouse);
                        if (theDude.tilePosition.X > currentMap.SizeX)
                        {
                            theDude.tilePosition.X = 0;
                            theDude.pixelPosition.X = 0;
                        }
                        if (theDude.tilePosition.Y > currentMap.SizeY)
                        {
                            theDude.tilePosition.Y = 0;
                            theDude.pixelPosition.Y = 0;
                        }
                        if (mouse.LeftButton == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed && mouse.X > GraphicsDevice.Viewport.Width - 195 && mouse.X < GraphicsDevice.Viewport.Width - 115 && mouse.Y > 50 && mouse.Y < 82)
                        {
                            editor.CreateNewMap = true;
                            editor.InputtingSizeX = true;
                        }
                        if (keys.IsKeyDown(Keys.Delete) && !editor.RedoCurrentMap)
                        {
                            editor.RedoCurrentMap = true;
                            editor.InputtingSizeX = true;
                        }
                        if ((editor.CreateNewMap || editor.RedoCurrentMap) && editor.InputtingSizeX && !editor.InputtingSizeY)
                        {
                            if (keys.IsKeyUp(Keys.Delete))
                                editor.InputtingSizeX = editor.FindingSizeX();
                            if (!editor.InputtingSizeX)
                                editor.InputtingSizeY = true;
                        }
                        if ((editor.CreateNewMap || editor.RedoCurrentMap) && !editor.InputtingSizeX && editor.InputtingSizeY)
                        {
                            editor.InputtingSizeY = editor.FindingSizeY();
                            if (!editor.InputtingSizeY)
                            {

                                currentMap = editor.createMap(currentMap, editor, theDude, Content, tile, entities);
                                if (editor.CreateNewMap)
                                {
                                    Maps.Add(currentMap);
                                    Maps[Maps.Count - 1].Index = Maps.Count - 1;
                                    editor.CreateNewMap = false;
                                }
                                if (editor.RedoCurrentMap)
                                {
                                    int i = 0;
                                    foreach (Map map in Maps)
                                    {
                                        if (currentMap.GetMapName == map.GetMapName)
                                        {
                                            currentMap.Index = i;
                                            Maps[map.Index] = currentMap;
                                            break;
                                        }
                                        i++;
                                    }
                                    editor.RedoCurrentMap = false;
                                }
                            }
                        }
                    }
                    if (editor.currentEditorState == Editor.EditorState.Transport)
                    {
                        if (editor.CreatingTransportTile)
                        {
                            if (Editing_MapIndex[2] == 0)
                            {
                                if (keys.IsKeyDown(Keys.Up))
                                    Editing_MapIndex[3] = 0;
                                if (keys.IsKeyDown(Keys.Down))
                                    Editing_MapIndex[3] = 1;
                                if (keys.IsKeyDown(Keys.Right))
                                    Editing_MapIndex[3] = 2;
                                if (keys.IsKeyDown(Keys.Left))
                                    Editing_MapIndex[3] = 3;
                            }
                            if (Editing_MapIndex[2] == 1)
                            {
                                if (keys.IsKeyDown(Keys.Up))
                                    Editing_MapIndex[4] = 0;
                                if (keys.IsKeyDown(Keys.Down))
                                    Editing_MapIndex[4] = 1;
                                if (keys.IsKeyDown(Keys.Right))
                                    Editing_MapIndex[4] = 2;
                                if (keys.IsKeyDown(Keys.Left))
                                    Editing_MapIndex[4] = 3;
                            }
                            if (oldmouse.MiddleButton == ButtonState.Pressed && mouse.MiddleButton == ButtonState.Released)
                            {
                                if (Editing_MapIndex[2] == 0)
                                {
                                    Editing_MapIndex[0] = currentMap.Index;
                                    Editing_TransportPositions[0] = new Vector2(editor.WindowX, editor.WindowY);
                                    Editing_MapIndex[2]++;
                                }
                                else if (Editing_MapIndex[2] == 1)
                                {
                                    Editing_MapIndex[1] = currentMap.Index;
                                    Editing_TransportPositions[1] = new Vector2(editor.WindowX, editor.WindowY);
                                    Maps[Editing_MapIndex[0]].transporttiles.Add(new TransportTile(Editing_TransportPositions[0], Editing_TransportPositions[1],
                                        Maps[Editing_MapIndex[0]], Maps[Editing_MapIndex[1]], Editing_MapIndex[4], editor.GetTransportTile, editor.GetArrow));
                                    Maps[Editing_MapIndex[1]].transporttiles.Add(new TransportTile(Editing_TransportPositions[1], Editing_TransportPositions[0],
                                        Maps[Editing_MapIndex[1]], Maps[Editing_MapIndex[0]], Editing_MapIndex[3], editor.GetTransportTile, editor.GetArrow));
                                    editor.CreatingTransportTile = false;
                                    Editing_MapIndex = new int[] { 0, 0, 0, 0, 0 };
                                }
                            }
                        }
                        if (oldmouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                        {
                            string pairedmapname = "";
                            int i = 0;
                            Vector2 pairedPosition = Vector2.Zero;
                            bool deletingTransports = false;
                            foreach (TransportTile tile in currentMap.transporttiles)
                            {
                                if (tile.GetPosition == new Vector2(editor.WindowX, editor.WindowY) && currentMap.GetMapName == tile.GetMap.GetMapName)
                                {
                                    pairedmapname = tile.GetTransportMap.GetMapName;
                                    pairedPosition = tile.GetTransportPosition;
                                    //currentMap.transporttiles.RemoveAt(i);
                                    Maps[currentMap.Index].transporttiles.RemoveAt(i);
                                    deletingTransports = true;
                                    break;
                                }
                                i++;
                            }
                            if (deletingTransports)
                            {
                                i = 0;
                                foreach (Map map in Maps)
                                {
                                    if (pairedmapname == map.GetMapName)
                                    {
                                        foreach (TransportTile tile in map.transporttiles)
                                        {
                                            if (tile.GetPosition == pairedPosition && pairedmapname == tile.GetMap.GetMapName)
                                            {
                                                deletingTransports = false;
                                                // if(currentMap.Index == map.Index)
                                                //   currentMap.transporttiles.RemoveAt(i);
                                                Maps[map.Index].transporttiles.RemoveAt(i);
                                                break;
                                            }
                                            i++;
                                        }
                                    }
                                    if (!deletingTransports)
                                        break;
                                    i = 0;
                                }
                            }
                        }
                    }



                }
            }
            #endregion
            if (currentMap.mapOn == false)
            {
                if (gamepadstate1.IsConnected)
                    theDude.UserInput(gamepadstate1);
                else theDude.UserInput(keys);
                theDude.Update(ref currentMap, ref editor.editorOn);
            }

            if (keys.IsKeyDown(Keys.B)||(gamepadstate1.IsConnected && gamepadstate1.Buttons.Back == ButtonState.Pressed))
                k = 0;//auto break button


            theDude.CameraCalculation(GraphicsDevice, editor.editorOn);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, theDude.transform);//This drawing segment draws everything that exists on the map, such as the map tiles, characters, monsters, etc.
            currentMap.Draw(spriteBatch, theDude);
            theDude.Draw(spriteBatch);//controls all dude drawing
            editor.DrawMouseTile(spriteBatch, mouse, GraphicsDevice, Constants.tilesize);//draws yellow highlight for editor mode
            spriteBatch.End();

            spriteBatch.Begin();//This drawing segment draws everthing that goes on the game hud, such as the map, character health, etc.
            editor.DrawEditorMode(spriteBatch, tile, GraphicsDevice, Constants.tilesize, font, map, charactersprites, currentMap, entities);//draws all overlays, tiles, text, buttons associated with editor mode
            #region game HUD
            if (editor.editorOn == false)
            {
                spriteBatch.DrawString(font, theDude.Health.ToString(), new Vector2(54, 9), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("Game Hud/HealthBar"), new Rectangle(5, 5, 75, 38), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("Game Hud/heart"), new Rectangle(12 + (18 - (int)(18 * ((float)theDude.Health / (float)theDude.totalHealth))), 8 + (16 - (int)(16 * ((float)theDude.Health / (float)theDude.totalHealth))), (int)(36 * ((float)theDude.Health / (float)theDude.totalHealth)), (int)(32 * ((float)theDude.Health / (float)theDude.totalHealth))), Color.White);
                spriteBatch.DrawString(font, "Slomo Bar", new Vector2(200, 650), Color.Black);
                spriteBatch.DrawString(font, "PixelPosition " + theDude.pixelPosition.ToString(), new Vector2(500, 650), Color.Black);
                spriteBatch.DrawString(font, "TilePosition " + theDude.tilePosition.ToString(), new Vector2(500, 625), Color.Black);

            }
            else
            {
                spriteBatch.DrawString(font, currentMap.GetMapName, new Vector2(5, 5), Color.White);
            }
            #endregion
            currentMap.DrawMiniMap(spriteBatch, GraphicsDevice);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
