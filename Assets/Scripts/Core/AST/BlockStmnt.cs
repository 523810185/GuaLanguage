namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class BlockStmnt : ASTList 
    {
        public BlockStmnt(List<ASTree> c) : base(c) {}

        public override object eval(Environment env)
        {
            var bs = this;
            object result = 0;
            foreach (var t in bs)
            {
                if(!(t is NullStmnt)) 
                {
                    result = t.eval(env);
                }
            }

            return result;
        }
    }
}