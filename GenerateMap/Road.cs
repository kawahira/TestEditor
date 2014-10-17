// Racanhack 法により自動生成
// original source http://racanhack.sourceforge.jp/rhdoc/index.html 

using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Road
    {
        public enum Direction
        {
            Veritical = 0,
            Horizonal = 1
        }
        public Direction direction;
        public Territory t0;
        public Territory t1;
        public int c0x;
        public int c0y;
        public int c1x;
        public int c1y;

        public Road(ref List<Road> roadList, Territory r0, Territory r1, Direction dir)
        {
            t0 = r0;
            t1 = r1;
            direction = dir;
            roadList.Add(this);
        }
        public void Draw(ref Mapchip mapchip, int icon)
        {
            bool ret = direction == Road.Direction.Horizonal;
            lines(ref mapchip, c0x, c0y, c1x, c1y, icon);                  // 境界同士の接続
            lines(ref mapchip, ret ? t0.room.hx : c0x, ret ? c0y : t0.room.hy, c0x, c0y, icon);
            lines(ref mapchip, ret ? t1.room.lx : c1x, ret ? c1y : t1.room.ly, c1x, c1y, icon);
        }
        public void Connect()
        {
            int padding = 2; // 接続ポイントを部屋の隅からどの程度離すか（部屋の角に道を繋げないため）
            bool ret = direction == Road.Direction.Horizonal;
            c0x = ret ? t0.hx : RandXorShift.Instance.Stage.Next(t0.room.lx + padding, t0.room.hx - padding); // 接続する部屋.1までのX座標
            c0y = ret ? RandXorShift.Instance.Stage.Next(t0.room.ly + padding, t0.room.hy - padding) : t0.hy; // 接続領域.1のY座標を固定化 
            c1x = ret ? t1.lx : RandXorShift.Instance.Stage.Next(t1.room.lx + padding, t1.room.hx - padding); // 接続する部屋.2までのX座標
            c1y = ret ? RandXorShift.Instance.Stage.Next(t1.room.ly + padding, t1.room.hy - padding) : t1.ly; // 接続領域.2のY座標を固定化
        }
        private void lines(ref Mapchip mapchip, int x0, int y0, int x1, int y1, int icon)
        {
            int index = 0; // 初期値は[↗]左下から右上に
            int min_x = Math.Min(x0, x1);
            int max_x = Math.Max(x0, x1);
            int min_y = Math.Min(y0, y1);
            int max_y = Math.Max(y0, y1);
            int[, ,] ids = new int[,,] { { { min_x, max_y, max_x, max_y }, { max_x, min_y, max_x, max_y } }
                                      , { { min_x, max_y, max_x, max_y }, { min_x, min_y, min_x, max_y } }
                                      , { { min_x, min_y, max_x, min_y }, { min_x, min_y, min_x, max_y } } };
            if ((x0 > x1) && (y0 > y1))
            {   // [↖]右下から左上にアルゴリズム上、ここは絶対に来ない（来たらおかしくなる）のでexception.
                throw new IndexOutOfRangeException();
            }
            if (!((x0 <= x1) && (y0 >= y1)))   // [↗]左下から右上でなければ[↘] or [↙]に限定される。
            {
                index = (x0 <= x1) && (y0 < y1) ? 1 : 2; // [↘]左上から右下に or [↙]右上から左下に((x0 > x1) && (y0 <= y1))
            }
            mapchip.Line(ids[index, 0, 0], ids[index, 0, 1], ids[index, 0, 2], ids[index, 0, 3], icon);
            mapchip.Line(ids[index, 1, 0], ids[index, 1, 1], ids[index, 1, 2], ids[index, 1, 3], icon);
        }
    };
}
