namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class NumberLiteral : ASTLeaf 
    {
        public NumberLiteral(Token t) : base(t) {}
        public int value() { return token().getNumber(); }
    }
}