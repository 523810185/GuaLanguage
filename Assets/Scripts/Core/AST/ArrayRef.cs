namespace GuaLanguage.AST 
{
    using System.Collections.Generic;
    public class ArrayRef : Postfix
    {
        public ArrayRef(List<ASTree> c) : base(c) {}
        public ASTree index() { return child(0); }

        public override string ToString()
        {
            return "[" + index() + "]";
        }

        public override object eval(Environment env, object value)
        {
            if(value is object[]) 
            {
                object _index = index().eval(env);
                if(_index is int) 
                {
                    return ((object[])value)[(int)_index];
                }
            }

            throw new GuaException("bad array access", this);
        }
    }
}