using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSplit
{
    class TEM
    {
        public int N;
        public int M;
        public int K;
        public int L;

        public TData Data;

        public TEM(TData Data, int K, int L)
        {
            this.Data = Data;
            this.K = K;
            this.L = L;

            N = Data.N;
            M = Data.M;

            Run();
        }

        public double[,] G()
        {
            return g;
        }


        void Run()
        {
            Init();

            for (int l = 0; l < L; l++)
            {
                Step();
            }
        }

        void Step()
        {
            // E

            for (int k = 0; k < K; k++)
            {
                for (int m = 0; m < M; m++)
                {
                    double b = 0;
                    for (int k2 = 0; k2 < K; k2++)
                    {
                        b += W[k2] * F(k2, m);
                    }

                    g[k, m] = W[k] * F(k, m) / b;
                }
            }

            // M

            for (int k = 0; k < K; k++)
            {
                W[k] = 0;

                for (int m = 0; m < M; m++)
                {
                    W[k] += g[k, m];
                }

                W[k] /= M;

                for (int n = 0; n < N; n++)
                {
                    mu[k, n] = 0;

                    for (int m = 0; m < M; m++)
                    {
                        mu[k, n] += g[k, m] * Data.X[m, n];
                    }

                    mu[k, n] /= M * W[k];
                }
            }

            for (int k = 0; k < K; k++)
            {
                for (int n = 0; n < N; n++)
                {
                    s[k, n] = 0;

                    for (int m = 0; m < M; m++)
                    {
                        s[k, n] += g[k, m] * (Data.X[m, n] - mu[k, n]) * (Data.X[m, n] - mu[k, n]);
                    }

                    s[k, n] /= M * W[k];
                }
            }

        }

        double[] W;
        double[,] mu;
        double[,] s;
        double[,] g;

        double F(int k, int m)
        {
            double res = 0;

            double s1 = 1;

            for (int n = 0; n < N; n++)
            {
                s1 *= s[k, n];
            }

            for (int n = 0; n < N; n++)
            {
                res += (Data.X[m, n] - mu[k, n]) * (Data.X[m, n] - mu[k, n]) / s[k, n];
            }

//            Console.WriteLine(res);

            res = Math.Exp(-res / 2);

            res /= Math.Sqrt(s1);
            res /= Math.Pow(2 * Math.PI, N / 2);

            return res;
        }


        void Init()
        {
            Random rnd = new Random();

            W = new double[K];
            for (int k = 0; k < K; k++)
            {
                W[k] = 1.0 / K;
            }

            mu = new double[K, N];

            for (int k = 0; k < K; k++)
            {
                for (int n = 0; n < N; n++)
                {
                    mu[k, n] = Data.X[k, n];
                }
            }

            s = new double[K, N];

            for (int k = 0; k < K; k++)
            {
                for (int n = 0; n < N; n++)
                {
                    s[k, n] = 0;

                    for (int m = 0; m < M; m++)
                    {
                        s[k, n] += (Data.X[m, n] - mu[k, n]) * (Data.X[m, n] - mu[k, n]);
                    }

                    s[k, n] /= M * K;

                    s[k, n] = 1;
                }
            }

            g = new double[K, M];
        }
    }
}
