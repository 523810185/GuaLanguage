namespace GuaLanguage.AST 
{
    using GuaLanguage;
    using System.Collections.Generic;
    public class Arguments : Postfix 
    {
        public Arguments(List<ASTree> c) :base(c) {}
        public int size() { return numChildren(); }

        public override object eval(Environment callerEnv, object value)
        {
            if(!(value is NativeFunction)) 
            {
                return callFunc(callerEnv, value as Function);
            }

            NativeFunction func = value as NativeFunction;
            int nparams = func.numOfParameters();
            if(size() != nparams) 
            {
                throw new GuaException("bad number of arguments", this);
            }
            object[] args = new object[nparams];
            int num = 0;
            foreach (var a in this)
            {
                args[num++] = a.eval(callerEnv);
            }
            return func.invoke(args, this);
        }

        private object callFunc(Environment callerEnv, Function func)
        {
            if(func == null)
            {
                throw new GuaException("bad function", this);
            }

            var _params = func.parameters();
            if(size() != _params.size())
            {
                throw new GuaException("bad number of arguments", this);
            }

            var newEnv = func.makeEnv();
            int num = 0;
            foreach (var a in this)
            {
                _params.eval(newEnv, num++, a.eval(callerEnv));
            }
            return func.body().eval(newEnv);
        }
    }
}