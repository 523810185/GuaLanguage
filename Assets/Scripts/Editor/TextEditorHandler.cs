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
    private Dictionary<int, string> m_mapLineStrInfo = new Dictionary<int, string>(); // 其实可以直接使用List..
    private Dictionary<int, int> m_mapLineTabCnt = new Dictionary<int, int>();
    private int m_iNowLine = -1, m_iNowCharID = -1; // 行号（1开始），当前字符下标（0开始，光标在这个字符前面，-1表示空行，TODO.. 后面去掉这个特殊处理）
    public void Clear() 
    {
        m_sContent = "";
        m_sStringBuilder.Clear();
        m_mapLineStrInfo.Clear();
        m_mapLineTabCnt.Clear();
        m_mapLineStrInfo.Add(1, "\n");
        m_mapLineTabCnt.Add(1, 0);
        m_iNowLine = -1;
        m_iNowCharID = -1;
    }

    public string HandleEvent(Event e) 
    {
        if(e.type == EventType.MouseUp) 
        {
            HandleMouseClick(e);
        }
        else if(e.type == EventType.KeyDown) 
        {
            var keyCode = e.keyCode;
            if(keyCode == KeyCode.None) 
            {
                // 为什么KeyDown会有None，KeyUp没有啊？
                e.Use();
            }
            else if(keyCode == KeyCode.Backspace) 
            {
                HandleBackspace();
            }
            else if(keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
            {
                HandleEnter();
            }
            else if(keyCode == KeyCode.End) 
            {
                HandleEnd();
            }
            else if(keyCode == KeyCode.CapsLock || keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift) 
            {
                e.Use();
            }
            else if(keyCode == KeyCode.LeftArrow || keyCode == KeyCode.RightArrow || keyCode == KeyCode.UpArrow || keyCode == KeyCode.DownArrow) 
            {
                HandleCursorMoveByKeyCode(keyCode);
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
        if(m_iNowLine != -1)
        {
            // 在当前点击的字符前面画一个光标
            var cursorPos = GetCursorPosByCurLineAndCharIndex();
            var rect = new Rect(cursorPos.x,
                cursorPos.y, 
                2, 
                TextBoardUtility.LINE_HEIGHT);
            EditorGUI.DrawRect(rect, Color.white);
        }
    }

    private List<string> m_stTempStrList = new List<string>(); // TODO.. 后面直接用这个把m_mapLineStrInfo替换掉
    private void RebuildContent()
    {
        m_sStringBuilder.Clear();
        m_stTempStrList.Clear();
        for(int i=0;i<m_mapLineStrInfo.Count;i++) 
        {
            m_stTempStrList.Add(null);
        }
        foreach (var kv in m_mapLineStrInfo)
        {
            int id = kv.Key;
            string str = kv.Value;
            m_stTempStrList[id - 1] = str;
        }
        foreach (var str in m_stTempStrList)
        {
            m_sStringBuilder.Append(str);
        }
        m_sContent = m_sStringBuilder.ToString();
    }

    private bool IsEmptyLine(int lineID)
    {
        string _str = null;
        if(!m_mapLineStrInfo.TryGetValue(lineID, out _str))
        {
            return true;
        }

        return _str == "\n";
    }

    private Vector2 GetCursorPosByCurLineAndCharIndex()
    {
        return new Vector2(m_iNowCharID == -1 ? TextBoardUtility.TEXT_MARGIN_LEFT : TextBoardUtility.GetStringWidthByFont(EditorStyles.standardFont, m_mapLineStrInfo[m_iNowLine].Substring(0, m_iNowCharID)), 
                (m_iNowLine - 1) * TextBoardUtility.LINE_HEIGHT + 3);
    }

    private void GetCurLineAndCharIndexByPos(Vector2 pos, out int curLine, out int curCharIndex)
    {
        curLine = curCharIndex = -1;
        curLine = Mathf.FloorToInt(pos.y / TextBoardUtility.LINE_HEIGHT) + 1;
        curCharIndex = -1;
        {
            string _str = null;
            m_mapLineStrInfo.TryGetValue(curLine, out _str);
            if(_str != null)
            {
                if(IsEmptyLine(curLine))
                {
                    // 如果为空行，不用二分了，认为id为-1
                    curCharIndex = -1;
                }
                else 
                {
                    int _l = 0;
                    int _r = _str.Length - 1;
                    while(_l <= _r) 
                    {
                        int _mid = (_l + _r) / 2;
                        if(TextBoardUtility.GetStringWidthByFont(EditorStyles.standardFont, _str.Substring(0, _mid)) <= pos.x)
                        {
                            curCharIndex = _mid;
                            _l = _mid + 1;
                        }
                        else 
                        {
                            _r = _mid - 1;
                        }
                    }
                }

                // if(curCharIndex != -1) 
                // {
                //     Debug.Log(" 当前字符为 " + _str[curCharIndex]);
                // }
                // else 
                // {
                //     Debug.Log("当前行为空");
                // }
            }
            else 
            {
                // 不存在当前行的话，回到最大行
                curLine = m_mapLineStrInfo.Count;
                _str = m_mapLineStrInfo[curLine];
                curCharIndex = _str.Length - 1;
            }
        }
    }

    private void HandleMouseClick(Event e) 
    {
        var _pos = e.mousePosition;
        int _nowLine = -1, _nowCow = -1;
        GetCurLineAndCharIndexByPos(_pos, out _nowLine, out _nowCow);
        m_iNowLine = _nowLine;
        m_iNowCharID = _nowCow;
    }

    private void HandleBackspace()
    {
        // 删除一个字符
        if(m_iNowLine != -1)
        {
            if(m_iNowCharID > 0) 
            {
                // 当前行有可以删除的
                var _str = m_mapLineStrInfo[m_iNowLine];
                // 如果在一行的开始，需要删除tab（TODO.. 这里只是简单的删除一个tab，和VSCode的功能有点区别，VSCode大概记录的是这一行的空格数，然后回退的话会回到上一个tab处）
                if(_str.IsBeginWithAnySpace(m_iNowCharID - 1))
                {
                    // 要回退一个tab
                    int _tabCnt = 0;
                    m_mapLineTabCnt.TryGetValue(m_iNowLine, out _tabCnt);
                    _tabCnt--;
                    if(_tabCnt < 0) 
                    {
                        _tabCnt = 0;
                    }
                    m_mapLineTabCnt.TrySet(m_iNowLine, _tabCnt);
                    m_mapLineStrInfo[m_iNowLine] = _str.Substring(0, _tabCnt * TextBoardUtility.TAB_SIZE) + _str.Substring(m_iNowCharID, _str.Length - m_iNowCharID);
                    m_iNowCharID = _tabCnt * TextBoardUtility.TAB_SIZE;
                    // TODO.. magic-code
                    if(IsEmptyLine(m_iNowLine))
                    {
                        m_iNowCharID = -1;
                    }
                }
                // 正常回退一个字符
                else
                {
                    m_mapLineStrInfo[m_iNowLine] = _str.Substring(0, m_iNowCharID < 1 ? 0 : m_iNowCharID - 1) +  _str.Substring(m_iNowCharID, _str.Length - m_iNowCharID);
                    m_iNowCharID--;
                }
            }
            else 
            {
                // 在顶格，需要往前退一行
                if(m_iNowLine > 1) 
                {
                    // 要把当前行合并到上一行
                    var _nowLineStr = m_mapLineStrInfo[m_iNowLine];
                    var _preLineStr = m_mapLineStrInfo[m_iNowLine - 1];
                    m_iNowCharID = _preLineStr == "\n" ? -1 : _preLineStr.Length - 1; // 新的光标在上一行的最后
                    _preLineStr = _preLineStr.Substring(0, _preLineStr.Length - 1) + _nowLineStr.Substring(0, _nowLineStr.Length - 1) + "\n";
                    m_mapLineStrInfo[m_iNowLine - 1] = _preLineStr;
                    // 把后面的往前复制
                    int preMaxLine = m_mapLineStrInfo.Count;
                    for(int i=m_iNowLine;i<preMaxLine;i++) 
                    {
                        m_mapLineStrInfo[i] = m_mapLineStrInfo[i + 1];
                    }
                    // 删除最后一行
                    m_mapLineStrInfo.Remove(preMaxLine);
                    // 回到上一行
                    m_iNowLine--;
                }
            }
        }
    }

    private void HandleInput(Event e) 
    {
        // TODO.. 麻了，突然发现输入法输入中文不支持！
        string GetInput(out int goSize) 
        {
            goSize = 1;
            var keyCode = e.keyCode;
            bool upper = e.capsLock; // System.Console.CapsLock; // TODO.. 怎么检测键盘的大小写已经被按住？
            bool shift = e.shift;
            // if(shift) 
            // {
            //     upper = !upper;
            // }
            var input = TextBoardUtility.GetDefaultKeyCodeString(keyCode, shift, out goSize);
            return input;
        }
        if(m_iNowLine != -1)
        {
            var _str = m_mapLineStrInfo[m_iNowLine];
            int goSize = 1;
            var input = GetInput(out goSize);
            if(input == null) 
            {
                return;
            }

            var newStr = _str;
            // 如果在行的开头，且是一次tab输入，要变更当前行的tab数
            if(_str.IsBeginWithAnySpace(m_iNowCharID) && e.keyCode == KeyCode.Tab)
            {
                m_mapLineTabCnt[m_iNowLine] = m_mapLineTabCnt[m_iNowLine] + 1;
            }
            if(m_iNowCharID == -1) 
            {
                // 此时，为空行或者"\n"
                newStr = input + _str;
                m_iNowCharID = 0;
            } 
            else 
            {
                newStr = _str.Substring(0, m_iNowCharID) + input + _str.Substring(m_iNowCharID, _str.Length - m_iNowCharID);
            }
            m_mapLineStrInfo[m_iNowLine] = newStr;
            m_iNowCharID += goSize;
        }
    }

    private void HandleCursorMoveByKeyCode(KeyCode keyCode) 
    {
        if(keyCode == KeyCode.LeftArrow) 
        {
            if(m_iNowCharID > 0)
            {
                m_iNowCharID--;
            }
            else 
            {
                if(m_iNowLine > 1) 
                {
                    m_iNowLine--;
                    var _str = m_mapLineStrInfo[m_iNowLine];
                    m_iNowCharID = _str.Length - 1; // "abc\n", len = 4, ==> charID = 3 = len - 1
                }
            }
        }
        else if(keyCode == KeyCode.RightArrow) 
        {
            // "abc\n" len = 4, charID = 3 的时候是最后一个，所以当2的时候还可以加，否则就下一行
            // 特别的，当charID为-1的时候，应该也直接跳转到下一行
            var _str = m_mapLineStrInfo[m_iNowLine];
            if(m_iNowCharID < _str.Length - 1 && m_iNowCharID != -1)
            {
                m_iNowCharID++;
            }
            else 
            {
                if(m_iNowLine < m_mapLineStrInfo.Count) 
                {
                    m_iNowLine++;
                    var _nextLineStr = m_mapLineStrInfo[m_iNowLine];
                    m_iNowCharID = _nextLineStr == "\n" ? -1 : 0;
                }
            }
        }
        // TODO.. 发现VSCode的到上下一行会记录缓存，下去哪个位置，最近上去会回到同样的位置；但是如果直接从下面上去，不一定是这个位置。暂且不考虑这种优化吧..
        else if(keyCode == KeyCode.UpArrow) 
        {
            if(m_iNowLine > 1) 
            {
                var cursorPos = GetCursorPosByCurLineAndCharIndex();
                cursorPos.y -= TextBoardUtility.LINE_HEIGHT * 0.5f;
                m_iNowLine--;
                int charIndex = -1, _ = -1;
                GetCurLineAndCharIndexByPos(cursorPos, out _, out charIndex);
                m_iNowCharID = charIndex;
            }
        }
        else if(keyCode == KeyCode.DownArrow) 
        {
            if(m_iNowLine < m_mapLineStrInfo.Count) 
            {
                var cursorPos = GetCursorPosByCurLineAndCharIndex();
                cursorPos.y += TextBoardUtility.LINE_HEIGHT * 1.5f;
                m_iNowLine++;
                int charIndex = -1, _ = -1;
                GetCurLineAndCharIndexByPos(cursorPos, out _, out charIndex);
                m_iNowCharID = charIndex;
            }
        }
    }

    private void HandleEnter()
    {
        if(m_iNowLine == -1) 
        {
            return;
        }

        int preMaxLine = m_mapLineStrInfo.Count;
        var curLineStr = m_mapLineStrInfo[m_iNowLine];
        int curLineTabCnt = 0;
        m_mapLineTabCnt.TryGetValue(m_iNowLine, out curLineTabCnt);
        // 特殊处理，在{}中间按下回车会换行并多一行（TODO.. 学习下VSCode的优化）
        if(m_iNowCharID - 1 >= 0 && m_iNowCharID + 1 < curLineStr.Length && curLineStr[m_iNowCharID - 1] == '{' && curLineStr[m_iNowCharID] == '}') 
        {
            m_mapLineStrInfo[m_iNowLine] = curLineStr.Substring(0, m_iNowCharID) + "\n"; // 重构当前这一行
            // 后面的数据往后移动两行
            for(int i=preMaxLine+2;i>=m_iNowLine+1+2;i--) 
            {
                m_mapLineStrInfo.TrySet(i, m_mapLineStrInfo[i - 2]);
                m_mapLineTabCnt.TrySet(i, m_mapLineTabCnt[i - 2]);
            }
            // 填充后面两行
            m_mapLineStrInfo.TrySet(m_iNowLine + 1, TextBoardUtility.GetStringByTableCnt(curLineTabCnt + 1) + "\n");
            m_mapLineTabCnt.TrySet(m_iNowLine + 1, curLineTabCnt + 1);
            m_mapLineStrInfo.TrySet(m_iNowLine + 2, TextBoardUtility.GetStringByTableCnt(curLineTabCnt) + "}\n");
            m_mapLineTabCnt.TrySet(m_iNowLine + 2, curLineTabCnt);
            // 往下移动一行
            m_iNowLine++;
            m_iNowCharID = (curLineTabCnt + 1) * TextBoardUtility.TAB_SIZE;
            return;
        }

        // 截断当前行
        m_mapLineStrInfo[m_iNowLine] = curLineStr.Substring(0, m_iNowCharID == -1 ? 0 : m_iNowCharID) + "\n";

        // 往后移动
        m_iNowLine++;
        m_mapLineStrInfo.Add(preMaxLine + 1, "\n");
        for(int i=preMaxLine+1;i>=m_iNowLine;i--) 
        {
            m_mapLineStrInfo[i] = m_mapLineStrInfo[i - 1];
            m_mapLineTabCnt[i] = m_mapLineTabCnt[i - 1];
        }
        // 新加的这一行比较特殊
        m_mapLineStrInfo[m_iNowLine] = TextBoardUtility.GetStringByTableCnt(curLineTabCnt) + curLineStr.Substring(m_iNowCharID == -1 ? 0 : m_iNowCharID, m_iNowCharID == -1 ? curLineStr.Length : curLineStr.Length - m_iNowCharID); // 后面已经带了回车
        m_mapLineTabCnt.TrySet(m_iNowLine, curLineTabCnt);
        // 然后要移动一下当前行光标的位置
        m_iNowCharID = m_mapLineStrInfo[m_iNowLine] == "\n" ? -1 : (curLineTabCnt) * TextBoardUtility.TAB_SIZE;
    }

    private void HandleEnd()
    {
        string _str = null;
        if(!m_mapLineStrInfo.TryGetValue(m_iNowLine, out _str)) 
        {
            return;
        }

        m_iNowCharID = _str == "\n" ? -1 : _str.Length - 1;
    }
}
