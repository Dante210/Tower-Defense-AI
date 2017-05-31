using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int Length, Width;
    public int TowerCount;
    Point min, max;
    [SerializeField] GameObject BackgroundTile;
    [SerializeField] bool drawingRoad = true;

    readonly Dictionary<Point, TileScript> groundTiles = new Dictionary<Point, TileScript>();

    readonly Dictionary<string, Func<RoadTile, IPoint, bool>> roadPath =
        new Dictionary<string, Func<RoadTile, IPoint, bool>>{
            ["Down"] = (thisTile, tileBefore) => thisTile.y == tileBefore.y + 1 && thisTile.x == tileBefore.x,
            ["Up"] = (thisTile, tileBefore) => thisTile.y == tileBefore.y - 1 && thisTile.x == tileBefore.x,
            ["Left"] = (thisTile, tileBefore) => thisTile.x == tileBefore.x + 1 && thisTile.y == tileBefore.y,
            ["Right"] = (thisTile, tileBefore) => thisTile.x == tileBefore.x - 1 && thisTile.y == tileBefore.y
        };

    [SerializeField] GameObject RoadTile;
    int currentTowerCount;
    List<RoadTile> roadTiles = new List<RoadTile>();
    List<TowerTile> towerTiles = new List<TowerTile>();

    void Awake(){
        min = new Point(0, 0);
        max = new Point(Length-1, Width-1);
    }

    void Start(){
        setLevel();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space))
        {
            drawingRoad = true;
            var data = new DataSaveLoad(Length * Width);
            if(roadTiles.Any())
                saveAndClearRoad(data);
        }
        if (Input.GetKeyDown(KeyCode.Return)){
            drawingRoad = false;

            var data = new DataSaveLoad(Length * Width);
            roadTiles.destroy(tile => tile.gameObject);
            roadTiles.Clear();

            var roadGenerator = new RoadGenerator(min, max);
            roadTiles = roadGenerator.generateRoad(RoadTile, transform.position, x => x == max.x,
                data.roadsInfo, roadPath);
        }
    }

    void saveAndClearRoad(DataSaveLoad data){
        data.updateRoadsData(roadTiles);
        roadTiles.destroy(tile => tile.gameObject);
        roadTiles.Clear();
    }

    //Point calls to handle click
    public void handleTileClick(IPoint point){
        if (roadTiles.containsPoint(point) || towerTiles.containsPoint(point)) return;

        if (drawingRoad) {
            if (roadTiles.Any()) {
                var roadTileBefore = roadTiles.Last();
                roadTileBefore.setRoadDirection(point, roadPath);
                if (pathIsValid(roadTileBefore)) {
                    var roadTile = TileFactory.makeRoadTile(RoadTile, transform.position, point);
                    roadTiles.Add(roadTile);
                }
            }
            else {
                var roadTile = TileFactory.makeRoadTile(RoadTile, transform.position, point);
                roadTiles.Add(roadTile);
            }
        }
        else {
            var towerTile = TileFactory.
            towerTiles.Add();
        }
    }

    void print<A>(IEnumerable<A> source, Action<A> action){
        foreach (var item in source) action(item);
    }

    bool pathIsValid(RoadTile roadTile){
        var counter = 0;
        foreach (var item in roadTile.leadsTo)
            if (item.Value)
                counter++;
        return counter == 1;
    }


    public void setLevel(){
        setBackground();
    }


    void setBackground(){
        for (var i = 0; i < Length; i++)
        for (var j = 0; j < Width; j++){
            var newTile = TileFactory.makeTile(BackgroundTile, transform.position, i, j);
            var temp = new Point(i, j);
            groundTiles.Add(temp, newTile);
            groundTiles[temp].gameObject.transform.SetParent(transform);
        }
    }

    public int getMaxSize(){
        return Length * Width;
    }
}