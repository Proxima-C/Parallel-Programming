using System.Diagnostics;

namespace DCS._1;

public static class Program1
{
    private static void Main(string[] args)
    {
        const int n = 10;
        Console.WriteLine($"Dimention: {n}");

        var c = Helper.GenerateVector(n);
        var d = Helper.GenerateVector(n);
        var m = Helper.GenerateVector(n);

        var mc = Helper.GenerateMatrix(n);
        var mm = Helper.GenerateMatrix(n);
        var mz = Helper.GenerateMatrix(n);

        Helper.SaveDataToFile(c, d, m, mc, mm, mz, "data1.txt");

        var stopwatch = Stopwatch.StartNew();

        var thread1 = new Thread(() => CalculateMF(c, d, mc, mm, mz));
        var thread2 = new Thread(() => CalculateX(c, d, m, mc));

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();

        stopwatch.Stop();

        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}");
    }

    private static double[,] CalculateMF(double[] c, double[] d, double[,] mC, double[,] mM, double[,] mZ)
    {
        Console.WriteLine("T1 Start CalculateMF");

        var dimension = c.Length;

        var localC = new double[dimension];
        var localD = new double[dimension];

        Array.Copy(c, localC, dimension);
        Array.Copy(d, localD, dimension);

        var localMC = (double[,])mC.Clone();
        var localMM = (double[,])mM.Clone();
        var localMZ = (double[,])mZ.Clone();

        double[,] minCD_MC_MZ = new double[dimension, dimension];
        double[,] MM_MC_MM = new double[dimension, dimension];

        Parallel.Invoke(
            () =>
            {
                Console.WriteLine("T1: Start calculating min(C-D)");
                var minCD = Helper.SubtractVectors(localC, localD).Min();
                Console.WriteLine("T1: Finish calculating min(C-D)");

                Console.WriteLine("T1: Start calculating min(C-D) * MC");
                var minCD_MC = Helper.MultiplyScalarMatrix(minCD, localMC);
                Console.WriteLine("T1: Finish calculating min(C-D) * MC");

                Console.WriteLine("T1: Start calculating min(C-D) * MC * MZ");
                minCD_MC_MZ = Helper.MultiplyMatrix(minCD_MC, localMZ);
                Console.WriteLine("T1: Finish calculating min(C-D) * MC * MZ");
            },
            () =>
            {
                Console.WriteLine("T1: Start calculating (MC + MM)");
                var MC_MM = Helper.AddMatrix(localMC, localMM);
                Console.WriteLine("T1: Finish calculating (MC + MM)");

                Console.WriteLine("T1: Start calculating MM * (MC + MM)");
                MM_MC_MM = Helper.MultiplyMatrix(localMM, MC_MM);
                Console.WriteLine("T1: Finish calculating MM * (MC + MM)");
            }
        );

        Console.WriteLine("T1: Start calculating min(C-D) * MC * MZ + MM * (MC + MM)");
        var MF = Helper.AddMatrix(minCD_MC_MZ, MM_MC_MM);
        Console.WriteLine("T1: Finish calculating min(C-D) * MC * MZ + MM * (MC + MM)");

        Console.WriteLine("T1: Finish CalculateMF");
        return MF;
    }

    private static double[] CalculateX(double[] c, double[] d, double[] m, double[,] mC)
    {
        Console.WriteLine("T2 Start CalculateX");

        var dimension = c.Length;

        var localC = new double[dimension];
        var localD = new double[dimension];
        var localM = new double[dimension];

        Array.Copy(c, localC, dimension);
        Array.Copy(d, localD, dimension);
        Array.Copy(localM, localM, dimension);

        var localMC = (double[,])mC.Clone();

        Console.WriteLine("T2 Start calculating MC * M");
        var MC_M = Helper.MultiplyVectorMatrix(localM, localMC);
        Console.WriteLine("T2 Finish calculating MC * M");

        Console.WriteLine("T2 Start calculating MC * M + D");
        var MC_M_D = Helper.AddVectors(MC_M, localD); ;
        Console.WriteLine("T2 Finish calculating MC * M + D");

        Console.WriteLine("T2 Start calculating SORT(MC * M + D - C)");
        var X = Helper.SubtractVectors(MC_M_D, localC);
        Console.WriteLine("T2 Finish calculating SORT(MC * M + D - C)");
        Array.Sort(X);

        Console.WriteLine("T2 Finish CalculateX");

        return X;
    }
}