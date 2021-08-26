namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class ClassStmnt : ASTList 
    {
        public ClassStmnt(List<ASTree> c) : base(c) {}
        public string name() { return (child(0) as ASTLeaf).token().getText(); }
        public string superClass() 
        {
            if(numChildren() < 3) 
            {
                return null;
            }
            else
            {
                return (child(1) as ASTLeaf).token().getText();
            }
        }

        public ClassBody body() { return child(numChildren() - 1) as ClassBody; }

        public override string ToString()
        {
            string parent = superClass();
            if(parent == null) 
            {
                return "*";
            }

            return "(class " + name() + " " + parent + " " + body() + ")";
        }

        public override object eval(Environment env)
        {
            ClassInfo ci = new ClassInfo(this, env);
            env.put(name(), ci);
            return name();
        }
    }
}