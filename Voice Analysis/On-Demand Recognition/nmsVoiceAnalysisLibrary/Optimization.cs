using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;

namespace nmsVoiceAnalysisLibrary
{
    public abstract class Optimization
    {
        protected double m_ALF = 1.0e-4;

        protected double m_BETA = 0.9;

        protected double m_TOLX = 1.0e-6;

        protected double m_STPMX = 100.0;

        protected int m_MaxITS = 200;

        protected static bool m_Debug = false;

        /** function value */
        protected double m_f;

        /** G'*p */
        private double m_Slope;

        /** Test if zero step in lnsrch */
        private bool m_IsZeroStep = false;

        /** Used when iteration overflow occurs */
        private double[] m_X;

        protected static double m_Epsilon, m_Zero;

        public void machineP()
        {
            m_Epsilon = 1.0;
            while (1.0 + m_Epsilon > 1.0)
            {
                m_Epsilon /= 2.0;
            }
            m_Epsilon *= 2.0;
            m_Zero = Math.Sqrt(m_Epsilon);
        }

        /**
         * Subclass should implement this procedure to evaluate objective
         * function to be Minimized
         * 
         * @param x the variable values
         * @return the objective function value
         * @throws Exception if something goes wrong
         */
        protected abstract double objectiveFunction(double[] x);

        /**
         * Subclass should implement this procedure to evaluate gradient
         * of the objective function
         * 
         * @param x the variable values
         * @return the gradient vector
         * @throws Exception if something goes wrong
         */
        protected abstract double[] evaluateGradient(double[] x);

        /**
         * Subclass is recommended to override this procedure to evaluate second-order
         * gradient of the objective function.  If it's not provided, it returns
         * null.
         *
         * @param x the variables
         * @param index the row index in the Hessian matrix
         * @return one row (the row #index) of the Hessian matrix, null as default
         * @throws Exception if something goes wrong
         */
        protected double[] evaluateHessian(double[] x, int index)
        {
            return null;
        }

        /**
         * Get the Minimal function value
         *
         * @return Minimal function value found
         */
        public double getMinFunction()
        {
            return m_f;
        }

        /**
         * Set the Maximal number of iterations in searching (Default 200)
         *
         * @param it the Maximal number of iterations
         */
        public void setMaxIteration(int it)
        {
            m_MaxITS = it;
        }

        /**
         * Set whether in debug mode
         *
         * @param db use debug or not
         */
        public void setDebug(bool db)
        {
            m_Debug = db;
        }

        /**
         * Get the variable values.  Only needed when iterations exceeds 
         * the Max threshold.
         *
         * @return the current variable values
         */
        public double[] getVarbValues()
        {
            return m_X;
        }

