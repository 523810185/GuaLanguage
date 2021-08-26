namespace GuaLanguage.AST 
{
    using System.Collections.Generic;
    public class ClassBody : ASTList 
    {
        public ClassBody(List<ASTree> c) :base(c) {}

        public override object eval(Environment env)
        {
            foreach (var t in this)
            {
                t.eval(env);
            }

            return null;
        }
    }
}