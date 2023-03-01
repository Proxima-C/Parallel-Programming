using System.Text;

namespace DCS._1;

internal static class Helper
{
    public static double[] AddVectors(double[] a, double[] b)
    {
        double[] result = new double[a.Length];

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = KahanAdd(a[i], b[i]);
        }

        return result;
    }

    public static string MatrixToString(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);
        var sb = new StringBuilder();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                sb.Append(matrix[i, j]);
                if (j < columns - 1)
                {
                    sb.Append(",");
                }
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static string VectorToString(double[] vector)
    {
        int length = vector.Length;
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(vector[i].ToString());
            if (i < length - 1)
            {
                sb.Append(",");
            }
        }
        sb.AppendLine();
        return sb.ToString();
    }

    public static double[,] AddMatrix(double[,] a, double[,] b)
    {
        double[,] result = new double[a.GetLength(0), a.GetLength(1)];

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                result[i, j] = KahanAdd(a[i, j], b[i, j]);
            }
        }

        return result;
    }

    public static double[,] GenerateMatrix(int n)
    {
        var matrix = new double[n, n];
        var random = new Random();

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                matrix[i, j] = random.NextDouble() * 100;
            }
        }

        return matrix;
    }

    public static double[] GenerateVector(int n)
    {
        var vector = new double[n];
        var random = new Random();

        for (var i = 0; i < n; i++)
        {
            vector[i] = random.NextDouble() * 100;
        }

        return vector;
    }

    private static double KahanAdd(params double[] values)
    {
        double sum = 0;
        double c = 0;

        foreach (double value in values)
        {
            double y = value - c;
            double t = sum + y;
            c = (t - sum) - y;
            sum = t;
        }

        return sum;
    }

    public static double[,] MultiplyMatrix(double[,] a, double[,] b)
    {
        double[,] result = new double[a.GetLength(0), b.GetLength(1)];

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < b.GetLength(1); j++)
            {
                double sum = 0;

                for (int k = 0; k < a.GetLength(1); k++)
                {
                    sum += a[i, k] * b[k, j];
                }

                result[i, j] = sum;
            }
        }

        return result;
    }

    public static double[,] MultiplyScalarMatrix(double a, double[,] b)
    {
        double[,] result = new double[b.GetLength(0), b.GetLength(1)];

        for (int i = 0; i < b.GetLength(0); i++)
        {
            for (int j = 0; j < b.GetLength(1); j++)
            {
                result[i, j] = a * b[i, j];
            }
        }

        return result;
    }

    public static double[] MultiplyVectorMatrix(double[] a, double[,] b)
    {
        double[] result = new double[a.Length];

        for (int i = 0; i < a.Length; i++)
        {
            double sum = 0;

            for (int j = 0; j < a.Length; j++)
            {
                sum += b[i, j] * a[j];
            }

            result[i] = sum;
        }

        return result;
    }

    public static void SaveDataToFile(double[] c, double[] d, double[] m, double[,] mc, double[,] mm, double[,] mz, string path)
    {
        var n = c.Length;

        using var writer = new StreamWriter(path);

        writer.WriteLine(n);

        writer.WriteLine($"C: [{VectorToString(c)}]");
        writer.WriteLine($"D: [{VectorToString(d)}]");
        writer.WriteLine($"M: [{VectorToString(m)}]");

        writer.WriteLine($"MC: [{MatrixToString(mc)}]");
        writer.WriteLine($"MM: [{MatrixToString(mm)}]");
        writer.WriteLine($"MZ: [{MatrixToString(mz)}]");
    }

    public static double[] SubtractVectors(double[] a, double[] b)
    {
        double[] result = new double[a.Length];

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] - b[i];
        }

        return result;
    }
}
