using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class TowerData
    {
        public static readonly string TOWERS_INFO_PATH = Application.dataPath + "/TowersInfo.txt";
        public static readonly string TOWERS_DATA_PATH = Application.dataPath + "/TowersDATA.dat";

        public TowerData(int maxElementCount) {
            this.maxElementCount = maxElementCount;
            if (!File.Exists(TOWERS_INFO_PATH)) {
                initialSetup();
            }
            else {
                towersInfo = towersInfo.load(TOWERS_DATA_PATH);
                if (!File.Exists(TOWERS_INFO_PATH))
                    towersInfo.writeToFile(TOWERS_INFO_PATH, info => info.ToString());
            }
        }

        public int maxElementCount { get; }

        public List<TowerInfo> towersInfo { get; private set; }

        public void updateData(List<TowerTile> towerTiles) {
            var counter = 0;
            for (var i = 0; i < towerTiles.Count; i++) {
                towersInfo[towerTiles[i].closestRoadTileNum].roadTileCount++;
                towersInfo[towerTiles[i].closestRoadTileNum].offset[towerTiles[i].offset]++;
            }
            towersInfo.writeToFile(TOWERS_INFO_PATH, info => info.ToString());
            towersInfo.save(TOWERS_DATA_PATH);
        }


        void initialSetup() {
            towersInfo = new List<TowerInfo>(maxElementCount);
            for (var i = 0; i < maxElementCount; i++) towersInfo.Add(new TowerInfo());
            towersInfo.writeToFile(TOWERS_INFO_PATH, info => info.ToString());
            towersInfo.save(TOWERS_DATA_PATH);
        }
    }

    [Serializable]
    class TowerInfo
    {
        internal TowerInfo() {
            offset = new Dictionary<int, int>();
            for (var i = -8; i <= 8; i++) if (i != 0) offset.Add(i, 0);
        }

        public Dictionary<int, int> offset { get; set; }
        public int roadTileCount { get; set; }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append($"{roadTileCount,-5}");
            foreach (var offse in offset) builder.Append($"{offse,-3}");
            return builder.ToString();
        }
    }
}