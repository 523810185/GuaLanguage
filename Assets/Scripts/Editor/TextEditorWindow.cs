using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using GuaLanguage;
using GuaLanguage.AST;
using System.Text;
using System.Text.RegularExpressions;
using GuaLanguage.Utility;

public class TextEditorWindow : EditorWindow
{
    [MenuItem("Tool/脚本工具")]
    public static void OpenWindow()
    {
        var window = EditorWindow.GetWindow(typeof(TextEditorWindow), false, "脚本编辑器", true) as TextEditorWindow;
        window.Init();
        window.Show();
    }

    private delegate string ScrollableTextAreaInternalDelegate(Rect position, string text, ref Vector2 scrollPosition, GUIStyle style);
    private static readonly ScrollableTextAreaInternalDelegate EditorGUI_ScrollableTextAreaInternal;
    static TextEditorWindow()
    {
        var method = typeof(EditorGUI).GetMethod("ScrollableTextAreaInternal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        if (method != null)
        {
            EditorGUI_ScrollableTextAreaInternal = (ScrollableTextAreaInternalDelegate)Delegate.CreateDelegate(typeof(ScrollableTextAreaInternalDelegate), method);
        }
    }

    private Lexer m_stHighlightLexer = new Lexer(null);
    private void Init()
    {
        TextEditorHandler.Instance.Clear();
    }

    private void Update() {
        Repaint();    
    }

    private Vector2 scrollPosition;
    private string m_sLastText = ""; // 上次的输入文本，带有富文本信息
    private string m_sContent = ""; // 源文本
    private Dictionary<int, string> m_mapLineStrInfo = new Dictionary<int, string>();
    private int m_iNowRow = -1, m_iNowCow = -1; 
    void OnGUI() 
    {
        var e = Event.current;
        m_sContent = TextEditorHandler.Instance.HandleEvent(e);
        // if(e.type == EventType.MouseUp) 
        // {
        //     Debug.Log(" mouse pos " + e.mousePosition);
        //     // if()
        //     // e.Use();
        //     Debug.Log(" ?? " + FontUtility.GetStringWidthByFont(EditorStyles.standardFont, "// p3d.p"));
        //     var _pos = e.mousePosition;
        //     int _nowLine = Mathf.FloorToInt(_pos.y / FontUtility.LINE_HEIGHT) + 1;
        //     int _nowCow = -1;
        //     {
        //         string _str = null;
        //         m_mapLineStrInfo.TryGetValue(_nowLine, out _str);
        //         if(_str != null)
        //         {
        //             int _l = 0;
        //             int _r = _str.Length - 1;
        //             while(_l <= _r) 
        //             {
        //                 int _mid = (_l + _r) / 2;
        //                 if(FontUtility.GetStringWidthByFont(EditorStyles.standardFont, _str.Substring(0, _mid + 1)) <= _pos.x)
        //                 {
        //                     _nowCow = _mid;
        //                     _l = _mid + 1;
        //                 }
        //                 else 
        //                 {
        //                     _r = _mid - 1;
        //                 }
        //             }

        //             Debug.Log(" 当前字符为 " + _str[_nowCow]);
        //         }
        //     }
        //     m_iNowRow = _nowLine;
        //     m_iNowCow = _nowCow;
        // }
        // else if(e.type == EventType.KeyUp) 
        // {
        //     if(e.keyCode == KeyCode.Backspace) 
        //     {
        //         // 删除
        //         if(m_iNowRow != -1 && m_iNowCow != -1)
        //         {
        //             var _str = m_mapLineStrInfo[m_iNowRow];
        //             m_mapLineStrInfo[m_iNowRow] = _str.Substring(0, m_iNowCow) +  _str.Substring(m_iNowCow + 1, _str.Length - 1 - m_iNowCow);
        //             m_iNowCow--;
        //             m_sRichTextBuilder.Clear();
        //             foreach (var v in m_mapLineStrInfo.Values)
        //             {
        //                 m_sRichTextBuilder.Append(v);
        //             }
        //             m_sContent = m_sRichTextBuilder.ToString();
        //         }
        //     }
        //     else 
        //     {
        //         if(m_iNowRow != -1 && m_iNowCow != -1)
        //         {
        //             var _str = m_mapLineStrInfo[m_iNowRow];
        //             m_mapLineStrInfo[m_iNowRow] = _str.Substring(0, m_iNowCow + 1) + e.keyCode.ToString().ToLower() + _str.Substring(m_iNowCow + 1, _str.Length - 1 - m_iNowCow);
        //             m_iNowCow++;
        //             m_sRichTextBuilder.Clear();
        //             foreach (var v in m_mapLineStrInfo.Values)
        //             {
        //                 m_sRichTextBuilder.Append(v);
        //             }
        //             m_sContent = m_sRichTextBuilder.ToString();
        //         }
        //     }
        // }

        // if(m_iNowRow != -1 && m_iNowCow != -1)
        // {
        //     var rect = new Rect(FontUtility.GetStringWidthByFont(EditorStyles.standardFont, m_mapLineStrInfo[m_iNowRow].Substring(0, m_iNowCow + 1)), (m_iNowRow - 1) * FontUtility.LINE_HEIGHT + 3, 2, FontUtility.LINE_HEIGHT);
        //     EditorGUI.DrawRect(rect, Color.white);
        // }

        if (EditorGUI_ScrollableTextAreaInternal == null)
        {
            EditorGUILayout.LabelField("Cannot draw TextArea because Unity's internal API has changed.");
            return;
        }

        var position = EditorGUILayout.GetControlRect(false, this.position.height - 45f);

        if(m_bHighlightMode) 
        {
            var _style = EditorStyles.textArea;
            _style.richText = true;
            GUI.Label(position, m_sHighlightText = GetHighlightText(), _style);
        }
        else 
        {
            var _style = EditorStyles.textArea; _style.richText = true;
            EditorGUI.BeginChangeCheck();
            m_sContent = EditorGUI_ScrollableTextAreaInternal(position, m_sContent, ref this.scrollPosition, _style);
            bool changed = EditorGUI.EndChangeCheck();
            // m_sContent = Regex.Replace(m_sLastText, "(<color=#[0-9a-zA-Z][0-9a-zA-Z][0-9a-zA-Z][0-9a-zA-Z][0-9a-zA-Z][0-9a-zA-Z]>)|</color>", "");
            if(changed) 
            {
                bool more = m_sContent.Length > m_sLastText.Length ? true : false;
                Debug.Log(" changed! " + more);
                m_mapLineStrInfo.Clear();
                int _st = 0;
                // int _ed = 0;
                int lineNum = 1;
                for(int i=0;i<m_sContent.Length;i++) 
                {
                    char nowC = m_sContent[i];
                    if(nowC == '\n') 
                    {
                        m_mapLineStrInfo.Add(lineNum, m_sContent.Substring(_st, i - _st + 1));
                        _st = i + 1;
                        lineNum++;
                    }
                }
                m_mapLineStrInfo.Add(lineNum, m_sContent.Substring(_st, m_sContent.Length - 1 - _st + 1));
            }
        }

        if(GUILayout.Button("解释"))
        {
            Lexer l = new Lexer(m_sContent);
            HighlightHelper.Instance.Clear();

            // 词法分析器
            // for(Token t; (t = l.read()) != Token.EOF; )
            // {
            //     Debug.LogFormat("=> {0}, st: {1}, ed: {2}", t.getText(), t.getST(), t.getED());
            // }

            // 语法分析器
            // BasicParser bp = new BasicParser();
            // while(l.peek(0) != Token.EOF)
            // {
            //     var ast = bp.parse(l);
            //     Debug.Log("=> " + ast.ToString() + " " + ast.GetType());
            // }

            // 解释器
            // var bp = new BasicParser();
            // var env = new BasicEnv();
            // while(l.peek(0) != Token.EOF) 
            // {
            //     var t = bp.parse(l);
            //     if(!(t is NullStmnt))
            //     {
            //         // Debug.Log(" bef eval = " + t.GetType());
            //         var r = t.eval(env);
            //         Debug.Log("=> " + r);
            //     }
            // }

            // 带函数的解释器
            // var bp = new FuncParser();
            // var env = new NestedEnv();
            // while(l.peek(0) != Token.EOF) 
            // {
            //     var t = bp.parse(l);
            //     if(!(t is NullStmnt))
            //     {
            //         // Debug.Log(" bef eval = " + t.GetType());
            //         var r = t.eval(env);
            //         Debug.Log("=> " + r);
            //     }
            // }

            // 带闭包的解释器
            // var bp = new ClosureParser();
            // var env = new NestedEnv();
            // while(l.peek(0) != Token.EOF) 
            // {
            //     var t = bp.parse(l);
            //     if(!(t is NullStmnt))
            //     {
            //         // Debug.Log(" bef eval = " + t.GetType());
            //         var r = t.eval(env);
            //         Debug.Log("=> " + r);
            //     }
            // }

            // 带原生方法的解释器
            // var bp = new ClosureParser();
            // var env = new Natives().environment(new NestedEnv());
            // while(l.peek(0) != Token.EOF) 
            // {
            //     var t = bp.parse(l);
            //     if(!(t is NullStmnt))
            //     {
            //         // Debug.Log(" bef eval = " + t.GetType());
            //         var r = t.eval(env);
            //         // Debug.Log("=> " + r);
            //     }
            // }

            // 带class的解释器
            // var bp = new ClassParser();
            // var env = new Natives().environment(new NestedEnv());
            // while(l.peek(0) != Token.EOF) 
            // {
            //     var t = bp.parse(l);
            //     if(!(t is NullStmnt))
            //     {
            //         // Debug.Log(" bef eval = " + t.GetType());
            //         var r = t.eval(env);
            //         // Debug.Log("=> " + r);
            //     }
            // }

            // 带数组的解释器
            var bp = new ArrayParser();
            var env = new Natives().environment(new NestedEnv());
            while(l.peek(0) != Token.EOF) 
            {
                var t = bp.parse(l);
                if(!(t is NullStmnt))
                {
                    // Debug.Log(" bef eval = " + t.GetType());
                    var r = t.eval(env);
                    // Debug.Log("=> " + r);
                }
            }

            // 测试
            // var bp = new TestParser();
            // var env = new Natives().environment(new NestedEnv());
            // while(l.peek(0) != Token.EOF) 
            // {
            //     var t = bp.parse(l);
            //     if(!(t is NullStmnt))
            //     {
            //         var ttt = t.ToString();
            //         Debug.Log(" bef eval = " + t.GetType() + t.ToString());
            //         // var r = t.eval(env);
            //         // Debug.Log("=> " + r);
            //     }
            // }
        }

        if(GUILayout.Button("高亮"))
        {
            m_bHighlightMode = !m_bHighlightMode;
            if(m_bHighlightMode)
            {
                m_sHighlightText = this.GetHighlightText();
            }
        }
    }

    private GuaLanguage.Environment m_stEnv = null;
    private bool m_bHighlightMode = false;
    private string m_sHighlightText = null;
    private StringBuilder m_sRichTextBuilder = new StringBuilder();
    private string GetHighlightText()
    {
        void Colorize(string str, Color color) 
        {
            this.m_sRichTextBuilder.Append("<color=#");
            this.m_sRichTextBuilder.Append(ColorUtility.ToHtmlStringRGBA(color));
            this.m_sRichTextBuilder.Append(">");
            this.m_sRichTextBuilder.Append(str);
            this.m_sRichTextBuilder.Append("</color>");
        }

        m_sRichTextBuilder.Clear();
        var l = new Lexer(m_sContent).SetHighlightMode(true);
        List<Token> list = new List<Token>();
        for(Token t; (t = l.read()) != Token.EOF; )
        {
            list.Add(t);
        }

        for(int i=0;i<list.Count;i++)
        {
            Token lastToken = i - 1 >= 0 ? list[i - 1] : null;
            Token nowToken = list[i];
            Token nextToken = i + 1 < list.Count ? list[i + 1] : null;
            // 附加空格
            // if(lastToken == null || lastToken.getLineNumber() != nowToken.getLineNumber()) 
            {
                int cnt = nowToken.getST() - ((lastToken == null || lastToken.getLineNumber() != nowToken.getLineNumber()) ? 0 : lastToken.getED() + 1);
                for(int _=0;_<cnt;_++) 
                {
                    m_sRichTextBuilder.Append(" ");
                }
            }
            var text = nowToken.getText();
            if(nowToken.getText() == Token.EOL)
            {
                m_sRichTextBuilder.Append("\n");
            }
            else if(text == "if" || text == "else" || text == "while") 
            {
                Colorize(nowToken.getText(), FunctionClaimColor);
            }
            else if(text == "class" || text == "extends" || text == "new" || text == "this") 
            {
                Colorize(text, KeyWordColor);
            }
            else if(nextToken != null && nextToken.getText() == "(") 
            {
                Colorize(text, FunctionCallColor);
            }
            else if(text == "{" || text == "}" || text == "(" || text == ")" || text == "[" || text == "]" || text == "+" || text == "-" || text == "*" || text == "/" || text == "=" || text == "==" || text == "<" || text == ">" || text == "<=" || text == ">=" || text == ";" || text == "," || text == ".") 
            {
                Colorize(text, Color.white);
            }
            else if(nowToken.getText() == "def" || nowToken.getText() == "fun")
            {
                Colorize(nowToken.getText(), FunctionClaimColor);
            }
            else if(nowToken is Lexer.StrToken) 
            {
                Colorize(nowToken.getText(), StringLiteralColor);
            }
            else if(nowToken is Lexer.NumToken) 
            {
                Colorize(nowToken.getText(), LiteralColor);
            }
            else if(nowToken is Lexer.IdToken) 
            {
                // if(nowToken is Lexer.OpToken) 
                // {
                //     Colorize(nowToken.getText(), Color.white);
                // }
                // else 
                {
                    Colorize(nowToken.getText(), IdentifierColor);
                }
            }
            else if(nowToken is Lexer.CommentToken) 
            {
                Colorize(text, CommentColor);
            }

            lastToken = nowToken;
        }

        return m_sRichTextBuilder.ToString();
    }

    public static Color BackgroundColor = new Color(0.118f, 0.118f, 0.118f, 1f);
    public static Color TextColor = new Color(0.863f, 0.863f, 0.863f, 1f);
    // public static Color KeywordColor = new Color(0.337f, 0.612f, 0.839f, 1f);
    public static Color IdentifierColor = new Color(149f / 255, 219f / 255, 252f / 255f, 1f);
    public static Color CommentColor = new Color(0.341f, 0.651f, 0.29f, 1f);
    public static Color LiteralColor = new Color(0.71f, 0.808f, 0.659f, 1f);
    public static Color StringLiteralColor = new Color(0.839f, 0.616f, 0.522f, 1f);
    public static Color FunctionClaimColor = new Color(203f / 255, 132f / 255, 190f / 255, 1f); // def fun
    public static Color FunctionCallColor = new Color(219f / 255, 221f / 255, 174f / 255, 1f);
    public static Color DefaultColor = new Color(200f / 255, 200f / 255, 200f / 255, 1f);
    public static Color KeyWordColor = new Color(79f / 255, 154f / 255, 211f / 255, 1f); // public class
}
