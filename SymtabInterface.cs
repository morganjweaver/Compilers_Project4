namespace studio8
{

	public interface SymtabInterface
	{
	   /// Open a new nested symbol table
	   void incrNestLevel();
	  
	   /// Close an existing nested symbol table

	   void decrNestLevel();

	   int CurrentNestLevel {get;}

	   SymInfo lookup(string id);

	   void enter(string id, SymInfo s);
	   
	   /// This lets you put out a message about a node, indented by the current nest level 
	//    void @out(AbstractNode n, string message);
	//    void err(AbstractNode n, string message);
	   void @out(string message);
	   void err(string message);
	}


}