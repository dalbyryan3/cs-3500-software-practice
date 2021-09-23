using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        /// <summary>
        /// Will manipulate a user passed in string and output it
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Write("Enter a word to be spaced and sent to uppercase: ");
            string ans = Console.ReadLine();
            foreach(char c in ans.ToList())
            {
                Console.Write(c.ToString().ToUpper() + " ");
            }
            Console.ReadLine();
        }
    }
}
