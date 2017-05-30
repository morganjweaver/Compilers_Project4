using studio8;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ASTBuilder
{
    public class Nodes
    {

        public enum NodeTag
        {
            error,
            name,
            literal,
            plus,
            minus,
            mul,
            div,
            rem,
            negate
        }

        public enum modType
        {
            PUBLIC,
            PRIVATE,
            STATIC
        }

        public enum ExprType
        {
            COMPLEX,
            PRIMARY,
            COMPLEXPRIMARY
        }

        public static List<T> nodesToList<T>(AbstractNode node) where T : AbstractNode
        { 
            List<T> ret = new List<T>();
            if (node == null) { return ret; }
            T cur = (T)node;
            ret.Add(cur);
            while (cur.Sib != null)
            {
                cur = (T)cur.Sib;
                ret.Add(cur);
            }
            return ret;
        }

        public enum primType
        {
            BOOLEAN,
            QNAME,
            STRING,
            CLASS,
            INT,
            VOID
        }
        public class CompilationUnit : AbstractNode
        {
            public List<modType> level;

            public CompilationUnit(AbstractNode n)
            {
                this.adoptChildren(n);
            }
        }
        public class Identifier : AbstractNode
        {
           public  string name;
           public Identifier(string yytext)
           {
                this.name = yytext;
//                Console.WriteLine(name);
            }
        }
        public class Literal : AbstractNode
        {
            public string name;
            public Literal(string yytext)
            {
                this.name = yytext;
            }
        }
     
        public class Number : AbstractNode
        {

            public int num;
            private AbstractNode parent;
            private AbstractNode sib;
            public Number(string yytext)
            {
                this.num = Int32.Parse(yytext);
                //Console.WriteLine(num);
            }

        }
        public class MethodCall : AbstractNode
        {
            public AbstractNode methodname; //complex primary, qualname, or special
            public AbstractNode args;

            public MethodCall(AbstractNode mRef, AbstractNode args)
            {
                methodname = mRef;
                this.args = args;
                adoptChildren(mRef);
                adoptChildren(args);
            }
            public MethodCall(AbstractNode mRef)
            {
                methodname = mRef;
                adoptChildren(mRef);
            
            }
            public string getMethodName()//FIXME: Not complete.  Need more handling here
            {
                string name = "error";
                if(this.methodname.GetType() == typeof(Nodes.QualName)){
                    name = ((QualName)methodname).q_name.Last();
                }
                if (this.methodname.GetType() == typeof(Nodes.Identifier))
                {
                    name = ((Nodes.Identifier)methodname).name;
                }
                if (this.methodname.GetType() == typeof(Nodes.Literal))
                {
                    name = ((Literal)methodname).name;
                }
                return name;
            }
        }
        public class Expression : AbstractNode
        {
            public string opType;
            public AbstractNode left, right;
       //     public Expression(AbstractNode parent, AbstractNode sib, ExprType type)
//            {
//                makeSibling(sib);
//                exType = type;
//            }
            public Expression(AbstractNode left, string op, AbstractNode right)  //ARITHMETIC BINARY
            {
                this.left = left;
                this.right = right;
                adoptChildren(left);
                adoptChildren(right);
                opType = op;
            }
            public Expression(string op, AbstractNode right)  //ARITHMETIC UNARY
            {
                adoptChildren(right);
                opType = op;
            }
        }

        public class IfNode : AbstractNode
        {
            public Expression condition;
            public AbstractNode task;
            public AbstractNode elseTask; 
            public IfNode(AbstractNode cond, AbstractNode task, AbstractNode elsetask)
            {
                condition = (Expression)cond;
                this.task = task;
                elseTask = elsetask;
                adoptChildren(cond);
                adoptChildren(task);
                adoptChildren(elsetask);
            }
        }

        public class Modifiers : AbstractNode
        {
            public modType mod;


            public Modifiers(modType t)
            {
                mod = t;
            }

            public Modifiers(modType t, AbstractNode sib)
            {
                mod = t;
                this.makeSibling(sib);
            }

            public override string ToString() {
                return mod.ToString().ToLower();
            }
        }
        public class Locals_and_Stmts: AbstractNode
        {      

            public Locals_and_Stmts(AbstractNode contents)
            {
                this.adoptChildren(contents);
            }

            public Locals_and_Stmts()
            {

            }
            
        }
        public class LocalVDS : AbstractNode
        {
            public Nodes.TypeSpec typechild;
            public List<string> varnames;
                        
            public LocalVDS(AbstractNode typeSpec, AbstractNode LVeclarators)
            {
                this.adoptChildren(typeSpec);
                this.adoptChildren(LVeclarators);
                varnames = new List<string>();
                typechild = (TypeSpec)typeSpec;
                foreach (Identifier id in nodesToList<Identifier>(LVeclarators))
                {
                    varnames.Add(id.name);
                }
            }

        }
        public class Parameter : AbstractNode
        {
            public Nodes.TypeSpec ts;
            public string name; 
            public Parameter(AbstractNode typeSpec, AbstractNode DecName)
            {
                ts = (TypeSpec)typeSpec;
                name = ((Identifier)DecName).name;
                this.adoptChildren(typeSpec);
                this.adoptChildren(DecName);
            }

        }
        public class Empty : AbstractNode
        {
            public Empty()
            {
           
            }
        }
        public class LocalFDOS : AbstractNode
        {
            public LocalFDOS(AbstractNode kid)
            {
                this.adoptChildren(kid);
            }
        }
        public class ClassBody : AbstractNode
        {

            public ClassBody(AbstractNode fieldDecl)
            {
                this.adoptChildren(fieldDecl);
            }

            public ClassBody() //HOW TO SIGNIFY EMPTY BODY?
            {

            }

        }

        public class While : AbstractNode
        {
            public While(AbstractNode cond, AbstractNode task)
            {
                adoptChildren(cond);
                adoptChildren(task);
            }
        }

        public class FieldDecl : AbstractNode
        {
            public FieldDecl(AbstractNode n) //Node type (Decl vs Decls) stored in constructor
            {
                this.adoptChildren(n); //fieldvariable declarations
            }
        } //Modifiers TypeSpecifier FieldVariableDeclarators

        public class FieldDecls : AbstractNode
        {
            public FieldDecls(AbstractNode n) 
            {
                this.adoptChildren(n); //fieldvariable declarations
            }
        
        }
        public class FieldVarDecl : AbstractNode
        {
            public TypeSpec typeChild;
            public List<Modifiers> modList;
            public List<string> fv_list;

           
            public FieldVarDecl(AbstractNode n) 
            {
                this.adoptChildren(n);
            }

            public FieldVarDecl(AbstractNode mods, AbstractNode type, AbstractNode fv_declarators)
            { 
                //Modifiers TypeSpecifier FieldVariableDeclarators

                this.typeChild = (Nodes.TypeSpec) type;
                modList = nodesToList<Modifiers>(mods);
                fv_list = new List<string>();
                foreach (FieldVarDeclarators fvd in nodesToList<FieldVarDeclarators>(fv_declarators))
                {
                    fv_list.Add(fvd.fv_name);
                }
                this.adoptChildren(mods);
                this.adoptChildren(type);
                this.adoptChildren(fv_declarators);
            }
        }

        public class FieldVarDeclarators : AbstractNode
        {
            public string fv_name;

            public FieldVarDeclarators(AbstractNode name) //Identifiernode
            {
                this.fv_name = ((Identifier)name).name;
            }
            public FieldVarDeclarators(AbstractNode name, AbstractNode sib) //Identifiernode
            {
                this.fv_name = ((Identifier)name).name;
                this.makeSibling(sib);
            }
        }

        public class Arith : AbstractNode
        {
            public string op;
            public Arith(string opr)
            {
                this.op = opr;
            }
        }

        public class MethodDeclaration : AbstractNode 
        {
            //public MethodDeclaration(AbstractNode abs)
            //{
            //    this.adoptChildren(abs);
            //} //ctrl-KC
            public List<Modifiers> modList;
            public Nodes.TypeSpec typeChild;
            public Nodes.MethodDeclarator methodDeclarator;
            public AbstractNode methodBody;
            public MethodDeclaration(AbstractNode mods, AbstractNode typespec, AbstractNode decltr, AbstractNode bod) 
                //Modifiers TypeSpecifier MethodDeclarator MethodBody
            {
                this.modList = nodesToList<Modifiers>(mods);
                typeChild = (Nodes.TypeSpec)typespec;
                methodDeclarator = (Nodes.MethodDeclarator)decltr;
                methodBody = bod;
                adoptChildren(mods);
                adoptChildren(typespec);
                adoptChildren(decltr);
                adoptChildren(bod);
            }

        }
        public class MethodDeclarator : AbstractNode
        {
            public string md_name;
            public MethodDeclarator(AbstractNode name) {//IDnode
           
                this.md_name = ((Identifier)name).name;
            }
            public MethodDeclarator(AbstractNode name, AbstractNode paramz)
            {
                this.md_name = ((Identifier)name).name;
                this.adoptChildren(paramz);
            }
        }
        public class ClassDeclaration : AbstractNode
        {
            public Identifier name;
            public List<Modifiers> modList;
            public FieldDecls body;
            public ClassDeclaration(AbstractNode mods, AbstractNode ident, AbstractNode classbod)
            {
                name = (Identifier)ident;
                modList = nodesToList<Modifiers>(mods);
                body = (FieldDecls)classbod.Child;
                this.adoptChildren(mods);
                this.adoptChildren(ident);
                this.adoptChildren(classbod);       
            }
        }
        public class QualName : AbstractNode
        {
            public List<string> q_name = new List<string>(); 

            public QualName(AbstractNode name)
            {
                this.q_name.Add(((Identifier)name).name);
            }

            public QualName(AbstractNode name, AbstractNode sib)
            {
                this.q_name.Add(((Identifier)name).name);
                makeSibling(sib);
            }

            public void append(AbstractNode nextNode)
            {
                q_name.Add(((Identifier)nextNode).name);
            }
        }
        public class Special : AbstractNode
        {
            public string stype;
            public Special(string type)
            {
                stype = type;
            }

        }
        public class Primitive : AbstractNode
        {
            public primType pType;
            public Primitive(primType p)
            {
                pType = p;
            }

            public Primitive(AbstractNode n)
            {
                pType = ((Primitive) n).pType;
            }
        }
        public class TypeName : AbstractNode
        {
            public TypeName(AbstractNode childType)
            {
                adoptChildren(childType);
            }
        }
        public class TypeSpec : AbstractNode
        {
            public bool isArray = false;
            public TypeSpec(AbstractNode childType, bool isArr)
            {
                isArray = isArr;
                adoptChildren(childType);
            }
        }

    }
}

/**
 public class UnaryNode : AbstractNode 
        {
            private static int nodeNums = 0;
            private int nodeNum;
            private AbstractNode mysib;
            private AbstractNode parent;
            private AbstractNode child;
            private AbstractNode firstSib;
            public List<modType> level;
            private Type type;

            public UnaryNode(AbstractNode n, AbstractNode m, modType type)
            {
                parent = null;
                mysib = null;
                firstSib = this;
                child = null;
                nodeNum = ++nodeNums;
                level.Add(type);
        }

            public string getTag()
            {
                return this.type.ToString();
            }
        }
      public class BinaryNode : AbstractNode
        {
            private static int nodeNums = 0;
            private int nodeNum;
            private AbstractNode mysib;
            private AbstractNode parent;
            private AbstractNode child;
            private AbstractNode firstSib;
            public NodeTag tag;
            private Type type;

            public BinaryNode(AbstractNode n, AbstractNode m, NodeTag tag)
            {
                parent = null;
                mysib = null;
                firstSib = this;
                child = null;
                nodeNum = ++nodeNums;
                this.tag = tag;
            }

            public string getTag()
            {
                return this.tag.ToString();
            }
        }
    **/
