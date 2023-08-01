using System;
using System.Collections.Generic;

namespace OptiCollections
{
    public class Heap<T> where T : IHeapComparable<T>
    {
        private readonly List<T> items = new();

        public int Count => items.Count;

        public void Clear()
        {
            items.Clear();
        }
        
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void UpdateItemPlace(T item)
        {
            SortUp(item);
            SortDown(item);
        }

        public void Add(T item)
        {
            if (items.Contains(item)) return;

            items.Add(item);
            SortUp(item);
        }

        public T GetAndRemoveFirstItem()
        {
            T firstItem = items[0];
            T lastItem = items[^1];
            
            items.Remove(firstItem);
            items.Remove(lastItem);
            
            items.Insert(0,lastItem);
            SortDown(lastItem);
            
            return firstItem;
        }

        private void SortDown(T item)
        {
            while(true)
            {
                int itemIndex = items.LastIndexOf(item);
                int childIndexLeft = itemIndex * 2 + 1;
                int childIndexRight = itemIndex * 2 + 2;

                if (childIndexLeft < items.Count)
                {
                    T swapItem = items[childIndexLeft];

                    if (childIndexRight < items.Count && swapItem.GetPriorityDifferenceWith(items[childIndexRight]) < 0)
                    {
                        swapItem = items[childIndexRight];
                    }

                    if (item.GetPriorityDifferenceWith(swapItem) < 0)
                    {
                        Swap(item, swapItem);
                    }
                    else
                    {
                        return;
                    }
                }
                else return;
            }
        }
        
        private void SortUp(T item)
        {
            if(!items.Contains(item)) return;
            bool noParentFound = false;

            do
            {
                int parentIndex = (items.LastIndexOf(item) - 1) / 2;
                T parentItem = items[parentIndex];
                
                if (item.GetPriorityDifferenceWith(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    noParentFound = true;
                }

            } while (!noParentFound);
        }

        private void Swap(T itemA, T itemB)
        {
            int indexOfA = items.LastIndexOf(itemA);
            int indexOfB = items.LastIndexOf(itemB);
            items[indexOfA] = itemB;
            items[indexOfB] = itemA;
        }
    }
}