namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class StringLiteral : ASTLeaf 
    {
        public StringLiteral(Token t) : base(t) {}
        public string value() { return token().getText(); }

        public override object eval(Environment env)
        {
            return value();
        }
    }
}