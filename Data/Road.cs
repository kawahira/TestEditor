using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replace.Data
{
    public class Road : GenerateMap.Replace
    {
        public void Curve1(int icon, float rate)
        {
            // 「をカーブに変形
            Datas datas = new Datas();
            datas.pieces = new Data[5, 5];
            int[,] tbl = new int[,] { { 1, 3 }, { 1, 4 }, { 2, 2 }, { 2, 3 }, { 3, 1 }, { 3, 2 }, { 4, 1 } };
            for (int i = 0; i < datas.pieces.GetLength(0); i++)
            {
                for (int j = 0; j < datas.pieces.GetLength(1); j++)
                {
                    datas.pieces[i, j] = new Data();
                }
            }
            for (int i = 0; i < 4; i++)
            {
                datas.pieces[i + 1, 1].org = datas.pieces[1, i + 1].org = icon;
            }
            for (int i = 0; i < tbl.GetLength(0); i++)
            {
                datas.pieces[tbl[i, 0], tbl[i, 1]].rep = icon;
            }
            datas.rate = rate;
            replaceList.Add(datas);
        }
    }
}
