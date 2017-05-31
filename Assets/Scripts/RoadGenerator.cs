using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class RoadGenerator
    {
        public RoadGenerator(Point min, Point max) {
            this.min = min;
            this.max = max;

            generatedRoadTiles = new List<RoadTile>();
            setConditions();
        }

        public List<RoadTile> generatedRoadTiles { get; }
        List<Func<IPoint, bool>> conditions { get; set; }
        Point min { get; }
        Point max { get; }


        void setConditions() {
            conditions = new List<Func<IPoint, bool>> {
                point => !generatedRoadTiles.containsPoint(point),
                point => point.x >= min.x && point.x <= max.x,
                point => point.y >= min.y && point.y <= max.y
            };
        }

        public List<RoadTile> generateRoad(
            GameObject prefab, Vector3 startPos, Func<int, bool> finish,
            List<RoadInfo> roadInfos, Dictionary<string, Func<RoadTile, IPoint, bool>> roadPath) {
            var indexInSequence = 0;
            var startPoint = getRandomStartPoint();
            var currentTile = TileFactory.makeRoadTile(prefab, startPos, startPoint);
            generatedRoadTiles.Add(currentTile);
            while (!finish(currentTile.x)) {
                var nextTile = next(currentTile, roadInfos[indexInSequence++], conditions);
                var somePoint = nextTile.fold(
                    //If next tile is none let's reset everything and start from start with different startPoint
                    () => {
                        generatedRoadTiles.Clear();
                        generateRoad(prefab, startPos, finish, roadInfos, roadPath);
                    },
                    point => point);
                var generatedTile = TileFactory.makeRoadTile(prefab, startPos, new Point(somePoint));
                generatedRoadTiles.Add(generatedTile);
                currentTile.setRoadDirection(new Point(generatedTile), roadPath);
                currentTile = generatedTile;
            }
            return generatedRoadTiles;
        }

        Point getRandomStartPoint() {
            var random = new Random();
            var randomY = random.Next(min.y, max.y);
            var startPoint = new Point(min.x, randomY);
            return startPoint;
        }

        static Option<IPoint> next(
            IPoint current, RoadInfo currentInfo,
            IEnumerable<Func<IPoint, bool>> conditions) {
            var leads = currentInfo.values.OrderByDescending(pair => pair.Value);
            foreach (var lead in leads) {
                var nextPoint = current.getNextPoint(lead.Key);
                if (nextPoint.checkPoint(conditions)) return Option.some(nextPoint);
            }
            return Option.none();
        }
    }
}