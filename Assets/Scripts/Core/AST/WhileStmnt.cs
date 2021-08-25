namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class WhileStmnt : ASTList 
    {
        public WhileStmnt(List<ASTree> c) : base(c) {}
        public ASTree condition() { return child(0); }
        public ASTree body() { return child(1); }
        public override string ToString()
        {
            return string.Format("(while {0} {1} )", condition(), body());
        }
    }
}