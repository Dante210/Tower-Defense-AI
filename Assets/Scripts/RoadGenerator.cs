using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class RoadGenerator
    {
        public Point startPoint { get; }

        public RoadGenerator(int startPosX, int minY, int maxY)
        {
            var random = new Random();
            var y = random.Next(minY, maxY);
            this.startPoint =  new Point(startPosX, y);
        }


        public List<RoadTile> generateRoad(GameObject prefab, Vector3 startPos, Func<int, int, bool> finish, List<RoadInfo> roadInfos)
        {
            var generatedRoad = new List<RoadTile>();
            var indexInSequence = 0;
            var roadTile = TileFactory.makeRoadTile(prefab, startPos, startPoint);
            generatedRoad.Add(roadTile);
            var currentTile = roadTile;
            while (!finish(currentTile.x, currentTile.y))
            {
                
            }
            return generatedRoad;
        }
    }
}
