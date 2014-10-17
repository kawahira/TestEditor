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
        public Road(ref Lists lists, Territory r0, Territory r1, Direction dir)
        {
            t0 = r0;
            t1 = r1;
            lists.road.Add(this);
            direction = dir;
        }
        public void Build(ref Mapchip mapchip , int icon)
        {
            if (direction == Road.Direction.Horizonal)
            {
                int c0x = t0.hx;
                int c0y = RandXorShift.Instance.Stage.Next(t0.room.ly + 2, t0.room.hy - 2);
                int c1x = t1.lx;
                int c1y = RandXorShift.Instance.Stage.Next(t1.room.ly + 2, t1.room.hy - 2);
                lines(ref mapchip, c0x, c0y, c1x, c1y, icon);
                lines(ref mapchip, t0.room.hx, c0y, c0x, c0y, icon);
                lines(ref mapchip, t1.room.lx, c1y, c1x, c1y, icon);
            }
            else
            {
                int c0x = RandXorShift.Instance.Stage.Next(t0.room.lx + 2, t0.room.hx - 2); // 接続する部屋.1までのX座標(random)
                int c0y = t0.hy;                                                             // 接続領域.1のY座標を固定化
                int c1x = RandXorShift.Instance.Stage.Next(t1.room.lx + 2, t1.room.hx - 2); // 接続する部屋.2までのX座標(random)
                int c1y = t1.ly;                                                             // 接続領域.2のY座標を固定化
                lines(ref mapchip, c0x, c0y, c1x, c1y, icon);                  // 境界同士の接続
                lines(ref mapchip, c0x, t0.room.hy, c0x, c0y, icon);      // 境界から領域内の部屋.0への接続
                lines(ref mapchip, c1x, t1.room.ly, c1x, c1y, icon);      // 境界から領域内の部屋.1への接続
            }

        }
        private void lines(ref Mapchip mapchip, int x0, int y0, int x1, int y1, int icon)
        {
            int min_x, max_x, min_y, max_y;
            min_x = Math.Min(x0, x1);
            max_x = Math.Max(x0, x1);
            min_y = Math.Min(y0, y1);
            max_y = Math.Max(y0, y1);
            int px0 = min_x;
            int py0 = max_y;
            int px1 = max_x;
            int py1 = max_y;
            int px2 = max_x;
            int py2 = min_y;
            int px3 = max_x;
            int py3 = max_y;
            if ((x0 <= x1) && (y0 >= y1))   // [↗]左下から右上に
            {
                mapchip.Line(min_x, max_y, max_x, max_y, icon);      //   |
                mapchip.Line(max_x, min_y, max_x, max_y, icon);     // ---+
                return;
            };
            if ((x0 > x1) && (y0 > y1))     // [↖]右下から左上に
            {                               // アルゴリズム上、ここは絶対に来ない（来たらおかしくなる）
                throw new IndexOutOfRangeException();
                // mapchip.Line(min_x, min_y, max_x, min_y, icon);     // ---+
                // mapchip.Line(max_x, min_y, max_x, max_y, icon);     //    |
                //return;
            };
            if ((x0 > x1) && (y0 <= y1))    // [↙]右上から左下に
            {
                mapchip.Line(min_x, min_y, max_x, min_y, icon);     // +---
                mapchip.Line(min_x, min_y, min_x, max_y, icon);     // |
                return;
            };
            if ((x0 <= x1) && (y0 < y1))   // [↘]左上から右下に
            {
                mapchip.Line(min_x, max_y, max_x, max_y, icon);     // |
                mapchip.Line(min_x, min_y, min_x, max_y, icon);     // +---
                return;
            };
        }
    };
}
