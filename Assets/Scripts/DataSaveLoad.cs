using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    internal class DataSaveLoad
    {
        public static readonly string ROADS_INFO_PATH = Application.dataPath + "/RoadsInfo.txt";
        public static readonly string ROADS_DATA_PATH = Application.dataPath + "/RoadsDATA.dat";
        public static readonly string TOWERS_INFO_PATH = Application.dataPath + "/RoadsInfo.txt";
        private readonly int maxElementCount;
        private List<RoadInfo> roadsInfo;

        public DataSaveLoad(int maxElementCount)
        {
            this.maxElementCount = maxElementCount;
            if (!File.Exists(ROADS_DATA_PATH))
            {
                initialSetupRoadInfo();
            }
            else
            {
                roadsInfo = roadsInfo.load(ROADS_DATA_PATH);
                if (!File.Exists(ROADS_INFO_PATH))
                    roadsInfo.writeToFile(ROADS_INFO_PATH, info => info.ToString());
            }
        }

        public void updateRoadsData(List<RoadTile> roadTiles)
        {
            var counter = 0;
            foreach (var roadTile in roadTiles)
            {
                roadsInfo[counter].increaseCount(roadTile.getLeadKey());
                roadsInfo[counter++].updateValues();
            }
            roadsInfo.writeToFile(ROADS_INFO_PATH, info => info.ToString());
            roadsInfo.save(ROADS_DATA_PATH);
        }


        private void initialSetupRoadInfo()
        {
            roadsInfo = new List<RoadInfo>(maxElementCount);
            for (var i = 0; i < maxElementCount; i++) roadsInfo.Add(new RoadInfo(i));
            roadsInfo.writeToFile(ROADS_INFO_PATH, info => info.ToString());
            roadsInfo.save(ROADS_DATA_PATH);
        }
    }

    [Serializable]
    public class RoadInfo
    {
        private  Dictionary<string, int> counts;
        private  Dictionary<string, double> values;

        public RoadInfo(int indexInSequence)
        {
            this.indexInSequence = indexInSequence;

            values = new Dictionary<string, double>
            {
                ["Up"] = 0,
                ["Down"] = 0,
                ["Left"] = 0,
                ["Right"] = 0
            };

            counts = new Dictionary<string, int>
            {
                ["Up"] = 0,
                ["Down"] = 0,
                ["Left"] = 0,
                ["Right"] = 0
            };
        }

        private int indexInSequence { get; }

        public void increaseCount(string key)
        {
            if (key != null)
                counts[key]++;
        }

        public void updateValues()
        {
            values =  counts.calculateValues(values);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{indexInSequence,3}");
            foreach (var count in counts) builder.Append($"{count.Key,7} {count.Value,3}");
            builder.Append(" | ");
            foreach (var value in values)
                builder.Append($"{value.Key,7} {value.Value:N3}");
            return builder.ToString();
        }
    }
}