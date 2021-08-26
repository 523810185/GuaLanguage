namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class Fun : ASTList
    {
        public Fun(List<ASTree> c) :base(c) {}
        public ParameterList parameters() { return child(0) as ParameterList; }
        public BlockStmnt body() { return child(1) as BlockStmnt; }
        public override string ToString()
        {
            return "(fun " + parameters() + " " + body() + ")";
        }

        public override object eval(Environment env)
        {
            return new Function(parameters(), body(), env);
        }
    }
}