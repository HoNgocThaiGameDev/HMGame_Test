using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HMGame_Test
{
    /// <summary>
    /// input n và m là số nguyên từ 2 đến 600
    /// ma trận đều là số nguyên từ 0 đến 1 tỷ
    /// goal : cho ma trận A n hàng và m cột , tìm 2 phần tử sao cho hàng và cột mỗi phần tử không trùng nhau
    /// sao cho sum 2 phần tử lớn nhất
    /// 
    /// độ phức tạp của hàm này O(N*M + N*N); ( 2 lần với 2 for lồng nhau)
    /// </summary>
    public class Task2
    {
        public static int Solution(int[][] A)
        {
            int N=A.Length;
            int M= A[0].Length;

            // trước tiên đi tìm giá trị top1 và top2 mỗi hàng
            // lý do : dễ thấy bởi vì ở ví dụ 2: 
            //xét hàng 0: 15 là top1 
            // hàng 1 : 16 là top1 nhưng lại trùng cột không thỏa ycbt
            // nên phải bắt chéo giá trị 

            // tạo 1 tuple để lưu giá trị và index cột
            var rowCollectionTop = new (int val, int colIndex)[N][];

            // tiếp đến tiến hành đi vét cạn tìm giá trị top1 và top2
            for(int i=0;i<N;i++)
            {
                int max1 = 0, col1 = 0, max2 = 0, col2 = 0;
                //tiến hành duyệt từng hàng của ma trận 
                for( int j=0;j<M;j++)
                {
                    int value_temp= A[i][j];
                    // nếu như giá trị đang duyệt lớn nhất, gán max1 là A[i][j]
                    if(value_temp >max1)
                    {
                        // vì xét đến trường hợp này thì phải gán giá trị max 2 chưa được gắn nên lần chạy trước lỗi
                        max2 = max1;
                        col2 = col1;

                        max1 = value_temp;
                        col1 = j;
                    }
                    // nếu như nhỏ hơn max 1 mà lớn hơn max 2 thì gán nó là max 2
                    else if(value_temp > max2)
                    {
                        max2 = value_temp;
                        col2 = j;
                    }
                    // sau đó lưu vào tuple 
                    rowCollectionTop[i] = new (int, int)[] { (max1, col1), (max2, col2) };
                    
                    // tiếp đến bước cộng 2 phần tử sao cho đạt max


                }    
            }


            int sumMax = 0;
            for(int i=0;i<N;i++)
            {
                // j=i để tối ưu số lần duyệt, không bị trùng lặp, mỗi cặp sẽ xuất hiện 1 lần 
                // ví dụ : có các số 0 1 2 3 4 5...N+1
                //i=0 j= 1 2 3
                // i=1 j= 2 3
                // I=2 J=3
                //ta được các cặp 0 1 0 2 0 3 i=0
                //                1 2 1 3     i=1
                //                2 3         i=2

                for(int j=i+1;j<N;j++)
                {
                    var r1Top= rowCollectionTop[i][0];
                    var r2Top= rowCollectionTop[j][0];

                    // xử lý trường hợp ứng với ví dụ 1 đề cho
                    if(r1Top.colIndex != r2Top.colIndex)
                    {
                        sumMax = Math.Max(sumMax, r1Top.val + r2Top.val);
                    }
                    else // xử lý trường hợp ứng với ví dụ 2 bị trùng cột (15 và 16)
                    {
                        // 15 1 5
                        // 16 3 8
                        // 2  6 4
                        
                        //lấy giá trị lớn nhất của hàng i + với giá trị lớn thứ 2 hàng j
                        int wayA = r1Top.val + rowCollectionTop[j][1].val;
                        // tương tự
                        int wayB = r2Top.val + rowCollectionTop[i][1].val;
                        sumMax = Math.Max(sumMax,Math.Max(wayA,wayB));

                    }    
                }    
            }    
            return sumMax;
        }
    }
}
