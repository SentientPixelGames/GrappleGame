using System;
using System.IO;
using System.Collections.Generic;

namespace GrappleGame
{
    class textFiles
    {
        public int[,] tileSet; //stores tile type at each array point, also what is loaded and saved
        public float[,] heightSet;
        public int[,] impassSet;
        public int[,] objectSet;
        public float[,] depthSet;
        public float[,] objHeightSet;
        public int[,] solidSet;
        public float[,] shadowSet;
        string MapName;


        public textFiles(int X, int Y, string newNapName)
        {
            tileSet = new int[X, Y];
            heightSet = new float[X, Y];
            MapName = newNapName;
            impassSet = new int[X, Y];
            objectSet = new int[X, Y];
            depthSet = new float[X, Y];
            objHeightSet = new float[X, Y];
            solidSet = new int[X, Y];
            shadowSet = new float[X, Y];
        }
        #region Saving/Loading
        public void SavedSmall(int SizeX, int SizeY, int SMC, int basetile, string mapname)
        {
            using (StreamWriter saveFile = new StreamWriter(mapname))
            {
                for (int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        saveFile.Write(basetile + ":0:0:0:0:.40003:0:0,");
                    }
                    saveFile.Write("\r\n");
                }
                saveFile.Close();

            }
        }

        public void Saved(int SizeX, int SizeY)
        {
            using (StreamWriter saveFile = new StreamWriter(this.MapName))
            {
                for (int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        saveFile.Write(tileSet[x, y] + ":" + heightSet[x, y] + ":" + impassSet[x, y] + ":" + solidSet[x,y] + ":" + objectSet[x,y] + ":" + depthSet[x,y] + ":" + objHeightSet[x,y] + ":" + shadowSet[x,y] + ",");
                    }
                    saveFile.Write("\r\n");
                }
                saveFile.Close();

            }
        }
        public void LoadLevel(int SizeX)
        {
            using (StreamReader reader = new StreamReader(this.MapName))
            {
                Random r = new Random();
                int y = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tile = line.Split(',');
                    
                    for (int x = 0; x < SizeX; x++)
                    {
                        string[] tileatts = tile[x].Split(':');
                        tileSet[x, y] = Convert.ToInt32(tileatts[0]);
                        heightSet[x, y] = (float)Convert.ToDecimal(tileatts[1]);
                        impassSet[x, y] = Convert.ToInt32(tileatts[2]);
                        solidSet[x, y] = Convert.ToInt32(tileatts[3]);
                        objectSet[x, y] = Convert.ToInt32(tileatts[4]);
                        depthSet[x, y] = (float)Convert.ToDecimal(tileatts[5]);
                        objHeightSet[x, y] = (float)Convert.ToDecimal(tileatts[6]);
                        shadowSet[x, y] = (float)Convert.ToDecimal(tileatts[7]);
                    }
                    y++;
                }
                reader.Close();
            }
        }
        #endregion

    }
}
