// Racanhack 法により自動生成
// original source http://racanhack.sourceforge.jp/rhdoc/index.html 

using System;

namespace GenerateMap
{
    public class Room
    {
        public int lx, ly, hx, hy;
        public Room(int sx, int sy, int ex, int ey, byte minRoomSize, byte marginRoomSize)
        {
            int w, h;
            w = RandXorShift.Instance.Stage.Next(minRoomSize, ex - sx - (marginRoomSize * 2) + 1);
            h = RandXorShift.Instance.Stage.Next(minRoomSize, ey - sy - (marginRoomSize * 2) + 1);
            float distX = (float)((float)w / (float)(ex - sx));
            if (distX < 0.4f)
            {
                //                    w *= 3;
            }
            float distY = (float)((float)h / (float)(ey - sy));
            if (distY < 0.4f)
            {
                //                    h *= 3;
            }
            lx = RandXorShift.Instance.Stage.Next(sx + marginRoomSize, ex - marginRoomSize - w + 1);
            ly = RandXorShift.Instance.Stage.Next(sy + marginRoomSize, ey - marginRoomSize - h + 1);
            hx = lx + w;
            hy = ly + h;
        }
    };
}
