namespace GuaLanguage
{
    using System.Collections.Generic;
    using System;
    using System.Text;
    public class ASTList : ASTree
    {
        protected List<ASTree> m_children;
        public ASTList(List<ASTree> list) { m_children = list; }
        public override ASTree child(int i) { return m_children[i]; }
        public override int numChildren() { return m_children.Count; }
        public override IEnumerator<ASTree> children() { return m_children.GetEnumerator(); }
        public String toString() {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            String sep = "";
            foreach(ASTree t in m_children) {
                builder.Append(sep);
                sep = " ";
                builder.Append(t.ToString());
            }
            return builder.Append(')').ToString();
        }
        public override String location() {
            foreach(ASTree t in m_children) {
                String s = t.location();
                if (s != null)
                    return s;
            }
            return null;
        }
    }
}
