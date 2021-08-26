namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class PrimaryExpr : ASTList 
    {
        public PrimaryExpr(List<ASTree> c) : base(c) {}
        public static ASTree create(List<ASTree> c) 
        {
            return c.Count == 1 ? c[0] : new PrimaryExpr(c);
        }

        public ASTree operand() { return child(0); }
        public Postfix postfix(int nest) 
        {
            return child(numChildren() - nest - 1) as Postfix; 
        }
        public bool hasPostfix(int nest) 
        {
            return numChildren() - nest > 1;
        }

        public override object eval(Environment env)
        {
            return evalSubExpr(env, 0);
        }

        public object evalSubExpr(Environment env, int nest) 
        {
            if(hasPostfix(nest)) 
            {
                var target = evalSubExpr(env, nest + 1);
                return postfix(nest).eval(env, target);
            }
            else 
            {
                return operand().eval(env);
            }
        }
    }
}