        /**
         * Find a new point x in the direction p from a point xold at which the
         * value of the function has decreased sufficiently, the positive 
         * definiteness of B matrix (approximation of the inverse of the Hessian)
         * is preserved and no bound constraints are violated.  Details see "Numerical 
         * Methods for Unconstrained Optimization and Nonlinear Equations".
         * "Numeric Recipes in C" was also consulted.
         *
         * @param xold old x value 
         * @param gradient gradient at that point
         * @param direct direction vector
         * @param stpMax Maximum step .Length
         * @param isFixed indicating whether a variable has been fixed
         * @param nwsBounds non-working set bounds.  Means these variables are free and
         *                  subject to the bound constraints in this step
         * @param wsBdsIndx index of variables that has working-set bounds.  Means
         *                  these variables are already fixed and no longer subject to
         *                  the constraints
         * @return new value along direction p from xold, null if no step was taken
         * @throws Exception if an error occurs
         */
        public double[] lnsrch(double[] xold, double[] gradient,
                   double[] direct, double stpMax,
                   bool[] isFixed, double[,] nwsBounds,
                   DynamicIntArray wsBdsIndx)
        {
            int i, k, len = xold.Length,
            fixedOne = -1; // idx of variable to be fixed
            double alam, alaMin; // lambda to be found, and its lower bound

            // For convergence and bound test
            double temp, test, alpha = Double.PositiveInfinity, fold = m_f, sum;

            // For cubic interpolation
            double a, alam2 = 0, b, disc = 0, Maxalam = 1.0, rhs1, rhs2, tmplam;

            double[] x = new double[len]; // New variable values

            // Scale the step 
            for (sum = 0.0, i = 0; i < len; i++)
            {
                if (!isFixed[i]) // For fixed variables, direction = 0
                    sum += direct[i] * direct[i];
            }
            sum = Math.Sqrt(sum);

            if (sum > stpMax)
            {
                for (i = 0; i < len; i++)
                    if (!isFixed[i])
                        direct[i] *= stpMax / sum;
            }
            else
                Maxalam = stpMax / sum;

            // Compute initial rate of decrease, g'*d 
            m_Slope = 0.0;
            for (i = 0; i < len; i++)
            {
                x[i] = xold[i];
                if (!isFixed[i])
                    m_Slope += gradient[i] * direct[i];
            }



            // Slope too small
            if (Math.Abs(m_Slope) <= m_Zero)
            {
                return x;
            }

            // Err: slope > 0

            // Compute LAMBDAMin and upper bound of lambda--alpha
            test = 0.0;
            for (i = 0; i < len; i++)
            {
                if (!isFixed[i])
                {// No need for fixed variables
                    temp = Math.Abs(direct[i]) / Math.Max(Math.Abs(x[i]), 1.0);
                    if (temp > test) test = temp;
                }
            }

            if (test > m_Zero) // Not converge
                alaMin = m_TOLX / test;
            else
            {
                return x;
            }

            // Check whether any non-working-set bounds are "binding"
            for (i = 0; i < len; i++)
            {
                if (!isFixed[i])
                {// No need for fixed variables
                    double alpi;
                    if ((direct[i] < -m_Epsilon) && !Double.IsNaN(nwsBounds[0, i]))
                    {//Not feasible
                        alpi = (nwsBounds[0, i] - xold[i]) / direct[i];
                        if (alpi <= m_Zero)
                        { // Zero
                            x[i] = nwsBounds[0, i];
                            isFixed[i] = true; // Fix this variable
                            alpha = 0.0;
                            nwsBounds[0, i] = Double.NaN; //Add cons. to working set
                            wsBdsIndx.addElement(i);
                        }
                        else if (alpha > alpi)
                        { // Fix one variable in one iteration
                            alpha = alpi;
                            fixedOne = i;
                        }
                    }
                    else if ((direct[i] > m_Epsilon) && !Double.IsNaN(nwsBounds[1, i]))
                    {//Not feasible
                        alpi = (nwsBounds[1, i] - xold[i]) / direct[i];
                        if (alpi <= m_Zero)
                        { // Zero
                            x[i] = nwsBounds[1, i];
                            isFixed[i] = true; // Fix this variable
                            alpha = 0.0;
                            nwsBounds[1, i] = Double.NaN; //Add cons. to working set
                            wsBdsIndx.addElement(i);
                        }
                        else if (alpha > alpi)
                        {
                            alpha = alpi;
                            fixedOne = i;
                        }
                    }
                }
            }



            if (alpha <= m_Zero)
            { // Zero	   
                m_IsZeroStep = true;
                return x;
            }

            alam = alpha; // Always try full feasible newton step 
            if (alam > 1.0)
                alam = 1.0;

            // Iteration of one newton step, if necessary, backtracking is done
            double initF = fold, // Initial function value
                hi = alam, lo = alam, newSlope = 0, fhi = m_f, flo = m_f;// Variables used for beta condition
            double[] newGrad;  // Gradient on the new variable values
            bool control = true;
        kloop:
            for (k = 0; control == true; k++)
            {

                for (i = 0; i < len; i++)
                {
                    if (!isFixed[i])
                    {
                        x[i] = xold[i] + alam * direct[i];  // Compute xnew
                        if (!Double.IsNaN(nwsBounds[0, i]) && (x[i] < nwsBounds[0, i]))
                        {
                            x[i] = nwsBounds[0, i]; //Rounding error	
                        }
                        else if (!Double.IsNaN(nwsBounds[1, i]) && (x[i] > nwsBounds[1, i]))
                        {
                            x[i] = nwsBounds[1, i]; //Rounding error	
                        }
                    }
                }

                m_f = objectiveFunction(x);    // Compute fnew

                while (Double.IsPositiveInfinity(m_f))
                { // Avoid infinity

                    alam *= 0.5; // Shrink by half
                    if (alam <= m_Epsilon)
                    {

                        return x;
                    }

                    for (i = 0; i < len; i++)
                        if (!isFixed[i])
                            x[i] = xold[i] + alam * direct[i];

                    m_f = objectiveFunction(x);

                    initF = Double.PositiveInfinity;
                }

                if (m_f <= fold + m_ALF * alam * m_Slope)
                {// Alpha condition: sufficient function decrease
                    newGrad = evaluateGradient(x);
                    for (newSlope = 0.0, i = 0; i < len; i++)
                        if (!isFixed[i])
                            newSlope += newGrad[i] * direct[i];

                    if (newSlope >= m_BETA * m_Slope)
                    { // Beta condition: ensure pos. defnty.	

                        if ((fixedOne != -1) && (alam >= alpha))
                        { // Has bounds and over
                            if (direct[fixedOne] > 0)
                            {
                                x[fixedOne] = nwsBounds[1, fixedOne]; // Avoid rounding error
                                nwsBounds[1, fixedOne] = Double.NaN; //Add cons. to working set
                            }
                            else
                            {
                                x[fixedOne] = nwsBounds[0, fixedOne]; // Avoid rounding error
                                nwsBounds[0, fixedOne] = Double.NaN; //Add cons. to working set
                            }

                            isFixed[fixedOne] = true; // Fix the variable
                            wsBdsIndx.addElement(fixedOne);
                        }
                        return x;
                    }
                    else if (k == 0)
                    { // First time: increase alam 
                        // Search for the smallest value not complying with alpha condition
                        double upper = Math.Min(alpha, Maxalam);
                        while (!((alam >= upper) || (m_f > fold + m_ALF * alam * m_Slope)))
                        {
                            lo = alam;
                            flo = m_f;
                            alam *= 2.0;
                            if (alam >= upper)  // Avoid rounding errors
                                alam = upper;

                            for (i = 0; i < len; i++)
                                if (!isFixed[i])
                                    x[i] = xold[i] + alam * direct[i];
                            m_f = objectiveFunction(x);

                            newGrad = evaluateGradient(x);
                            for (newSlope = 0.0, i = 0; i < len; i++)
                                if (!isFixed[i])
                                    newSlope += newGrad[i] * direct[i];

                            if (newSlope >= m_BETA * m_Slope)
                            {
                                if ((fixedOne != -1) && (alam >= alpha))
                                { // Has bounds and over
                                    if (direct[fixedOne] > 0)
                                    {
                                        x[fixedOne] = nwsBounds[1, fixedOne]; // Avoid rounding error
                                        nwsBounds[1, fixedOne] = Double.NaN; //Add cons. to working set
                                    }
                                    else
                                    {
                                        x[fixedOne] = nwsBounds[0, fixedOne]; // Avoid rounding error
                                        nwsBounds[0, fixedOne] = Double.NaN; //Add cons. to working set
                                    }

                                    isFixed[fixedOne] = true; // Fix the variable
                                    wsBdsIndx.addElement(fixedOne);
                                }
                                return x;
                            }
                        }
                        hi = alam;
                        fhi = m_f;
                        control = false;
                        goto kloop;
                    }
                    else
                    {
                        hi = alam2; lo = alam; flo = m_f;
                        control = false;
                        goto kloop;
                    }
                }
                else if (alam < alaMin)
                { // No feasible lambda found       
                    if (initF < fold)
                    {
                        alam = Math.Min(1.0, alpha);
                        for (i = 0; i < len; i++)
                            if (!isFixed[i])
                                x[i] = xold[i] + alam * direct[i]; //Still take Alpha

                        if ((fixedOne != -1) && (alam >= alpha))
                        { // Has bounds and over
                            if (direct[fixedOne] > 0)
                            {
                                x[fixedOne] = nwsBounds[1, fixedOne]; // Avoid rounding error
                                nwsBounds[1, fixedOne] = Double.NaN; //Add cons. to working set
                            }
                            else
                            {
                                x[fixedOne] = nwsBounds[0, fixedOne]; // Avoid rounding error
                                nwsBounds[0, fixedOne] = Double.NaN; //Add cons. to working set
                            }

                            isFixed[fixedOne] = true; // Fix the variable
                            wsBdsIndx.addElement(fixedOne);
                        }
                    }
                    else
                    {   // Convergence on delta(x)
                        for (i = 0; i < len; i++)
                            x[i] = xold[i];
                        m_f = fold;
                    }

                    return x;
                }
                else
                { // Backtracking by polynomial interpolation
                    if (k == 0)
                    { // First time backtrack: quadratic interpolation
                        if (!Double.IsPositiveInfinity(initF))
                            initF = m_f;
                        // lambda = -g'(0)/(2*g''(0))
                        tmplam = -0.5 * alam * m_Slope / ((m_f - fold) / alam - m_Slope);
                    }
                    else
                    {    // Subsequent backtrack: cubic interpolation 
                        rhs1 = m_f - fold - alam * m_Slope;
                        rhs2 = fhi - fold - alam2 * m_Slope;
                        a = (rhs1 / (alam * alam) - rhs2 / (alam2 * alam2)) / (alam - alam2);
                        b = (-alam2 * rhs1 / (alam * alam) + alam * rhs2 / (alam2 * alam2)) / (alam - alam2);
                        if (a == 0.0) tmplam = -m_Slope / (2.0 * b);
                        else
                        {
                            disc = b * b - 3.0 * a * m_Slope;
                            if (disc < 0.0) disc = 0.0;
                            double numerator = -b + Math.Sqrt(disc);
                            if (numerator >= Double.MaxValue)
                            {
                                numerator = Double.MaxValue;
                            }
                            tmplam = numerator / (3.0 * a);
                        }
                        if (tmplam > 0.5 * alam)
                            tmplam = 0.5 * alam;             // lambda <= 0.5*lambda_old
                    }
                }
                alam2 = alam;
                fhi = m_f;
                alam = Math.Max(tmplam, 0.1 * alam);          // lambda >= 0.1*lambda_old

            } // Endfor(k=0;;k++)

            // Quadratic interpolation between lamda values between lo and hi.
            // If cannot find a value satisfying beta condition, use lo.
            double ldiff = hi - lo, lincr;

            while ((newSlope < m_BETA * m_Slope) && (ldiff >= alaMin))
            {
                lincr = -0.5 * newSlope * ldiff * ldiff / (fhi - flo - newSlope * ldiff);

                if (lincr < 0.2 * ldiff) lincr = 0.2 * ldiff;
                alam = lo + lincr;
                if (alam >= hi)
                { // We cannot go beyond the bounds, so the best we can try is hi
                    alam = hi;
                    lincr = ldiff;
                }
                for (i = 0; i < len; i++)
                    if (!isFixed[i])
                        x[i] = xold[i] + alam * direct[i];
                m_f = objectiveFunction(x);

                if (m_f > fold + m_ALF * alam * m_Slope)
                {
                    // Alpha condition fails, shrink lambda_upper
                    ldiff = lincr;
                    fhi = m_f;
                }
                else
                { // Alpha condition holds	    
                    newGrad = evaluateGradient(x);
                    for (newSlope = 0.0, i = 0; i < len; i++)
                        if (!isFixed[i])
                            newSlope += newGrad[i] * direct[i];

                    if (newSlope < m_BETA * m_Slope)
                    {
                        // Beta condition fails, shrink lambda_lower
                        lo = alam;
                        ldiff -= lincr;
                        flo = m_f;
                    }
                }
            }

            if (newSlope < m_BETA * m_Slope)
            { // Cannot satisfy beta condition, take lo
                alam = lo;
                for (i = 0; i < len; i++)
                    if (!isFixed[i])
                        x[i] = xold[i] + alam * direct[i];
                m_f = flo;
            }

            if ((fixedOne != -1) && (alam >= alpha))
            { // Has bounds and over
                if (direct[fixedOne] > 0)
                {
                    x[fixedOne] = nwsBounds[1, fixedOne]; // Avoid rounding error
                    nwsBounds[1, fixedOne] = Double.NaN; //Add cons. to working set
                }
                else
                {
                    x[fixedOne] = nwsBounds[0, fixedOne]; // Avoid rounding error
                    nwsBounds[0, fixedOne] = Double.NaN; //Add cons. to working set
                }

                isFixed[fixedOne] = true; // Fix the variable
                wsBdsIndx.addElement(fixedOne);
            }

            return x;
        }

