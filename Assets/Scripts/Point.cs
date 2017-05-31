namespace Assets.Scripts
{
    public interface IPoint
    {
        int x { get; }
        int y { get; }


    }


    public class Point : IPoint
    {
        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public Point(IPoint point) {
            x = point.x;
            y = point.y;
        }

        public int x { get; }
        public int y { get; }
    }
}