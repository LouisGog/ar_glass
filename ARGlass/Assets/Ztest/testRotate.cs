using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class testRotate : MonoBehaviour {

    public Text text;

	public Transform cube1;
	public Transform cube2;
    Dictionary<string, string> dic = new Dictionary<string, string>();
    private List<int> list;

    private Solution st;
    Stopwatch sw;
   void TestMethod()
    {
        for (int i = 0; i < 10000000; i++)
        {
        }
    }

    void test()
    {
        sw.Start();
        TestMethod();
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("total: {0} ms", sw.ElapsedMilliseconds));
    }

    private ItemCreator creator;

    void Start () {

     //   creator = new ItemCreator();

     //   sw = new Stopwatch();



      //  InvokeRepeating("test",0,0.3f);
        // text.text = Application.persistentDataPath;
        //dic["name"] = "yifan " +AppConst.dataToken;
        //dic.Add("sex","men");
        //foreach(KeyValuePair<string,string> temp in dic)
        //{
        //    Debug.Log(temp.Key + " : " + temp.Value);
        //}
        // list = new List<int>();
        // for(int i= 0; i < 3;i ++)
        // {
        //     int n = i;
        //     list.Add(n);
        //     Debug.Log(n);
        // }

        //for(int i=0; i < list.Count; i++)
        // {
        //     Debug.Log(list[i] + 4);
        // }
        st = new Solution();
        int[] coins = new int[3] { 1,2,5}; 
        int n = st.CoinChange(coins, 11);
        // UnityEngine.Debug.Log(n);



        // int max = st.LengthofLongestSubstring("abcabcbb");
        // UnityEngine.Debug.Log( "max :" +max);



        //string temp = st.LongestPalindrome("abccbade");
        int temp = st.RemoveDuplicates(new int[]{1,1,1,1,2 });
        UnityEngine.Debug.Log(temp);



      //  int result = st.ReverseInteger(123);
      //  int result1 = st.ThreeSumClosest(new int[4] { -1,2,1,-4},1);
        UnityEngine.Debug.Log(st.IsValid("()[]"));

       
        
        // UnityEngine.Debug.Log();
        //-------------------------------------------------------------------------------------
        ArraySegment<int> segent = new ArraySegment<int>(coins,1,1);
        for(int a= segent.Offset; a<segent.Offset + segent.Count; a++)
        {
            UnityEngine.Debug.Log(segent.Array[a]);
        }
        //------------------------------------------------------------------------------------

        //泛型代理
        int i, j;
        i = 3; j = 5;
        delegateSample<int> sample = new delegateSample<int>(Swap);
        sample(ref i,ref j);
        UnityEngine.Debug.Log("i = " + i+ " ," + "j = " + j);
	}

    public delegate void delegateSample<T>(ref T x, ref T y);

    public void Swap<T>(ref T a,ref T b)
    {
        T temp = a;
        a = b;
        b = temp; 
    }

	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            creator.SpawnObjByName("unity_1");


        }

        // Vector3 t = cube1.transform.rotation.eulerAngles;
        // Debug.Log("x :" + t.z);
        //Debug.Log("y :" + t.y);
        //Debug.Log("z :" + t.z);

    }
}

public class Solution
{
    /// <summary>
    /// 计算硬币
    /// </summary>
    /// <param name="coins"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int CoinChange(int[] coins, int amount)
    {
        if (amount == 0)
        {
            return 0;
        }
        if (coins.Length == 0)
        {
            return -1;
        }
        Array.Sort(coins);

        int[] DP = new int[amount + 1];

        for (int i = 1; i <= amount; i++)
        {
            int min2 = int.MaxValue;
            for (int j = 0; j < coins.Length; j++)
            {

                if (i >= coins[j] && DP[i - coins[j]] != -1)
                {
                    min2 = Math.Min(min2, DP[i - coins[j]] + 1);
                }

            }
            DP[i] = min2 == int.MaxValue ? -1 : min2;
        }


        return DP[amount] == int.MaxValue ? -1 : DP[amount];
    }



