using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public static class Extensions
    {
        public static void setRoadDirection(this RoadTile roadTile, IPoint currentPoint,
            Dictionary<string, Func<RoadTile, IPoint, bool>> predicates){
            foreach (var predicate in predicates)
                roadTile.leadsTo[predicate.Key] = predicate.Value(roadTile, currentPoint);
        }

        public static Dictionary<string, double> calculateValues(this Dictionary<string, int> @this,
            Dictionary<string, double> values){
            var temp = values;
            var sum = (double) @this.Values.Sum();
            foreach (var i in @this)
                if (sum > 0 && i.Value > 0){
                    var value = i.Value / sum;
                    temp[i.Key] = value;
                }
            return temp;
        }

        public static void writeToFile<A>(this IEnumerable<A> source, [NotNull] string path, Func<A, string> func){
            using (var writer = new StreamWriter(path)) {
                var i = 0;
                foreach (var item in source) {
                    writer.Write($"{i++,-3} ");
                    writer.WriteLine(func(item));
                }
            }
        }

        public static void save<A>(this IEnumerable<A> source, [NotNull] string path){
            var bf = new BinaryFormatter();
            var file = File.Create(path);
            bf.Serialize(file, source);
            file.Close();
        }

        public static List<A> load<A>(this List<A> @this, [NotNull] string path){
            if (File.Exists(path)){
                if (@this == null) @this = new List<A>();
                var bf = new BinaryFormatter();
                var file = File.Open(path, FileMode.Open);
                @this = (List<A>) bf.Deserialize(file);
                file.Close();
                return @this;
            }
            throw new ArgumentException("Data file was not found");
        }

        public static void destroy<A>(this IEnumerable<A> @this, Func<A, GameObject> func){
            foreach (var i in @this)
                Object.Destroy(func(i));
        }

        public static bool containsPoint<A>(this IEnumerable<A> @this, IPoint point) where A : IPoint{
            return @this.Any(roadTile => roadTile.x == point.x && roadTile.y == point.y);
        }

        public static bool checkPoint(this IPoint @this, IEnumerable<Func<IPoint, bool>> predicates){
            return predicates.All(predicate => predicate(@this));
        }

        public static IPoint getPointOffset(this IPoint @this, int x, int y) {
            return new Point(@this.x + x, @this.y + y);
        }

        public static Point getNextPoint<A> (this A @this, string direction) where A: IPoint{ 
            switch (direction)
            {
                case "Up":
                    return new Point(@this.x, @this.y + 1);
                case "Down":
                    return new Point(@this.x, @this.y - 1);
                case "Left":
                    return new Point(@this.x - 1, @this.y);
                case "Right":
                    return new Point(@this.x + 1, @this.y);
                default:
                    throw new ArgumentException();
            }
        }
    }
}