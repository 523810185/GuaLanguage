using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using GuaLanguage.Utility;

public class TextEditorHandler
{
    private TextEditorHandler() {}
    private static TextEditorHandler m_pInstance = null;
    public static TextEditorHandler Instance
    {
        get 
        {
            if(m_pInstance == null) 
            {
                m_pInstance = new TextEditorHandler();
            }

            return m_pInstance;
        }
    }

    private StringBuilder m_sStringBuilder = new StringBuilder();
    private string m_sContent = "";
    private Dictionary<int, string> m_mapLineStrInfo = new Dictionary<int, string>();
    private int m_iNowRow = -1, m_iNowCow = -1; 
    public void Clear() 
    {
        m_sContent = "";
        m_sStringBuilder.Clear();
        m_mapLineStrInfo.Clear();
        m_mapLineStrInfo.Add(1, "\n");
    }

    public string HandleEvent(Event e) 
    {
        if(e.type == EventType.MouseUp) 
        {
            HandleMouseClick(e);
        }
        else if(e.type == EventType.KeyUp) 
        {
            var keyCode = e.keyCode;
            if(keyCode == KeyCode.Backspace) 
            {
                HandleBackspace();
            }
            else 
            {
                HandleInput(e);
            }
        }

        DrawGUI();

        RebuildContent();
        return m_sContent;
    }

    private void DrawGUI()
    {
        if(m_iNowRow != -1 && m_iNowCow != -1)
        {
            var rect = new Rect(FontUtility.GetStringWidthByFont(EditorStyles.standardFont, m_mapLineStrInfo[m_iNowRow].Substring(0, m_iNowCow + 1)), (m_iNowRow - 1) * FontUtility.LINE_HEIGHT + 3, 2, FontUtility.LINE_HEIGHT);
            EditorGUI.DrawRect(rect, Color.white);
        }
    }

    private void RebuildContent()
    {
        m_sStringBuilder.Clear();
        foreach (var v in m_mapLineStrInfo.Values)
        {
            m_sStringBuilder.Append(v);
        }
        m_sContent = m_sStringBuilder.ToString();
    }

    private void HandleMouseClick(Event e) 
    {
        var _pos = e.mousePosition;
        int _nowLine = Mathf.FloorToInt(_pos.y / FontUtility.LINE_HEIGHT) + 1;
        int _nowCow = -1;
        {
            string _str = null;
            m_mapLineStrInfo.TryGetValue(_nowLine, out _str);
            if(_str != null)
            {
                int _l = 0;
                int _r = _str.Length - 1;
                while(_l <= _r) 
                {
                    int _mid = (_l + _r) / 2;
                    if(FontUtility.GetStringWidthByFont(EditorStyles.standardFont, _str.Substring(0, _mid + 1)) <= _pos.x)
                    {
                        _nowCow = _mid;
                        _l = _mid + 1;
                    }
                    else 
                    {
                        _r = _mid - 1;
                    }
                }

                Debug.Log(" 当前字符为 " + _str[_nowCow]);
            }
        }
        m_iNowRow = _nowLine;
        m_iNowCow = _nowCow;
    }

    private void HandleBackspace()
    {
        // 删除
        if(m_iNowRow != -1 && m_iNowCow != -1)
        {
            var _str = m_mapLineStrInfo[m_iNowRow];
            m_mapLineStrInfo[m_iNowRow] = _str.Substring(0, m_iNowCow) +  _str.Substring(m_iNowCow + 1, _str.Length - 1 - m_iNowCow);
            m_iNowCow--;
        }
    }

    private void HandleInput(Event e) 
    {
        if(m_iNowRow != -1 && m_iNowCow != -1)
        {
            var _str = m_mapLineStrInfo[m_iNowRow];
            m_mapLineStrInfo[m_iNowRow] = _str.Substring(0, m_iNowCow + 1) + e.keyCode.ToString().ToLower() + _str.Substring(m_iNowCow + 1, _str.Length - 1 - m_iNowCow);
            m_iNowCow++;
        }
    }
}
