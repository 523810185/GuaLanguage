namespace GuaLanguage.AST 
{
    using System.Collections.Generic;
    public class ArrayLiteral : ASTList
    {
        public ArrayLiteral(List<ASTree> list) : base(list) {}
        public int size() { return numChildren(); }

        public override object eval(Environment env)
        {
            int s = numChildren();
            object[] res = new object[s];
            int i = 0;
            foreach (var t in this)
            {
                res[i++] = t.eval(env);
            }

            return res;
        }
    }
}