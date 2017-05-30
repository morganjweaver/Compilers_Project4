using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTBuilder;

namespace ASTBuilder
{
    // Substitute your GrammarNode.cs file for this one
    public class CompilationUnit : AbstractNode
    {
        // just for the compilation unit because it's the top node
        //public override AbstractNode LeftMostSibling => this;
        public override AbstractNode Sib => null;

        public CompilationUnit(AbstractNode classDecl)
        {
            adoptChildren(classDecl);
        }

    }

    
}

