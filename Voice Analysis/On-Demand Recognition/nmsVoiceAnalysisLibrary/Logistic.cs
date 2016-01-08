using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Media;

namespace nmsVoiceAnalysisLibrary
{
    public class Logistic
    {
        /** Debug option */
        public bool m_Debug = false; 

        /** Mean and Standard Deviation */
        public static double[] xMean;
        public static double[] xSD;

        /** The coefficients (optimized parameters) of the model */
        public static double[,] m_Par;

        /** The data saved as a matrix */
        public static double[,] m_Data;

        /** The number of attributes in the model */
        protected static int m_NumPredictors = 19;

        /** The index of the class attribute */
        protected static int m_ClassIndex = 19;

        /** The number of the class labels */
        protected static int m_NumClasses = 6;

        /** The ridge parameter. */
        protected static double m_Ridge = 1e-8;

        /** Log-likelihood of the searched model */
        protected static double m_LL;

        /** The maximum number of iterations. */
        private static int m_MaxIts = -1;

        /**
         * Sets the ridge in the log-likelihood.
         *
         * @param ridge the ridge
         */
        public void setVar(double[,] Data, double[,] Par, double[] Mean, double[] SD)
        {
            m_Data = Data;
            m_Par = Par;
            xMean = Mean;
            xSD = SD;
        }

        /**
         * Sets the ridge in the log-likelihood.
         *
         * @param ridge the ridge
         */
        public void setRidge(double ridge)
        {
            m_Ridge = ridge;
        }

        /**
         * Sets the debug option.
         *
         * @param bool to turn debug on of off
         */
        public void setDebug(bool Debug)
        {
            m_Debug = Debug;
        }
        /**
         * Gets the ridge in the log-likelihood.
         *
         * @return the ridge
         */
        public double getRidge()
        {
            return m_Ridge;
        }

        /**
         * Get the value of MaxIts.
         *
         * @return Value of MaxIts.
         */
        public int getMaxIts()
        {

            return m_MaxIts;
        }

        /**
         * Set the value of MaxIts.
         *
         * @param newMaxIts Value to assign to MaxIts.
         */
        public void setMaxIts(int newMaxIts)
        {

            m_MaxIts = newMaxIts;
        }

        public class LogicOpt : Optimization
        {
            /** Class labels of instances */
            private int[] cls;

            /** 
             * Set the class labels of instances
             * @param c the class labels to be set
             */
            public void setClassLabels(int[] c)
            {
                cls = c;
            }

            /** 
             * Evaluate objective function
             * @param x the current values of variables
             * @return the value of the objective function 
             */
            protected override double objectiveFunction(double[] x)
            {
                double nll = 0; // -LogLikelihood
                int dim = m_NumPredictors + 1; // Number of variables per class

                for (int i = 0; i < cls.Length; i++) // ith instance
                {
                    double[] exp = new double[m_NumClasses - 1];
                    int index;
                    for (int offset = 0; offset < m_NumClasses - 1; offset++)
                    {
                        index = offset * dim;
                        for (int j = 0; j < dim; j++)
                            exp[offset] += m_Data[i, j] * x[index + j];
                    }
                    double max = exp[Utils.maxIndex(exp)];
                    double denom = Math.Exp(-max);
                    double num;
                    if (cls[i] == m_NumClasses - 1) // Class of this instance
                    {
                        num = -max;
                    }
                    else
                    {
                        num = exp[cls[i]] - max;
                    }
                    for (int offset = 0; offset < m_NumClasses - 1; offset++)
                    {
                        denom += Math.Exp(exp[offset] - max);
                    }
                    nll -= (num - Math.Log(denom)); // Weighted NLL
                }

                // Ridge: note that intercepts NOT included
                for (int offset = 0; offset < m_NumClasses - 1; offset++)
                {
                    for (int r = 1; r < dim; r++)
                        nll += m_Ridge * x[offset * dim + r] * x[offset * dim + r];
                }
                return nll;
            }

