using System;

namespace studio8

{
  
	/// <summary>
	/// Abstract class so you can print out messages that are properly indented to
	/// reflect the current nest level.
	/// </summary>

	public abstract class Symtab
	{
        
	   public abstract int CurrentNestLevel {get;}

	   public virtual void @out(string s)
	   {
			 string tab = "";
			 for (int i = 1; i <= CurrentNestLevel; ++i)
			 {
				 tab += "  ";
			 }
			 Console.WriteLine(tab + s);
	   }

	   public virtual void err(string s)
	   {
		  @out("Error: " + s);
		  Console.Error.WriteLine("Error: " + s);
		  Environment.Exit(-1);
	   }

	  // public virtual void @out(AbstractNode n, string s)
	  // {
			//@out("" + n.NodeNum + ": " + s + " " + n);
	  // }
	  // public virtual void err(AbstractNode n, string s)
	  // {
			//err("" + n.NodeNum + ": " + s + " " + n);
	  // }

	}


}