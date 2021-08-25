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
    }
}