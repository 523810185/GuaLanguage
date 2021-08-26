namespace GuaLanguage 
{
    using System;
    public class GuaObject 
    {
        public class AccessException : Exception {}

        protected Environment env;
        public GuaObject(Environment e) { env = e; }

        public override string ToString()
        {
            return "<object:" + GetHashCode() + ">";
        }

        public object read(string member)
        {
            return getEnv(member).get(member);
        }

        public void write(string member, object value) 
        {
            getEnv(member).putNew(member, value);
        }

        protected Environment getEnv(string member) 
        {
            Environment e = env.where(member);
            if(e != null && e == env) 
            {
                return e;
            }
            else 
            {
                throw new AccessException();
            }
        }
    }
}