        /**
         * Main algorithm.  Descriptions see "Practical Optimization"
         *
         * @param initX initial point of x, assuMing no value's on the bound!
         * @param constraints the bound constraints of each variable
         *                    constraints[0] is the lower bounds and 
         *                    constraints[1] is the upper bounds
         * @return the solution of x, null if number of iterations not enough
         * @throws Exception if an error occurs
         */
        public double[] findArgMin(double[] initX, double[,] constraints)
        {
            int l = initX.Length;

            // Initially all variables are free, all bounds are constraints of
            // non-working-set constraints
            bool[] isFixed = new bool[l];
            double[,] nwsBounds = new double[2, l];
            // Record indice of fixed variables, simply for efficiency
            DynamicIntArray wsBdsIndx = new DynamicIntArray(constraints.Length);
            // Vectors used to record the variable indices to be freed 	
            DynamicIntArray toFree = null, oldToFree = null;

            // Initial value of obj. function, gradient and inverse of the Hessian
            m_f = objectiveFunction(initX);

            double sum = 0;
            double[] grad = evaluateGradient(initX), oldGrad, oldX,
                     deltaGrad = new double[l], deltaX = new double[l],
                     direct = new double[l], x = new double[l];
            Matrix L = new Matrix(l, l);  // Lower triangle of Cholesky factor 
            double[] D = new double[l];   // Diagonal of Cholesky factor
            for (int i = 0; i < l; i++)
            {
                L.setRow(i, new double[l]);
                L.setElement(i, i, 1.0);
                D[i] = 1.0;
                direct[i] = -grad[i];
                sum += grad[i] * grad[i];
                x[i] = initX[i];
                nwsBounds[0, i] = constraints[0, i];
                nwsBounds[1, i] = constraints[1, i];
                isFixed[i] = false;
            }
            double stpMax = m_STPMX * Math.Max(Math.Sqrt(sum), l);

        iterates:
            for (int step = 0; step < m_MaxITS; step++)
            {

                // Try at most one feasible newton step, i.e. 0<lamda<=alpha
                oldX = x;
                oldGrad = grad;

                // Also update grad
                m_IsZeroStep = false;
                x = lnsrch(x, grad, direct, stpMax,
                     isFixed, nwsBounds, wsBdsIndx);


                if (m_IsZeroStep)
                { // Zero step, simply delete rows/cols of D and L
                    for (int f = 0; f < wsBdsIndx.msize(); f++)
                    {
                        int idx = wsBdsIndx.elementAt(f);
                        L.setRow(idx, new double[l]);
                        L.setColumn(idx, new double[l]);
                        D[idx] = 0.0;
                    }
                    grad = evaluateGradient(x);
                    step--;
                }
                else
                {
                    // Check converge on x
                    bool finish = false;
                    double test = 0.0;
                    for (int h = 0; h < l; h++)
                    {
                        deltaX[h] = x[h] - oldX[h];
                        double tmp = Math.Abs(deltaX[h]) /
                        Math.Max(Math.Abs(x[h]), 1.0);
                        if (tmp > test) test = tmp;
                    }
                    if (test < m_Zero)
                    {

                        finish = true;
                    }

                    // Check zero gradient	    
                    grad = evaluateGradient(x);
                    test = 0.0;
                    double denom = 0.0, dxSq = 0.0, dgSq = 0.0, newlyBounded = 0.0;
                    for (int g = 0; g < l; g++)
                    {
                        if (!isFixed[g])
                        {
                            deltaGrad[g] = grad[g] - oldGrad[g];
                            // Calculate the denoMinators			    
                            denom += deltaX[g] * deltaGrad[g];
                            dxSq += deltaX[g] * deltaX[g];
                            dgSq += deltaGrad[g] * deltaGrad[g];
                        }
                        else // Only newly bounded variables will be non-zero
                            newlyBounded += deltaX[g] * (grad[g] - oldGrad[g]);

                        // Note: CANNOT use projected gradient for testing 
                        // convergence because of newly bounded variables
                        double tmp = Math.Abs(grad[g]) *
                        Math.Max(Math.Abs(direct[g]), 1.0) /
                        Math.Max(Math.Abs(m_f), 1.0);
                        if (tmp > test) test = tmp;
                    }

                    if (test < m_Zero)
                    {
                        finish = true;
                    }

                    // dg'*dx could be < 0 using inexact lnsrch
                    // dg'*dx = 0
                    if (Math.Abs(denom + newlyBounded) < m_Zero)
                        finish = true;

                    int size = wsBdsIndx.msize();
                    bool isUpdate = true;  // Whether to update BFGS formula	    
                    // Converge: check whether release any current constraints
                    if (finish)
                    {

                        if (toFree != null)
                            oldToFree = (DynamicIntArray)toFree.copy();
                        toFree = new DynamicIntArray(wsBdsIndx.msize());

                        for (int m = size - 1; m >= 0; m--)
                        {
                            int index = wsBdsIndx.elementAt(m);
                            double[] hessian = evaluateHessian(x, index);
                            double deltaL = 0.0;
                            if (hessian != null)
                            {
                                for (int mm = 0; mm < hessian.Length; mm++)
                                    if (!isFixed[mm]) // Free variable
                                        deltaL += hessian[mm] * direct[mm];
                            }

                            // First and second order Lagrangian multiplier estimate
                            // If user didn't provide Hessian, use first-order only
                            double L1 = 0, L2 = 0;
                            if (x[index] >= constraints[1, index]) // Upper bound
                            {
                                L1 = -grad[index];
                            }
                            else
                            {
                                if (x[index] <= constraints[0, index])// Lower bound
                                {
                                    L1 = grad[index];
                                }
                            }

                            // L2 = L1 + deltaL
                            L2 = L1 + deltaL;

                            //Check validity of Lagrangian multiplier estimate
                            bool isConverge =
                                (2.0 * Math.Abs(deltaL)) < Math.Min(Math.Abs(L1),
                                                  Math.Abs(L2));
                            if ((L1 * L2 > 0.0) && isConverge)
                            { //Same sign and converge: valid
                                if (L2 < 0.0)
                                {// Negative Lagrangian: feasible
                                    toFree.addElement(index);
                                    wsBdsIndx.removeElementAt(m);
                                    finish = false; // Not optimal, cannot finish
                                }
                            }

                            // Although hardly happen, better check it
                            // If the first-order Lagrangian multiplier estimate is wrong,
                            // avoid zigzagging
                            if ((hessian == null) && (toFree != null) && toFree.Equal(oldToFree))
                                finish = true;
                        }

                        if (finish)
                        {// Min. found

                            m_f = objectiveFunction(x);

                            return x;
                        }

                        // Free some variables
                        for (int mmm = 0; mmm < toFree.msize(); mmm++)
                        {
                            int freeIndx = toFree.elementAt(mmm);
                            isFixed[freeIndx] = false; // Free this variable
                            if (x[freeIndx] <= constraints[0, freeIndx])
                            {// Lower bound
                                nwsBounds[0, freeIndx] = constraints[0, freeIndx];

                            }
                            else
                            { // Upper bound
                                nwsBounds[1, freeIndx] = constraints[1, freeIndx];

                            }
                            L.setElement(freeIndx, freeIndx, 1.0);
                            D[freeIndx] = 1.0;
                            isUpdate = false;
                        }
                    }

                    if (denom < Math.Max(m_Zero * Math.Sqrt(dxSq) * Math.Sqrt(dgSq), m_Zero))
                    {

                        isUpdate = false; // Do not update		    
                    }
                    // If Hessian will be positive definite, update it
                    if (isUpdate)
                    {

                        // modify once: dg*dg'/(dg'*dx)	
                        double coeff = 1.0 / denom; // 1/(dg'*dx)	
                        updateCholeskyFactor(L, D, deltaGrad, coeff, isFixed);

                        // modify twice: g*g'/(g'*p)	
                        coeff = 1.0 / m_Slope; // 1/(g'*p)
                        updateCholeskyFactor(L, D, oldGrad, coeff, isFixed);
                    }
                }

                // Find new direction 
                Matrix LD = new Matrix(l, l); // L*D
                double[] b = new double[l];

                for (int k = 0; k < l; k++)
                {
                    if (!isFixed[k]) b[k] = -grad[k];
                    else b[k] = 0.0;

                    for (int j = k; j < l; j++)
                    { // Lower triangle	
                        if (!isFixed[j] && !isFixed[k])
                            LD.setElement(j, k, L.getElement(j, k) * D[k]);
                    }
                }

                // Solve (LD)*y = -g, where y=L'*direct
                double[] LDIR = solveTriangle(LD, b, true, isFixed);
                LD = null;

                // Solve L'*direct = y
                direct = solveTriangle(L, LDIR, false, isFixed);

                //System.gc();
            }

            m_X = x;
            return null;
        }

