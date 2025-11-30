using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMGame_Test
{
    //input: N là 1 số nguyên range từ 1 đến 200000
    //      mỗi phần tử mảng Á là số nguyên phạm vi từ 1 đến N
    //      >1 tỷ trả về -1
    //goal : tìm số bước di chuyển nhỏ nhất để mảng A có có tính liên tục 1,2,3,4,...N
    // làm cho tất cả phần tử trong mảng không trùng nhau
    // bài này khá giống với các bài dùng greedy method khi yêu cầu tính chi phí
    public class Task3
    {
        const int Max = 1000000000;
        public static int Solution(int[] A)
        {
            // bài này để tối ưu nhất thì phải tiến hành sort mảng
            // xét ví dụ sau: không sort
            //      A= 4 1 4 2
            //target   1 2 3 4
            //A[i]  target chi phí
            //  4   1       3
            //  1   2       1
            //  4   3       1
            //  2   4       2
            // tổng move là 7

            // ví dụ có sort 
            //      A= 1 2 4 4 sort
            //target   1 2 3 4
            //A[i]  target chi phí
            //  1   1       0
            //  2   2       0
            //  4   3       1
            //  4   4       0
            // tổng move là 1 ( đúng đắn với ycbt)


            Array.Sort(A);
            int count = 0;
            for (int i = 0; i < A.Length; i++)
            {
                int target = i + 1;
                if (A[i] > target)
                {
                    count += A[i] - target;
                }
                else
                {
                    count += target - A[i];
                }    
            }
            if (count > Max)
                return -1;

            return count;
        }
    }
}
