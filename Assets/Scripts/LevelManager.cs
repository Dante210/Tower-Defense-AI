using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    static Point min;
    static Point max;
    static List<TowerTile> towerTiles = new List<TowerTile>();
    static List<RoadTile> roadTiles = new List<RoadTile>();
    readonly Dictionary<Point, TileScript> groundTiles = new Dictionary<Point, TileScript>();

    readonly Dictionary<string, Func<IPoint, IPoint, bool>> roadPath =
        new Dictionary<string, Func<IPoint, IPoint, bool>>
        {
            ["Down"] = (thisTile, tileBefore) => thisTile.y == tileBefore.y + 1 && thisTile.x == tileBefore.x,
            ["Up"] = (thisTile, tileBefore) => thisTile.y == tileBefore.y - 1 && thisTile.x == tileBefore.x,
            ["Left"] = (thisTile, tileBefore) => thisTile.x == tileBefore.x + 1 && thisTile.y == tileBefore.y,
            ["Right"] = (thisTile, tileBefore) => thisTile.x == tileBefore.x - 1 && thisTile.y == tileBefore.y
        };

    readonly List<Func<IPoint, bool>> towerConditions = new List<Func<IPoint, bool>>
    {
        point => !towerTiles.containsPoint(point),
        point => point.x >= min.x && point.x <= max.x,
        point => point.y >= min.y && point.y <= max.y
    };

    readonly List<Func<IPoint, bool>> towerGenerateConditions = new List<Func<IPoint, bool>>
    {
        point => !towerTiles.containsPoint(point),
        point => !roadTiles.containsPoint(point),
        point => point.x >= min.x && point.x <= max.x,
        point => point.y >= min.y && point.y <= max.y
    };

    [SerializeField] GameObject BackgroundTile;

    int currentTowerCount;
    [SerializeField] bool drawingRoad = true;
    public int Length, Width;

    [SerializeField] GameObject RoadTile;
    public int TowerCount, TowerRange;
    [SerializeField] GameObject TowerTile;


    void Awake() {
        min = new Point(0, 0);
        max = new Point(Length - 1, Width - 1);
    }

    void Start() { setLevel(); }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            drawingRoad = true;
            var data = new RoadData(Length * Width);
            if (roadTiles.Any())
                saveAndClearRoad(data);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            drawingRoad = false;

            roadTiles.destroy(tile => tile.gameObject);
            roadTiles.Clear();

            var data = new RoadData(Length * Width);
            var roadGenerator = new RoadGenerator(min, max, towerGenerateConditions);
            roadTiles = roadGenerator.generate(
                RoadTile, transform.position, point => point.x == max.x, data.info, roadPath);
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            var data = new TowerData(Length * Width);
            saveAndClearTowers(data);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            towerTiles.destroy(tile => tile.gameObject);
            towerTiles.Clear();

            var data = new TowerData(Length * Width);
            var towerGenerator = new TowerGenerator(3);
            towerTiles = towerGenerator.generate(
                TowerTile, transform.position, data.info, roadTiles, towerConditions, TowerRange);
        }
    }

    void saveAndClearRoad(RoadData data) {
        data.updateRoadsData(roadTiles);
        roadTiles.destroy(tile => tile.gameObject);
        roadTiles.Clear();
    }

    void saveAndClearTowers(TowerData data) {
        data.updateData(towerTiles);
        towerTiles.destroy(tile => tile.gameObject);
        towerTiles.Clear();
    }

    //Point calls to handle click
    public void handleTileClick(IPoint point) {
        if (roadTiles.containsPoint(point) || towerTiles.containsPoint(point)) return;

        if (drawingRoad)
        {
            if (roadTiles.Any())
            {
                var roadTileBefore = roadTiles.Last();
                roadTileBefore.setRoadDirection(point, roadPath);
                if (pathIsValid(roadTileBefore))
                {
                    var roadTile = TileFactory.makeRoadTile(RoadTile, transform.position, point);
                    roadTiles.Add(roadTile);
                }
            }
            else
            {
                var roadTile = TileFactory.makeRoadTile(RoadTile, transform.position, point);
                roadTiles.Add(roadTile);
            }
        }
        else
        {
            var towerTile = TileFactory.makeTowerTile(
                TowerTile, transform.position, point, roadTiles, TowerRange, towerConditions);
            towerTiles.Add(towerTile);
        }
    }


    void print<A>(IEnumerable<A> source, Action<A> action) {
        foreach (var item in source) action(item);
    }

    bool pathIsValid(RoadTile roadTile) {
        var counter = 0;
        foreach (var item in roadTile.leadsTo)
            if (item.Value)
                counter++;
        return counter == 1;
    }


    public void setLevel() { setBackground(); }


    void setBackground() {
        for (var i = 0; i < Length; i++)
        for (var j = 0; j < Width; j++)
        {
            var newTile = TileFactory.makeTile(BackgroundTile, transform.position, i, j);
            var temp = new Point(i, j);
            groundTiles.Add(temp, newTile);
            groundTiles[temp].gameObject.transform.SetParent(transform);
        }
    }

    public int getMaxSize() { return Length * Width; }
}