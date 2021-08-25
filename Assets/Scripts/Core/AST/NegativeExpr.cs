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

        public override object eval(Environment env)
        {
            var ne = this;
            object v = ne.operand().eval(env);
            if(v is int) 
            {
                return -(int)v;
            }
            else 
            {
                throw new GuaException("bad type for -", ne);
            }
        }
    }
}