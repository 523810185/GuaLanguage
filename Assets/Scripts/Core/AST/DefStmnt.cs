namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class DefStmnt : ASTList 
    {
        public DefStmnt(List<ASTree> c) : base(c) {}

        public string name() { return (child(0) as ASTLeaf).token().getText(); }
        public ParameterList parameters() { return child(1) as ParameterList; }
        public BlockStmnt body() { return child(2) as BlockStmnt; }
        public override string ToString()
        {
            return "(def " + name() + " " + parameters() + " " + body() + ")";
        }

        public override object eval(Environment env)
        {
            env.putNew(name(), new Function(parameters(), body(), env));
            return name();
        }
    }
}