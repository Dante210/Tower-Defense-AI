using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    class TowerTile : IPoint
    {

        public IPoint closestRoadTile { get; private set; }
        public int offset { get; private set; }
        public int roadInRange { get; private set; }

        public int x { get; private set; }
        public int y { get; private set; }

        public void factorySetup(List<RoadTile> roadTiles, IPoint point, ICollection<Func<IPoint, bool>> conditions, int towerRange) {
            closestRoadTile = getClosestTile(roadTiles, conditions);
            offset = closestRoadTile.y - point.y;
            roadInRange = roadInRange;
            x = point.x;
            y = point.y;
        }

        IPoint getClosestTile<A>(List<A> tiles, ICollection<Func<IPoint, bool>> conditions)
            where A : IPoint {
            //Go up
            var current = new Point(x, y);
            current = (Point)current.getNextPoint("Up");
            while (current.checkPoint(conditions))
            {
                if (tiles.containsPoint(current))
                    return current;
                current = (Point)current.getNextPoint("Up");
            }
            //Go Down
            current = new Point(x, y);
            current = (Point) current.getNextPoint("Down");
            while (current.checkPoint(conditions)) {
                if (tiles.containsPoint(current))
                    return current;
                current = (Point) current.getNextPoint("Down");
            }
            throw new ArgumentException();
        }

        int getRoadCountInRange(IPoint point, int range) {
            
        }
    }
}