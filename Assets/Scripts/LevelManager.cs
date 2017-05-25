﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject BackgroundTile;
    [SerializeField] bool drawingRoad = true;

    readonly Dictionary<Point, TileScript> groundTiles = new Dictionary<Point, TileScript>();

    readonly Dictionary<string, Func<RoadTile, IPoint, bool>> roadPath =
        new Dictionary<string, Func<RoadTile, IPoint, bool>> {
            ["Down"]  = ((thisTile, tileBefore) => (thisTile.y == tileBefore.y + 1) && (thisTile.x == tileBefore.x)),
            ["Up"]    = ((thisTile, tileBefore) => (thisTile.y == tileBefore.y - 1) && (thisTile.x == tileBefore.x)),
            ["Left"]  = ((thisTile, tileBefore) => (thisTile.x == tileBefore.x + 1) && (thisTile.y == tileBefore.y)),
            ["Right"] = ((thisTile, tileBefore) => (thisTile.x == tileBefore.x - 1) && (thisTile.y == tileBefore.y)),
        };

    public  int Length, Width;

    [SerializeField] GameObject RoadTile;

    readonly Dictionary<Point, RoadTile> roadTiles = new Dictionary<Point, RoadTile>();
    //    Dictionary<Point, TileScript> towers = new Dictionary<Point, TileScript>();

    void Awake(){
    }

    void Start(){
        setLevel();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("");
            var data = new DataSaveLoad(Width * Length);
            data.updateRoadsData(roadTiles);
        }
    }

    //Point calls to handle click
    public void handleTileClick(Point point){
        if (drawingRoad){
            if (roadTiles.ContainsKey(point)) return;

            if (roadTiles.Any()){
                var roadTileBefore = roadTiles.Last();
                roadTileBefore.Value.setRoadDirection(point, roadPath);
                if (pathIsValid(roadTileBefore.Value)){
                    var roadTile = TileFactory.makeRoadTile(RoadTile, transform.position, point);
                    roadTiles.Add(new Point(roadTile.x, roadTile.y), roadTile);
                }
            }
            else{
                var roadTile = TileFactory.makeRoadTile(RoadTile, transform.position, point);
                roadTiles.Add(new Point(roadTile.x, roadTile.y), roadTile);
            }
            
        }
    }

    void print<A>(IEnumerable<A> source, Action<A> action){
        foreach (var item in source) action(item);
    }

    bool pathIsValid(RoadTile roadTile){
        var counter = 0;
        foreach (var item in roadTile.leadsTo){
            if (item.Value)
                counter++;
        }
        return counter == 1;
    }


    public void setLevel(){
        setBackground();
    }

//    int goToIndex<A>(int index, IEnumerable<A> source, Func<IEnumerable<A>, int> predicate){
//        if (index)
//            return predicate(source);
//    }

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