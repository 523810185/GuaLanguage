namespace GuaLanguage 
{
    using GuaLanguage.AST;

    public class Function 
    {
        protected ParameterList m_parameters;
        protected BlockStmnt m_body;
        protected Environment m_env;
        public Function(ParameterList parameters, BlockStmnt body, Environment env) 
        {
            this.m_parameters = parameters;
            this.m_body = body;
            this.m_env = env;
        }
        public ParameterList parameters() { return m_parameters; }
        public BlockStmnt body() { return m_body; }
        public Environment makeEnv() { return new NestedEnv(m_env); }
        public override string ToString()
        {
            return string.Format("<fun{0}>", GetHashCode());
        }
    }
}