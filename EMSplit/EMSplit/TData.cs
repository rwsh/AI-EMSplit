using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace EMSplit
{
    class TData
    {
        public int N = 0;
        public int M = 0;

        public double[,] X;

        public double[] Min, Max;

        public TData(string Name)
        {
            ArrayList SData = new ArrayList();

            StreamReader f = new StreamReader(Name);

            string line;

            while ((line = f.ReadLine()) != null)
            {
                string[] SS = line.Split('\t');
                M++;

                double[] D = new double[SS.Length];

                for (int n = 0; n < SS.Length; n++)
                {
                    D[n] = double.Parse(SS[n]);
                }

                SData.Add(D);
            }

            f.Close();

            N = ((double[])SData[0]).Length;

            X = new double[M, N];

            Min = new double[N];
            Max = new double[N];


            for (int n = 0; n < N; n++)
            {
                Min[n] = double.MaxValue;
                Max[n] = double.MinValue;

                for (int m = 0; m < M; m++)
                {
                    X[m, n] = ((double[])SData[m])[n];

                    if (X[m, n] < Min[n])
                    {
                        Min[n] = X[m, n];
                    }

                    if (X[m, n] > Max[n])
                    {
                        Max[n] = X[m, n];
                    }
                }
            }

            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < M; m++)
                {
                    X[m, n] = (X[m, n] - Min[n]) / (Max[n] - Min[n]);
                }
            }
        }
    }
}