    /// <summary>
    /// 计算最大连续的字符个数
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public int LengthofLongestSubstring(string s)
    {
        string temp = "";
        int temp_int = 0;
        int max = 0;
        for (int i = 0; i < s.Length; i++)
        {
            temp_int = check(temp, s[i]);
            temp += s[i];
            if (temp_int == -1)
            {
                if (temp.Length > max)
                {
                    max = temp.Length;
                }
            }
            else
            {
                //  temp = temp.Substring(temp_int + 1, temp.Length - temp_int - 1);
                temp = temp.Substring(0, max);
                // UnityEngine.Debug.Log(temp);
            }
        }
        return max;
    }

    private int check(string str, char temp)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == temp)
            {
                return i;
            }
        }
        return -1;
    }



    /// <summary>
    /// 求一字符串里唯一的最长的回文子串，例 abccbade ,即为abccba
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string LongestPalindrome(string s)
    {
        if (s == null || s.Length == 0)

            return null;

        int start = 0;
        int end = 0;
        int len = 0;
        bool[,] dp = new bool[s.Length, s.Length];
        for (int i = s.Length - 1; i >= 0; i--)
        {
            for (int j = i; j < s.Length; j++)
            {
                if (i == j || (s[i] == s[j] && j - i < 2) || (s[i] == s[j] && dp[i + 1, j - 1]))
                {
                    dp[i, j] = true;
                    if (j - i + 1 > len)
                    {
                        len = j - i;
                        start = i;
                        end = j + 1;
                    }
                }
            }
        }
        return s.Substring(start, end - start);

    }



    /// <summary>
    /// 求一个int型的反转数 ，如123， reture 321 ； -123，return -321
    /// 下面这个方法自己写的，不好，没考虑到 int型的最大值和最小值
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int ReverseInteger(int num)
    {
        int result_int = 0;
        string resultStr = "";
        string temp = "";
        if (num == 0)
        {
            return 0;
        }
        else if (num > 0)
        {
            temp = num.ToString();
        }
        else
        {
            string s = num.ToString();
            temp = s.Substring(1, s.Length - 1);
        }

        for (int i = temp.Length - 1; i >= 0; i--)
        {
            resultStr += temp[i];
            // UnityEngine.Debug.Log(resultStr);
        }

        result_int = Int32.Parse(resultStr);
        if (num > 0)
        {
            return result_int;
        }
        else
        {
            return -result_int;
        }

    }

    /// <summary>
    /// 这个考虑全面
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public int Reverse(int x)
    {
        string str = "";
        string str_fan = "";
        if (x == int.MinValue)
        {
            return 0;
        }
        str = ((x > 0 ? 1 : -1) * x).ToString();

        for (int i = str.Length - 1; i >= 0; i--)
        {
            str_fan += str[i];
        }

        if (int.MaxValue > long.Parse(str_fan))
        {
            return (x > 0 ? 1 : -1) * int.Parse(str_fan);
        }
        else
        {
            return 0;
        }
    }





    /// <summary>
    /// 将string 转换为 int 类型 ，考虑的方面比较多
    /// 1.首先需要丢弃字符串前面的空格
    /// 2.然后可能有正负号（注意只取一个，如果有多个正负号，那么这个字符是无法转换的，返回0，如+-2）
    /// 3.字符串可以包含 0~9以外的字符，如果遇到非数字字符，只取改字符前面部分，如“-0123b12”,取 -123
    /// 4 如果超出int范围 ，返回边界值（2147483647或 -2147483648）
    /// </summary> (这个方法有个地方有些错误，在判断是 '+'的地方会数组越界)
    /// <param name="str"></param>
    /// <returns></returns>
    public int Atoi(string str)
    {
        if (str == null || str.Length == 0)
        {
            return 0;
        }
        char[] arr = str.ToCharArray();
        int i = 0;
        bool space = false;
        bool negative = false;
        while (arr[i] == ' ') i++;
        if (arr[i] == '-')
        {
            negative = true;
            ++i;
        }
        if (arr[i] == '+') ++i;
        long sum = 0;
        for (; i < arr.Length; i++)
        {
            if (arr[i] == ' ')
            {
                space = true;
            }
            else if (space == true)
            {
                break;
            }
            else if (arr[i] >= '0' && arr[i] <= '9')
            {
                // 这里判断如果是两位数的话，前面那个字符肯定是十位数上的
                sum = sum * 10 + arr[i] - '0';
            }
            else
            {
                // 这里是如果数字后面显示的不是数字，则直接跳出本次循环
                break;
            }
        }
        int temp = (int)(negative ? -sum : sum);
        if (temp > int.MaxValue)
        {
            return int.MaxValue;
        }
        else if (temp < int.MinValue)
        {
            return int.MinValue;
        }
        else
        {
            return temp;
        }
    }


    public int MyAtoi(string str)
    {
        str = str.Trim();
        if (str.Length == 0)
            return 0;
        System.Text.ASCIIEncoding asciiEncoding;
        int intAsciiCode = 0;
        bool sign = false;
        for (int i = 0; i < str.Length; i++)
        {
            asciiEncoding = new System.Text.ASCIIEncoding();
            intAsciiCode = asciiEncoding.GetBytes(str)[i];
            if (i == 0)
            {
                if (!sign && (intAsciiCode == 43 || intAsciiCode == 45))
                {
                    if (i != str.Length - 1)
                    {
                        intAsciiCode = (int)asciiEncoding.GetBytes(str)[i + 1];
                        if (intAsciiCode >= 48 && intAsciiCode <= 57)
                        {
                            sign = true;
                            continue;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                if (intAsciiCode < 48 || intAsciiCode > 57)
                {
                    return 0;
                }
            }
            if (intAsciiCode < 48 || intAsciiCode > 57)
            {
                str = str.Substring(0, i);
                break;
            }
        }

        if (str.Length > 11)
        {
            asciiEncoding = new System.Text.ASCIIEncoding();
            intAsciiCode = (int)asciiEncoding.GetBytes(str)[0];
            if (intAsciiCode == 43)
                return int.MaxValue;
            if (intAsciiCode == 45)
                return int.MinValue;
            return int.MaxValue;
        }
        long temp = long.Parse(str);
        if (temp > int.MaxValue)
        {
            return int.MaxValue;
        }
        if (temp < int.MinValue)
        {
            return int.MinValue;
        }
        return int.Parse(str);
    }



    /// <summary>
    /// 输出是否是回文数字 123321
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public bool IsPalindrome2(int x)
    {
        String str = x.ToString();
        char[] strChar = str.ToCharArray();
        int intL = strChar.Length;

        int length = strChar.Length >> 1;  // 右移位运算(二进制)， 相当于是遍历该数组长度的一半 ，因为回环数的第一位和最后一位必须相同

        int i = 0;
        while (i < length)
        {
            if (strChar[i] != strChar[intL - i - 1])
            {
                return false;
            }
            i++;
        }
        return true;
    }



    /// <summary>
    /// 输入一个int ，返回string类型的罗马数字(1-3999)
    /// 1~9 :{"I", "II", "III","IV","V","VI","VII","VIII","IX"}
    /// 10~90: {"X", "XX","XXX","XL","L","LX","LXX","LXXX","XC"}
    /// 100~900: {"C","CC","CCC","CD","D","DC","DCC","DCCC","CM"}
    /// 1000~3000: {"M","MM","MMM"}
    /// 使用c#的锯齿数组,要注意100,10这样末尾是0的数，会在-1时发生超界，可以像博主一样加上判断，
    /// 还可以是矩阵的每行第0个数为空像下面这样
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public string IntegerToRoman1(int num)
    {
        if (num == 0)
        {
            return "";
        }
        string[][] roman = new string[4][];
        roman[0] = new string[9] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
        roman[1] = new string[9] { "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
        roman[2] = new string[9] { "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
        roman[3] = new string[3] { "M", "MM", "MMM" };

        string value = num.ToString();
        string final = "";
        for (int i = 0; i < value.Length; i++)
        {
            //在这里判断末尾为0的情况
            if (int.Parse(value[i].ToString()) != 0)
            {
                final += roman[value.Length - 1 - i][int.Parse(value[i].ToString()) - 1];
            }

        }
        return final;
    }


    /// <summary>
    ///  方法一的空间复杂度高于方法二，而时间复杂度低于 方法二
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public string IntegerToRoman2(int num)

    {
        string str = "";
        string[] roman = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        int[] value = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        for (int i = 0; num != 0; ++i)
        {
            while (num >= value[i])
            {
                num -= value[i];
                str += roman[i];
            }
        }
        return str;

    }




    /// <summary>
    /// 罗马数字转化成int整型
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public int RomanToInteger(string roman)
    {
        int ret = tonum(roman[0]);
       
        for(int i= 1; i < roman.Length; i++)
        {
            
            if(tonum(roman[i]) > tonum(roman[i - 1]))
            {
                ret += tonum(roman[i]) - 2 * tonum(roman[i - 1]);
            }
            else
            {
                ret += tonum(roman[i]);
            }
        }

        return ret;
    }


    public int tonum(char ch)
    {
        switch(ch)
        {
            case 'I': return 1;
            case 'V': return 5;
            case 'X': return 10;
            case 'L': return 50;
            case 'C': return 100;
            case 'D': return 500;
            case 'M': return 1000;
        }
        return 0;
    }




    /// <summary>
    /// 要求给入一个数组，在求出所有相加等于0的三个数的集合
    /// 注意： 1.集合中不能有重复的三位数
    ///        2.三位数必须按照从小到大排序
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public IList<IList<int>> ThreeSum(int[] nums)
    {
        IList<IList<int>> result = new List<IList<int>>();
        if (nums.Length < 3)
            return result;

        Array.Sort(nums);
        List<int> temp;
        // 1. 循环次数只需前面四次就可以确定
        for(int i= 0; i<nums.Length - 2; i++)
        {
            // ！！！ 这里一定要主要判断下顺序，保证是从小到大的
            if(i == 0 || nums[i] > nums[i - 1])
            {
                int negate = -nums[i];
                int start = i + 1;
                int end = nums.Length - 1;
                while (start < end)
                {
                    //case1
                    if (nums[start] + nums[end] == negate)
                    {
                        temp = new List<int>();
                        temp.Add(nums[i]);
                        temp.Add(nums[start]);
                        temp.Add(nums[end]);

                        // 
                        result.Add(temp);

                        start++;
                        end--;
                        //
                        while (start < end && nums[end] == nums[end + 1])
                        {
                            end--;
                        }
                        while (start < end && nums[start] == nums[start - 1])
                        {
                            start++;
                        }
                    }
                    //case 2
                    else if (nums[start] + nums[end] < negate)
                    {
                        start++;
                    }
                    // case 3
                    else
                    {
                        end--;
                    }
                }
            

            }
        }
        return result;
    }


    /// <summary>
    /// 给定一个int数组和一个目标值，确定该数组中三个数之和最接近目标数 的三个数
    /// </summary>
    /// <param name="nums"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public int ThreeSumClosest(int[] nums, int target)
    {
        if (nums.Length < 3)
            return 0;
        Array.Sort(nums);
        int sum = nums[0] + nums[1] + nums[2];
        if (nums.Length == 3)
            return sum;
        
        for (int i = 0; i < nums.Length - 2; i++)
        {
            int start = i + 1;
            int end = nums.Length - 1;
            while(start < end)
            {
                int temp1 = Math.Abs(target - (nums[start] + nums[end] + nums[i]));
                int temp2 = Math.Abs(target - sum);

                // 这里可以判断下是否等于0
                if(temp1 == 0)
                {
                    return target;
                }

                if(temp1 < temp2)
                {
                    sum = nums[start] + nums[end] + nums[i];
                }
                else
                {
                    //在这里进行之后的多次循环，two pointers遍历
                    if (nums[start] + nums[end] + nums[i] < target)
                    {
                        start++;
                    }
                    else if(nums[start] + nums[end] + nums[i] > target)
                    {
                        end--;
                    }
                }    
            }
        }
        return sum;
    }

    /// <summary>
    ///  四个数之和，规定四个数值递增的
    /// </summary>
    /// <param name="num"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public IList<IList<int>> FourSum(int[] num, int target)
    {
        IList<IList<int>> ret = new List<IList<int>>();
        if(num.Length < 4)
        {
            return ret;
        }
        // 先排序
        Array.Sort(num);
        string str;
        Dictionary<string, bool> map = new Dictionary<string, bool>();
        // 四个数的话，判断的循环长度为 长度减3
        for(int a =0; a < num.Length -3; a++)
        {
            // 第二个循环从第二个值开始，也就是a+1
            for(int b = a + 1; b < num.Length - 2; b++)
            {
                int c = b + 1;
                int d = num.Length - 1;
                // 这里加判断的原因是保证第三个数永远小于第四个，即顺序递增
                while(c < d)
                {
                    if (num[a] + num[b] + num[c] + num[d] == target)
                    {
                        str = num[a].ToString() + num[b].ToString() + num[c].ToString() + num[d].ToString();
                        // 如果包含则继续+1遍历
                        if (map.ContainsKey(str))
                        {
                            c++;
                            d--;
                        }
                        else
                        {
                            List<int> temp = new List<int> { num[a], num[b], num[c], num[d] };
                            map.Add(str, true);
                            ret.Add(temp);
                            c++;
                            d--;
                        }
                    }
                    else if (num[a] + num[b] + num[c] + num[d] < target)
                    {
                        c++;
                    }
                    else
                    {
                        d--;
                    }
                }
            }
        }
        return ret;
    }



    /// <summary>
    /// 移除节点 
    /// </summary>
    public class ListNode
    {
        public int value;
        public ListNode next;
        public ListNode(int x) { value = x; }
    }
    /// <summary>
    /// 例如 给出 1->2->3->4->5 ，n =2 ， 结果为1->2->3->5
    /// </summary>
    /// <param name="head"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public ListNode RemoveNthFromEnd(ListNode head, int n)
    {
        if (head == null)
            return null;
        int max = 0;
        ListNode temp_node = head;
        while(temp_node != null)
        {
            temp_node = temp_node.next;
            max++;
        }
        if( n >max || n < 0)
        {
            return null;
        }
        if(n == max)
        {
            if(head.next != null)
            {
                return head.next;
            }
            else
            {
                return null;
            }
        }
        temp_node = head;
        if (head.next == null)
            return null;
        for (int i = 0; i < max - n; i++)
        {
            temp_node = temp_node.next;
        }

        temp_node.next = temp_node.next.next;
        return head;
    }



    /// <summary>
    /// 判断输入的字符串  '(', ')', '{', '}', '[' and']', 是否有效
    /// </summary>匹配括号用栈的数据结构实现，这里是用int数组模拟了一个类似栈的结构
    /// <param name="s"></param>
    /// <returns></returns>
    public bool IsValid(string s)
    {
        int[] stack = new int[s.Length];
        int temp = 0;
        for (int i = 0; i < s.Length; i++)
        {
            switch (s[i].ToString())
            {
                case "(":
                    stack[temp] = 0;
                    ++temp;  // ++i 和 i++ 的区别， ++i是先+然后参与运算，后者是先参与运算再+
                    break;
                case "[":
                    stack[temp] = 2;
                    ++temp;
                    break;
                case "{":
                    stack[temp] = 3;
                    ++temp;
                    break;
                case ")":  // 这里得匹配上，不能是单个
                    if (temp <= 0)
                    {
                        return false;
                    }
                    // 判断和前一个是否是一样的，一样的话取出字符
                    if (stack[temp - 1] == 0)
                    {
                        --temp;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case "]":
                    if (temp <= 0)
                    {
                        return false;
                    }
                    if (stack[temp - 1] == 2)
                    {
                        --temp;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case "}":
                    if (temp <= 0)
                        return false;
                    if (stack[temp - 1] == 3)
                        --temp;
                    else
                        return false;
                    break;
            }
        }
        // 这里判断相当于是遍历完后如果全部匹配上，则temp为0，否则就是没有匹配上
        if (temp != 0)
        {
            return false;
        }
        else
        {
            return true;
        }
      

    }


    /// <summary>
    /// 合并链表,把两个排列好的链表（默认从小到大）混成一个从小到大的链表
    /// </summary>
    /// <returns></returns>
    public ListNode MergeTwoList(ListNode l1, ListNode l2)
    {
        if(l1 == null && l2 == null)
        {
            return null;
        }
        if (l1 == null)
            return l2;
        if (l2 == null)
            return l1;
        ListNode small;
        ListNode big;
        ListNode temp;
        if(l1.value <= l2.value)
        {
            small = l1;
            big = l2;
        }
        else
        {
            small = l2;
            big = l1;
        }

        ListNode head = small;
        while(true)
        {
            if (big == null)
                return head;
            if(small.next != null)
            {
                if(small.next.value <= big.value)
                {
                    small = small.next;
                }
                else
                {
                    temp = small.next;
                    small.next = big;
                    big = big.next;
                    small.next.next = temp;
                    small = small.next;
                }
            }
            else
            {
                small.next = big;
                return head;
            }
        }

    }



    /// <summary>
    /// 生成括号（GenerateParenthesis）
    /// 思路：采用递归树的思想，当左括号数大于右括号数时可以加左或者右括号，
    /// 否则只能加左括号，当左括号数达到n时，剩下全部加右括号
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public IList<string> GenerateParenthesis(int  n)
    {
        IList<string> res = new List<string>();
        generate(res, "", 0, 0, n);
        return res;
    }

    public void generate(IList<string> res, string tmp, int lhs, int rhs, int n)
    {
        if(lhs == n)
        {
            for(int i =0; i < n - rhs; i++)
            {
                tmp += ")";
            }
            res.Add(tmp);
            return;
        }
        generate(res, tmp + "(", lhs + 1, rhs, n);
        if(lhs > rhs)
        {
            generate(res, tmp + ")" ,lhs, rhs + 1, n);
        }
    }



    /// <summary>
    ///  合并给定数量的已排序链表(默认从小到大),输出一个最后的链表
    /// </summary>
    /// <returns></returns>
    public ListNode MergeKList(ListNode[] lists)
    {
        ListNode[] temp_list = lists;
        List<int> temp_int = new List<int>();
        for(int i= 0; i < temp_list.Length; i++)
        {
            while(temp_list[i] != null)
            {
                // 把所有链表的值都存在一个int列表中，然后排序后重新去构造链表
                temp_int.Add(temp_list[i].value);
                temp_list[i] = temp_list[i].next;
            }
        }

        if(temp_int.Count == 0)
        {
            return null;
        }
        temp_int.Sort();
        ListNode head = new ListNode(temp_int[0]);
        ListNode temp = head;
        for (int i = 1; i < temp_int.Count; i++)
        {
            // 这里是错误的，这样的话只是赋值，并不是创建链表
            //temp.next.value = temp_int[i];
            temp.next = new ListNode(temp_int[i]);
            temp = temp.next;
        }
        return head;

    }



    /// <summary>
    /// 给一个链表，每两个一 交换， FE ：1->2->3->4 , return 2->1->4->3
    /// </summary>
    /// <param name="head"></param>
    /// <returns></returns>
    public ListNode SwapPairs(ListNode head)
    {
        if (head == null || head.next == null)
            return head;
        ListNode temp = head;
        ListNode exchange;
        ListNode exchange2 = head.next.next;
        exchange = head;
        head = head.next;
        head.next = exchange;
        head.next.next = exchange2;
        exchange2 = head;
        temp = head.next;
        while(temp.next != null && temp.next.next != null)
        {
            exchange = temp.next.next;
            exchange2 = temp.next;
            temp.next.next = temp.next.next.next;
            temp.next = exchange;
            temp.next.next = exchange2;
            temp = temp.next;
        }

        return head;
    }



    /// <summary>
    ///  给定一个有序数组，删除重复的地方，使每个元素只出现一次并返回新的长度
    /// </summary>
    /// <param name="nums"></param> 
    /// <returns></returns>
    public int RemoveDuplicates(int[] nums)
    {
        if (nums.Length == 0)
            return 0;
        int temp = 1;
        for (int i = 1; i < nums.Length; i++)
        {
            if(nums[i - 1] == nums[i])
            {
                continue;
            }
            else
            {
                if(temp < i)
                {
                    nums[temp] = nums[i];
                }
                ++temp;
            }
        }
        return temp;
    }



    /// <summary>
    /// 给定一数组和一个value，移除数组中和value相同的元素，并返回移除后的数组长度
    /// </summary>
    /// <param name="nums"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int RemoveElement(int[] nums, int value)
    {
        if (nums.Length == 0)
            return 0;
        int temp = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            if(nums[i] == value)
            {
                continue;
            }
            else
            {
               if(temp < i)
                {
                    nums[temp] = nums[i];
                }
                ++temp;
            }
        }
        return temp;
    }


    /// <summary>
    /// 给定一个int数组和target值，取数组中两个数和等于target的数的下标，并且返回改下标数组
    /// 其中index1必须小于index2。请注意，您返回的答案（index1和index2）不是从零开始的。
    /// </summary>
    /// <param name="nums"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public int[] TwoSum(int[] nums, int target)
    {
        if(nums.Length < 2)
        {
            return null;
        }
        int[] index = new int[2];
        
        for (int i = 0; i < nums.Length; i++)
        {
            index[0] = i+ 1;
            for (int j = 0; j < nums.Length; j++)
            {
                if(j != i)
                {
                    if(nums[j] + nums[index[0] - 1] == target)
                    {
                        index[1] = j + 1;
                        // 得到index[1] 的值后直接return就可以
                        return index;
                    }
                }
            }
        }
        return null;
    }



    /// <summary>
    /// 全排列问题，在原数组中返回下一个全排列的数组（升序排列）
    /// </summary>
    /// <param name="nums"></param>
    public void NextPermutation(int[] nums)
    {
        // 1. 找到最后一个升序位置pos
        int pos = -1;
        for (int i = nums.Length -1; i > 0 ; i--)
        {
            if(nums[i] > nums[i - 1])
            {
                pos = i - 1;
                break;
            }
        }
        // 2. 如果不存在升序，即这个数是最大的，那么反排这个数组
        if(pos < 0)
        {
            Array.Reverse(nums,0,nums.Length);
            return;
        }

        // 3.存在升序，那么找到pos之后最后一个比它大的位置
        for (int i = nums.Length - 1; i > pos; i++)
        {
            if(nums[i] > nums[pos])
            {
                int temp = nums[i];
                nums[i] = nums[pos];
                nums[pos] = temp;
                break;
            }
        }
        Array.Reverse(nums,pos +1 , nums.Length -1 - pos);
    }


    /// <summary>
    /// 返回最长连续可行的括号length，存在嵌套。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public int LongestValidParentheses(string s)
    {
        int res = 0, l = 0;
        Stack<int> si = new Stack<int>();
        for (int i = 0; i < s.Length; i++)
        {
            if(s[i] == '(')
            {
                si.Push(i);
            }
            else
            {
                if (si.Count == 0)
                    l = i + 1;
                else
                {
                    si.Pop(); // 返回栈顶元素并删除
                    if (si.Count == 0)
                        res = Math.Max(res, i - l + 1);
                    else
                        res = Math.Max(res, i - si.Peek());
                }
            }
        }
        return res;
    }


     
    public int Search(int[] nums, int target)
    {
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] == target)
                return i;
        }
        return -1;
    }
}

