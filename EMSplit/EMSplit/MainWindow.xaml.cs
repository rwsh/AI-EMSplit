using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace EMSplit
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TData Data = new TData("t.txt");

            //TEMClust EM = new TEMClust(Data);

            //EM.Run(4, 10, 10);

            TEM EM = new TEM(Data, 2, 10);

            StreamWriter f = new StreamWriter("out.txt");

            for (int m = 0; m < Data.M; m++)
            {
                for (int k = 0; k < EM.K; k++)
                {
                    f.Write("{0}\t", EM.G()[k, m]);
                    Console.Write("{0}\t", EM.G()[k, m]);
                }

                f.WriteLine();
                Console.WriteLine();
            }

            f.Close();
        }
    }
}
