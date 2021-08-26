namespace GuaLanguage
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    public class Lexer
    {
        /* a = "a\\aa"; b = "\"" */
        public static String regexPat
                = "\\s*((//.*)"
                    + "|([0-9]+)"
                    + "|(\"(\\\\\"|[^\"])*\")"
                    + "|[A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\||[!\"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~])?";
        private List<Token> queue = new List<Token>();
        private bool hasMore;

        private int m_iLineNum = -1;
        private int m_iTextPos = -1;
        private string m_sText;
        public Lexer(string text)
        {
            hasMore = true;
            m_sText = text;
            m_iTextPos = 0;
            m_iLineNum = 0;
        }

        private bool m_bHighlightMode = false;
        public Lexer SetHighlightMode(bool val) 
        {
            m_bHighlightMode = val;
            return this;
        }

        public Token read() 
        {
            if(fillQueue(0))
            {
                var ans = queue[0];
                queue.RemoveAt(0);
                return ans;
            }
            else 
            {
                return Token.EOF;
            }
        }

        public Token peek(int i) 
        {
            if(fillQueue(i))
            {
                return queue[0];
            }
            else 
            {
                return Token.EOF;
            }
        }

        public bool fillQueue(int i) 
        {
            while(i >= queue.Count) 
            {
                if(hasMore) 
                {
                    readLine();
                }
                else 
                {
                    return false;
                }
            }
            return true;
        }

        protected void readLine()
        {
            string line = readLineInner();
            if(line == null)
            {
                hasMore = false;
                return;
            }
            int lineNo = this.m_iLineNum;
            Match matcher = Regex.Match(line, regexPat);
            while(matcher.Success)
            {
                //显示匹配开始处的索引值和匹配到的值
                // Debug.Log("Match=[" + matcher + "]");
                // // CaptureCollection cc = matcher.Captures;
                // // foreach (Capture c in cc)
                // // {
                // //     Debug.Log("\tCapture=[" + c + "]");
                // // }
                // for (int i = 0; i < matcher.Groups.Count; i++)
                // {
                //     Group group = matcher.Groups[i];
                //     Debug.LogFormat("\t\tGroups[{0}]=[{1}]", i, group);
                //     // for (int j = 0; j < group.Captures.Count; j++)
                //     // {
                //     //     Capture capture = group.Captures[j];
                //     //     Debug.LogFormat("\t\t\tCaptures[{0}]=[{1}]", j, capture);
                //     // }
                // }

                addToken(lineNo, matcher);

                //进行下一次匹配.
                matcher = matcher.NextMatch();
            }

            queue.Add(new IdToken(lineNo, line.Length, line.Length, Token.EOL));

            // 显示所有Token
            // foreach (var item in queue)
            // {
            //     Debug.Log("=> " + item);
            // }
        }

        protected void addToken(int lineNo, Match matcher) 
        {
            bool GroupIsNull(Group g) 
            {
                return g == null || string.IsNullOrEmpty(g.ToString());
            }
            var m = matcher.Groups[1];
            if(GroupIsNull(m) == false) // if not a space
            {
                if (GroupIsNull(matcher.Groups[2])) { // if not a comment
                    Token token;
                    int st = m.Index;
                    int ed = st + m.Length - 1;
                    if (GroupIsNull(matcher.Groups[3]) == false)
                        token = new NumToken(lineNo, st, ed, int.Parse(m.ToString()));
                    else if (GroupIsNull(matcher.Groups[4]) == false)
                        token = new StrToken(lineNo, st, ed, m_bHighlightMode ? m.ToString() : toStringLiteral(m.ToString()));
                    else
                    {
                        var str = m.ToString();
                        // TODO.. 符号集专门写在一个地方
                        if(str == "+" || str == "-" || str == "*" || str == "/") 
                        {
                            token = new OpToken(lineNo, st, ed, str);
                        }
                        else 
                        {
                            token = new IdToken(lineNo, st, ed, str);
                        }
                    }
                    queue.Add(token);
                }
                else
                {
                    if(m_bHighlightMode) 
                    {
                        var commentGroup = matcher.Groups[2];
                        queue.Add(new CommentToken(lineNo, commentGroup.Index, commentGroup.Index + commentGroup.Length - 1, commentGroup.ToString()));
                    }
                }
            }
        } 

        protected String toStringLiteral(String s) {
            StringBuilder sb = new StringBuilder();
            int len = s.Length - 1;
            for (int i = 1; i < len; i++) {
                char c = s[i];
                if (c == '\\' && i + 1 < len) {
                    int c2 = s[i + 1];
                    if (c2 == '"' || c2 == '\\')
                        c = s[++i];
                    else if (c2 == 'n') {
                        ++i;
                        c = '\n';
                    }
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public class NumToken : Token {
            private int value;

            public NumToken(int line, int st, int ed, int v) : base(line, st, ed) {
                value = v;
            }
            public override bool isNumber() { return true; }
            public override string getText() { return value.ToString(); }
            public override int getNumber() { return value; }
        }

        public class IdToken : Token {
            private string text; 
            public IdToken(int line, int st, int ed, string id) : base(line, st, ed) {
                text = id;
            }
            public override bool isIdentifier() { return true; }
            public override string getText() { return text; }
        }

        public class StrToken : Token {
            private string literal;
            public StrToken(int line, int st, int ed, string str) :base(line, st, ed) {
                literal = str;
            }
            public override bool isString() { return true; }
            public override string getText() { return literal; }
        }

        public class OpToken : IdToken 
        {
            public OpToken(int line, int st, int ed, string id) : base(line, st, ed, id) 
            {
            }
        }

        public class CommentToken : Token 
        {
            private string comment;
            public CommentToken(int line, int st, int ed, string str) :base(line, st, ed) {
                comment = str;
            }
            public override string getText() { return comment; }
        }

        private string readLineInner()
        {
            if(string.IsNullOrEmpty(m_sText) || m_iTextPos < 0 || m_iTextPos >= m_sText.Length) 
            {
                return null;
            }

            StringBuilder sb = new StringBuilder("");
            while(0 <= m_iTextPos && m_iTextPos < m_sText.Length) 
            {
                var nowChar = m_sText[m_iTextPos++];
                if(nowChar == '\n') 
                {
                    break;
                }

                sb.Append(nowChar);
            }

            m_iLineNum++;
            return sb.ToString();
        }
    }
}
