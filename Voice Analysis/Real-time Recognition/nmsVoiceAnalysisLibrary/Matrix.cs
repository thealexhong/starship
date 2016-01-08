using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Media;

namespace nmsVoiceAnalysisLibrary
{
    public class Matrix : CoreMatrix
    {
        /**
         * The actual matrix */
        protected CoreMatrix m_Matrix = null;

        /**
         * Constructs a matrix and initializes it with default values.
         *
         * @param nr the number of rows
         * @param nc the number of columns
         */
        public Matrix(int nr, int nc)
        {
            m_Matrix = new CoreMatrix(nr, nc);
        }

        /**
         * Constructs a matrix using a given array.
         *
         * @param array the values of the matrix
         */
        public Matrix(double[,] array)
        {
            m_Matrix = new CoreMatrix(array);
        }

        /**
         * Creates and returns a clone of this object.
         *
         * @return a clone of this instance.
         * @throws Exception if an error occurs
         */
        public Object clone()
        {
            try
            {
                return new CoreMatrix(m_Matrix.getArray());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /**
         * returns the internal matrix
         * @see #m_Matrix
         */
        public CoreMatrix getMatrix()
        {
            return m_Matrix;
        }

        /**
         * Returns the value of a cell in the matrix.
         *
         * @param rowIndex the row's index
         * @param columnIndex the column's index
         * @return the value of the cell of the matrix
         */
        public double getElement(int rowIndex, int columnIndex)
        {
            return m_Matrix.get(rowIndex, columnIndex);
        }

        /**
         * Add a value to an element.
         *
         * @param rowIndex the row's index.
         * @param columnIndex the column's index.
         * @param value the value to add.
         */
        public void addElement(int rowIndex, int columnIndex, double value)
        {
            m_Matrix.set(rowIndex,
                         columnIndex,
                         m_Matrix.get(rowIndex, columnIndex) + value);
        }

        /**
         * Returns the number of rows in the matrix.
         *
         * @return the number of rows
         */
        public int numRows()
        {
            return m_Matrix.getRowDimension();
        }

        /**
         * Returns the number of columns in the matrix.
         *
         * @return the number of columns
         */
        public int numColumns()
        {
            return m_Matrix.getColumnDimension();
        }

        /**
         * Sets an element of the matrix to the given value.
         *
         * @param rowIndex the row's index
         * @param columnIndex the column's index
         * @param value the value
         */
        public void setElement(int rowIndex, int columnIndex, double value)
        {
            m_Matrix.set(rowIndex, columnIndex, value);
        }

        /**
         * Sets a row of the matrix to the given row. Performs a deep copy.
         *
         * @param index the row's index
         * @param newRow an array of doubles
         */
        public void setRow(int index, double[] newRow)
        {
            for (int i = 0; i < newRow.Length; i++)
                m_Matrix.set(index, i, newRow[i]);
        }

        /**
         * Gets a row of the matrix and returns it as double array.
         *
         * @param index the row's index
         * @return an array of doubles
         */
        public double[] getRow(int index)
        {
            double[] newRow = new double[this.numColumns()];
            for (int i = 0; i < newRow.Length; i++)
                newRow[i] = getElement(index, i);

            return newRow;
        }

        /**
         * Gets a column of the matrix and returns it as a double array.
         *
         * @param index the column's index
         * @return an array of doubles
         */
        public double[] getColumn(int index)
        {
            double[] newColumn = new double[this.numRows()];
            for (int i = 0; i < newColumn.Length; i++)
                newColumn[i] = getElement(i, index);

            return newColumn;
        }

        /**
         * Sets a column of the matrix to the given column. Performs a deep copy.
         *
         * @param index the column's index
         * @param newColumn an array of doubles
         */
        public void setColumn(int index, double[] newColumn)
        {
            for (int i = 0; i < numRows(); i++)
                m_Matrix.set(i, index, newColumn[i]);
        }

        /**
         * Returns the sum of this matrix with another.
         *
         * @return a matrix containing the sum.
         */
        public CoreMatrix add(Matrix other)
        {
            try
            {
                return new CoreMatrix(m_Matrix.plus(other.getMatrix()).getArrayCopy());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /**
         * Returns the transpose of a matrix.
         *
         * @return the transposition of this instance.
         */
        public Matrix transpose()
        {
            try
            {
                return new Matrix(m_Matrix.transpose().getArrayCopy());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /**
         * Returns the multiplication of two matrices
         *
         * @param b the multiplication matrix
         * @return the product matrix
         */
        public CoreMatrix multiply(Matrix b)
        {
            try
            {
                return new CoreMatrix(getMatrix().times(b.getMatrix()).getArrayCopy());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /**
         * Returns the L part of the matrix.
         * This does only make sense after LU decomposition.
         *
         * @return matrix with the L part of the matrix; 
         * @see #LUDecomposition()
         */
        public CoreMatrix getL()
        {
            int nr = numRows();    // num of rows
            int nc = numColumns(); // num of columns
            double[,] ld = new double[nr, nc];

            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; (j < i) && (j < nc); j++)
                {
                    ld[i, j] = getElement(i, j);
                }
                if (i < nc) ld[i, i] = 1;
            }
            CoreMatrix l = new CoreMatrix(ld);
            return l;
        }

        /**
         * Returns the U part of the matrix.
         * This does only make sense after LU decomposition.
         *
         * @return matrix with the U part of a matrix; 
         * @see #LUDecomposition()
         */
        public CoreMatrix getU()
        {
            int nr = numRows();    // num of rows
            int nc = numColumns(); // num of columns
            double[,] ud = new double[nr, nc];

            for (int i = 0; i < nr; i++)
            {
                for (int j = i; j < nc; j++)
                {
                    ud[i, j] = getElement(i, j);
                }
            }
            CoreMatrix u = new CoreMatrix(ud);
            return u;
        }

        /**
         * Performs a LUDecomposition on the matrix.
         * It changes the matrix into its LU decomposition.
         *
         * @return the indices of the row permutation
         */
        public int[] LUDecomposition()
        {
            // decompose
            LUDecomposition lu = m_Matrix.lu();

            // singular? old class throws Exception!
            if (!lu.isNonsingular())
                throw new Exception("Matrix is singular");

            CoreMatrix u = lu.getU();
            CoreMatrix l = lu.getL();

            // modify internal matrix
            int nr = numRows();
            int nc = numColumns();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    if (j < i)
                        setElement(i, j, l.get(i, j));
                    else
                        setElement(i, j, u.get(i, j));
                }
            }

            u = null;
            l = null;

            return lu.getPivot();
        }

        /**
         * Solve A*X = B using backward substitution.
         * A is current object (this). Note that this matrix will be changed! 
         * B parameter bb.
         * X returned in parameter bb.
         *
         * @param bb first vector B in above equation then X in same equation.
         */
        public void solve(double[] bb)
        {
            // solve
            CoreMatrix x = m_Matrix.solve(new CoreMatrix(bb, bb.Length));

            // move X into bb
            int nr = x.getRowDimension();
            for (int i = 0; i < nr; i++)
                bb[i] = x.get(i, 0);
        }

        /**
         * Performs Eigenvalue Decomposition using Householder QR Factorization
         *
         * Matrix must be symmetrical.
         * Eigenvectors are return in parameter V, as columns of the 2D array.
         * (Real parts of) Eigenvalues are returned in parameter d.
         *
         * @param V double array in which the eigenvectors are returned 
         * @param d array in which the eigenvalues are returned
         * @throws Exception if matrix is not symmetric
         */
        public void EigenvalueDecomposition(double[,] V, double[] d)
        {

            // perform eigenvalue decomposition
            EigenvalueDecomposition eig = m_Matrix.eig();
            CoreMatrix v = eig.getV();
            double[] d2 = eig.getRealEigenvalues();

            // transfer data
            int nr = numRows();
            int nc = numColumns();
            for (int i = 0; i < nr; i++)
                for (int j = 0; j < nc; j++)
                    V[i, j] = v.get(i, j);

            for (int i = 0; i < d2.Length; i++)
                d[i] = d2[i];
        }

        /**
         * Returns sqrt(a^2 + b^2) without under/overflow.
         *   
         * @param a length of one side of rectangular triangle
         * @param b length of other side of rectangular triangle
         * @return lenght of third side
         */
        protected static double hypot(double a, double b)
        {
            return Maths.hypot(a, b);
        }

    }
}