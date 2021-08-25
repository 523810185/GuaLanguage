using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using GuaLanguage;

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

        var position = EditorGUILayout.GetControlRect(false, 500f);

        m_sContent = EditorGUI_ScrollableTextAreaInternal(position, m_sContent, ref this.scrollPosition, EditorStyles.textArea);
        if(GUILayout.Button("解释"))
        {
            Lexer l = new Lexer(m_sContent);

            // for(Token t; (t = l.read()) != Token.EOF; )
            // {
            //     Debug.Log("=> " + t.getText());
            // }

            BasicParser bp = new BasicParser();
            while(l.peek(0) != Token.EOF)
            {
                var ast = bp.parse(l);
                Debug.Log("=> " + ast.ToString() + " " + ast.GetType());
            }
        }
    }
}
