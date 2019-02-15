namespace JSONSaver.Types
{
    /// <summary>
    /// An interface which has a single value
    /// </summary>
    /// <typeparam name="T">The type of the key</typeparam>
    public interface ISaveable<T>
    {
        /// <summary>
        /// The key which identifies a part of the collection
        /// </summary>
        T Key { get; set; }
    }
  
}