using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nmsVoiceAnalysisLibrary
{
    public class Utils
    {
        /** The small deviation allowed in double comparisons. */
        public static double SMALL = 1e-6;


        /**
         * Returns the correlation coefficient of two double vectors.
         *
         * @param y1 double vector 1
         * @param y2 double vector 2
         * @param n the length of two double vectors
         * @return the correlation coefficient
         */
        public static double correlation(double[] y1, double[] y2, int n)
        {

            int i;
            double av1 = 0.0, av2 = 0.0, y11 = 0.0, y22 = 0.0, y12 = 0.0, c;

            if (n <= 1)
            {
                return 1.0;
            }
            for (i = 0; i < n; i++)
            {
                av1 += y1[i];
                av2 += y2[i];
            }
            av1 /= (double)n;
            av2 /= (double)n;
            for (i = 0; i < n; i++)
            {
                y11 += (y1[i] - av1) * (y1[i] - av1);
                y22 += (y2[i] - av2) * (y2[i] - av2);
                y12 += (y1[i] - av1) * (y2[i] - av2);
            }
            if (y11 * y22 == 0.0)
            {
                c = 1.0;
            }
            else
            {
                c = y12 / Math.Sqrt(Math.Abs(y11 * y22));
            }

            return c;
        }


        /**
         * Tests if a is equal to b.
         *
         * @param a a double
         * @param b a double
         */
        public static /*@pure@*/ bool eq(double a, double b)
        {

            return (a - b < SMALL) && (b - a < SMALL);
        }

        /**
         * Computes entropy for an array of integers.
         *
         * @param counts array of counts
         * @return - a log2 a - b log2 b - c log2 c + (a+b+c) log2 (a+b+c)
         * when given array [a b c]
         */
        public static /*@pure@*/ double info(int[] counts)
        {

            int total = 0;
            double x = 0;
            for (int j = 0; j < counts.Length; j++)
            {
                x -= xlogx(counts[j]);
                total += counts[j];
            }
            return x + xlogx(total);
        }

        /**
         * Tests if a is smaller or equal to b.
         *
         * @param a a double
         * @param b a double
         */
        public static /*@pure@*/ bool smOrEq(double a, double b)
        {

            return (a - b < SMALL);
        }

        /**
         * Tests if a is greater or equal to b.
         *
         * @param a a double
         * @param b a double
         */
        public static /*@pure@*/ bool grOrEq(double a, double b)
        {

            return (b - a < SMALL);
        }

        /**
         * Tests if a is smaller than b.
         *
         * @param a a double
         * @param b a double
         */
        public static /*@pure@*/ bool sm(double a, double b)
        {

            return (b - a > SMALL);
        }

        /**
         * Tests if a is greater than b.
         *
         * @param a a double
         * @param b a double 
         */
        public static /*@pure@*/ bool gr(double a, double b)
        {

            return (a - b > SMALL);
        }

        /**
         * Returns the kth-smallest value in the array.
         *
         * @param array the array of integers
         * @param k the value of k
         * @return the kth-smallest value
         */
        public static double kthSmallestValue(int[] array, int k)
        {

            int[] index = new int[array.Length];

            for (int i = 0; i < index.Length; i++)
            {
                index[i] = i;
            }

            return array[index[select(array, index, 0, array.Length - 1, k)]];
        }

        /**
         * Returns the kth-smallest value in the array
         *
         * @param array the array of double
         * @param k the value of k
         * @return the kth-smallest value
         */
        public static double kthSmallestValue(double[] array, int k)
        {

            int[] index = new int[array.Length];

            for (int i = 0; i < index.Length; i++)
            {
                index[i] = i;
            }

            return array[index[select(array, index, 0, array.Length - 1, k)]];
        }


        /**
         * Returns index of maximum element in a given
         * array of doubles. First maximum is returned.
         *
         * @param doubles the array of doubles
         * @return the index of the maximum element
         */
        public static /*@pure@*/ int maxIndex(double[] doubles)
        {

            double maximum = 0;
            int maxIndex = 0;

            for (int i = 0; i < doubles.Length; i++)
            {
                if ((i == 0) || (doubles[i] > maximum))
                {
                    maxIndex = i;
                    maximum = doubles[i];
                }
            }

            return maxIndex;
        }

        /**
         * Returns index of maximum element in a given
         * array of integers. First maximum is returned.
         *
         * @param ints the array of integers
         * @return the index of the maximum element
         */
        public static /*@pure@*/ int maxIndex(int[] ints)
        {

            int maximum = 0;
            int maxIndex = 0;

            for (int i = 0; i < ints.Length; i++)
            {
                if ((i == 0) || (ints[i] > maximum))
                {
                    maxIndex = i;
                    maximum = ints[i];
                }
            }

            return maxIndex;
        }

        /**
         * Computes the mean for an array of doubles.
         *
         * @param vector the array
         * @return the mean
         */
        public static /*@pure@*/ double mean(double[] vector)
        {

            double sum = 0;

            if (vector.Length == 0)
            {
                return 0;
            }
            for (int i = 0; i < vector.Length; i++)
            {
                sum += vector[i];
            }
            return sum / (double)vector.Length;
        }

        /**
         * Returns index of minimum element in a given
         * array of integers. First minimum is returned.
         *
         * @param ints the array of integers
         * @return the index of the minimum element
         */
        public static /*@pure@*/ int minIndex(int[] ints)
        {

            int minimum = 0;
            int minIndex = 0;

            for (int i = 0; i < ints.Length; i++)
            {
                if ((i == 0) || (ints[i] < minimum))
                {
                    minIndex = i;
                    minimum = ints[i];
                }
            }

            return minIndex;
        }

        /**
         * Returns index of minimum element in a given
         * array of doubles. First minimum is returned.
         *
         * @param doubles the array of doubles
         * @return the index of the minimum element
         */
        public static /*@pure@*/ int minIndex(double[] doubles)
        {

            double minimum = 0;
            int minIndex = 0;

            for (int i = 0; i < doubles.Length; i++)
            {
                if ((i == 0) || (doubles[i] < minimum))
                {
                    minIndex = i;
                    minimum = doubles[i];
                }
            }

            return minIndex;
        }

        /**
         * Normalizes the doubles in the array by their sum.
         *
         * @param doubles the array of double
         * @exception IllegalArgumentException if sum is Zero or NaN
         */
        public static void normalize(double[] doubles)
        {

            double sum = 0;
            for (int i = 0; i < doubles.Length; i++)
            {
                sum += doubles[i];
            }
            normalize(doubles, sum);
        }

        /**
         * Normalizes the doubles in the array using the given value.
         *
         * @param doubles the array of double
         * @param sum the value by which the doubles are to be normalized
         * @exception IllegalArgumentException if sum is zero or NaN
         */
        public static void normalize(double[] doubles, double sum)
        {

            if (Double.IsNaN(sum))
            {
                throw new Exception("Can't normalize array. Sum is NaN.");
            }
            if (sum == 0)
            {
                // Maybe this should just be a return.
                throw new Exception("Can't normalize array. Sum is zero.");
            }
            for (int i = 0; i < doubles.Length; i++)
            {
                doubles[i] /= sum;
            }
        }

        /**
         * Sorts a given array of integers in ascending order and returns an 
         * array of integers with the positions of the elements of the original 
         * array in the sorted array. The sort is stable. (Equal elements remain
         * in their original order.)
         *
         * @param array this array is not changed by the method!
         * @return an array of integers with the positions in the sorted
         * array.
         */
        public static /*@pure@*/ int[] sort(int[] array)
        {

            int[] index = new int[array.Length];
            int[] newIndex = new int[array.Length];
            int[] helpIndex;
            int numEqual;

            for (int i = 0; i < index.Length; i++)
            {
                index[i] = i;
            }
            quickSort(array, index, 0, array.Length - 1);

            // Make sort stable
            int i1 = 0;
            while (i1 < index.Length)
            {
                numEqual = 1;
                for (int j = i1 + 1; ((j < index.Length)
                         && (array[index[i1]] == array[index[j]]));
                 j++)
                {
                    numEqual++;
                }
                if (numEqual > 1)
                {
                    helpIndex = new int[numEqual];
                    for (int j = 0; j < numEqual; j++)
                    {
                        helpIndex[j] = i1 + j;
                    }
                    quickSort(index, helpIndex, 0, numEqual - 1);
                    for (int j = 0; j < numEqual; j++)
                    {
                        newIndex[i1 + j] = index[helpIndex[j]];
                    }
                    i1 += numEqual;
                }
                else
                {
                    newIndex[i1] = index[i1];
                    i1++;
                }
            }
            return newIndex;
        }

        /**
         * Sorts a given array of doubles in ascending order and returns an
         * array of integers with the positions of the elements of the
         * original array in the sorted array. NOTE THESE CHANGES: the sort
         * is no longer stable and it doesn't use safe floating-point
         * comparisons anymore. Occurrences of Double.NaN are treated as 
         * Double.MAX_VALUE
         *
         * @param array this array is not changed by the method!
         * @return an array of integers with the positions in the sorted
         * array.  
         */
        public static /*@pure@*/ int[] sort(/*@non_null@*/ double[] array)
        {

            int[] index = new int[array.Length];
            array = (double[])array.Clone();
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = i;
                if (Double.IsNaN(array[i]))
                {
                    array[i] = Double.MaxValue;
                }
            }
            quickSort(array, index, 0, array.Length - 1);
            return index;
        }

        /**
         * Sorts a given array of doubles in ascending order and returns an 
         * array of integers with the positions of the elements of the original 
         * array in the sorted array. The sort is stable (Equal elements remain
         * in their original order.) Occurrences of Double.NaN are treated as 
         * Double.MAX_VALUE
         *
         * @param array this array is not changed by the method!
         * @return an array of integers with the positions in the sorted
         * array.
         */
        public static /*@pure@*/ int[] stableSort(double[] array)
        {

            int[] index = new int[array.Length];
            int[] newIndex = new int[array.Length];
            int[] helpIndex;
            int numEqual;

            array = (double[])array.Clone();
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = i;
                if (Double.IsNaN(array[i]))
                {
                    array[i] = Double.MaxValue;
                }
            }
            quickSort(array, index, 0, array.Length - 1);

            // Make sort stable

            int i2 = 0;
            while (i2 < index.Length)
            {
                numEqual = 1;
                for (int j = i2 + 1; ((j < index.Length) && Utils.eq(array[index[i2]],
                                      array[index[j]])); j++)
                    numEqual++;
                if (numEqual > 1)
                {
                    helpIndex = new int[numEqual];
                    for (int j = 0; j < numEqual; j++)
                        helpIndex[j] = i2 + j;
                    quickSort(index, helpIndex, 0, numEqual - 1);
                    for (int j = 0; j < numEqual; j++)
                        newIndex[i2 + j] = index[helpIndex[j]];
                    i2 += numEqual;
                }
                else
                {
                    newIndex[i2] = index[i2];
                    i2++;
                }
            }

            return newIndex;
        }

        /**
         * Computes the variance for an array of doubles.
         *
         * @param vector the array
         * @return the variance
         */
        public static /*@pure@*/ double variance(double[] vector)
        {

            double sum = 0, sumSquared = 0;

            if (vector.Length <= 1)
            {
                return 0;
            }
            for (int i = 0; i < vector.Length; i++)
            {
                sum += vector[i];
                sumSquared += (vector[i] * vector[i]);
            }
            double result = (sumSquared - (sum * sum / (double)vector.Length)) /
              (double)(vector.Length - 1);

            // We don't like negative variance
            if (result < 0)
            {
                return 0;
            }
            else
            {
                return result;
            }
        }

        /**
         * Computes the sum of the elements of an array of doubles.
         *
         * @param doubles the array of double
         * @return the sum of the elements
         */
        public static /*@pure@*/ double sum(double[] doubles)
        {

            double sum = 0;

            for (int i = 0; i < doubles.Length; i++)
            {
                sum += doubles[i];
            }
            return sum;
        }

        /**
         * Computes the sum of the elements of an array of integers.
         *
         * @param ints the array of integers
         * @return the sum of the elements
         */
        public static /*@pure@*/ int sum(int[] ints)
        {

            int sum = 0;

            for (int i = 0; i < ints.Length; i++)
            {
                sum += ints[i];
            }
            return sum;
        }

        /**
         * Returns c*log2(c) for a given integer value c.
         *
         * @param c an integer value
         * @return c*log2(c) (but is careful to return 0 if c is 0)
         */
        public static /*@pure@*/ double xlogx(int c)
        {

            if (c == 0)
            {
                return 0.0;
            }
            return c * Utils.log2((double)c);
        }
        /**
         * Returns the logarithm of a for base 2.
         * @param a 	a double
         * @return	the logarithm for base 2
         */
        public static /*@pure@*/ double log2(double a)
        {

            return Math.Log(a) / Math.Log(2);
        }

        /**
         * Partitions the instances around a pivot. Used by quicksort and
         * kthSmallestValue.
         *
         * @param array the array of doubles to be sorted
         * @param index the index into the array of doubles
         * @param l the first index of the subset 
         * @param r the last index of the subset 
         *
         * @return the index of the middle element
         */
        private static int partition(double[] array, int[] index, int l, int r)
        {

            double pivot = array[index[(l + r) / 2]];
            int help;

            while (l < r)
            {
                while ((array[index[l]] < pivot) && (l < r))
                {
                    l++;
                }
                while ((array[index[r]] > pivot) && (l < r))
                {
                    r--;
                }
                if (l < r)
                {
                    help = index[l];
                    index[l] = index[r];
                    index[r] = help;
                    l++;
                    r--;
                }
            }
            if ((l == r) && (array[index[r]] > pivot))
            {
                r--;
            }

            return r;
        }

        /**
         * Partitions the instances around a pivot. Used by quicksort and
         * kthSmallestValue.
         *
         * @param array the array of integers to be sorted
         * @param index the index into the array of integers
         * @param l the first index of the subset 
         * @param r the last index of the subset 
         *
         * @return the index of the middle element
         */
        private static int partition(int[] array, int[] index, int l, int r)
        {

            double pivot = array[index[(l + r) / 2]];
            int help;

            while (l < r)
            {
                while ((array[index[l]] < pivot) && (l < r))
                {
                    l++;
                }
                while ((array[index[r]] > pivot) && (l < r))
                {
                    r--;
                }
                if (l < r)
                {
                    help = index[l];
                    index[l] = index[r];
                    index[r] = help;
                    l++;
                    r--;
                }
            }
            if ((l == r) && (array[index[r]] > pivot))
            {
                r--;
            }

            return r;
        }

        /**
         * Implements quicksort according to Manber's "Introduction to
         * Algorithms".
         *
         * @param array the array of doubles to be sorted
         * @param index the index into the array of doubles
         * @param left the first index of the subset to be sorted
         * @param right the last index of the subset to be sorted
         */
        //@ requires 0 <= first && first <= right && right < array.Length;
        //@ requires (\forall int i; 0 <= i && i < index.Length; 0 <= index[i] && index[i] < array.Length);
        //@ requires array != index;
        //  assignable index;
        private static void quickSort(/*@non_null@*/ double[] array, /*@non_null@*/ int[] index,
                                      int left, int right)
        {

            if (left < right)
            {
                int middle = partition(array, index, left, right);
                quickSort(array, index, left, middle);
                quickSort(array, index, middle + 1, right);
            }
        }

        /**
         * Implements quicksort according to Manber's "Introduction to
         * Algorithms".
         *
         * @param array the array of integers to be sorted
         * @param index the index into the array of integers
         * @param left the first index of the subset to be sorted
         * @param right the last index of the subset to be sorted
         */
        //@ requires 0 <= first && first <= right && right < array.Length;
        //@ requires (\forall int i; 0 <= i && i < index.Length; 0 <= index[i] && index[i] < array.Length);
        //@ requires array != index;
        //  assignable index;
        private static void quickSort(/*@non_null@*/ int[] array, /*@non_null@*/  int[] index,
                                      int left, int right)
        {

            if (left < right)
            {
                int middle = partition(array, index, left, right);
                quickSort(array, index, left, middle);
                quickSort(array, index, middle + 1, right);
            }
        }

        /**
         * Implements computation of the kth-smallest element according
         * to Manber's "Introduction to Algorithms".
         *
         * @param array the array of double
         * @param index the index into the array of doubles
         * @param left the first index of the subset 
         * @param right the last index of the subset 
         * @param k the value of k
         *
         * @return the index of the kth-smallest element
         */
        //@ requires 0 <= first && first <= right && right < array.Length;
        private static int select(/*@non_null@*/ double[] array, /*@non_null@*/ int[] index,
                                  int left, int right, int k)
        {

            if (left == right)
            {
                return left;
            }
            else
            {
                int middle = partition(array, index, left, right);
                if ((middle - left + 1) >= k)
                {
                    return select(array, index, left, middle, k);
                }
                else
                {
                    return select(array, index, middle + 1, right, k - (middle - left + 1));
                }
            }
        }

        /**
         * Implements computation of the kth-smallest element according
         * to Manber's "Introduction to Algorithms".
         *
         * @param array the array of integers
         * @param index the index into the array of integers
         * @param left the first index of the subset 
         * @param right the last index of the subset 
         * @param k the value of k
         *
         * @return the index of the kth-smallest element
         */
        //@ requires 0 <= first && first <= right && right < array.Length;
        private static int select(/*@non_null@*/ int[] array, /*@non_null@*/ int[] index,
                                  int left, int right, int k)
        {

            if (left == right)
            {
                return left;
            }
            else
            {
                int middle = partition(array, index, left, right);
                if ((middle - left + 1) >= k)
                {
                    return select(array, index, left, middle, k);
                }
                else
                {
                    return select(array, index, middle + 1, right, k - (middle - left + 1));
                }
            }
        }


    }
}
