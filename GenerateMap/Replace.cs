using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateMap
{
    class Replace
    {
        private class Data
        {
            public int org;
            public int rep;
        }
        private List<Data[,]> replaceList = new List<Data[,]>();
        void CreateDebugData()
        {
            {   // 「をカーブに変形
                int icon = 3;
                Data[,] topleft = new Data[5, 5];
                for (int i = 0; i < topleft.GetLength(0); i++)
                {
                    for (int j = 0; j < topleft.GetLength(1); j++)
                    {
                        topleft[i, j] = new Data();
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    topleft[i + 1, 1].org = icon;
                    topleft[1, i + 1].org = icon;
                }
                topleft[1, 3].rep = icon;
                topleft[1, 4].rep = icon;
                topleft[2, 2].rep = icon;
                topleft[2, 3].rep = icon;
                topleft[3, 1].rep = icon;
                topleft[3, 2].rep = icon;
                topleft[4, 1].rep = icon;
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
        public void Build(int mapwidth , int mapheight, ref Mapchip mapchip)
        {
            CreateDebugData();
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
