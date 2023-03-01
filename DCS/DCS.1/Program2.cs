using System.Diagnostics;

namespace DCS._1;

public static class Program2
{
    private const int n = 10;

    private static double[] c = Helper.GenerateVector(n);
    private static double[] d = Helper.GenerateVector(n);
    private static double[] m = Helper.GenerateVector(n);

    private static double[,] mc = Helper.GenerateMatrix(n);
    private static double[,] mm = Helper.GenerateMatrix(n);
    private static double[,] mz = Helper.GenerateMatrix(n);

    private static SemaphoreSlim semaphore = new(1);

    private static void Main(string[] args)
    {
        Console.WriteLine($"Dimention: {n}");

        Helper.SaveDataToFile(c, d, m, mc, mm, mz, "data2.txt");

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

        double[,] minCD_MC_MZ = new double[dimension, dimension];
        double[,] MM_MC_MM = new double[dimension, dimension];

        Console.WriteLine("T1 Start calculating min(C-D)");
        var minCD = Helper.SubtractVectors(c, d).Min();
        Console.WriteLine("T1 Finish calculating min(C-D)");

        semaphore.Wait();
        Console.WriteLine("T1 Start calculating min(C-D) * MC");
        var minCD_MC = Helper.MultiplyScalarMatrix(minCD, mC);
        Console.WriteLine("T1 Finish calculating min(C-D) * MC");
        semaphore.Release();

        semaphore.Wait();
        Console.WriteLine("T1 Start calculating min(C-D) * MC * MZ");
        minCD_MC_MZ = Helper.MultiplyMatrix(minCD_MC, mZ);
        Console.WriteLine("T1 Finish calculating min(C-D) * MC * MZ");
        semaphore.Release();

        semaphore.Wait();
        Console.WriteLine("T1 Start calculating (MC + MM)");
        var MC_MM = Helper.AddMatrix(mC, mM);
        Console.WriteLine("T1 Finish calculating (MC + MM)");
        semaphore.Release();

        semaphore.Wait();
        Console.WriteLine("T1 Start calculating MM * (MC + MM)");
        MM_MC_MM = Helper.MultiplyMatrix(mM, MC_MM);
        Console.WriteLine("T1 Finish calculating MM * (MC + MM)");
        semaphore.Release();

        semaphore.Wait();
        Console.WriteLine("T1 Start calculating min(C-D) * MC * MZ + MM * (MC + MM)");
        var MF = Helper.AddMatrix(minCD_MC_MZ, MM_MC_MM);
        Console.WriteLine("Finish calculating min(C-D) * MC * MZ + MM * (MC + MM)");
        semaphore.Release();

        Console.WriteLine("T1 Finish CalculateMF");
        return MF;
    }

    private static double[] CalculateX(double[] c, double[] d, double[] m, double[,] mC)
    {
        Console.WriteLine("T2 Start CalculateX");

        var dimension = c.Length;

        semaphore.Wait();
        Console.WriteLine("T2 Start calculating MC * M");
        var MC_M = Helper.MultiplyVectorMatrix(c, mC);
        Console.WriteLine("T2 Finish calculating MC * M");
        semaphore.Release();

        semaphore.Wait();
        Console.WriteLine("T2 Start calculating MC * M + D");
        var MC_M_D = Helper.AddVectors(MC_M, d);
        Console.WriteLine("T2 Finish calculating MC * M + D");
        semaphore.Release();

        semaphore.Wait();
        Console.WriteLine("T2 Start calculating SORT(MC * M + D - C)");
        var X = Helper.SubtractVectors(MC_M_D, c);
        Array.Sort(X);
        Console.WriteLine("T2 Finish calculating SORT(MC * M + D - C)");
        semaphore.Release();

        Console.WriteLine("T2 Finish CalculateX");
        return X;
    }
}
