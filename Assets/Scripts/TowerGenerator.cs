﻿using System;
using System.Collections.Generic;
using System.Linq;
using OptionLib;
using UnityEngine;

namespace Assets.Scripts
{
    class TowerGenerator
    {
        public TowerGenerator(int towersToGenerate) { this.towersToGenerate = towersToGenerate; }

        public TowerGenerator() { }

        int towersToGenerate { get; }
        public List<TowerTile> generatedTowerTiles { get; }

        public List<TowerTile> generate(
            GameObject prefab, Vector3 startPos,
            List<TowerInfo> towerInfos, List<RoadTile> roadTiles,
            ICollection<Func<IPoint, bool>> conditions, int towerRange) {

            var generatedTiles = new List<TowerTile>();
            var currentTowerCount = 0;
            var roadTileIndex = 0;

            while (currentTowerCount < towersToGenerate && roadTileIndex < roadTiles.Count) {
                var currentRoadTile = roadTiles[roadTileIndex];

                var currentOpt = currentRoadTile.getNextPoint("Up");
                while (currentOpt.checkPoint(conditions))
                {
                    var current = currentOpt.fold(
                        () => { throw new Exception("Tower cannot be generated"); }, point => point);

                    var offset = current.y - currentRoadTile.y;
                    if (getProbability(towerInfos[roadTileIndex], offset, towerInfos) >= 0.4) {

                        Debug.Log(getProbability(towerInfos[roadTileIndex], offset, towerInfos));

                        var generatedTile = TileFactory.makeTowerTile(
                            prefab, startPos, current, roadTiles, towerRange, conditions);
                        generatedTiles.Add(generatedTile);
                        currentTowerCount++;
                    }
                    currentOpt = current.getNextPoint("Up");
                }
 
                currentOpt = currentRoadTile.getNextPoint("Down");
                while (currentOpt.checkPoint(conditions))
                {
                    var current = currentOpt.fold(
                        () => { throw new Exception("Tower cannot be generated"); }, point => point);

                    var offset = current.y - currentRoadTile.y;
                    if (getProbability(towerInfos[roadTileIndex], offset, towerInfos) >= 0.4)
                    {

                        Debug.Log(getProbability(towerInfos[roadTileIndex], offset, towerInfos));

                        var generatedTile = TileFactory.makeTowerTile(
                            prefab, startPos, current, roadTiles, towerRange, conditions);
                        generatedTiles.Add(generatedTile);
                        currentTowerCount++;
                    }
                    currentOpt = current.getNextPoint("Down");
                }
                roadTileIndex++;
            }
            
            return generatedTiles;
        }

        double getProbability(TowerInfo towerInfo, int offsetIndex, List<TowerInfo> towerInfos) {
            double roadsCountSum = towerInfos.Sum(info => info.roadTileCount);
            var P_A = towerInfo.roadTileCount / roadsCountSum;
            double offsetCountSum = towerInfos.Sum(info => info.offset.Single(pair => pair.Key == offsetIndex).Value);
            var P_B = towerInfo.offset[offsetIndex] / offsetCountSum;
            double P_B_A_Sum = towerInfo.offset.Sum(pair => pair.Value);
            var P_B_A = towerInfo.offset[offsetIndex] / P_B_A_Sum;
            return P_B_A * P_A / P_B;
        }
    }
}