        /** 
         * Solve the linear equation of TX=B where T is a triangle matrix
         * It can be solved using back/forward substitution, with O(N^2) 
         * complexity
         * @param t the matrix T
         * @param b the vector B 
         * @param isLower whether T is a lower or higher triangle matrix
         * @param isZero which row(s) of T are not used when solving the equation. 
         *               If it's null or all 'false', then every row is used.
         * @return the solution of X
         */
        public static double[] solveTriangle(Matrix t, double[] b,
                         bool isLower, bool[] isZero)
        {
            int n = b.Length;
            double[] result = new double[n];
            if (isZero == null)
                isZero = new bool[n];

            if (isLower)
            { // lower triangle, forward-substitution
                int j = 0;
                while ((j < n) && isZero[j]) { result[j] = 0.0; j++; } // go to the first row

                if (j < n)
                {
                    result[j] = b[j] / t.getElement(j, j);

                    for (; j < n; j++)
                    {
                        if (!isZero[j])
                        {
                            double numerator = b[j];
                            for (int k = 0; k < j; k++)
                                numerator -= t.getElement(j, k) * result[k];
                            result[j] = numerator / t.getElement(j, j);
                        }
                        else
                            result[j] = 0.0;
                    }
                }
            }
            else
            { // Upper triangle, back-substitution
                int j = n - 1;
                while ((j >= 0) && isZero[j]) { result[j] = 0.0; j--; } // go to the last row

                if (j >= 0)
                {
                    result[j] = b[j] / t.getElement(j, j);

                    for (; j >= 0; j--)
                    {
                        if (!isZero[j])
                        {
                            double numerator = b[j];
                            for (int k = j + 1; k < n; k++)
                                numerator -= t.getElement(k, j) * result[k];
                            result[j] = numerator / t.getElement(j, j);
                        }
                        else
                            result[j] = 0.0;
                    }
                }
            }

            return result;
        }

