namespace GuaLanguage 
{
    using System;
    using System.Collections.Generic;
    using GuaLanguage.AST;

    public class ClassInfo 
    {
        protected ClassStmnt m_definition;
        protected Environment m_environment;
        protected ClassInfo m_superClass;
        public ClassInfo(ClassStmnt cs, Environment env)
        {
            m_definition = cs;
            m_environment = env;
            object obj = env.get(cs.superClass());
            if(obj == null) 
            {
                m_superClass = null;
            }
            else if(obj is ClassInfo) 
            {
                m_superClass = obj as ClassInfo;
            }
            else 
            {
                throw new GuaException("unknown super class: " + cs.superClass(), cs);
            }
        }

        public string name() { return m_definition.name(); }
        public ClassInfo superClass() { return m_superClass; }
        public ClassBody body() { return m_definition.body(); }
        public Environment environment() { return m_environment; }

        public override string ToString()
        {
            return "<class " + name() + ">";
        }
    }
}