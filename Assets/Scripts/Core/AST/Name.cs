namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class Name : ASTLeaf 
    {
        public Name(Token t) : base(t) {}
        public string name() { return token().getText(); }
    }
}