        /**
         * One rank update of the Cholesky factorization of B matrix in BFGS updates,
         * i.e. B = LDL', and B_{new} = LDL' + coeff*(vv') where L is a unit lower triangle
         * matrix and D is a diagonal matrix, and v is a vector.<br/>
         * When coeff > 0, we use C1 algorithm, and otherwise we use C2 algorithm described
         * in ``Methods for Modifying Matrix Factorizations'' 
         *
         * @param L the unit triangle matrix L
         * @param D the diagonal matrix D
         * @param v the update vector v
         * @param coeff the coeffcient of update
         * @param isFixed which variables are not to be updated
         */
        protected void updateCholeskyFactor(Matrix L, double[] D,
                        double[] v, double coeff,
                        bool[] isFixed)
        {
            double t, p, b;
            int n = v.Length;
            double[] vp = new double[n];
            for (int i = 0; i < v.Length; i++)
                if (!isFixed[i])
                    vp[i] = v[i];
                else
                    vp[i] = 0.0;

            if (coeff > 0.0)
            {
                t = coeff;
                for (int j = 0; j < n; j++)
                {
                    if (isFixed[j]) continue;

                    p = vp[j];
                    double d = D[j], dbarj = d + t * p * p;
                    D[j] = dbarj;

                    b = p * t / dbarj;
                    t *= d / dbarj;
                    for (int r = j + 1; r < n; r++)
                    {
                        if (!isFixed[r])
                        {
                            double l = L.getElement(r, j);
                            vp[r] -= p * l;
                            L.setElement(r, j, l + b * vp[r]);
                        }
                        else
                            L.setElement(r, j, 0.0);
                    }
                }
            }
            else
            {
                double[] P = solveTriangle(L, v, true, isFixed);
                t = 0.0;
                for (int i = 0; i < n; i++)
                    if (!isFixed[i])
                        t += P[i] * P[i] / D[i];

                double Sqrt = 1.0 + coeff * t;
                Sqrt = (Sqrt < 0.0) ? 0.0 : Math.Sqrt(Sqrt);

                double alpha = coeff, sigma = coeff / (1.0 + Sqrt), rho, theta;

                for (int j = 0; j < n; j++)
                {
                    if (isFixed[j]) continue;

                    double d = D[j];
                    p = P[j] * P[j] / d;
                    theta = 1.0 + sigma * p;
                    t -= p;
                    if (t < 0.0) t = 0.0; // Rounding error

                    double plus = sigma * sigma * p * t;
                    if ((j < n - 1) && (plus <= m_Zero))
                        plus = m_Zero; // Avoid rounding error
                    rho = theta * theta + plus;
                    D[j] = rho * d;

                    b = alpha * P[j] / (rho * d);
                    alpha /= rho;
                    rho = Math.Sqrt(rho);
                    double sigmaOld = sigma;
                    sigma *= (1.0 + rho) / (rho * (theta + rho));

                    for (int r = j + 1; r < n; r++)
                    {
                        if (!isFixed[r])
                        {
                            double l = L.getElement(r, j);
                            vp[r] -= P[j] * l;
                            L.setElement(r, j, l + b * vp[r]);
                        }
                        else
                            L.setElement(r, j, 0.0);
                    }
                }
            }
        }

