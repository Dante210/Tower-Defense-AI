using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class DataSaveLoad
    {
        int maxElementCount;

        public static readonly string ROADS_INFO_PATH = Application.dataPath + "/RoadsInfo.txt";
        public static readonly string ROADS_DATA_PATH = Application.dataPath + "/RoadsDATA.dat";
        public static readonly string TOWERS_INFO_PATH = Application.dataPath + "/RoadsInfo.txt";
        List<RoadInfo> roadsInfo;

        public DataSaveLoad(int maxElementCount){
            this.maxElementCount = maxElementCount;
            if (!File.Exists(ROADS_DATA_PATH))
                initialSetupRoadInfo();
            else{
                roadsInfo.load(ROADS_DATA_PATH);
                if(!File.Exists(ROADS_INFO_PATH))
                    roadsInfo.writeToFile(ROADS_INFO_PATH, info => info.ToString());
            }
        }

        public void updateRoadsData(Dictionary<Point, RoadTile> roadTiles){
            var counter = 0;
            foreach (var roadTile in roadTiles){
                roadsInfo[counter++].increaseCount(roadTile.Value.getLeadKey());
            }
        }



        void initialSetupRoadInfo(){
            roadsInfo = new List<RoadInfo>(maxElementCount);
            for (var i = 0; i < maxElementCount; i++) roadsInfo.Add(new RoadInfo(i));
            roadsInfo.writeToFile(ROADS_INFO_PATH, info => info.ToString());
            roadsInfo.save(ROADS_DATA_PATH);
        }
    }

    [Serializable]
    class RoadInfo
    {
         Dictionary<string, int> counts;
         Dictionary<string, double> values;

        public RoadInfo(int indexInSequence){
            this.indexInSequence = indexInSequence;

            values = new Dictionary<string, double>{
                ["Up"] = 0,
                ["Down"] = 0,
                ["Left"] = 0,
                ["Right"] = 0
            };

            counts = new Dictionary<string, int>{
                ["Up"] = 0,
                ["Down"] = 0,
                ["Left"] = 0,
                ["Right"] = 0
            };
        }

        public void increaseCount(string key){
            counts[key]++;
        }

        int indexInSequence{ get; set; }

        public override string ToString(){
            var builder = new StringBuilder();
            builder.Append($"{indexInSequence:-3}");
            foreach (var count in counts) builder.Append($"{count.Key:-5} {count.Value:-3}");
            foreach (var value in values)
                builder.Append($"{value.Key:-5} {value.Value:-3}");
            return builder.ToString();
        }
    }
}