            /** 
             * Evaluate Jacobian vector
             * @param x the current values of variables
             * @return the gradient vector 
             */
            protected override double[] evaluateGradient(double[] x)
            {
                double[] grad = new double[x.Length];
                int dim = m_NumPredictors + 1; // Number of variables per class

                for (int i = 0; i < cls.Length; i++) // ith instance
                {
                    double[] num = new double[m_NumClasses - 1]; // numerator of [-log(1+sum(exp))]'
                    int index;
                    for (int offset = 0; offset < m_NumClasses - 1; offset++) // Which part of x
                    {
                        double exp = 0.0;
                        index = offset * dim;
                        for (int j = 0; j < dim; j++)
                            exp += m_Data[i, j] * x[index + j];
                        num[offset] = exp;
                    }
                    double max = num[Utils.maxIndex(num)];
                    double denom = Math.Exp(-max); // Denominator of [-log(1+sum(exp))]'
                    for (int offset = 0; offset < m_NumClasses - 1; offset++)
                    {
                        num[offset] = Math.Exp(num[offset] - max);
                        denom += num[offset];
                    }
                    Utils.normalize(num, denom);

                    // Update denominator of the gradient of -log(Posterior)
                    double firstTerm;
                    for (int offset = 0; offset < m_NumClasses - 1; offset++) // Which part of x
                    {
                        index = offset * dim;
                        firstTerm = num[offset];
                        for (int q = 0; q < dim; q++)
                        {
                            grad[index + q] += firstTerm * m_Data[i, q];
                        }
                    }
                    if (cls[i] != m_NumClasses - 1) // Not the last class
                    {
                        for (int p = 0; p < dim; p++)
                        {
                            grad[cls[i] * dim + p] -= m_Data[i, p];
                        }
                    }
                }

                // Ridge: note that intercepts NOT included
                for (int offset = 0; offset < m_NumClasses - 1; offset++)
                {
                    for (int r = 1; r < dim; r++)
                        grad[offset * dim + r] += 2 * m_Ridge * x[offset * dim + r];
                }
                return grad;
            }
        }

        /**
         * Builds the classifier
         *
         * @param train the training data to be used for generating the
         * boosted classifier.
         * @throws Exception if the classifier could not be built successfully
         */
        public void buildClassifier()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Extracting Data");
            for (int i = 2; i >= 0; i--)
            {
                Console.Write(".");
                Thread.Sleep(330);
                Console.Write(".");
                Thread.Sleep(330);
                Console.Write(".");
                Thread.Sleep(330);
                Console.Write("\b\b\b   \b\b\b");
                Thread.Sleep(330);
            }
            char[] delimiters = new char[] { '\r', '\n' };
            /* Load Labeled Data */
            StreamReader s = new StreamReader("traindata.txt");

            /* Read the data in the file */
            string AllData = s.ReadToEnd();

            /* Split off each row at the Carriage Return/Line Feed
               This will work for Excel, Access, etc. default exports */
            string[] rows = AllData.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            int i_1 = 0, j_1 = 0;

            double[,] Data = new double[rows.Length, 20];

            /* Add each row to the matrix */
            foreach (string r in rows)
            {
                /* Split the row at the delimiter */
                string[] items = r.Split('\t');
                foreach (string ite in items)
                {
                    Data[i_1, j_1] = Convert.ToDouble(ite);
                    j_1++;
                }
                j_1 = 0;
                i_1++;
            }
            s.Close();

            //const int m_ClassIndex = 19;
            //const int m_NumClasses = 6;

            int nK = m_NumClasses - 1;                  // Only K-1 class labels needed 
            int nR = Data.GetLength(1) - 1;             // Parameters
            int nC = Data.GetLength(0);                 // Trained Instances

            double[,] m_Data = new double[nC, nR + 1];      // Data in new format
            int[] Y = new int[nC];                          // Class labels
            double[] xMean = new double[nR + 1];            // Attribute means
            double[] xSD = new double[nR + 1];              // Attribute stddev's
            double[] sY = new double[nK + 1];               // Number of classes
            double[,] m_Par = new double[nR + 1, nK];       // Optimized parameter values

            for (int i = 0; i < nC; i++)
            {
                // initialize X[][]
                Y[i] = (int)(Data[i, 19] - 1);
                m_Data[i, 0] = 1;
                int j = 1;
                for (int k = 0; k <= nR; k++)
                {
                    if (k != m_ClassIndex)
                    {
                        double z = Data[i, k];
                        m_Data[i, j] = z;
                        xMean[j] += z;
                        xSD[j] += z * z;
                        j++;
                    }
                }
                // Class count
                sY[Y[i]]++;
            }
            xMean[0] = 0; xSD[0] = 1;
            for (int j = 1; j <= nR; j++)
            {
                xMean[j] = xMean[j] / nC;
                xSD[j] = Math.Sqrt(Math.Abs(xSD[j] - nC * xMean[j] * xMean[j]) / (nC - 1));
            }
            if (m_Debug)
            {
                // Output stats about input data
                Console.WriteLine("Descriptives...");
                for (int m = 0; m <= nK; m++)
                    Console.WriteLine("{0} cases have class {1}", sY[m], m);
                Console.WriteLine("\n Variable            Avg                   SD    ");
                for (int j = 1; j <= nR; j++)
                    Console.WriteLine("   {0}        {1}       {2}", j, xMean[j], xSD[j]);
            }

