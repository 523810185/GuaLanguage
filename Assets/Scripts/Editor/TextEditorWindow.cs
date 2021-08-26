using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using GuaLanguage;
using GuaLanguage.AST;
using System.Text;

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

    }

    private Vector2 scrollPosition;
    private string m_sContent = "";
    void OnGUI() 
    {
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
            GUI.Label(position, m_sHighlightText, _style);
        }
        else 
        {
            m_sContent = EditorGUI_ScrollableTextAreaInternal(position, m_sContent, ref this.scrollPosition, EditorStyles.textArea);
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
                if(nowToken is Lexer.CommentToken) 
                {
                    int xxxx = 55;
                }
                if(nowToken.getText().StartsWith("// 测试"))
                {
                    int x = 5;
                    int a1 = nowToken.getST();
                    int a2 = lastToken.getLineNumber();
                    int a3 = nowToken.getLineNumber();
                    int a4 = lastToken.getED();
                }
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
            else if(text == "if" || text == "while") 
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
            else if(text == "{" || text == "}" || text == "(" || text == ")" || text == "[" || text == "]" || text == "+" || text == "-" || text == "*" || text == "/" || text == "=" || text == "==" || text == "<=" || text == ">=" || text == ";" || text == "," || text == ".") 
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
