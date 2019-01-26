namespace JSONSaver.Types
{
    public interface ISaveable<T>
    {
        T Key { get; set; }
    }
}