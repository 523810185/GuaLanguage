namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class Name : ASTLeaf 
    {
        public Name(Token t) : base(t) {}
        public string name() { return token().getText(); }

        public override object eval(Environment env)
        {
            var name = this;
            object value = env.get(name.name());
            if(value == null) 
            {
                throw new GuaException("undefined name: " + name.name(), name);
            }
            else 
            {
                return value;
            }
        }
    }
}