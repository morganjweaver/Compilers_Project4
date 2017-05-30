using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ASTBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new TCCLParser();
           

            while (true)
            {
                Console.Write("Enter a file name: ");
                var name = Console.ReadLine();
                Console.WriteLine("Parsing file " + name);
                parser.Parse(name + ".txt");
                Console.WriteLine("Parsing complete");
               
            }

        }
    }
}
