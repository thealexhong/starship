using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nmsVoiceAnalysisLibrary
{
    public class CoreMatrix
    {
        /* ------------------------
             Class variables
        * ------------------------ */
        /** Array for internal storage of elements.
        @serial internal array storage.
        */
        private double[,] A;

        /** Row and column dimensions.
        @serial row dimension.
        @serial column dimension.
        */
        private int m, n;

        /* ------------------------
             Constructors
        * ------------------------ */

        /** Construct an m-by-n CoreMatrix of zeros. 
        @param m    Number of rows.
        @param n    Number of colums.
        */

        public CoreMatrix(int m, int n)
        {
            this.m = m;
            this.n = n;
            A = new double[m, n];
        }
        /* Constructor for child class
         * @param no parameters.
         */
        public CoreMatrix()
        {
        }



        /** Construct an m-by-n constant CoreMatrix.
        @param m    Number of rows.
        @param n    Number of colums.
        @param s    Fill the CoreMatrix with this scalar value.
        */

        public CoreMatrix(int m, int n, double s)
        {
            this.m = m;
            this.n = n;
            A = new double[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = s;
                }
            }
        }

        /** Construct a CoreMatrix from a 2-D array.
        @param A    Two-dimensional array of doubles.
        @exception  IllegalArgumentException All rows must have the same length
        @see        #constructWithCopy
        */

        public CoreMatrix(double[,] A)
        {
            m = A.GetLength(0);
            n = A.GetLength(1);

            this.A = A;
        }

        /** Construct a CoreMatrix quickly without checking arguments.
        @param A    Two-dimensional array of doubles.
        @param m    Number of rows.
        @param n    Number of colums.
        */

        public CoreMatrix(double[,] A, int m, int n)
        {
            this.A = A;
            this.m = m;
            this.n = n;
        }

        /** Construct a CoreMatrix from a one-dimensional packed array
        @param vals One-dimensional array of doubles, packed by columns (ala Fortran).
        @param m    Number of rows.
        @exception  IllegalArgumentException Array length must be a multiple of m.
        */

        public CoreMatrix(double[] vals, int m)
        {
            this.m = m;
            n = (m != 0 ? vals.Length / m : 0);
            if (m * n != vals.Length)
            {
                throw new Exception("Array length must be a multiple of m.");
            }
            A = new double[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = vals[i + j * m];
                }
            }
        }

        /* ------------------------
           Public Methods
         * ------------------------ */

        /** Construct a CoreMatrix from a copy of a 2-D array.
        @param A    Two-dimensional array of doubles.
        @exception  IllegalArgumentException All rows must have the same length
        */

        public static CoreMatrix constructWithCopy(double[,] A)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j];
                }
            }
            return X;
        }

        /* 
         * Make a deep copy of a CoreMatrix
         */

        public CoreMatrix copy()
        {
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j];
                }
            }
            return X;
        }

        /** Clone the CoreMatrix object.
        */

        public Object clone()
        {
            return this.copy();
        }

        /** Access the internal two-dimensional array.
        @return     Pointer to the two-dimensional array of CoreMatrix elements.
        */

        public double[,] getArray()
        {
            return A;
        }

        /** Copy the internal two-dimensional array.
        @return     Two-dimensional array copy of CoreMatrix elements.
        */

        public double[,] getArrayCopy()
        {
            double[,] C = new double[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j];
                }
            }
            return C;
        }

        /** Make a one-dimensional column packed copy of the internal array.
        @return     CoreMatrix elements packed in a one-dimensional array by columns.
        */

        public double[] getColumnPackedCopy()
        {
            double[] vals = new double[m * n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    vals[i + j * m] = A[i, j];
                }
            }
            return vals;
        }

        /** Make a one-dimensional row packed copy of the internal array.
        @return     CoreMatrix elements packed in a one-dimensional array by rows.
        */

        public double[] getRowPackedCopy()
        {
            double[] vals = new double[m * n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    vals[i * n + j] = A[i, j];
                }
            }
            return vals;
        }

        /** Get row dimension.
        @return     m, the number of rows.
        */

        public int getRowDimension()
        {
            return m;
        }

        /** Get column dimension.
        @return     n, the number of columns.
        */

        public int getColumnDimension()
        {
            return n;
        }

        /** Get a single element.
        @param i    Row index.
        @param j    Column index.
        @return     A(i,j)
        @exception  Exception
        */

        public double get(int i, int j)
        {
            return A[i, j];
        }

        /** Get a subCoreMatrix.
        @param i0   Initial row index
        @param i1   Final row index
        @param j0   Initial column index
        @param j1   Final column index
        @return     A(i0:i1,j0:j1)
        @exception  Exception SubCoreMatrix indices
        */

        public CoreMatrix getMatrix(int i0, int i1, int j0, int j1)
        {
            CoreMatrix X = new CoreMatrix(i1 - i0 + 1, j1 - j0 + 1);
            double[,] B = X.getArray();
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        B[i - i0, j - j0] = A[i, j];
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("SubCoreMatrix indices");
            }
            return X;
        }

        /** Get a subCoreMatrix.
        @param r    Array of row indices.
        @param c    Array of column indices.
        @return     A(r(:),c(:))
        @exception  Exception SubCoreMatrix indices
        */

        public CoreMatrix getMatrix(int[] r, int[] c)
        {
            CoreMatrix X = new CoreMatrix(r.Length, c.Length);
            double[,] B = X.getArray();
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        B[i, j] = A[r[i], c[j]];
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("SubCoreMatrix indices");
            }
            return X;
        }

        /** Get a subCoreMatrix.
        @param i0   Initial row index
        @param i1   Final row index
        @param c    Array of column indices.
        @return     A(i0:i1,c(:))
        @exception  Exception SubCoreMatrix indices
        */

        public CoreMatrix getMatrix(int i0, int i1, int[] c)
        {
            CoreMatrix X = new CoreMatrix(i1 - i0 + 1, c.Length);
            double[,] B = X.getArray();
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        B[i - i0, j] = A[i, c[j]];
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("SubCoreMatrix indices");
            }
            return X;
        }

        /** Get a subCoreMatrix.
        @param r    Array of row indices.
        @param i0   Initial column index
        @param i1   Final column index
        @return     A(r(:),j0:j1)
        @exception  Exception SubCoreMatrix indices
        */

        public CoreMatrix getMatrix(int[] r, int j0, int j1)
        {
            CoreMatrix X = new CoreMatrix(r.Length, j1 - j0 + 1);
            double[,] B = X.getArray();
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        B[i, j - j0] = A[r[i], j];
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("SubCoreMatrix indices");
            }
            return X;
        }

        /** Set a single element.
        @param i    Row index.
        @param j    Column index.
        @param s    A(i,j).
        @exception  Exception
        */

        public void set(int i, int j, double s)
        {
            A[i, j] = s;
        }

        /** Set a subCoreMatrix.
        @param i0   Initial row index
        @param i1   Final row index
        @param j0   Initial column index
        @param j1   Final column index
        @param X    A(i0:i1,j0:j1)
        @exception  Exception SubCoreMatrix indices
        */

        public void setMatrix(int i0, int i1, int j0, int j1, CoreMatrix X)
        {
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        A[i, j] = X.get(i - i0, j - j0);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("SubCoreMatrix indices");
            }
        }

        /** Set a subCoreMatrix.
        @param r    Array of row indices.
        @param c    Array of column indices.
        @param X    A(r(:),c(:))
        @exception  Exception SubCoreMatrix indices
        */

        public void setMatrix(int[] r, int[] c, CoreMatrix X)
        {
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        A[r[i], c[j]] = X.get(i, j);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("SubCoreMatrix indices");
            }
        }

        /** Set a subCoreMatrix.
        @param r    Array of row indices.
        @param j0   Initial column index
        @param j1   Final column index
        @param X    A(r(:),j0:j1)
        @exception  Exception SubCoreMatrix indices
        */

        public void setMatrix(int[] r, int j0, int j1, CoreMatrix X)
        {
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        A[r[i], j] = X.get(i, j - j0);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("SubCoreMatrix indices");
            }
        }

        /** Set a subCoreMatrix.
        @param i0   Initial row index
        @param i1   Final row index
        @param c    Array of column indices.
        @param X    A(i0:i1,c(:))
        @exception  Exception SubCoreMatrix indices
        */

        public void setMatrix(int i0, int i1, int[] c, CoreMatrix X)
        {
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        A[i, c[j]] = X.get(i - i0, j);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("SubCoreMatrix indices");
            }
        }

        /** CoreMatrix transpose.
        @return    A'
        */

        public CoreMatrix transpose()
        {
            CoreMatrix X = new CoreMatrix(n, m);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[j, i] = A[i, j];
                }
            }
            return X;
        }

        /** One norm
        @return    maximum column sum.
        */

        public double norm1()
        {
            double f = 0;
            for (int j = 0; j < n; j++)
            {
                double s = 0;
                for (int i = 0; i < m; i++)
                {
                    s += Math.Abs(A[i, j]);
                }
                f = Math.Max(f, s);
            }
            return f;
        }

        /** Infinity norm
        @return    maximum row sum.
        */

        public double normInf()
        {
            double f = 0;
            for (int i = 0; i < m; i++)
            {
                double s = 0;
                for (int j = 0; j < n; j++)
                {
                    s += Math.Abs(A[i, j]);
                }
                f = Math.Max(f, s);
            }
            return f;
        }

        /** Frobenius norm
        @return    sqrt of sum of squares of all elements.
        */

        public double normF()
        {
            double s = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    s += (A[i, j] * A[i, j]);
                }
            }
            return Math.Sqrt(s);
        }

        /**  Unary minus
        @return    -A
        */

        public CoreMatrix uminus()
        {
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = -A[i, j];
                }
            }
            return X;
        }

        /** C = A + B
        @param B    another CoreMatrix
        @return     A + B
        */

        public CoreMatrix plus(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j] + B.A[i, j];
                }
            }
            return X;
        }

        /** A = A + B
        @param B    another CoreMatrix
        @return     A + B
        */

        public CoreMatrix plusEquals(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = A[i, j] + B.A[i, j];
                }
            }
            return this;
        }

        /** C = A - B
        @param B    another CoreMatrix
        @return     A - B
        */

        public CoreMatrix minus(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j] - B.A[i, j];
                }
            }
            return X;
        }

        /** A = A - B
        @param B    another CoreMatrix
        @return     A - B
        */

        public CoreMatrix minusEquals(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = A[i, j] - B.A[i, j];
                }
            }
            return this;
        }

        /** Element-by-element multiplication, C = A.*B
        @param B    another CoreMatrix
        @return     A.*B
        */

        public CoreMatrix arrayTimes(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j] * B.A[i, j];
                }
            }
            return X;
        }

        /** Element-by-element multiplication in place, A = A.*B
        @param B    another CoreMatrix
        @return     A.*B
        */

        public CoreMatrix arrayTimesEquals(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = A[i, j] * B.A[i, j];
                }
            }
            return this;
        }

        /** Element-by-element right division, C = A./B
        @param B    another CoreMatrix
        @return     A./B
        */

        public CoreMatrix arrayRightDivide(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j] / B.A[i, j];
                }
            }
            return X;
        }

        /** Element-by-element right division in place, A = A./B
        @param B    another CoreMatrix
        @return     A./B
        */

        public CoreMatrix arrayRightDivideEquals(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = A[i, j] / B.A[i, j];
                }
            }
            return this;
        }

        /** Element-by-element left division, C = A.\B
        @param B    another CoreMatrix
        @return     A.\B
        */

        public CoreMatrix arrayLeftDivide(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = B.A[i, j] / A[i, j];
                }
            }
            return X;
        }

        /** Element-by-element left division in place, A = A.\B
        @param B    another CoreMatrix
        @return     A.\B
        */

        public CoreMatrix arrayLeftDivideEquals(CoreMatrix B)
        {
            checkCoreMatrixDimensions(B);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = B.A[i, j] / A[i, j];
                }
            }
            return this;
        }

        /** Multiply a CoreMatrix by a scalar, C = s*A
        @param s    scalar
        @return     s*A
        */

        public CoreMatrix times(double s)
        {
            CoreMatrix X = new CoreMatrix(m, n);
            double[,] C = X.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = s * A[i, j];
                }
            }
            return X;
        }

        /** Multiply a CoreMatrix by a scalar in place, A = s*A
        @param s    scalar
        @return     replace A by s*A
        */

        public CoreMatrix timesEquals(double s)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = s * A[i, j];
                }
            }
            return this;
        }

        /** Linear algebraic CoreMatrix multiplication, A * B
        @param B    another CoreMatrix
        @return     CoreMatrix product, A * B
        @exception  IllegalArgumentException CoreMatrix inner dimensions must agree.
        */

        public CoreMatrix times(CoreMatrix B)
        {
            if (B.m != n)
            {
                throw new Exception("CoreMatrix inner dimensions must agree.");
            }
            CoreMatrix X = new CoreMatrix(m, B.n);
            double[,] C = X.getArray();
            double[] Bcolj = new double[n];
            for (int j = 0; j < B.n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Bcolj[k] = B.A[k, j];
                }
                for (int i = 0; i < m; i++)
                {
                    double[] Arowi = new double[n];
                    for (int z = 0; z < n; z++)
                        Arowi[z] = A[i, z];
                    double s = 0;
                    for (int k = 0; k < n; k++)
                    {
                        s += Arowi[k] * Bcolj[k];
                    }
                    C[i, j] = s;
                }
            }
            return X;
        }

        /** LU Decomposition
        @return     LUDecomposition
        @see LUDecomposition
        */

        public LUDecomposition lu()
        {
            return new LUDecomposition(this);
        }

        /** QR Decomposition
        @return     QRDecomposition
        @see QRDecomposition
        */

        public QRDecomposition qr()
        {
            return new QRDecomposition(this);
        }

        /** Eigenvalue Decomposition
        @return     EigenvalueDecomposition
        @see EigenvalueDecomposition
        */

        public EigenvalueDecomposition eig()
        {
            return new EigenvalueDecomposition(this);
        }

        /** Solve A*X = B
        @param B    right hand side
        @return     solution if A is square, least squares solution otherwise
        */

        public CoreMatrix solve(CoreMatrix B)
        {
            return (m == n ? (new LUDecomposition(this)).solve(B) :
                             (new QRDecomposition(this)).solve(B));
        }

        /** Solve X*A = B, which is also A'*X' = B'
        @param B    right hand side
        @return     solution if A is square, least squares solution otherwise.
        */

        public CoreMatrix solveTranspose(CoreMatrix B)
        {
            return transpose().solve(B.transpose());
        }

        /** CoreMatrix inverse or pseudoinverse
        @return     inverse(A) if A is square, pseudoinverse otherwise.
        */

        public CoreMatrix inverse()
        {
            return solve(identity(m, m));
        }

        /** CoreMatrix determinant
        @return     determinant
        */

        public double det()
        {
            return new LUDecomposition(this).det();
        }


        /** CoreMatrix trace.
        @return     sum of the diagonal elements.
        */

        public double trace()
        {
            double t = 0;
            for (int i = 0; i < Math.Min(m, n); i++)
            {
                t += A[i, i];
            }
            return t;
        }

        /** Generate identity CoreMatrix
        @param m    Number of rows.
        @param n    Number of colums.
        @return     An m-by-n CoreMatrix with ones on the diagonal and zeros elsewhere.
        */

        public static CoreMatrix identity(int m, int n)
        {
            CoreMatrix A = new CoreMatrix(m, n);
            double[,] X = A.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    X[i, j] = (i == j ? 1.0 : 0.0);
                }
            }
            return A;
        }


        /* ------------------------
           Private Methods
         * ------------------------ */

        /** Check if size(A) == size(B) **/

        private void checkCoreMatrixDimensions(CoreMatrix B)
        {
            if (B.m != m || B.n != n)
            {
                throw new Exception("CoreMatrix dimensions must agree.");
            }
        }

    }

}

