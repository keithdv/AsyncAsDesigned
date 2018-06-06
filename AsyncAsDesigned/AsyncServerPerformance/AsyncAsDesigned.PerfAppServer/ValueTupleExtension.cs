

namespace System
{
    public static class ValueTupleExtension
    {


        public static bool Contains<T, T1, T2>(this ValueTuple<T1, T2> tuple, T item) where T : IComparable where T1 : IComparable where T2 : IComparable        {

            if (tuple.Item1.CompareTo(item) == 0 || tuple.Item2.CompareTo(item) == 0)
            {
                return true;
            }

            return false;
        }


        public static bool Contains<T, T1, T2, T3>(this ValueTuple<T1, T2, T3> tuple, T item) where T : IComparable where T1 : IComparable where T2 : IComparable where T3 : IComparable        {

            if (tuple.Item1.CompareTo(item) == 0 || tuple.Item2.CompareTo(item) == 0 || tuple.Item3.CompareTo(item) == 0)
            {
                return true;
            }

            return false;
        }


        public static bool Contains<T, T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> tuple, T item) where T : IComparable where T1 : IComparable where T2 : IComparable where T3 : IComparable where T4 : IComparable        {

            if (tuple.Item1.CompareTo(item) == 0 || tuple.Item2.CompareTo(item) == 0 || tuple.Item3.CompareTo(item) == 0 || tuple.Item4.CompareTo(item) == 0)
            {
                return true;
            }

            return false;
        }


        public static bool Contains<T, T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> tuple, T item) where T : IComparable where T1 : IComparable where T2 : IComparable where T3 : IComparable where T4 : IComparable where T5 : IComparable        {

            if (tuple.Item1.CompareTo(item) == 0 || tuple.Item2.CompareTo(item) == 0 || tuple.Item3.CompareTo(item) == 0 || tuple.Item4.CompareTo(item) == 0 || tuple.Item5.CompareTo(item) == 0)
            {
                return true;
            }

            return false;
        }


        public static bool Contains<T, T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> tuple, T item) where T : IComparable where T1 : IComparable where T2 : IComparable where T3 : IComparable where T4 : IComparable where T5 : IComparable where T6 : IComparable        {

            if (tuple.Item1.CompareTo(item) == 0 || tuple.Item2.CompareTo(item) == 0 || tuple.Item3.CompareTo(item) == 0 || tuple.Item4.CompareTo(item) == 0 || tuple.Item5.CompareTo(item) == 0 || tuple.Item6.CompareTo(item) == 0)
            {
                return true;
            }

            return false;
        }


        public static bool Contains<T, T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> tuple, T item) where T : IComparable where T1 : IComparable where T2 : IComparable where T3 : IComparable where T4 : IComparable where T5 : IComparable where T6 : IComparable where T7 : IComparable        {

            if (tuple.Item1.CompareTo(item) == 0 || tuple.Item2.CompareTo(item) == 0 || tuple.Item3.CompareTo(item) == 0 || tuple.Item4.CompareTo(item) == 0 || tuple.Item5.CompareTo(item) == 0 || tuple.Item6.CompareTo(item) == 0 || tuple.Item7.CompareTo(item) == 0)
            {
                return true;
            }

            return false;
        }


        public static bool Contains<T, T1, T2, T3, T4, T5, T6, T7, TRest>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple, T item) where T : IComparable where T1 : IComparable where T2 : IComparable where T3 : IComparable where T4 : IComparable where T5 : IComparable where T6 : IComparable where T7 : IComparable where TRest : struct
        {

            if (tuple.Item1.CompareTo(item) == 0 || tuple.Item2.CompareTo(item) == 0 || tuple.Item3.CompareTo(item) == 0 || tuple.Item4.CompareTo(item) == 0 || tuple.Item5.CompareTo(item) == 0 || tuple.Item6.CompareTo(item) == 0 || tuple.Item7.CompareTo(item) == 0)
            {
                return true;
            }

            return false;
        }

    }
}
