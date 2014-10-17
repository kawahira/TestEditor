// Racanhack 法により自動生成
// original source http://racanhack.sourceforge.jp/rhdoc/index.html 

using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Replace
    {
        public class Data
        {
            public int org;
            public int rep;
        }
        public class Datas
        {
            public float rate;
            public Data[,] pieces;
        }
        public List<Datas> replaceList = new List<Datas>();

        bool ChackSame(ref int[,] entity, Data[,] r, int x, int y)
        {
            for (int i = 0; i < r.GetLength(0); i++)
            {
                for (int j = 0; j < r.GetLength(1); j++)
                {
                    if (entity[x + i, y + j] != r[i, j].org) return false;
                }
            }
            return true;
        }
        void Copy(ref int[,] entity , Data[,] r, int x, int y)
        {
            for (int i = 0; i < r.GetLength(0); i++)
            {
                for (int j = 0; j < r.GetLength(1); j++)
                {
                    entity[x + i, y + j] = r[i, j].rep;
                }
            }
        }
        public void Draw(ref Mapchip mapchip, int mapwidth , int mapheight)
        {
            foreach (Datas r in replaceList)
            {
                int w = mapwidth - r.pieces.GetLength(0);
                int h = mapheight - r.pieces.GetLength(1);
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (ChackSame(ref mapchip.entity, r.pieces, i, j))
                        {
                            Copy(ref mapchip.entity, r.pieces, i, j);
                        }
                    }
                }
            }
        }
    }
}
