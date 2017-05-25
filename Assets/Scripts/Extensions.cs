using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Extensions
    {
        public static Color? getColor(this GameObject @this){
            var reference = @this.GetComponent<SpriteRenderer>();
            if (reference != null)
                return reference.color;
            return null;
        }

        public static void setRoadDirection(this RoadTile roadTile, IPoint currentPoint,
            Dictionary<string, Func<RoadTile, IPoint, bool>> predicates){
            foreach (var predicate in predicates)
                roadTile.leadsTo[predicate.Key] = predicate.Value(roadTile, currentPoint);
        }

        public static IEnumerable<KeyValuePair<string, double>> calculateValues(this Dictionary<string, int> @this,
            Func<int, double> predicate){
            var sum = (double) @this.Values.Sum();
            foreach (var i in @this)
                if (sum > 0 && i.Value > 0){
                    var value = sum / i.Value;
                    yield return new KeyValuePair<string, double>(i.Key, value);
                }
        }

        public static void writeToFile<A>(this IEnumerable<A> source, [NotNull] string path, Func<A, string> func){
            using (var writer = new StreamWriter(path)){
                foreach (var item in source)
                    writer.WriteLine(func(item));
            }
        }

        public static void save<A>(this IEnumerable<A> source, [NotNull] string path){
            var bf = new BinaryFormatter();
            var file = File.Create(path);
            bf.Serialize(file, source);
            file.Close();
        }

        public static void load<A>(this List<A> @this, [NotNull] string path){
            if (File.Exists(path)){
                if (@this == null) @this = new List<A>();
                var bf = new BinaryFormatter();
                var file = File.Open(path, FileMode.Open);
                @this = (List<A>) bf.Deserialize(file);
                file.Close();
            }
            else{
                throw new ArgumentException("Data file was not found");
            }
        }
    }
}