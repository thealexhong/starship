using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nmsVoiceAnalysisLibrary
{
    public class LUDecomposition
    {

        /* ------------------------
            Class variables
        * ------------------------ */

        /** Array for internal storage of decomposition.
        @serial internal array storage.
        */
        private double[,] LU;

        /** Row and column dimensions, and pivot sign.
        @serial column dimension.
        @serial row dimension.
        @serial pivot sign.
        */
        private int m, n, pivsign;

        /** Internal storage of pivot vector.
        @serial pivot vector.
        */
        private int[] piv;

        /* ------------------------
           Constructor
         * ------------------------ */

        /** LU Decomposition
        @param  A   Rectangular matrix
        @return     Structure to access L, U and piv.
        */

        public LUDecomposition(CoreMatrix A)
        {

            // Use a "left-looking", dot-product, Crout/Doolittle algorithm.

            LU = A.getArrayCopy();
            m = A.getRowDimension();
            n = A.getColumnDimension();
            piv = new int[m];
            for (int i = 0; i < m; i++)
            {
                piv[i] = i;
            }
            pivsign = 1;
            double[] LUrowi = new double[m];
            double[] LUcolj = new double[m];

            // Outer loop.

            for (int j = 0; j < n; j++)
            {

                // Make a copy of the j-th column to localize references.

                for (int i = 0; i < m; i++)
                {
                    LUcolj[i] = LU[i, j];
                }

                // Apply previous transformations.

                for (int i = 0; i < m; i++)
                {
                    for (int h = 0; i < m; i++)
                        LUrowi[h] = LU[i, h];

                    // Most of the time is spent in the following dot product.

                    int kmax = Math.Min(i, j);
                    double s = 0.0;
                    for (int k = 0; k < kmax; k++)
                    {
                        s += LUrowi[k] * LUcolj[k];
                    }

                    LUrowi[j] = LUcolj[i] -= s;
                }

                // Find pivot and exchange if necessary.

                int p = j;
                for (int i = j + 1; i < m; i++)
                {
                    if (Math.Abs(LUcolj[i]) > Math.Abs(LUcolj[p]))
                    {
                        p = i;
                    }
                }
                if (p != j)
                {
                    for (int ki = 0; ki < n; ki++)
                    {
                        double t = LU[p, ki]; LU[p, ki] = LU[j, ki]; LU[j, ki] = t;
                    }
                    int k = piv[p]; piv[p] = piv[j]; piv[j] = k;
                    pivsign = -pivsign;
                }

                // Compute multipliers.

                if (j < m & LU[j, j] != 0.0)
                {
                    for (int i = j + 1; i < m; i++)
                    {
                        LU[i, j] /= LU[j, j];
                    }
                }
            }
        }

        /* ------------------------
           Temporary, experimental code.
           ------------------------ *\

           \** LU Decomposition, computed by Gaussian elimination.
           <P>
           This constructor computes L and U with the "daxpy"-based elimination
           algorithm used in LINPACK and MATLAB.  In Java, we suspect the dot-product,
           Crout algorithm will be faster.  We have temporarily included this
           constructor until timing experiments confirm this suspicion.
           <P>
           @param  A             Rectangular matrix
           @param  linpackflag   Use Gaussian elimination.  Actual value ignored.
           @return               Structure to access L, U and piv.
           *\

           public LUDecomposition (Matrix A, int linpackflag) {
              // Initialize.
              LU = A.getArrayCopy();
              m = A.getRowDimension();
              n = A.getColumnDimension();
              piv = new int[m];
              for (int i = 0; i < m; i++) {
                 piv[i] = i;
              }
              pivsign = 1;
              // Main loop.
              for (int k = 0; k < n; k++) {
                 // Find pivot.
                 int p = k;
                 for (int i = k+1; i < m; i++) {
                    if (Math.Abs(LU[i,k]) > Math.Abs(LU[p,k])) {
                       p = i;
                    }
                 }
                 // Exchange if necessary.
                 if (p != k) {
                    for (int j = 0; j < n; j++) {
                       double t = LU[p,j]; LU[p,j] = LU[k,j]; LU[k,j] = t;
                    }
                    int t = piv[p]; piv[p] = piv[k]; piv[k] = t;
                    pivsign = -pivsign;
                 }
                 // Compute multipliers and eliminate k-th column.
                 if (LU[k,k] != 0.0) {
                    for (int i = k+1; i < m; i++) {
                       LU[i,k] /= LU[k,k];
                       for (int j = k+1; j < n; j++) {
                          LU[i,j] -= LU[i,k]*LU[k,j];
                       }
                    }
                 }
              }
           }

        \* ------------------------
           End of temporary code.
         * ------------------------ */

        /* ------------------------
           Public Methods
         * ------------------------ */

        /** Is the matrix nonsingular?
        @return     true if U, and hence A, is nonsingular.
        */

        public bool isNonsingular()
        {
            for (int j = 0; j < n; j++)
            {
                if (LU[j, j] == 0)
                    return false;
            }
            return true;
        }

        /** Return lower triangular factor
        @return     L
        */

        public CoreMatrix getL()
        {
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] L = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i > j)
                    {
                        L[i, j] = LU[i, j];
                    }
                    else if (i == j)
                    {
                        L[i, j] = 1.0;
                    }
                    else
                    {
                        L[i, j] = 0.0;
                    }
                }
            }
            return X;
        }

        /** Return upper triangular factor
        @return     U
        */

        public CoreMatrix getU()
        {
            CoreMatrix X = new CoreMatrix(n, n);
            double[,] U = X.getArray();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i <= j)
                    {
                        U[i, j] = LU[i, j];
                    }
                    else
                    {
                        U[i, j] = 0.0;
                    }
                }
            }
            return X;
        }

        /** Return pivot permutation vector
        @return     piv
        */

        public int[] getPivot()
        {
            int[] p = new int[m];
            for (int i = 0; i < m; i++)
            {
                p[i] = piv[i];
            }
            return p;
        }

        /** Return pivot permutation vector as a one-dimensional double array
        @return     (double) piv
        */

        public double[] getDoublePivot()
        {
            double[] vals = new double[m];
            for (int i = 0; i < m; i++)
            {
                vals[i] = (double)piv[i];
            }
            return vals;
        }

        /** Determinant
        @return     det(A)
        @exception  IllegalArgumentException  Matrix must be square
        */

        public double det()
        {
            if (m != n)
            {
                throw new Exception("Matrix must be square.");
            }
            double d = (double)pivsign;
            for (int j = 0; j < n; j++)
            {
                d *= LU[j, j];
            }
            return d;
        }

        /** Solve A*X = B
        @param  B   A Matrix with as many rows as A and any number of columns.
        @return     X so that L*U*X = B(piv,:)
        @exception  IllegalArgumentException Matrix row dimensions must agree.
        @exception  RuntimeException  Matrix is singular.
        */

        public CoreMatrix solve(CoreMatrix B)
        {
            if (B.getRowDimension() != m)
            {
                throw new Exception("Matrix row dimensions must agree.");
            }
            if (!this.isNonsingular())
            {
                throw new Exception("Matrix is singular.");
            }

            // Copy right hand side with pivoting
            int nx = B.getColumnDimension();
            CoreMatrix Xmat = B.getMatrix(piv, 0, nx - 1);
            double[,] X = Xmat.getArray();

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < n; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[i, j] -= X[k, j] * LU[i, k];
                    }
                }
            }
            // Solve U*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                {
                    X[k, j] /= LU[k, k];
                }
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[i, j] -= X[k, j] * LU[i, k];
                    }
                }
            }
            return Xmat;
        }
    }
}
