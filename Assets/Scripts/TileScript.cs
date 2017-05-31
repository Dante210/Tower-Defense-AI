using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class TileScript : MonoBehaviour
    {
        public Point point;
        LevelManager levelManager;


        void Awake(){
            levelManager = FindObjectOfType<LevelManager>();
        }

        void OnMouseDown(){
            levelManager.handleTileClick(point);
        }
    }

    public class TileFactory : MonoBehaviour
    {
        public static TileScript makeTile(GameObject prefab, Vector3 startingPos, int i, int j){
            if (prefab == null) throw new ArgumentException("Point was not created!");
            var tileScript = make(prefab, startingPos, i, j).GetComponent<TileScript>();
            tileScript.point = new Point(i, j);
            return tileScript;
        }

        public static RoadTile makeRoadTile(GameObject prefab, Vector3 startingPos, IPoint point){
            if (prefab == null) throw new ArgumentException("Point was not created!");
            var roadTile = make(prefab, startingPos, point.x, point.y).GetComponent<RoadTile>();
            roadTile.factorySetup(point.x, point.y);
            return roadTile;
        }

//        public static RoadTile makeTowerTile(GameObject prefab, Vector3 startingPos, IPoint point)
//        {
//            if (prefab == null) throw new ArgumentException("Point was not created!");
//            var towerTile = make(prefab, startingPos, point.x, point.y).GetComponent<TowerTile>();
//            towerTile.factorySetup(point.x, point.y);
//            return roadTile;
//        }

        static GameObject make(GameObject prefab, Vector3 startingPos, int i, int j){
            return Instantiate(prefab,
                new Vector3(startingPos.x + i, startingPos.y + j, 0), Quaternion.identity);
        }
    }
}