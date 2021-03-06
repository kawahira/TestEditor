﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RandXorShift.Instance.Seed(1);
            GenerateMap.Config config = new GenerateMap.Config();

            config.minRoomSize = 8;
            config.marginRoomSize = 3;
            config.height = 400 / 4;
            config.width = 400 / 4;
            config.addRoadMax = 100;
            config.iconRoomWall = 4;
            config.iconRoad = 3;
            config.iconRoomFloor = 2;
            config.randomSeed = 1;
            config.startCount = 1;
            config.goalCount = 1;

            Replace.Data.Road r = new Replace.Data.Road();
            r.Curve1(config.iconRoad, 30);
            config.replaceList.Add(r);


            Form1 f = new Form1();
            f.generator = new GenerateMap.Generator();
            f.generator.Build(config);

            f.pathFinder = new SpatialAStar.PathFinder(config.width, config.height);
            f.pathFinder.SetTile(f.generator.mapchip.entity, config.iconRoomWall);

//            f.pathFinder.Get(f.pathFinderResult, );

            Application.Run(f);

        }
    }
}
