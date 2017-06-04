using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class RoadTile : MonoBehaviour, IPoint
    {
        public RoadTile(){
        }

        public RoadTile(IPoint point){
            x = point.x;
            y = point.y;
        }

        public Dictionary<string, bool> leadsTo{ get; set; }

        public int x{ get; private set; }
        public int y{ get; private set; }

        public void factorySetup(int x, int y){
            this.x = x;
            this.y = y;

            leadsTo = new Dictionary<string, bool>{
                ["Up"] = false,
                ["Down"] = false,
                ["Left"] = false,
                ["Right"] = false
            };
        }

        public string getLeadKey(){
            return leadsTo.SingleOrDefault(pair => pair.Value).Key;
        }
    }
}