using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSplit
{
    class TEMClust
    {
        public TData Data;

        public int K;
        public int L;

        Random rnd;

        public double[,] mu;
        public double[,] R;
        public double[] W;
        public double[] SP;
        public double[,] P;
        public double[,] delta;
        public double[,] x;

        public double eps;

        public TEMClust(TData Data)
        {
            this.Data = Data;
            rnd = new Random();
        }

        public void Run(int K, double eps, int L)
        {
            this.K = K;
            this.L = L;

            this.eps = eps;

            mu = new double[Data.N, K];
            R = new double[Data.N, K];
            W = new double[K];

            SP = new double[Data.M];
            P = new double[Data.M, K];
            delta = new double[Data.M, K];
            x = new double[Data.M, K];

            for (int n = 0; n < Data.N; n++)
            {
                for (int k = 0; k < K; k++)
                {
                    mu[n, k] = (0.5 - rnd.NextDouble());
                    R[n, k] = 1;
                }

            }

            for (int k = 0; k < K; k++)
            {
                W[k] = 1.0 / K;
            }

            double llh = 0;

            for (int l = 0; l < L; l++)
            {

                double llh1 = Step();

                if (Math.Abs(llh - llh1) < eps)
                {
                    break;
                }

                llh = llh1;

            }
        }

        double Step()
        {
            InitTemp();

            // E

            double llh = 0;

            for (int m = 0; m < Data.M; m++)
            {
                SP[m] = 0;

                for (int k = 0; k < K; k++)
                {
                    delta[m, k] = innerR(m, k);

//                    Console.WriteLine("delta = {0}", delta[m, k]);

                    P[m, k] = Gauss(m, k, delta[m, k]);

                    if(P[m, k] != P[m, k])
                    {
                        Console.WriteLine("P[mk] = {0}", P[m, k]);
                    }

//                    Console.WriteLine("P[mk] = {0}", P[m, k]);

                    SP[m] += P[m, k];
                }

                for (int k = 0; k < K; k++)
                {
                    x[m, k] = P[m, k] / SP[m];

                    if (x[m, k] != x[m, k])
                    {
                        Console.WriteLine("P[{0}, {1}] = {2}", m, k, P[m, k]);
                        Console.WriteLine("SP[{0}] = {1}", m, SP[m]);

                        x[m, k] = 0;
                    }


                }

                llh += Math.Log(SP[m]);

                for (int n = 0; n < Data.N; n++)
                {
                    for (int k = 0; k < K; k++)
                    {
                        mu1[n, k] += Data.X[m, n] * x[m, k];

                        if (mu1[n, k] != mu1[n, k])
                        {
//                            Console.WriteLine("mu1[{0}, {1}] не число", n, k);
//                            Console.WriteLine("x[{0}, {1}] = {2}", m, k, x[m, k]);
                        }

                        //                        Console.WriteLine("mu1[{0}, {1}] = {2}", n, k, mu1[n, k]);
                    }
                }

                for (int k = 0; k < K; k++)
                {
                    W1[k] += x[m, k];
//                    Console.WriteLine("W1[{0}] = {1}", k, W1[k]);
                }
            }

            // M

            for (int k = 0; k < K; k++)
            {
                for (int n = 0; n < Data.N; n++)
                {
                    if (W1[k] == 0)
                    {
                        Console.WriteLine("W1[{0}] = {1}", k, W1[k]);
                    }

                    if(mu1[n, k] != mu1[n, k])
                    {
                        Console.WriteLine("mu1[{0}, {1}] не число", n, k);
                    }

//                    Console.WriteLine(" / W1[{0}] = {1}", k, W1[k]);
                    mu[n, k] = mu1[n, k] / W1[k];
                }

                for (int m = 0; m < Data.M; m++)
                {
                    for (int n = 0; n < Data.N; n++)
                    {
                        R1[n, k] += (Data.X[m, n] - mu[n, k]) * (Data.X[m, n] - mu[n, k]) * x[m, k];
                    }
                }
            }

            

            for (int k = 0; k < K; k++)
            {
                for (int n = 0; n < Data.N; n++)
                {
                    R[n, k] = R1[n, k] / Data.M;

                    if (R[n, k] == 0)
                    {
//                        Console.WriteLine("!!! R = 0!");
                    }
                }
            }

            for (int k = 0; k < K; k++)
            {
                W[k] = W1[k] / Data.M;
            }

            return llh;
        }

        double Gauss(int m, int k, double delta)
        {
            double res = 0;

//            Console.WriteLine("delta = {0}", delta);

            if (delta > 100)
            {
//                Console.WriteLine("delta = {0}\t{1}", delta, Sigma(k));
//                return 0;
            }

            double sigma = Sigma(k);

            //if (sigma < 1e-17)
            //{
            //    return 1e17;
            //}

            //if (sigma > 1e17)
            //{
            //    return 1e-17;
            //}

            res = Math.Exp(-delta / 2) * W[k] / (Math.Pow(2 * Math.PI, Data.N / 2) * sigma);

            if (res != res)
            {
                Console.WriteLine("Gauss не число\tdelta = {0}\t sigma = {1}", delta, sigma);
            }

            return res;
        }

        double Sigma(int k)
        {
            double res = 1;

            for (int n = 0; n < Data.N; n++)
            {
                res *= R[n, k];

            }

//            Console.WriteLine("Sigma = {0}", res);
            
            res = Math.Sqrt(res);

            return res;
        }

        double inner(double[] a, double[] b)
        {
            double res = 0;

            for (int i = 0; i < a.Length; i++)
            {
                res += a[i] * b[i];
            }

            return res;
        }

        double innerR(int m, int k)
        {
            double res = 0;

            for (int n = 0; n < Data.N; n++)
            {
                if (R[n, k] < 1e-17)
                {
                    return 1024;
                }

                res += (Data.X[m, n] - mu[n, k]) * (Data.X[m, n] - mu[n, k]) / R[n, k];
            }

//            Console.WriteLine("\t\t\tfor delta = {0}", res);

            if (res != res)
            {
                Console.WriteLine("!!!");

                for (int n = 0; n < Data.N; n++)
                {
                    Console.WriteLine("Data = {0}\tmu = {1}\tR = {2}", Data.X[m, n], mu[n, k], R[n, k]);
                }
            }

            return res;
        }

        public double[,] mu1;
        public double[,] R1;
        public double[] W1;

        void InitTemp()
        {
            mu1 = new double[Data.N, K];
            R1 = new double[Data.N, K];
            W1 = new double[K];

        }
    }
}
