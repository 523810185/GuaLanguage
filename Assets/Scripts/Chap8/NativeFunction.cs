namespace GuaLanguage 
{
    using System;
    using GuaLanguage.AST;
    using System.Reflection;

    public class NativeFunction 
    {
        protected MethodInfo method;
        protected string name;
        protected int numParams;
        public NativeFunction(string n, MethodInfo m) 
        {
            name = n;
            method = m;
            numParams = m.GetParameters().Length;
        }

        public int numOfParameters() { return numParams; }
        
        public override string ToString()
        {
            return "<native:" + GetHashCode() + ">";
        }

        public object invoke(object[] args, ASTree tree) 
        {
            try
            {
                return method.Invoke(null, args);
            }
            catch (Exception e) 
            {
                throw new GuaException("bad native function call: " + name, tree);
            }
        }
    }
}