        /**
         * Implements a simple dynamic array for ints.
         */
        public class DynamicIntArray
        {

            /** The int array. */
            private int[] m_Objects;

            /** The current size; */
            private int m_Size = 0;

            /** The capacity increment */
            private int m_CapacityIncrement = 1;

            /** The capacity multiplier. */
            private int m_CapacityMultiplier = 2;

            /**
             * Constructs a vector with the given capacity.
             *
             * @param capacity the vector's initial capacity
             */
            public DynamicIntArray(int capacity)
            {

                m_Objects = new int[capacity];
            }

            /**
             * Adds an element to this vector. Increases its
             * capacity if its not large enough.
             *
             * @param element the element to add
             */
            public void addElement(int element)
            {

                if (m_Size == m_Objects.Length)
                {
                    int[] newObjects;
                    newObjects = new int[m_CapacityMultiplier *
                                 (m_Objects.Length +
                                  m_CapacityIncrement)];
                    Array.Copy(m_Objects, 0, newObjects, 0, m_Size);
                    m_Objects = newObjects;
                }
                m_Objects[m_Size] = element;
                m_Size++;
            }

            /**
             * Produces a copy of this vector.
             *
             * @return the new vector
             */
            public Object copy()
            {


                DynamicIntArray copy = new DynamicIntArray(m_Objects.Length);

                copy.m_Size = m_Size;
                copy.m_CapacityIncrement = m_CapacityIncrement;
                copy.m_CapacityMultiplier = m_CapacityMultiplier;
                Array.Copy(m_Objects, 0, copy.m_Objects, 0, m_Size);
                return copy;
            }

