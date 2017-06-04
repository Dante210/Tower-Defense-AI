using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{

    public class TileScript : MonoBehaviour
    {
        LevelManager levelManager;
        public Point point;


        void Awake() { levelManager = FindObjectOfType<LevelManager>(); }

        void OnMouseDown() { levelManager.handleTileClick(point); }
    }

    public class TileFactory : MonoBehaviour
    {
        public static TileScript makeTile(GameObject prefab, Vector3 startingPos, int i, int j) {
            if (prefab == null) throw new ArgumentException("Point was not created!");
            var tileScript = make(prefab, startingPos, i, j).GetComponent<TileScript>();
            tileScript.point = new Point(i, j);
            return tileScript;
        }

        public static RoadTile makeRoadTile(GameObject prefab, Vector3 startingPos, IPoint point) {
            if (prefab == null) throw new ArgumentException("Point was not created!");
            var roadTile = make(prefab, startingPos, point.x, point.y).GetComponent<RoadTile>();
            roadTile.factorySetup(point.x, point.y);
            return roadTile;
        }

        public static TowerTile makeTowerTile(
            GameObject prefab, Vector3 startingPos, IPoint point, List<RoadTile> roadTiles, int towerRange,
            ICollection<Func<IPoint, bool>> conditions) {
            if (prefab == null) throw new ArgumentException("Point was not created!");
            var towerTile = make(prefab, startingPos, point.x, point.y).GetComponent<TowerTile>();
            towerTile.factorySetup(roadTiles, point, conditions, towerRange);
            return towerTile;
        }


        static GameObject make(GameObject prefab, Vector3 startingPos, int i, int j) {
            return Instantiate(
                prefab,
                new Vector3(startingPos.x + i, startingPos.y + j, 0), Quaternion.identity);
        }
    }
}