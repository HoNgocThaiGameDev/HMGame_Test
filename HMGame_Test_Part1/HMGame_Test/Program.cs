using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMGame_Test
{
    internal class Program
    {
        private const int CONSTANT_TASK1 = 100000;
        private const int CONSTANT_TASK2 = 1000000000;
        static void Main(string[] args)
        {
            //TASK1---------------------------------------------
            //while (true)
            //{
            //    Console.Write("nhap chuoi S: ");
            //    string input = Console.ReadLine();

            //    // kiem tra điều kiện trước
            //    if (string.IsNullOrEmpty(input) || input.Length > CONSTANT_TASK1 || input.Length < 2)
            //    {
            //        Console.WriteLine("chua thoa yeu cau bai toan ve input");
            //        return;
            //    }

            //    // điều kiện input thứ 2
            //    foreach (char c in input)
            //    {
            //        if (c < 'a' || c > 'z')
            //        {
            //            Console.WriteLine("chi duoc chua ki tu thuong tu a den z");
            //            return;
            //        }
            //    }

            //    string res = Task1.Solution(input);
            //    Console.WriteLine(res);
            //    Console.ReadKey();
            //}



            //TASK2---------------------------------------------------------------------
            //while (true)
            //{
            //    Console.WriteLine("\nNhap du lieu ma tran: ");
            //    int N = TypingInputData("Nhap so hang N(2 den 600): ", 2, 600);
            //    int M = TypingInputData("Nhap so cot M(2 den 600): ", 2, 600);

            //    int[][] matrix = new int[N][];
            //    bool isError = false;
            //    Console.WriteLine($"nhap {N} dong, moi dong chua {M} so nguyen cach nhau khoang trang :");
            //    for (int i = 0; i < N; i++)
            //    {
            //        Console.Write($"Hang {i}: ");
            //        string line = Console.ReadLine();
            //        try
            //        {
            //            //tách string để lấy giá trị từng phần tử ma trận
            //            int[] row = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            //            if (row.Length != M)
            //            {
            //                Console.WriteLine($"loi: hang {i} ban nhap phai co dung {M} so ");
            //                isError = true;
            //                break;
            //            }

            //            foreach (int value in row)
            //            {
            //                // kiem tra điều kiện 2
            //                if (value < 0 && value > CONSTANT_TASK2)
            //                {
            //                    Console.WriteLine($"loi: gia tri phai tu 0 den 1,000,000,000");
            //                    isError = true;
            //                    break;
            //                }
            //            }
            //            if (isError)
            //                break;

            //            matrix[i] = row;
            //        }
            //        catch
            //        {
            //            Console.WriteLine("loi sai dinh dang so");
            //            isError = true;
            //            break;
            //        }
            //    }

            //    if (isError)
            //        continue;
            //    int res_task2= Task2.Solution(matrix);
            //    Console.WriteLine($"ket qua: {res_task2}");
            //}

            //TASK 3----------------------------------------------------------------

            //while(true)
            //{
            //    int N = TypingInputData("Nhap N: ",1,200000);
            //    if(N <= 0)
            //        break;
            //    int[] A = null;
            //    while(A == null)
            //    {
            //        Console.WriteLine($"nhap {N} so nguyen cach nhau khoang trang: ");
            //        string line= Console.ReadLine();
            //        try
            //        {
            //            var part = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            //            if (part.Length != N)
            //            {
            //                Console.WriteLine($" ban da nhap {part.Length} so ( de yeu cau nhap {N} so)");
            //                continue;
            //            }

            //            A = new int[N];
            //            bool valid = true;

            //            for (int i = 0; i < N; i++)
            //            {
            //                int val = int.Parse(part[i]);
            //                if (val < 1 || val > N)
            //                {
            //                    Console.WriteLine($"phan tu thu {i + 1} co gia tri {val} phai trong khoang 1 den {N}");
            //                    valid = false;
            //                    break;
            //                }
            //                A[i] = val;
            //            }
            //            if (!valid) A = null;
            //        }
            //        catch
            //        {
            //            Console.WriteLine("kieu so nhap khong hop le");
            //        }
            //    }
            //    int res = Task3.Solution(A);
            //    Console.WriteLine($"ket qua buoc di chuyen it nhat:{res} ");
            //}    
        }

        public static int TypingInputData(string str, int min, int max)
        {
            while (true)
            {
                Console.Write(str);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value >= min && value <= max)
                {
                    return value;
                }
                Console.WriteLine($"loi: hay nhap so nguyen tu  {min} den {max}");
            }
        }
    }
}
