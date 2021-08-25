namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class NegativeExpr : ASTList 
    {
        public NegativeExpr(List<ASTree> c) : base(c) {}
        public ASTree operand() { return child(0); }
        public override string ToString()
        {
            return "-" + operand();
        }
    }
}