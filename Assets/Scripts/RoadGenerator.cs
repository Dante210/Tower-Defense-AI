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
            var random = new Random();
            var randomY = random.Next(min.y, max.y);
            startPoint = new Point(min.x, randomY);
            generatedRoadTiles = new List<RoadTile>();
            setConditions();
        }

        public Point startPoint { get; }
        public List<RoadTile> generatedRoadTiles { get; }
        List<Func<IPoint, bool>> conditions { get; set; }
        Point min { get; }
        Point max { get; }

        Func<string, IPoint, IPoint> getNextPoint => (direction, point) => {
            switch (direction) {
                case "Up":
                    return new Point(point.x, point.y + 1);
                case "Down":
                    return new Point(point.x, point.y - 1);
                case "Left":
                    return new Point(point.x - 1, point.y);
                case "Right":
                    return new Point(point.x + 1, point.y);
                default:
                    return point;
            }
        };

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
            var roadTile = TileFactory.makeRoadTile(prefab, startPos, startPoint);
            generatedRoadTiles.Add(roadTile);
            var currentTile = roadTile;
            while (!finish(currentTile.x)) {
                var nextTile = next(currentTile, roadInfos[indexInSequence++], getNextPoint, conditions);
                var generatedTile = TileFactory.makeRoadTile(prefab, startPos, new Point(nextTile.x, nextTile.y));
                generatedRoadTiles.Add(generatedTile);
                currentTile.setRoadDirection(new Point(generatedTile.x, generatedTile.y), roadPath);
                currentTile = generatedTile;
            }
            return generatedRoadTiles;
        }

        static IPoint next(
            IPoint current, RoadInfo currentInfo, Func<string, IPoint, IPoint> getNextPoint,
            IEnumerable<Func<IPoint, bool>> conditions) {
            var leads = currentInfo.values.OrderByDescending(pair => pair.Value);
            foreach (var lead in leads) {
                var nextPoint = getNextPoint(lead.Key, current);
                if (nextPoint.checkPoint(conditions)) return nextPoint;
            }
            throw new Exception("Couldn't create next road tile");
        }
    }
}