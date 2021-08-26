namespace GuaLanguage.AST 
{
    using System.Collections.Generic;
    public abstract class Postfix : ASTList 
    {
        public Postfix(List<ASTree> c) :base(c) {}
        public abstract object eval(Environment env, object value);
    }
}