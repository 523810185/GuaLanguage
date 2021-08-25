namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class WhileStmnt : ASTList 
    {
        public WhileStmnt(List<ASTree> c) : base(c) {}
        public ASTree condition() { return child(0); }
        public ASTree body() { return child(1); }
        public override string ToString()
        {
            return string.Format("(while {0} {1} )", condition(), body());
        }

        public override object eval(Environment env)
        {
            var ws = this;
            object result = 0;
            for(;;) 
            {
                object c = ws.condition().eval(env);
                if(c is int && (int)c == FALSE) 
                {
                    return result;
                }
                else 
                {
                    result = ws.body().eval(env);
                }
            }
        }
    }
}