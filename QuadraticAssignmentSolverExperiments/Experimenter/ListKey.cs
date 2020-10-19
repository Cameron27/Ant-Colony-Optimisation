// CameronSalisbury_1293897

using System.Collections.Generic;

namespace QuadraticAssignmentSolver.Experiments.Experimenter
{
    public class ListKey<T> : List<T>
    {
        public ListKey(IEnumerable<T> collection) : base(collection)
        {
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            for (int index = 0; index < Count; index++)
            {
                T o = this[index];
                hashCode ^= o.GetHashCode() * index;
            }

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ListKey<T>)) return false;
            ListKey<T> listKey1 = this;
            ListKey<T> listKey2 = (ListKey<T>) obj;
            if (listKey1.Count != listKey2.Count) return false;
            for (int i = 0; i < Count; i++)
                if (!listKey1[i].Equals(listKey2[i]))
                    return false;
            return true;
        }
    }

    public static class ListKeyUtils
    {
        public static ListKey<TSource> ToListKey<TSource>(this IEnumerable<TSource> source)
        {
            return new ListKey<TSource>(source);
        }
    }
}