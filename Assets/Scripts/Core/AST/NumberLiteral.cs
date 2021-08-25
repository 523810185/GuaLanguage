namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class NumberLiteral : ASTLeaf 
    {
        public NumberLiteral(Token t) : base(t) {}
        public int value() { return token().getNumber(); }

        public override object eval(Environment env)
        {
            return value();
        }
    }
}