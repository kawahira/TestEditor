// Racanhack 法により自動生成
// original source http://racanhack.sourceforge.jp/rhdoc/index.html 

using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Generator
    {
        public List<Territory> territory = new List<Territory>();
        public List<Road> road = new List<Road>();
        public Mapchip mapchip;
        private Config config;
        public List<Room> start = new List<Room>();
        public List<Room> goal = new List<Room>();
        public ushort minTerritorySize
        {
            get
            {
                return (ushort)(config.minRoomSize + (config.marginRoomSize * 2));
            }
        }
        public Config GetConfig() { return config; }

        public void Build(Config cfg)
        {
            config = cfg;
            if (config.randomSeed != 0)
            {   // 乱数初期化 ( 0 なら初期化せず )
                RandXorShift.Instance.Stage.Seed(config.randomSeed);
            }

            // リストクリア
            territory.Clear();
            road.Clear();

            // mapchipバッファを確保
            mapchip = new Mapchip(config.width, config.height);

            // テリトリー生成と最低限の道と部屋の設定
            {
                Territory root = new Territory(ref territory, 0, 0, config.width - 1, config.height - 1);
                root.Build(ref territory, ref road, minTerritorySize, config.minRoomSize, config.marginRoomSize);
            }

            // 道の追加生成(迷わせるための道)
            AddRoad(config.addRoadMax);

            // 道を実際に接続する
            foreach (Road r in road)
            {
                r.Connect();
            }

            RenderToMapchip();

            for (int i = 0; i < config.startCount; i++)
            {
                start.Add(territory[RandXorShift.Instance.Stage.Next(0, territory.Count)].room);
            }
            {
                for ( int i = 0 ; i < config.goalCount ; i ++ )
                {
                    int farIndex = -1;
                    double max = -1.0f;
                    int count = 0;
                    int sindex = 0;
                    foreach (Territory t in territory)
                    {
                        if (start[sindex].index != t.room.index)
                        {
                            start[sindex].GoalNode = t.room;
                            Node.Pathfinding.AStar astar = new Node.Pathfinding.AStar();
                            astar.FindPath(start[sindex], t.room);
                            double totalcost = ((Room)astar.Solution[astar.Solution.Count - 1]).TotalCost;
                            Console.WriteLine(totalcost);
                            if (totalcost >= max)
                            {
                                max = totalcost;
                                farIndex = count;
                            }
                        }
                        ++count;
                    }
                    if (farIndex != -1)
                    {
                        goal.Add(territory[RandXorShift.Instance.Stage.Next(0, territory.Count)].room);
                    }
                    goal.Add(territory[farIndex].room);
                }
            }
        }
        private void RenderToMapchip()
        {
            // 部屋の壁をmapchipにfeedback
            foreach (Territory r in territory)
            {
                r.DrawBefore(ref mapchip, config.iconRoomWall);
            }

            // 道をmapchipにfeedback
            foreach (Road r in road)
            {
                r.Draw(ref mapchip, config.iconRoad);
            }

            // 部屋の床をmapchipにfeedback
            foreach (Territory r in territory)
            {
                r.DrawAfter(ref mapchip, config.iconRoomFloor);
            }

            // mapchipの特定パターンを置換
            foreach (Replace r in config.replaceList)
            {
                r.Draw(ref mapchip, config.width, config.height);
            }

            // mapchipの指定番号の周りの空白を指定番号に置換
            mapchip.AroundReplace(3, 5);
            mapchip.AroundReplace(5, 6);
            mapchip.AroundReplace(4, 7);
        }
        struct tempResult
        {
            public Territory t0;
            public Territory t1;
            public int direction;
            public tempResult(Territory te1, Territory te2, int dir)
            {
                t0 = te1;
                t1 = te2;
                direction = dir;
            }
        }
        private void AddRoad(ushort createMax)
        {
            if (createMax == 0) return;
            Territory[,] refmap = new Territory[config.width, config.height];
            List<tempResult> result = new List<tempResult>();
            foreach (Territory r in territory)
            {
                for (int i = r.lx; i < r.hx; i++)
                {
                    for (int j = r.ly; j < r.hy; j++)
                    {
                        refmap[i, j] = r;
                    }
                }
            }
            int[,] tbl = new int[,] { { 0, -1 }, { 0, -1 }, { +1, -1 }, { -1, 0 }, { +1, 0 }, { -1, +1 }, { 0, +1 }, { +1, +1 } };
            for (int i = 0; i < config.width - 2; i++)
            {
                for (int j = 0; j < config.height - 2; j++)
                {
                    // 下方向に対する検索
                    Territory r0 = refmap[i, j];        // 基準
                    Territory r1 = refmap[i, j + 1];    // 比較（下側)
                    Territory r2 = refmap[i + 1, j];    // 比較 (右側)
                    for (int k = 0; k < 2; k++)
                    {
                        if (r0 != r1)
                        {
                            bool active = true;
                            // すでに存在している道に同じ経路があれば登録をしない
                            foreach (Road r in road)
                            {
                                if ((r.t0 == r0 || r.t0 == r1) && (r.t1 == r0 || r.t1 == r1))
                                {
                                    active = false;
                                    break;
                                }
                            }
                            // 新規として一度登録したResultに同じ場所への道があれば、登録をしない(これを処理しないと同じテリトリーに複数の道が出来る（これもよい）)
                            if (active)
                            {
                                foreach (tempResult t in result)
                                {
                                    if ((t.t0 == r0 || t.t0 == r1) && (t.t1 == r0 || t.t1 == r1))
                                    {
                                        active = false;
                                        break;
                                    }
                                }
                            }
                            if (active)
                            {
                                result.Add(new tempResult(r0, r1, k));
                            }
                        }
                        r1 = r2;    // 横方向に対する検索に対象を切り替え
                    }
                }
            }

            // 最終的な総数の中から、要求数の道をランダムに抽選してから生成する
            int max = Math.Min(createMax, result.Count);
            for (int i = 0; i < max; i++)
            {
                int idx = RandXorShift.Instance.Stage.Next(0, result.Count);
                new Road(ref road, result[idx].t0, result[idx].t1, result[idx].direction == 0 ? Road.Direction.Veritical : Road.Direction.Horizonal);
                result.RemoveAt(idx);
            }
        }
        /*        
                void TerritoryToMap(int icon)
                {
                    int i, j;
                    foreach (Territory r in lists.territory)
                    {
                        for (i = r.lx, j = r.ly; i <= r.hx; i++) entity[i, j] = icon;
                        for (i = r.lx, j = r.hy; i <= r.hx; i++) entity[i, j] = icon;
                        for (i = r.lx, j = r.ly; j <= r.hy; j++) entity[i, j] = icon;
                        for (i = r.hx, j = r.ly; j <= r.hy; j++) entity[i, j] = icon;
                    }
                }
         */
    }
}
