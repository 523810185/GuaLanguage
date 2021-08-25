namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class BinaryExpr : ASTList 
    {
        public BinaryExpr(List<ASTree> c) : base(c) {}
        public ASTree left() { return child(0); }
        public string operand() { return ((ASTLeaf)child(1)).token().getText(); }
        public ASTree right() { return child(2); }
    }
}