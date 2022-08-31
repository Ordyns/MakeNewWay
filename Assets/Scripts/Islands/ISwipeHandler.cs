public interface ISwipeHandler
{
    bool OnSwipe(Direction direction, System.Action onUpdated);
}