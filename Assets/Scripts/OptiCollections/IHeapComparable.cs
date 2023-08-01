namespace OptiCollections
{
    public interface IHeapComparable<in T>
    {
        /// <summary>
        /// more than 0 difference means priority of this item is higher. Less than 0 means that priority of the other item is higher 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float GetPriorityDifferenceWith(T other);
    }
}