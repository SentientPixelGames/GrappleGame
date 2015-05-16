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
    public class mainScript : Microsoft.Xna.Framework.Game
    {
        #region Global Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Dude theDude; //stores all variables, data, movement, camerawork associated with the dude
        Editor editor; //storese all calculations and drawings associated with editor mode
        Texture2D[] charactersprites = new Texture2D[3];
        Tile[] tiles = new Tile[93];
        Object[] objects = new Object[17];
        Texture2D map; //blank image for map
        Texture2D conversationBlock;
        SpriteFont font;
        KeyboardState keys;
        KeyboardState oldkeys;
        GamePadState gamepadstate1;
        GamePadState oldgamepadstate1;
        MouseState mouse = Mouse.GetState();
        Constants Constants = new Constants();
        Map currentMap;
        MapNames Load_SaveMapNames = new MapNames();
        List<Map> Maps;
        List<string[]> MapName;
        int[] Editing_MapIndex = { 0, 0, 0, 0, 0 };
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
            #region tiles
            #region Tiles 0- 9
            tiles[0] = new Tile(0, Content.Load<Texture2D>("Tiles/main/grass"), false, "green");
            tiles[1] = new Tile(1, Content.Load<Texture2D>("Tiles/main/watertile"), true, "blue");
            tiles[2] = new Tile(2, Content.Load<Texture2D>("Tiles/Castle/castlefloortile"), false, "gray");
            tiles[3] = new Tile(3, Content.Load<Texture2D>("Tiles/main/cavefloortile"), false, "brown");
            tiles[4] = new Tile(4, Content.Load<Texture2D>("Tiles/grass&sand/gstile"), false, "yellow");
            tiles[5] = new Tile(5, Content.Load<Texture2D>("Tiles/main/stile"), false, "white");
            tiles[6] = new Tile(6, Content.Load<Texture2D>("Tiles/main/dtile"), false, "yellow");
            tiles[7] = new Tile(7, Content.Load<Texture2D>("Tiles/water&sand/wstiletrans"), true, "blue");
            tiles[8] = new Tile(8, Content.Load<Texture2D>("Tiles/water&sand/wstiletransbot"), true, "blue");
            tiles[9] = new Tile(9, Content.Load<Texture2D>("Tiles/water&sand/wstiletransright"), true, "blue");
            #endregion
            #region Tiles 10-19
            tiles[10] = new Tile(10, Content.Load<Texture2D>("Tiles/water&sand/wstiletranstop"), true, "blue");
            tiles[11] = new Tile(11, Content.Load<Texture2D>("Tiles/grass&sand/gScornertile"), false, "yellow");
            tiles[12] = new Tile(12, Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass"), false, "yellow");
            tiles[13] = new Tile(13, Content.Load<Texture2D>("Tiles/main/blueflowertile"), false, "green");
            tiles[14] = new Tile(14, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1tile"), false, "white");
            tiles[15] = new Tile(15, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner2tile"), false, "white");
            tiles[16] = new Tile(16, Content.Load<Texture2D>("Tiles/main/forestpathtile"), false, "white");
            tiles[17] = new Tile(17, Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile"), false, "white");
            tiles[18] = new Tile(18, Content.Load<Texture2D>("Tiles/main/purpleflowertile"), false, "green");
            tiles[19] = new Tile(19, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner2-1tile"), false, "white");
            #endregion
            #region Tiles 20-29
            tiles[20] = new Tile(20, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner2-2tile"), false, "white");
            tiles[21] = new Tile(21, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner2-3tile"), false, "white");
            tiles[22] = new Tile(22, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1-1tile"), false, "white");
            tiles[23] = new Tile(23, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1-2tile"), false, "white");
            tiles[24] = new Tile(24, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1-3tile"), false, "white");
            tiles[25] = new Tile(25, Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-1"), false, "yellow");
            tiles[26] = new Tile(26, Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-2"), false, "yellow");
            tiles[27] = new Tile(27, Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-3"), false, "yellow");
            tiles[28] = new Tile(28, Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile-1"), false, "white");
            tiles[29] = new Tile(29, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1-3tile"), false, "white");
            #endregion
            #region Tiles 30-39
            tiles[30] = new Tile(30, Content.Load<Texture2D>("Tiles/path&grass/forestpathcorner1-3tile"), false, "white");
            tiles[31] = new Tile(31, Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-1"), false, "yellow");
            tiles[32] = new Tile(32, Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-2"), false, "yellow");
            tiles[33] = new Tile(33, Content.Load<Texture2D>("Tiles/grass&sand/GscornertileGrass-3"), false, "yellow");
            tiles[34] = new Tile(34, Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile-1"), false, "white");
            tiles[35] = new Tile(35, Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile-2"), false, "white");
            tiles[36] = new Tile(36, Content.Load<Texture2D>("Tiles/path&grass/grassforestpathtile-3"), false, "white");
            tiles[37] = new Tile(37, Content.Load<Texture2D>("Tiles/grass&sand/gstile-1"), false, "yellow");
            tiles[38] = new Tile(38, Content.Load<Texture2D>("Tiles/grass&sand/gstile-2"), false, "yellow");
            tiles[39] = new Tile(39, Content.Load<Texture2D>("Tiles/grass&sand/gstile-3"), false, "yellow");
            #endregion
            #region Tiles 40-49
            tiles[40] = new Tile(40, Content.Load<Texture2D>("Tiles/grass&sand/gScornertile-1"), false, "yellow");
            tiles[41] = new Tile(41, Content.Load<Texture2D>("Tiles/grass&sand/gScornertile-2"), false, "yellow");
            tiles[42] = new Tile(42, Content.Load<Texture2D>("Tiles/grass&sand/gScornertile-3"), false, "yellow");
            tiles[43] = new Tile(43, Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner1"), true, "blue");
            tiles[44] = new Tile(44, Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner11"), true, "blue");
            tiles[45] = new Tile(45, Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner12"), true, "blue");
            tiles[46] = new Tile(46, Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner13"), true, "blue");
            tiles[47] = new Tile(47, Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner2"), true, "blue");
            tiles[48] = new Tile(48, Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner21"), true, "blue");
            tiles[49] = new Tile(49, Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner22"), true, "blue");
            #endregion
            #region Tiles 50-59
            tiles[50] = new Tile(50, Content.Load<Texture2D>("Tiles/water&sand/wstiletranscorner23"), true, "blue");
            tiles[51] = new Tile(51, Content.Load<Texture2D>("Tiles/mountain/mountain"), false, "brown");
            tiles[52] = new Tile(52, Content.Load<Texture2D>("Tiles/mountain/mountain3"), true, "brown");
            tiles[53] = new Tile(53, Content.Load<Texture2D>("Tiles/mountain/mountain2"), false, "brown");
            tiles[54] = new Tile(54, Content.Load<Texture2D>("Tiles/mountain/mountainopening"), false, "brown");
            tiles[55] = new Tile(55, Content.Load<Texture2D>("Tiles/Dock/dockhoriz"), false, "white");
            tiles[56] = new Tile(56, Content.Load<Texture2D>("Tiles/Dock/dockpost"), true, "white");
            tiles[57] = new Tile(57, Content.Load<Texture2D>("Tiles/Dock/dockvert"), false, "white");
            tiles[58] = new Tile(58, Content.Load<Texture2D>("Tiles/mountain/mountain4"), true, "brown");
            tiles[59] = new Tile(59, Content.Load<Texture2D>("Tiles/mountain/mountain5"), false, "brown");
            #endregion
            #region Tiles 60-69
            tiles[60] = new Tile(60, Content.Load<Texture2D>("Tiles/mountain/mountain7"), false, "brown");
            tiles[61] = new Tile(61, Content.Load<Texture2D>("Tiles/mountain/mountain8"), false, "brown");
            tiles[62] = new Tile(62, Content.Load<Texture2D>("Tiles/mountain/mountain9"), false, "brown");
            tiles[63] = new Tile(63, Content.Load<Texture2D>("Tiles/mountain/mountain11"), true, "brown");
            tiles[64] = new Tile(64, Content.Load<Texture2D>("Tiles/mountain/mountain12"), false, "brown");
            tiles[65] = new Tile(65, Content.Load<Texture2D>("Tiles/mountain/mountain17"), false, "brown");
            tiles[66] = new Tile(66, Content.Load<Texture2D>("Tiles/mountain/mountain18"), false, "brown");
            tiles[67] = new Tile(67, Content.Load<Texture2D>("Tiles/mountain/mountain19"), false, "brown");
            tiles[68] = new Tile(68, Content.Load<Texture2D>("Tiles/grass&edge/gtileedge1"), false, "green");
            tiles[69] = new Tile(69, Content.Load<Texture2D>("Tiles/grass&edge/gtileedge2"), false, "green");
            #endregion
            #region Tiles 70-79

            tiles[70] = new Tile(70, Content.Load<Texture2D>("Tiles/grass&edge/gtileedge3"), false, "green");
            tiles[71] = new Tile(71, Content.Load<Texture2D>("Tiles/grass&edge/gtileedge4"), false, "green");
            tiles[72] = new Tile(72, Content.Load<Texture2D>("Tiles/grass&edge/gtileedge5"), false, "green");
            tiles[73] = new Tile(73, Content.Load<Texture2D>("Tiles/grass&edge/gtileedge6"), false, "green");
            tiles[74] = new Tile(74, Content.Load<Texture2D>("Tiles/grass&edge/gtileedge7"), false, "green");
            tiles[75] = new Tile(75, Content.Load<Texture2D>("Tiles/grass&edge/gtileedge8"), false, "green");
            tiles[76] = new Tile(76, Content.Load<Texture2D>("Tiles/stairs/forestpathstairs1"), false, "white");
            tiles[77] = new Tile(77, Content.Load<Texture2D>("Tiles/Castle/castlewindow1"), true, "gray");
            tiles[78] = new Tile(78, Content.Load<Texture2D>("Tiles/Castle/castlewindow2"), true, "gray");
            tiles[79] = new Tile(79, Content.Load<Texture2D>("Tiles/grass&water/grasswater1"), true, "blue");
            #endregion
            #region Tiles 80-89
            tiles[80] = new Tile(80, Content.Load<Texture2D>("Tiles/grass&water/grasswater11"), true, "blue");
            tiles[81] = new Tile(81, Content.Load<Texture2D>("Tiles/grass&water/grasswater12"), true, "blue");
            tiles[82] = new Tile(82, Content.Load<Texture2D>("Tiles/grass&water/grasswater13"), true, "blue");
            tiles[83] = new Tile(83, Content.Load<Texture2D>("Tiles/grass&water/grasswater14"), true, "blue");
            tiles[84] = new Tile(84, Content.Load<Texture2D>("Tiles/grass&water/grasswater2"), true, "blue");
            tiles[85] = new Tile(85, Content.Load<Texture2D>("Tiles/grass&water/grasswater21"), true, "blue");
            tiles[86] = new Tile(86, Content.Load<Texture2D>("Tiles/grass&water/grasswater22"), true, "blue");
            tiles[87] = new Tile(87, Content.Load<Texture2D>("Tiles/grass&water/grasswater23"), true, "blue");
            tiles[88] = new Tile(88, Content.Load<Texture2D>("Tiles/grass&water/grasswater24"), true, "blue");
            tiles[89] = new Tile(89, Content.Load<Texture2D>("Tiles/grass&water/grasswater3"), true, "blue");
            #endregion
            #region Tiles 90-99

            tiles[90] = new Tile(90, Content.Load<Texture2D>("Tiles/grass&water/grasswater4"), true, "blue");
            tiles[91] = new Tile(91, Content.Load<Texture2D>("Other/Black"), true, "black");
            tiles[92] = new Tile(92, Content.Load<Texture2D>("Tiles/house/House Flooring"), false, "orange");
            #endregion
            #endregion
            #region special tiles
            map = Content.Load<Texture2D>("Other/map");
            #endregion
            #region Character Textures
            charactersprites[0] = Content.Load<Texture2D>("Characters/NPCs/Knight");
            charactersprites[0].Name = "Characters/NPCs/Knight";
            charactersprites[1] = Content.Load<Texture2D>("Characters/NPCs/townmember1");
            charactersprites[1].Name = "Characters/NPCs/townmember1";
            charactersprites[2] = Content.Load<Texture2D>("Characters/NPCs/townmember2");
            charactersprites[2].Name = "Characters/NPCs/townmember2";
            #endregion
            #region objects
            objects[0] = null;
            objects[1] = new Object(1, Content.Load<Texture2D>("Objects/practiceTree_top"), Content.Load<Texture2D>("Objects/practiceTree_bottom"),
                new float[,] { { 2, 2, 2 }, { 2, 2, 2 }, { 0, 1, 0 }, { 0, 0, 0 } },
                new float[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } },
                new float[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 0 } });
            objects[2] = new Object(2, Content.Load<Texture2D>("Objects/basic_bush"), null,
                new float[,] { { 1, 1 }, { 1, 1 } },
                new float[,] { { 1, 1 }, { 1, 1 } },
                new float[,] { { 0, 0 }, { 0, 0 } });
            objects[3] = new Object(3, Content.Load<Texture2D>("Objects/basic_bush2"), null,
                new float[,] { { 1, 1 }, { 1, 1 } },
                new float[,] { { 1, 1 }, { 1, 1 } },
                new float[,] { { 0, 0 }, { 0, 0 } });
            objects[4] = new Object(4, Content.Load<Texture2D>("Objects/basic_bush3"), null,
                new float[,] { { 1, 1 }, { 1, 1 } },
                new float[,] { { 1, 1 }, { 1, 1 } },
                new float[,] { { 0, 0 }, { 0, 0 } });
            objects[5] = new Object(5, Content.Load<Texture2D>("Objects/stump2"), null,
                new float[,] { { 1 } },
                new float[,] { { 1 } },
                new float[,] { { 0 } });
            objects[6] = new Object(6, Content.Load<Texture2D>("Objects/wild bush 1"), null,
                new float[,] { { 1, 1 }, { 0, 0 } },
                new float[,] { { 1, 1 }, { 0, 0 } },
                new float[,] { { 0, 0 }, { 0, 0 } });
            objects[7] = new Object(7, Content.Load<Texture2D>("Objects/Shadow1"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[8] = new Object(8, Content.Load<Texture2D>("Objects/Shadow2"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[9] = new Object(9, Content.Load<Texture2D>("Objects/Shadow3"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[10] = new Object(10, Content.Load<Texture2D>("Objects/Shadow4"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[11] = new Object(11, Content.Load<Texture2D>("Objects/Shadow5"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[12] = new Object(12, Content.Load<Texture2D>("Objects/Shadow6"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[13] = new Object(13, Content.Load<Texture2D>("Objects/Shadow7"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[14] = new Object(14, Content.Load<Texture2D>("Objects/Shadow8"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[15] = new Object(15, Content.Load<Texture2D>("Objects/Shadow9"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });
            objects[16] = new Object(16, Content.Load<Texture2D>("Objects/Shadow10"), null,
                new float[,] { { 0 } },
                new float[,] { { 0 } },
                new float[,] { { 1 } });

            #endregion
            #region HUD
            conversationBlock = Content.Load<Texture2D>("Game HUD/TextBox");
            #endregion
            font = Content.Load<SpriteFont>("Fonts/words");
            editor = new Editor(0);
            editor.Load(Content, font);
            base.Initialize();
        }
        protected override void LoadContent()
        {
            theDude = new Dude(new Vector2(0, 0), -1, Content);
            MapName = Load_SaveMapNames.LoadMapNames();
            Maps = new List<Map>();
            foreach (string[] temp in MapName)
            {
                Maps.Add(new Map(editor, theDude, Content, temp[0], tiles, objects));
                Maps[Maps.Count - 1].smallmapCount = Convert.ToInt32(temp[1]);
                Maps[Maps.Count - 1].Index = Maps.Count - 1;
            }
            currentMap = Maps[0];
            theDude.updateCharacterEventHandler(ref currentMap);//set the first maps characterevent handler to the dude
            Load_SaveMapNames.LoadTransportTiles(Maps, editor.GetTransportTile, editor.GetArrow);
            Load_SaveMapNames.LoadCharacters(Maps, Content);

        }
        protected override void UnloadContent()
        {
            spriteBatch.Dispose();
        }
        protected override void Update(GameTime gameTime)
        {
            oldmouse = mouse;
            mouse = Mouse.GetState();
            currentMap.setTheDude(theDude);
            currentMap.setTheEditor(editor);
            currentMap.Update(mouse, oldmouse);
            currentMap.TransportUpdate(ref currentMap, ref theDude);
            int k;
            oldkeys = keys;
            keys = Keyboard.GetState();
            oldgamepadstate1 = gamepadstate1;
            gamepadstate1 = GamePad.GetState(PlayerIndex.One);


            //REMOVE IN FINAL
            //This checks to see if we are currently editing a character, if we are then we don't want anyother editor or keyboard checks
            //happening so they should all be skipped
            if (currentMap.GetCharacterEditingState != -1)
            {
                goto CharacterJump;
            }


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
                    currentMap.objectSorter(objects);
                    currentMap.textfiles.Saved(currentMap.SizeX, currentMap.SizeY);
                    Load_SaveMapNames.SaveMapNames(Maps);
                    Load_SaveMapNames.SaveTransportTiles(Maps);
                    Load_SaveMapNames.SaveCharacters(Maps);
                }
                editor.EditorMouseControls(mouse, GraphicsDevice, Constants.tilesize, theDude.zoom, gameTime, theDude.tilePosition);
                //editor.EditorMouseControls tracks all mouse movement, button pressing in editor mode
                if (editor.DrawingAble == true)
                {
                    if (editor.currentEditorState == Editor.EditorState.Tiles && mouse.LeftButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {//replaces tiles with new designated tile, replaces the value in the tileset and the texture in the tiledata
                        currentMap.textfiles.tileSet[editor.WindowX, editor.WindowY] = (int)editor.brushers;
                        currentMap.textfiles.impassSet[editor.WindowX, editor.WindowY] = tiles[(int)editor.brushers].impassible == true ? 1 : 0;
                        for (int x = 0; x < currentMap.SizeX; x++)
                        {
                            for (int y = 0; y < currentMap.SizeY; y++)
                            {
                                currentMap.tileData[x, y] = new tileClass(new Vector2(x, y), tiles[currentMap.textfiles.tileSet[x, y]], objects[currentMap.textfiles.objectSet[x, y]]);
                                currentMap.tileData[x, y].loadTileData(1f, currentMap.textfiles.impassSet[x, y], currentMap.textfiles.heightSet[x, y]);
                                currentMap.tileData[x, y].loadObjectData(currentMap.textfiles.depthSet[x, y], currentMap.textfiles.depthSet[x, y] + 0.3f, currentMap.textfiles.objHeightSet[x, y], currentMap.textfiles.solidSet[x, y], currentMap.textfiles.shadowSet[x, y]);
                            }
                        }
                        editor.DrawingAble = false;
                    }
                    if (editor.currentEditorState == Editor.EditorState.Objects && mouse.LeftButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {
                        currentMap.textfiles.objectSet[editor.WindowX, editor.WindowY] = (int)editor.brushers;
                        for (int x = 0; x < currentMap.SizeX; x++)
                        {
                            for (int y = 0; y < currentMap.SizeY; y++)
                            {
                                currentMap.tileData[x, y] = new tileClass(new Vector2(x, y), tiles[currentMap.textfiles.tileSet[x, y]], objects[currentMap.textfiles.objectSet[x, y]]);
                                currentMap.tileData[x, y].loadTileData(1f, currentMap.textfiles.impassSet[x, y], currentMap.textfiles.heightSet[x, y]);
                                currentMap.tileData[x, y].loadObjectData(currentMap.textfiles.depthSet[x, y], currentMap.textfiles.depthSet[x, y] + 0.3f, currentMap.textfiles.objHeightSet[x, y], currentMap.textfiles.solidSet[x, y], currentMap.textfiles.shadowSet[x, y]);
                            }
                        }
                        editor.DrawingAble = false;

                    }
                    if (editor.currentEditorState == Editor.EditorState.Objects && mouse.RightButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {
                        currentMap.textfiles.objectSet[editor.WindowX, editor.WindowY] = 0;
                        for (int x = 0; x < currentMap.SizeX; x++)
                        {
                            for (int y = 0; y < currentMap.SizeY; y++)
                            {
                                currentMap.tileData[x, y] = new tileClass(new Vector2(x, y), tiles[currentMap.textfiles.tileSet[x, y]], objects[currentMap.textfiles.objectSet[x, y]]);
                                currentMap.tileData[x, y].loadTileData(1f, currentMap.textfiles.impassSet[x, y], currentMap.textfiles.heightSet[x, y]);
                                currentMap.tileData[x, y].loadObjectData(currentMap.textfiles.depthSet[x, y], currentMap.textfiles.depthSet[x, y] + 0.3f, currentMap.textfiles.objHeightSet[x, y], currentMap.textfiles.solidSet[x, y], currentMap.textfiles.shadowSet[x, y]);
                            }
                        }
                        editor.DrawingAble = false;

                    }
                    if (editor.currentEditorState == Editor.EditorState.Edit && mouse.LeftButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {
                        if (editor.currentEdittingState == Editor.Edit.Height)
                            currentMap.textfiles.heightSet[editor.WindowX, editor.WindowY] = editor.brushers;
                        if (editor.currentEdittingState == Editor.Edit.Impassible)
                            currentMap.textfiles.impassSet[editor.WindowX, editor.WindowY] = (int)editor.brushers;
                        for (int x = 0; x < currentMap.SizeX; x++)
                        {
                            for (int y = 0; y < currentMap.SizeY; y++)
                            {
                                currentMap.tileData[x, y] = new tileClass(new Vector2(x, y), tiles[currentMap.textfiles.tileSet[x, y]], objects[currentMap.textfiles.objectSet[x, y]]);
                                currentMap.tileData[x, y].loadTileData(1f, currentMap.textfiles.impassSet[x, y], currentMap.textfiles.heightSet[x, y]);
                                currentMap.tileData[x, y].loadObjectData(currentMap.textfiles.depthSet[x, y], currentMap.textfiles.depthSet[x, y] + 0.3f, currentMap.textfiles.objHeightSet[x, y], currentMap.textfiles.solidSet[x, y], currentMap.textfiles.shadowSet[x, y]);
                            }
                        }
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

                                currentMap = editor.createMap(currentMap, editor, theDude, Content, tiles, objects);
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
                    if (editor.currentEditorState == Editor.EditorState.Characters && mouse.LeftButton == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {//Character was selected to be placed
                        currentMap.CreateNewCharacter(Content, charactersprites[(int)editor.brushers]);
                    }
                    if (editor.currentEditorState == Editor.EditorState.Characters && mouse.RightButton == ButtonState.Pressed && mouse.X < GraphicsDevice.Viewport.Width - 200 && mouse.Y < GraphicsDevice.Viewport.Height && editor.WindowX >= 0 && editor.WindowY >= 0 && editor.WindowX < currentMap.SizeX && editor.WindowY < currentMap.SizeY)
                    {//right click occurred 
                        if (currentMap.tileData[editor.WindowX, editor.WindowY].tileData.characterOnTile != -1)
                        {//character on tile that was right clicked so enable character for editing
                            currentMap.EditCharacter();
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
                    theDude.UserInput(gamepadstate1, oldgamepadstate1);
                else theDude.UserInput(keys, oldkeys);
                theDude.Update(ref currentMap, ref editor.editorOn);
            }

            #region Text Input
            CharacterJump:
            #endregion
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
            editor.DrawEditorMode(spriteBatch, tiles, GraphicsDevice, Constants.tilesize, font, map, charactersprites, currentMap, objects);//draws all overlays, tiles, text, buttons associated with editor mode
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
