using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngineV4.Data.Collections
{
    public class Shufflebag<T> : List<T>
    {
        public List<T> CurrentItems { get; private set; }
        private Random _random = new Random();

        public Shufflebag()
        {
            CurrentItems = new List<T>();
        }

        public T GetNextItem()
        {
            if (CurrentItems.Count == 0)
            {
                Reset();
            }

            T item = CurrentItems[_random.Next(0, CurrentItems.Count)];
            CurrentItems.Remove(item);
            return item;
        }

        public void Reset()
        {
            CurrentItems = this.ToList();
        }
    }
}