            /**
             * Returns the element at the given position.
             *
             * @param index the element's index
             * @return the element with the given index
             */
            public int elementAt(int index)
            {

                return m_Objects[index];
            }

            /**
             * Check whether the two integer vectors equal to each other
             * Two integer vectors are equal if all the elements are the 
             * same, regardless of the order of the elements
             *
             * @param b another integer vector
             * @return whether they are equal
             */
            public bool Equal(DynamicIntArray b)
            {

                if ((b == null) || (msize() != b.msize()))
                    return false;

                int size = msize();

                // Only values matter, order does not matter
                int[] sorta = m_Objects,
                      sortb = b.m_Objects;
                for (int j = 0; j < size; j++)
                    if (m_Objects[sorta[j]] != b.m_Objects[sortb[j]])
                        return false;

                return true;
            }

            /**
             * Deletes an element from this vector.
             *
             * @param index the index of the element to be deleted
             */
            public void removeElementAt(int index)
            {

                Array.Copy(m_Objects, index + 1, m_Objects, index,
                         m_Size - index - 1);
                m_Size--;
            }

            /**
             * Removes all components from this vector and sets its 
             * size to zero. 
             */
            public void removeAllElements()
            {

                m_Objects = new int[m_Objects.Length];
                m_Size = 0;
            }

            /**
             * Returns the vector's current size.
             *
             * @return the vector's current size
             */
            public int msize()
            {

                return m_Size;
            }

        }
    }
}
