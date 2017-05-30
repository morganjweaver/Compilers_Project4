﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using studio8;


namespace ASTBuilder
{
    class SemanticsVisitor : IReflectiveVisitor //IMPLEMENT PG 302
    {
        // This method is the key to implenting the Reflective Visitor pattern
        // in C#, using the 'dynamic' type specification to accept any node type.
        // The subsequent call to VisitNode does dynamic lookup to find the
        // appropriate Version.
        internal SymInfo.SymTbl_Stack symTable;

        public void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        // Call this method to begin the semantic checking process
        public void CheckSemantics(AbstractNode node)
        {
            symTable = new SymInfo.SymTbl_Stack();
            if (node == null) { 
                return;
            }
            node.Accept(this);
        }

        public void VisitChildren(AbstractNode node)
        {
            if (node == null)
            {
                return;
            }
            AbstractNode child = node.Child;
            while (child != null)
            {
                child.Accept(this);
                child = child.Sib;
            };
        }
        public void populateTypeSpec(Nodes.TypeSpec ts, ref SymInfo sim)
        {
            if (ts.isArray)
            {
                sim.isArray = true;
            }
            if (ts.Child.GetType() == typeof(Nodes.Primitive))
            {
                sim.pType = ((Nodes.Primitive)ts.Child).pType;
            }
            else if (ts.Child.GetType() == typeof(Nodes.QualName))
            {
                // e.g. compilers.symtable.Table -- i.e., reference to a named class
                sim.pType = Nodes.primType.CLASS;
                // suck name into
                sim.customTypeName = String.Join(".", ((Nodes.QualName)ts.Child).q_name);
            }
        }
        public void VisitNode(AbstractNode node)
        {
        }
        // Scope node
        public void VisitNode(Nodes.CompilationUnit node)
        {
            this.symTable.incrNestLevel();
            VisitChildren(node);
            Console.WriteLine("Printing CompilationUnit symtbl:");
            this.symTable.PrintTop();
            this.symTable.decrNestLevel();
        }
        // Scope node
        public void VisitNode(Nodes.ClassDeclaration node)
        {
            // Page 351
            node.symInfo.pType = Nodes.primType.CLASS;
            node.symInfo.customTypeName = node.name.name;
            node.symInfo.setTypesFromModifiers(node.modList);
            this.symTable.enter(node.name.name, node.symInfo);
            this.symTable.incrNestLevel();
            VisitChildren(node.body); // Need to implement visitors for things in class bodies
            Console.WriteLine("Printing CLASS symtbl: " + node.name.name);
            this.symTable.PrintTop();
            this.symTable.decrNestLevel();   
        }
        public void VisitNode(Nodes.MethodDeclaration node) //METHOD DECLARATION
        {
            Console.WriteLine("Printing METHOD: " + node.methodDeclarator.md_name);
            node.symInfo.setTypesFromModifiers(node.modList);
            node.symInfo.isMethod = true;
            populateTypeSpec(node.typeChild, ref node.symInfo); //RETURN TYPE
            this.symTable.enter(node.methodDeclarator.md_name, node.symInfo);
            this.symTable.incrNestLevel();
            VisitChildren(node.methodDeclarator); // Visit params
            VisitChildren(node.methodBody); // Visit bod
            this.symTable.PrintTop();
            this.symTable.decrNestLevel();
            Console.WriteLine("DONE METHOD: " + node.methodDeclarator.md_name);

        }
        //visit params
        public void VisitNode(Nodes.Parameter node) //typespec + decname in PARAMETER
        {
            this.populateTypeSpec(node.ts, ref node.symInfo);
            this.symTable.enter(node.name, node.symInfo);
        }
        //visit Nodes.Locals_and_Stmts
        public void VisitNode(Nodes.Locals_and_Stmts node) //other decls stored as children
        {
            VisitChildren(node);
        }
        public void VisitNode(Nodes.FieldDecls node) //other decls stored as children
        {
            VisitChildren(node);
        }
        public void VisitNode(Nodes.FieldDecl node) //other decls stored as children
        {
            VisitChildren(node);
        }
        public void VisitNode(Nodes.LocalFDOS node) //other decls stored as children
        {
            VisitChildren(node);
        }
        public void VisitNode(Nodes.FieldVarDecl node) //other decls stored as children
        {
            node.symInfo.setTypesFromModifiers(node.modList);
            this.populateTypeSpec(node.typeChild, ref node.symInfo);
            foreach(string name in node.fv_list)
            {
                this.symTable.enter(name, node.symInfo);
            }
        }
        public void VisitNode(Nodes.Expression node) //left OP right
        {
            VisitChildren(node);
            //do type check by comparing op1 and op 2 for type
            if (node.left.symInfo.pType == node.right.symInfo.pType) //ptypes match
            {
                if (node.left.symInfo.pType == Nodes.primType.CLASS) //if both are classes
                {
                    if (!node.left.symInfo.customTypeName.Equals(node.right.symInfo.customTypeName)) //if custom types not equal
                    {
                        Console.WriteLine("TYPES {0} AND {1} NOT EQUAL! OPERATOR: {2} ", node.left.symInfo.customTypeName, node.right.symInfo.customTypeName, node.opType);
                    }
                }
            } else
            {
                Console.WriteLine("TYPES {0} and {1} NOT EQUAL! OP type: {2}", node.left.symInfo.pType, node.right.symInfo.pType, node.opType);
            }
            //how to handle arrays?
            node.symInfo.pType = node.left.symInfo.pType;
            //this.symTable.enter(node.left.)
        }
        public void VisitNode(Nodes.QualName node) //make signatures for Qualified Name, Literal, FIeldAccess, MethodCall, Number
        {
            SymInfo sym = this.symTable.lookup(node.q_name.Last());
            if (sym != null) // if null, lookup prints an error
            {
                node.symInfo = sym; 
            }
            else
            {
                //not implementing for now
                Console.WriteLine("****Long Qualified Names currently not handled, or symbol not defined: " +
                    String.Join(".", node.q_name));
            }
        }
        public void VisitNode(Nodes.MethodCall node)
        {
            if(node.getMethodName() == "WriteLine" | node.getMethodName() == "Write")
            {
                if(node.args == null)
                {
                    Console.WriteLine();
                }
                else if(node.args.GetType() == typeof(Nodes.Literal))
                {
                    Console.WriteLine();
                    Console.WriteLine(((Nodes.Literal)node.args).name);

                }
                else {
                   
                    Console.WriteLine("Writing of expressions, vars and other types not currently handled");
                }
            }
           node.symInfo = this.symTable.lookup(node.getMethodName()); //has return type
            VisitChildren(node.args);
        }
        public void VisitNode(Nodes.Literal node)
        {
            node.symInfo.pType = Nodes.primType.STRING;
        }
        public void VisitNode(Nodes.Number node) //visitor should set semtype to int
        {
            node.symInfo.pType = Nodes.primType.INT;
        }
        public void VisitNode(Nodes.Identifier node) //visitor should set semtype to int
        {
            node.symInfo = this.symTable.lookup(node.name);
        }
        public void VisitNode(Nodes.LocalVDS node) // local (inside scope), int x,y,z , etc
        {
            this.populateTypeSpec(node.typechild, ref node.symInfo);
            foreach (string name in node.varnames)
            {
                this.symTable.enter(name, node.symInfo);
            }
        }
    }
}
