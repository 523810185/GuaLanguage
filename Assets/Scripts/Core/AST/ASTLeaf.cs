namespace GuaLanguage
{
    using System.Collections.Generic;
    using System;
    public class ASTLeaf : ASTree
    {
        private static List<ASTree> empty = new List<ASTree>(); 
        protected Token m_token;
        public ASTLeaf(Token t) { m_token = t; }
        public override ASTree child(int i) { throw new IndexOutOfRangeException(); }
        public override int numChildren() { return 0; }
        public override IEnumerator<ASTree> children() { return empty.GetEnumerator(); }
        public override String ToString() { return m_token.getText(); }
        public override String location() { return "at line " + m_token.getLineNumber(); }
        public Token token() { return m_token; }

        public override object eval(Environment env)
        {
            throw new GuaException("not imp eval " + ToString(), this);
        }
    }
}
