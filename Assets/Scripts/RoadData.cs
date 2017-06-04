using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Storing learning data for road generation
    /// </summary>
    class RoadData
    {
        public static readonly string ROADS_INFO_PATH = Application.dataPath + "/RoadsInfo.txt";
        public static readonly string ROADS_DATA_PATH = Application.dataPath + "/RoadsDATA.dat";
        readonly int maxElementCount;

        public RoadData(int maxElementCount) {
            this.maxElementCount = maxElementCount;
            if (!File.Exists(ROADS_DATA_PATH)) {
                initialSetupRoadInfo();
            }
            else {
                info = info.load(ROADS_DATA_PATH);
                if (!File.Exists(ROADS_INFO_PATH))
                    info.writeToFile(ROADS_INFO_PATH, info => info.ToString());
            }
        }

        public List<RoadInfo> info { get; private set; }

        public void updateRoadsData(List<RoadTile> roadTiles) {
            var counter = 0;
            foreach (var roadTile in roadTiles) {
                info[counter++].increaseCount(roadTile.getLeadKey());
            }
            info.writeToFile(ROADS_INFO_PATH, info => info.ToString());
            info.save(ROADS_DATA_PATH);
        }


        void initialSetupRoadInfo() {
            info = new List<RoadInfo>(maxElementCount);
            for (var i = 0; i < maxElementCount; i++) info.Add(new RoadInfo(i));
            info.writeToFile(ROADS_INFO_PATH, info => info.ToString());
            info.save(ROADS_DATA_PATH);
        }
    }

    [Serializable]
    public class RoadInfo
    {
        public readonly Dictionary<string, int> counts;

        public RoadInfo(int indexInSequence) {
            this.indexInSequence = indexInSequence;

            counts = new Dictionary<string, int> {
                ["Up"] = 0,
                ["Down"] = 0,
                ["Left"] = 0,
                ["Right"] = 0
            };
        }

        int indexInSequence { get; }

        internal void increaseCount(string key) {
            if (key != null)
                counts[key]++;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append($"{indexInSequence,3}");
            foreach (var count in counts) builder.Append($"{count.Key,7} {count.Value,3}");
            var values = counts.calculateValues();
            foreach (var value in values)
                builder.Append($"{value.Key,7} {value.Value:N3}");
            return builder.ToString();
        }
    }
}