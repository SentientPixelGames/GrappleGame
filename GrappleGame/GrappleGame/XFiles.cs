using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GrappleGame
{
    class UserInput
    {
        public string text = "";
        KeyboardState currentkeyboard;
        KeyboardState oldkeyboard;
        /// <summary>
        /// Used to Input Strings From User Keyboard
        /// </summary>
        /// <returns>
        /// True: String has finished being entered.
        /// False: String has not been completed
        /// </returns>
        public bool userinput()
        {
            oldkeyboard = currentkeyboard;
            currentkeyboard = Keyboard.GetState();
            bool Done = false;
            Keys[] pressedkeys;

            pressedkeys = currentkeyboard.GetPressedKeys();
            //One of the Shift buttons are down so give upper case options
            if (currentkeyboard.IsKeyDown(Keys.LeftShift) || currentkeyboard.IsKeyDown(Keys.RightShift))
            {
                foreach (Keys keys in pressedkeys)
                {
                    if (oldkeyboard.IsKeyUp(keys))
                    {
                        if (keys == Keys.Back && text.Length > 0)
                        {
                            text = text.Remove(text.Length - 1, 1);
                        }
                        else if (keys == Keys.LeftShift || keys == Keys.RightShift)
                            continue;
                        //character ":" is banned due to save and loading characters this character is used to cue splits
                        else if (keys == Keys.Space)
                            text = text.Insert(text.Length, " ");
                        else if (keys == Keys.Enter)
                            text = text.Insert(text.Length, "\n\n");
                        else if (keys == Keys.OemComma)
                            text = text.Insert(text.Length, "<");
                        else if (keys == Keys.OemPeriod)
                            text = text.Insert(text.Length, ">");
                        else if (keys == Keys.OemQuestion)
                            text = text.Insert(text.Length, "?");
                        else if (keys == Keys.D1)
                            text = text.Insert(text.Length, "!");
                        else if (keys == Keys.OemTilde)
                            text = text.Insert(text.Length, "\"");
                        else if (keys == Keys.OemOpenBrackets)
                            text = text.Insert(text.Length, "{");
                        else if (keys == Keys.OemCloseBrackets)
                            text = text.Insert(text.Length, "}");
                        else if (keys == Keys.OemMinus)
                            text = text.Insert(text.Length, "_");
                        else if (keys == Keys.OemPlus)
                            text = text.Insert(text.Length, "+");
                        else if (keys == Keys.D2)
                            text = text.Insert(text.Length, "@");
                        else if (keys == Keys.D3)
                            text = text.Insert(text.Length, "#");
                        else if (keys == Keys.D4)
                            text = text.Insert(text.Length, "$");
                        else if (keys == Keys.D5)
                            text = text.Insert(text.Length, "%");
                        else if (keys == Keys.D6)
                            text = text.Insert(text.Length, "^");
                        else if (keys == Keys.D7)
                            text = text.Insert(text.Length, "&");
                        else if (keys == Keys.D8)
                            text = text.Insert(text.Length, "*");
                        else if (keys == Keys.D9)
                            text = text.Insert(text.Length, "(");
                        else if (keys == Keys.D0)
                            text = text.Insert(text.Length, ")");
                        else if (keys == Keys.Escape)
                            Done = true;
                        else if (keys == Keys.Tab)
                        {
                            text = text.Insert(text.Length, "\t");
                            Done = true;
                        }
                        else text += keys.ToString();
                    }
                }
            }
            //Shift buttons are not down so all things are lower case option
            else
            {
                foreach (Keys keys in pressedkeys)
                {
                    if (oldkeyboard.IsKeyUp(keys))
                    {
                        if (keys == Keys.Back && text.Length > 0)
                        {
                            text = text.Remove(text.Length - 1, 1);
                        }
                        //character ":" is banned due to save and loading characters this character is used to cue splits
                        else if (keys == Keys.Space)
                            text = text.Insert(text.Length, " ");
                        else if (keys == Keys.Enter)
                            text = text.Insert(text.Length, "\n\n");
                        else if (keys == Keys.OemComma)
                            text = text.Insert(text.Length, ",");
                        else if (keys == Keys.OemPeriod)
                            text = text.Insert(text.Length, ".");
                        else if (keys == Keys.OemQuestion)
                            text = text.Insert(text.Length, "/");
                        else if (keys == Keys.OemTilde)
                            text = text.Insert(text.Length, "'");
                        else if (keys == Keys.OemOpenBrackets)
                            text = text.Insert(text.Length, "[");
                        else if (keys == Keys.OemCloseBrackets)
                            text = text.Insert(text.Length, "]");
                        else if (keys == Keys.OemMinus)
                            text = text.Insert(text.Length, "-");
                        else if (keys == Keys.OemPlus)
                            text = text.Insert(text.Length, "=");
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
                        else if (keys == Keys.Escape)
                            Done = true;
                        else if (keys == Keys.Tab)
                        {
                            text = text.Insert(text.Length, "\t");
                            Done = true;
                        }
                        else
                        {
                            #region LowerCase
                            switch (keys)
                            {
                                case Keys.A:
                                    text = text.Insert(text.Length, "a");
                                    break;
                                case Keys.B:
                                    text = text.Insert(text.Length, "b");
                                    break;
                                case Keys.C:
                                    text = text.Insert(text.Length, "c");
                                    break;
                                case Keys.D:
                                    text = text.Insert(text.Length, "d");
                                    break;
                                case Keys.E:
                                    text = text.Insert(text.Length, "e");
                                    break;
                                case Keys.F:
                                    text = text.Insert(text.Length, "f");
                                    break;
                                case Keys.G:
                                    text = text.Insert(text.Length, "g");
                                    break;
                                case Keys.H:
                                    text = text.Insert(text.Length, "h");
                                    break;
                                case Keys.I:
                                    text = text.Insert(text.Length, "i");
                                    break;
                                case Keys.J:
                                    text = text.Insert(text.Length, "j");
                                    break;
                                case Keys.K:
                                    text = text.Insert(text.Length, "k");
                                    break;
                                case Keys.L:
                                    text = text.Insert(text.Length, "l");
                                    break;
                                case Keys.M:
                                    text = text.Insert(text.Length, "m");
                                    break;
                                case Keys.N:
                                    text = text.Insert(text.Length, "n");
                                    break;
                                case Keys.O:
                                    text = text.Insert(text.Length, "o");
                                    break;
                                case Keys.P:
                                    text = text.Insert(text.Length, "p");
                                    break;
                                case Keys.Q:
                                    text = text.Insert(text.Length, "q");
                                    break;
                                case Keys.R:
                                    text = text.Insert(text.Length, "r");
                                    break;
                                case Keys.S:
                                    text = text.Insert(text.Length, "s");
                                    break;
                                case Keys.T:
                                    text = text.Insert(text.Length, "t");
                                    break;
                                case Keys.U:
                                    text = text.Insert(text.Length, "u");
                                    break;
                                case Keys.V:
                                    text = text.Insert(text.Length, "v");
                                    break;
                                case Keys.W:
                                    text = text.Insert(text.Length, "w");
                                    break;
                                case Keys.X:
                                    text = text.Insert(text.Length, "x");
                                    break;
                                case Keys.Y:
                                    text = text.Insert(text.Length, "y");
                                    break;
                                case Keys.Z:
                                    text = text.Insert(text.Length, "z");
                                    break;
                            }
                            #endregion
                        }
                    }
                }
            }
            return Done;
        }
        public bool NumericalInput()
        {
            oldkeyboard = currentkeyboard;
            currentkeyboard = Keyboard.GetState();
            bool Done = false;
            Keys[] pressedkeys;

            pressedkeys = currentkeyboard.GetPressedKeys();
            foreach (Keys keys in pressedkeys)
            {
                if (oldkeyboard.IsKeyUp(keys))
                {
                    if (pressedkeys[0] == Keys.D0)
                        text = text.Insert(text.Length, "0");
                    else if (pressedkeys[0] == Keys.D1)
                        text = text.Insert(text.Length, "1");
                    else if (pressedkeys[0] == Keys.D2)
                        text = text.Insert(text.Length, "2");
                    else if (pressedkeys[0] == Keys.D3)
                        text = text.Insert(text.Length, "3");
                    else if (pressedkeys[0] == Keys.D4)
                        text = text.Insert(text.Length, "4");
                    else if (pressedkeys[0] == Keys.D5)
                        text = text.Insert(text.Length, "5");
                    else if (pressedkeys[0] == Keys.D6)
                        text = text.Insert(text.Length, "6");
                    else if (pressedkeys[0] == Keys.D7)
                        text = text.Insert(text.Length, "7");
                    else if (pressedkeys[0] == Keys.D8)
                        text = text.Insert(text.Length, "8");
                    else if (pressedkeys[0] == Keys.D9)
                        text = text.Insert(text.Length, "9");
                    else if (pressedkeys[0] == Keys.OemComma)
                        text = text.Insert(text.Length, ",");
                    else if (pressedkeys[0] == Keys.Enter)
                        Done = true;
                }
            }
            return Done;
        }
    }
    class XFiles : Game
    {
        public void writetoxml(string filename, List<Character> characterlist)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Character>));
            TextWriter writer = new StreamWriter(/*"XMLTest.xml"*/filename);
            xs.Serialize(writer, characterlist);
            writer.Close();

        }
        public List<Character> readfromxml(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Character>));
            FileStream myfilestream = new FileStream(/*"XMLTest.xml"*/filename, FileMode.Open);
            List<Character> characterlist;
            characterlist = (List<Character>)xs.Deserialize(myfilestream);
            myfilestream.Close();
            return characterlist;
        }

    }

    class MapNames
    {
        XFiles xsd = new XFiles();
        public void SaveMapNames(List<Map> Maps)
        {
            using (StreamWriter saveFile = new StreamWriter("MapNames.txt"))
            {
                foreach (Map currentmap in Maps)
                {
                    saveFile.Write(currentmap.GetMapName.Remove(currentmap.GetMapName.Length - 4, 4));
                    saveFile.Write(":" + currentmap.smallmapCount.ToString());
                    saveFile.Write("\r\n");
                    //xsd.writetoxml("Character_" + currentmap.GetMapName + ".xml", currentmap.characterList);
                }
                saveFile.Close();

            }
        }
        public List<string[]> LoadMapNames()
        {
            List<string[]> Maps = new List<string[]>();
            using (StreamReader reader = new StreamReader("MapNames.txt"))
            {

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] splitstring = line.Split(':');
                    string[] temporary = new string[2];
                    temporary[0] = splitstring[0] + ".txt";
                    temporary[1] = splitstring[1];
                    Maps.Add(temporary);
                }
                reader.Close();
            }

            return Maps;
        }
        public void SaveTransportTiles(List<Map> Maps)
        {
            foreach (Map currentmap in Maps)
            {
                string filename = currentmap.GetMapName.Remove(currentmap.GetMapName.Length - 4, 4) + "T.txt";
                using (StreamWriter saveFile = new StreamWriter(filename))
                {
                    foreach (TransportTile tile in currentmap.transporttiles)
                    {
                        saveFile.Write(tile.GetMap.GetMapName);
                        saveFile.Write(":" + tile.GetPosition.X.ToString());
                        saveFile.Write(":" + tile.GetPosition.Y.ToString());
                        saveFile.Write(":" + tile.GetTransportMap.GetMapName);
                        saveFile.Write(":" + tile.GetTransportPosition.X.ToString());
                        saveFile.Write(":" + tile.GetTransportPosition.Y.ToString());
                        saveFile.Write(":" + tile.GetWalkDirectionOut.ToString());
                        saveFile.Write("\r\n");
                    }
                    saveFile.Close();
                }
            }
        }
        public void LoadTransportTiles(List<Map> Maps, Texture2D texture, Texture2D arrow)
        {
            foreach (Map currentmap in Maps)
            {
                List<string[]> TransportInfo = new List<string[]>();
                string filename = currentmap.GetMapName.Remove(currentmap.GetMapName.Length - 4, 4) + "T.txt";
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] splitstring = line.Split(':');
                        foreach (Map mapnamesearch in Maps)
                        {
                            if (splitstring[3].Equals(mapnamesearch.GetMapName))
                            {
                                currentmap.transporttiles.Add(new TransportTile(new Vector2(Convert.ToInt32(splitstring[1]),
                                Convert.ToInt32(splitstring[2])), new Vector2(Convert.ToInt32(splitstring[4]), Convert.ToInt32(splitstring[5])),
                                currentmap, mapnamesearch, Convert.ToInt32(splitstring[6]), texture, arrow));
                                break;
                            }
                        }

                    }
                    reader.Close();
                }

            }
        }


        public void SaveCharacters(List<Map> Maps)
        {
            foreach (Map currentMap in Maps)
            {
                string filename = currentMap.GetMapName.Remove(currentMap.GetMapName.Length - 4, 4) + "_C.txt";
                using (StreamWriter saveFile = new StreamWriter(filename))
                {
                    foreach (Character character in currentMap.onMapCharacters)
                    {
                        character.SetSaveValues();
                        saveFile.Write("<Character>\r\n");
                        saveFile.Write("<Texture>\r\n");
                        saveFile.Write(character.saveData.TextureName);
                        saveFile.Write("\r\n<\\Texture>\r\n");
                        saveFile.Write("<Position>\r\n");
                        saveFile.Write(character.saveData.HomePosition.X.ToString());
                        saveFile.Write("," + character.saveData.HomePosition.Y.ToString());
                        saveFile.Write("\r\n<\\Position>\r\n");
                        saveFile.Write("<Range>\r\n");
                        saveFile.Write(character.saveData.Range.X.ToString());
                        saveFile.Write("," + character.saveData.Range.Y.ToString());
                        saveFile.Write("\r\n<\\Range>\r\n");
                        saveFile.Write("<ID>\r\n");
                        saveFile.Write(character.saveData.ID.ToString());
                        saveFile.Write("\r\n<\\ID>\r\n");
                        saveFile.Write("<Height>\r\n");
                        saveFile.Write(character.saveData.Height.ToString());
                        saveFile.Write("\r\n<\\Height>\r\n");
                        saveFile.Write("<Conversation>\r\n");
                        foreach (String conversation in character.saveData.Conversation)
                        {
                            saveFile.Write("<Sub Conversation>\r\n");
                            saveFile.Write(conversation);
                            saveFile.Write("\r\n<\\Sub Conversation>\r\n");
                        }
                        saveFile.Write("<\\Conversation>\r\n");
                        saveFile.Write("<\\Character>\r\n");
                    }
                    saveFile.Close();
                }
            }
        }

        public void LoadCharacters(List<Map> Maps, ContentManager Content)
        {
            foreach (Map currentmap in Maps)
            {
                string filename = currentmap.GetMapName.Remove(currentmap.GetMapName.Length - 4, 4) + "_C.txt";
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    List<string> convo = new List<string>();
                    Vector2 Position = new Vector2(0,0);
                    Point Range = new Point(0,0);
                    int ID = -1;
                    float height = 0.0f;
                    string texture = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if(line.Contains("<\\Character>"))
                        {
                            currentmap.onMapCharacters.Add(new Character(Content, Position, 
                                height, ID, Content.Load<Texture2D>(texture), currentmap.characterEventHandler));
                            currentmap.onMapCharacters[ID].SetLoadValues(convo, Range);
                            currentmap.tileData[(int)Position.X, (int)Position.Y].tileData.characterOnTile = ID;
                            Position = new Vector2(0,0);
                            Range = new Point(0,0);
                            ID = -1;
                            height = 0.0f;
                            convo = new List<string>();
                            texture = null;
                        }
                        else if(line.Contains("<Texture>"))
                        {
                            line = reader.ReadLine();
                            texture = line;
                        }
                        else if(line.Contains("<Position>"))
                        {
                            line = reader.ReadLine();
                            string[] splitstr = line.Split(',');
                            Position = new Vector2(Convert.ToInt32(splitstr[0]), Convert.ToInt32(splitstr[1]));
                        }
                        else if (line.Contains("<Range>"))
                        {
                            line = reader.ReadLine();
                            string[] splitstr = line.Split(',');
                            Range = new Point(Convert.ToInt32(splitstr[0]), Convert.ToInt32(splitstr[1]));
                        }
                        else if(line.Contains("<ID>"))
                        {
                            line = reader.ReadLine();
                            ID = Convert.ToInt32(line);
                        }
                        else if(line.Contains("<Height>"))
                        {
                            line = reader.ReadLine();
                            height = (float)Convert.ToDouble(line);
                        }
                        else if(line.Contains("<Conversation>"))
                        {//future expansion for having multiple List<List<string>> for characters
                        }
                        else if(line.Contains("<Sub Conversation>"))
                        {
                            string subconvo = "";
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Contains("<\\Sub Conversation>"))
                                    break;
                                subconvo += line + "\n";
                            }
                            convo.Add(subconvo);
                        }

                    }
                    reader.Close();
                }

            }
        }
    }
}
