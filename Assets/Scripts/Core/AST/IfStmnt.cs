namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class IfStmnt : ASTList 
    {
        public IfStmnt(List<ASTree> c) : base(c) {}
        public ASTree condition() { return child(0); }
        public ASTree thenBlock() { return child(1); }
        public ASTree elseBlock() { return numChildren() > 2 ? child(2) : null; }
        public override string ToString()
        {
            return string.Format("(if {0} {1} else {2})", condition(), thenBlock(), elseBlock());
        }
    }
}