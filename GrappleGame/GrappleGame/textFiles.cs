using System;
using System.IO;
using System.Collections.Generic;

namespace GrappleGame
{
    class textFiles
    {
        //public struct TileAttributes
        //{

        //}
        public int[,] tileSet; //stores tile type at each array point, also what is loaded and saved
        public float[,] heightSet;
        public int[,] impassSet;
        public int[,] grappleSet;
        public int[,] entitySet;
        public float[,] depthSet;
        public float[,] coverageSet;
        string MapName;


        public textFiles(int X, int Y, string newNapName)
        {
            tileSet = new int[X, Y];
            heightSet = new float[X, Y];
            MapName = newNapName;
            impassSet = new int[X, Y];
            grappleSet = new int[X, Y];
            entitySet = new int[X, Y];
            depthSet = new float[X, Y];
            coverageSet = new float[X, Y];
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
                        saveFile.Write(basetile + ":0:0:0:0:0.8:-1,");
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
                        saveFile.Write(tileSet[x, y] + ":" + heightSet[x, y] + ":" + impassSet[x, y] + ":" + grappleSet[x,y] + ":" + entitySet[x,y] + ":" + depthSet[x,y] + ":" + coverageSet[x,y] + ",");
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
                        grappleSet[x, y] = Convert.ToInt32(tileatts[3]);
                        entitySet[x, y] = Convert.ToInt32(tileatts[4]);
                        depthSet[x, y] = (float)Convert.ToDecimal(tileatts[5]);
                        coverageSet[x, y] = (float)Convert.ToDecimal(tileatts[6]);
                    }
                    y++;
                }
                reader.Close();
            }
        }
        #endregion

    }
}
