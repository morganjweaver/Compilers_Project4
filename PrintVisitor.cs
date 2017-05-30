using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class PrintVisitor : IReflectiveVisitor
    {
        // This method is the key to implenting the Reflective Visitor pattern
        // in C#, using the 'dynamic' type specification to accept any node type.
        // The subsequent call to VisitNode does dynamic lookup to find the
        // appropriate Version.

        public void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        // Call this method to begin the tree printing process
        public void PrintTree(AbstractNode node, string prefix = "")
        {
            if (node == null) { 
                return;
            }
            Console.Write(prefix);
            node.Accept(this);
            VisitChildren(node, prefix + "   ");
       }
        public void VisitChildren(AbstractNode node, String prefix)
        {
            AbstractNode child = node.Child;
            while (child != null) {             
               PrintTree(child, prefix);
               child = child.Sib;
            };
        }

        public void VisitNode(AbstractNode node)
        {
            Console.WriteLine("<" + node.ClassName() + ">");
        }

        public void VisitNode(Nodes.Modifiers node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            var stringEnums = node.mod.ToString();
            Console.WriteLine(string.Join(", ", stringEnums));
        }

        public void VisitNode(Nodes.Identifier node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.WriteLine(node.name);

        }
        public void VisitNode(Nodes.Primitive node)
        {
            Console.Write("<" + node.GetType() + ">: ");
            Console.WriteLine(node.pType);
        }
        public void VisitNode(Nodes.Expression node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.WriteLine(node.opType);
        }

        public void VisitNode(Nodes.Special node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.WriteLine(node.stype);
        }
    }
}
