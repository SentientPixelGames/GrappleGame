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
            foreach (Keys keys in pressedkeys)
            {
                if (oldkeyboard.IsKeyUp(keys))
                {
                    if (keys == Keys.Back && text.Length > 0)
                    {
                        text = text.Remove(text.Length - 1, 1);
                    }
                    else if (keys == Keys.Space)
                        text = text.Insert(text.Length, " ");
                    else if (keys == Keys.Enter)
                        text = text.Insert(text.Length, "\n");
                    else if (keys == Keys.OemComma)
                        text = text.Insert(text.Length, ",");
                    else if (keys == Keys.OemPeriod)
                        text = text.Insert(text.Length, ".");
                    else if (keys == Keys.OemQuestion)
                        text = text.Insert(text.Length, "?");
                    else if (keys == Keys.D1)
                        text = text.Insert(text.Length, "!");
                    else if (keys == Keys.OemTilde)
                        text = text.Insert(text.Length, "'");
                    else if (keys == Keys.Escape)
                        Done = true;
                    else if (keys == Keys.Tab)
                    {
                        text += text.Insert(text.Length, "\t");
                        Done = true;
                    }
                    else text += keys.ToString();
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
                if (keys == Keys.D0)
                    text = text.Insert(text.Length, "0");
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
                else if (keys == Keys.Enter)
                    Done = true;
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
                        foreach(Map mapnamesearch in Maps)
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
    }
}
