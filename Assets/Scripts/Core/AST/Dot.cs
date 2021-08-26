namespace GuaLanguage.AST 
{
    using System.Collections.Generic;
    using static GuaObject;
    public class Dot : Postfix 
    {
        public Dot(List<ASTree> c) :base(c) {}
        public string name() { return (child(0) as ASTLeaf).token().getText(); }

        public override string ToString()
        {
            return "." + name();
        }

        public override object eval(Environment env, object value)
        {
            string member = name();
            if(value is ClassInfo) 
            {
                if(member == "new") 
                {
                    ClassInfo ci = value as ClassInfo;
                    NestedEnv e = new NestedEnv(ci.environment());
                    GuaObject go = new GuaObject(e);
                    e.putNew("this", go);
                    initObject(ci, e);
                    return go;
                }
            }
            else if(value is GuaObject) 
            {
                try 
                {
                    return (value as GuaObject).read(member);
                }
                catch (AccessException e) {}
            }

            throw new GuaException("bad member access: " + member, this);
        }

        protected void initObject(ClassInfo ci, Environment env) 
        {
            if(ci.superClass() != null) 
            {
                initObject(ci.superClass(), env);
            }

            ci.body().eval(env);
        }
    }
}