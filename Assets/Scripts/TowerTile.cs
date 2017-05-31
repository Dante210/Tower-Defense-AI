﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TowerTile : MonoBehaviour, IPoint
    {
        public IPoint closestRoadTile { get; private set; }
        public int closestRoadTileNum { get; private set; }
        public int offset { get; private set; }
        public int roadInRange { get; private set; }

        public int x { get; private set; }
        public int y { get; private set; }

        public void factorySetup(
            List<RoadTile> roadTiles, IPoint point, ICollection<Func<IPoint, bool>> conditions, int towerRange) {
            x = point.x;
            y = point.y;
            closestRoadTile = getClosestTile(roadTiles, conditions);
            offset = closestRoadTile.y - point.y;
            roadInRange = getRoadCountInRange(roadTiles, point, towerRange);
            closestRoadTileNum = getNumInSequence(roadTiles, closestRoadTile);
        }

        IPoint getClosestTile<A>(List<A> tiles, ICollection<Func<IPoint, bool>> conditions)
            where A : IPoint {
            //Go up
            var current = new Point(x, y);
            current = current.getNextPoint("Up");
            while (current.checkPoint(conditions)) {
                if (tiles.containsPoint(current))
                    return current;
                current = current.getNextPoint("Up");
            }
            //Go Down
            current = new Point(x, y);
            current = current.getNextPoint("Down");
            while (current.checkPoint(conditions)) {
                if (tiles.containsPoint(current))
                    return current;
                current = current.getNextPoint("Down");
            }
            throw new ArgumentException();
        }

        int getRoadCountInRange<A>(List<A> tiles, IPoint point, int range) where A : IPoint {
            var count = 0;
            for (var i = 1; i < range; i++) {
                if (tiles.containsPoint(point.getPointOffset(-i, i)))
                    count++;
                if (tiles.containsPoint(point.getPointOffset(i, -i)))
                    count++;
                if (tiles.containsPoint(point.getPointOffset(i, i)))
                    count++;
                if (tiles.containsPoint(point.getPointOffset(-i, -i)))
                    count++;

                if (tiles.containsPoint(point.getPointOffset(0, i)))
                    count++;
                if (tiles.containsPoint(point.getPointOffset(0, -i)))
                    count++;
                if (tiles.containsPoint(point.getPointOffset(i, 0)))
                    count++;
                if (tiles.containsPoint(point.getPointOffset(-i, 0)))
                    count++;
            }
            return count;
        }

        int getNumInSequence<A>(List<A> tiles, IPoint point) where A : IPoint {
            for (var i = 0; i < tiles.Count; i++)
                if (tiles[i].x == point.x && tiles[i].y == point.y)
                    return i;
            return -1;
        }
    }
}