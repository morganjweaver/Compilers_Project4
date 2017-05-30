using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASTBuilder
{
    internal partial class TCCLParser
    {
        public TCCLParser() : base(null) { }

        public void Parse(string filename)
        {
            this.Scanner = new TCCLScanner(File.OpenRead(filename));
            this.Parse();
            DoSemantics();
            PrintTree();
            GenerateCode(filename);
        }
        public void Parse(Stream strm)
        {
            this.Scanner = new TCCLScanner(strm);
            this.Parse();
            DoSemantics();
            PrintTree();
        }
        public void PrintTree()
        {
            PrintVisitor visitor = new PrintVisitor();
            Console.WriteLine("Starting to print AST ");
            visitor.PrintTree(CurrentSemanticValue);
        }

        public void DoSemantics()
        {
            SemanticsVisitor visitor = new SemanticsVisitor();
            Console.WriteLine("Starting semantic checking");
            visitor.CheckSemantics(CurrentSemanticValue);
        }

        public void GenerateCode(string filename)
        {
            CodeGenVisitor visitor = new CodeGenVisitor();
            Console.WriteLine("Starting code generation");
            visitor.GenCode(CurrentSemanticValue, filename);
        }


    }
}
