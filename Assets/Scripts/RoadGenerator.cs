using System;
using System.Collections.Generic;
using System.Linq;
using OptionLib;
using UnityEngine;

namespace Assets.Scripts
{
    public class RoadGenerator
    {
        public RoadGenerator(Point min, Point max, List<Func<IPoint, bool>> conditions) {
            this.min = min;
            this.max = max;
            this.conditions = conditions;

            generatedRoadTiles = new List<RoadTile>();
        }

        public List<RoadTile> generatedRoadTiles { get; }

        readonly List<Func<IPoint, bool>> conditions;
        Point min { get; }
        Point max { get; }


        public List<RoadTile> generate(
            GameObject prefab, Vector3 startPos, Func<IPoint, bool> finish,
            List<RoadInfo> roadInfos, Dictionary<string, Func<IPoint, IPoint, bool>> roadPath) {
            var indexInSequence = 0;
            var startPoint = Point.randomPoint(new Point(0,0), new Point(min.y,max.y));
            var currentTile = TileFactory.makeRoadTile(prefab, startPos, startPoint);
            generatedRoadTiles.Add(currentTile);

            while (!finish(currentTile)) {
                var nextTile = next(currentTile, roadInfos[indexInSequence++], conditions);
                nextTile.fold(
                    onNone: () => {
                        generatedRoadTiles.Clear();
                        generate(prefab, startPos, finish, roadInfos, roadPath);
                    }, onSome: point => {
                        var generatedTile = TileFactory.makeRoadTile(prefab, startPos, point);
                        generatedRoadTiles.Add(generatedTile);
                        currentTile.setRoadDirection(point, roadPath);
                        currentTile = generatedTile;
                    });
            }
            return generatedRoadTiles;
        }

        
        //Pure
        static Option<Point> next(
            IPoint current, RoadInfo currentInfo,
            IEnumerable<Func<IPoint, bool>> conditions) {
            var leads = currentInfo.counts.calculateValues().OrderByDescending(pair => pair.Value);
            foreach (var lead in leads) {
                var nextPoint = current.getNextPoint(lead.Key);
                if (nextPoint.checkPoint(conditions)) return nextPoint;
            }
            return Option.None;
        }
     }
}