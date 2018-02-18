using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    class Collect
    {
        public int[] list;
        public string Buffering()
        {
            string buf = "";
            foreach (int n in list)
            {
                buf += n.ToString() + " ";
            }
            return buf;
        }
        public string Buffering(IEnumerable<int> arr)
        {
            string buf = "";
            foreach (int n in arr)
            {
                buf += n.ToString() + " ";
            }
            return buf;
        }
        public Collect(int size)
        {
            list = new int[size];
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                list[i] = rand.Next(size);
            }
        }
        private string SortH()
        {
            IEnumerable<int> result = list.OrderBy(n => n);
            return Buffering(result);
        }
        private string SortL()
        {
            IEnumerable<int> result = list.OrderByDescending(n => n);
            return Buffering(result);
        }
        delegate string Comparator();
        public string Sort(bool tr)
        {
            Comparator getSort;
            if (tr) getSort = SortH;
            else getSort = SortL;
            return getSort();
        }
    }
}
