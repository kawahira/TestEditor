using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Mapchip
    {
        static int NoIconID = 0;
        public int[,] entity;
        public Mapchip(int w, int h)
        {
            entity = new int[w,h];
        }
        public void Line(int x0, int y0, int x1, int y1, int icon, bool check = false)
        {
            if ( (x0 != x1) && (y0 != y1) )
            {
                // 今のところ垂直、水平しか使わないので斜めが必要になったら作る
                throw new IndexOutOfRangeException();
            }
            Fill(x0, y0, x1, y1, icon, check);
        }

        public void Fill(int x, int y, int w, int h, int icon, bool check = false)
        {
            if (check == true && (CheckArea(x, y, w, h, NoIconID) == false)) return;
            for (int i = x; i <= w; i++)
            {
                for (int j = y; j <= h; j++)
                {
                    entity[i,j] = icon;
                }
            }
        }
        public bool CheckArea(int x, int y, int w, int h,int icon)
        {
            for (int i = x; i <= w ; i++)
            {
                for (int j = y; j <= h ; j++)
                {
                    if (entity[i, j] != icon) return false;
                }
            }
            return true;
        }
        public void AroundReplace(int searchIcon, int replaceIcon)
        {
            int[,] tbl = new int[,] { { -1, -1 }, { 0, -1 }, { +1, -1 }, { -1, 0 }, { +1, 0 }, { -1, +1 }, { 0, +1 }, { +1, +1 } };
            for (int i = 1; i < entity.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < entity.GetLength(1) - 1; j++)
                {
                    if (entity[i, j] == searchIcon)
                    {
                        for (int k = 0; k < tbl.GetLength(0); k++)
                        {
                            int x = i + tbl[k, 0];
                            int y = j + tbl[k, 1];
                            if (entity[x, y] == NoIconID) entity[x, y] = replaceIcon;
                        }
                    }
                }
            }
        }
    }
}
