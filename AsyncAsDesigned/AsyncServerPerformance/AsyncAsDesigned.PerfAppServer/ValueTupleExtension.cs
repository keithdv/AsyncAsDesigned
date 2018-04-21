using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncAsDesigned.PerfAppServer
{
    public static class ValueTupleExtension
    {
        public static bool Contains<T>(this ValueTuple<T, T> tuple, T item) where T:IComparable
        {
            if(tuple.Item1.CompareTo(item) == 0 || tuple.Item2.CompareTo(item) == 0)
            {
                return true;
            }
            return false;
        }
    }
}