            // Normalise input data 
            for (int i = 0; i < nC; i++)
            {
                for (int j = 0; j <= nR; j++)
                {
                    if (xSD[j] != 0)
                        m_Data[i, j] = (m_Data[i, j] - xMean[j]) / xSD[j];
                }
            }

            Console.WriteLine("\nBuilding Model(This may take a while)");

            double[] x = new double[(nR + 1) * nK];
            double[,] b = new double[2, x.Length];// Boundary constraints, N/A here

            // Initialize
            for (int p = 0; p < nK; p++)
            {
                int offset = p * (nR + 1);
                x[offset] = Math.Log(sY[p] + 1.0) - Math.Log(sY[nK] + 1.0); // Null model
                b[0, offset] = Double.NaN;
                b[1, offset] = Double.NaN;
                for (int q = 1; q <= nR; q++)
                {
                    x[offset + q] = 0.0;
                    b[0, offset + q] = Double.NaN;
                    b[1, offset + q] = Double.NaN;
                }
            }
            setVar(m_Data, m_Par, xMean, xSD);
            LogicOpt opt = new LogicOpt();
            opt.setClassLabels(Y);

            if (m_MaxIts == -1) // Search until convergence
            {
                x = opt.findArgMin(x, b);
                while (x == null)
                {
                    x = opt.getVarbValues();
                    Console.WriteLine("200 iterations finished, not enough!");
                    x = opt.findArgMin(x, b);
                }
                Console.WriteLine(" -------------<Converged>--------------");
            }
            else
            {
                opt.setMaxIteration(m_MaxIts);
                x = opt.findArgMin(x, b);
                if (x == null) // Not enough, but use the current value
                    x = opt.getVarbValues();
            }

            m_LL = -opt.getMinFunction(); // Log-likelihood

            // Convert coefficients back to non-normalized attribute units
            for (int i = 0; i < nK; i++)
            {
                m_Par[0, i] = x[i * (nR + 1)];
                for (int j = 1; j <= nR; j++)
                {
                    m_Par[j, i] = x[i * (nR + 1) + j];
                    if (xSD[j] != 0)
                    {
                        m_Par[j, i] /= xSD[j];
                        m_Par[0, i] -= m_Par[j, i] * xMean[j];
                    }
                }
            }
            // Copy for global Parameters 
            setVar(m_Data, m_Par, xMean, xSD);
            // Don't need data matrix anymore
            m_Data = null;

        }


        /**
         * Computes the distribution for a given instance
         *
         * @param instance the instance for which distribution is computed
         * @return the distribution
         * @throws Exception if the distribution can't be computed successfully
         */
        public double[] distributionForInstance(double[] CurrentAtts)
        {
            // Extract the predictor columns into an array
            double[] instDat = new double[m_NumPredictors + 1];
            int j = 1;
            instDat[0] = 1;
            for (int k = 0; k <= m_NumPredictors; k++)
            {
                if (k != m_ClassIndex)
                {
                    instDat[j++] = CurrentAtts[k];
                }
            }

            double[] distribution = evaluateProbability(instDat);
            return distribution;
        }

        /**
         * Compute the posterior distribution using optimized parameter values
         * and the testing instance.
         * @param data the testing instance
         * @return the posterior probability distribution
         */
        private double[] evaluateProbability(double[] data)
        {
            double[] prob = new double[m_NumClasses],
              v = new double[m_NumClasses];

            // Log-posterior before normalizing
            for (int j = 0; j < m_NumClasses - 1; j++)
            {
                for (int k = 0; k <= m_NumPredictors; k++)
                {
                    v[j] += m_Par[k, j] * data[k];
                }
            }
            v[m_NumClasses - 1] = 0;

            // Do so to avoid scaling problems
            for (int m = 0; m < m_NumClasses; m++)
            {
                double sum = 0;
                for (int n = 0; n < m_NumClasses - 1; n++)
                    sum += Math.Exp(v[n] - v[m]);
                prob[m] = 1 / (sum + Math.Exp(-v[m]));
            }

            return prob;
        }
    }
}