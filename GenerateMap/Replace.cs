using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateMap
{
    public class Replace
    {
        private class Data
        {
            public int org;
            public int rep;
        }
        private List<Data[,]> replaceList = new List<Data[,]>();
        public void CreateDebugData(int icon)
        {
            {   // 「をカーブに変形
                Data[,] topleft = new Data[5, 5];
                int[,] tbl = new int[,] { { 1, 3 }, { 1, 4 }, { 2, 2 }, { 2, 3 }, { 3, 1 }, { 3, 2 }, { 4, 1 } };
                for (int i = 0; i < topleft.GetLength(0); i++)
                {
                    for (int j = 0; j < topleft.GetLength(1); j++)
                    {
                        topleft[i, j] = new Data();
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    topleft[i + 1, 1].org = topleft[1, i + 1].org = icon;
                }
                for (int i = 0; i < tbl.GetLength(0); i ++  )
                {
                    topleft[tbl[i, 0], tbl[i, 1]].rep = icon;
                }
                replaceList.Add(topleft);
            }
        }

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
            foreach (Data[,] r in replaceList)
            {
                int w = mapwidth - r.GetLength(0);
                int h = mapheight - r.GetLength(1);
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (ChackSame(ref mapchip.entity, r, i, j))
                        {
                            Copy(ref mapchip.entity, r, i, j);
                        }
                    }
                }
            }
        }
    }
}
