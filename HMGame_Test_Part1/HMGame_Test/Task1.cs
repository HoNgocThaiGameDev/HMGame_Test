using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMGame_Test
{

    /// <summary>
    /// a b c d e f g h i j k l m n o p q r s t u v w x y z
    ///input n: from 2 to 100000
    ///        chữ thường từ a-z
    ///goal: tăng dần theo giá trị ascii
    /// độ phức tạp O(n) (1 vòng for)
    /// </summary>
    

    public class Task1
    {
        public static string Solution(string str)
        {
            //0 aa
            //1 aa
            //2 aa
            for(int i=0;i< str.Length-1;i++)
            {
                if (str[i] > str[i+1])
                {
                    return str.Remove(i,1);
                }    
            }
            // nếu tăng dần thì có thể dễ dàng thấy loại bỏ kí tự cuối 
            // ví dụ hot thì value cuối là ho
            return str.Remove(str.Length - 1, 1);
        }
    }
}
