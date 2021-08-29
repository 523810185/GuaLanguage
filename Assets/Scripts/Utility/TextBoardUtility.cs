namespace GuaLanguage.Utility 
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Text;

    public static class TextBoardUtility
    {
        public static float LINE_HEIGHT = 13f;
        public static float TEXT_MARGIN_LEFT = 4f;
        public static int TAB_SIZE = 4;
        /// <summary>
        /// 通过字体获取字符串的宽度
        /// </summary>
        /// <param name="font">字体</param>
        /// <param name="str">需要计算的字符串</param>
        /// <returns>字符串宽度</returns>
        public static float GetStringWidthByFont(Font font, string str)
        {
            float width = 0;
            font.RequestCharactersInTexture(str, font.fontSize, FontStyle.Normal);
            CharacterInfo charInfo;
            foreach(var strItem in str)
            {
                font.GetCharacterInfo(strItem, out charInfo);
                if(charInfo.advance == 0)
                    {}// width += font.fontSize;
                else
                    width += charInfo.advance;
            }
            return width + TEXT_MARGIN_LEFT;
        }

        private static StringBuilder m_sStringBuilder = new StringBuilder();
        public static string GetStringByTableCnt(int tabCnt)
        {
            m_sStringBuilder.Clear();
            for(int i=0;i<tabCnt * TextBoardUtility.TAB_SIZE;i++) 
            {
                m_sStringBuilder.Append(" ");
            }
            return m_sStringBuilder.ToString();
        }

        /// <summary>
        /// 返回一个字符串从 0 ~ clampTo 位置上是否都是空格
        /// </summary>
        /// <param name="str"></param>
        /// <param name="clampTo"></param>
        /// <returns></returns>
        public static bool IsBeginWithAnySpace(this string str, int clampTo) 
        {
            for(int i=0;i<clampTo;i++) 
            {
                if(str[i] != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        private static Dictionary<KeyCode, List<string>> m_mapDefaultKeyCodeToString = new Dictionary<KeyCode, List<string>>
        {
            {KeyCode.BackQuote, new List<string>{"`", "~"}},
            {KeyCode.Alpha1, new List<string>{"1", "!"}},
            {KeyCode.Alpha2, new List<string>{"2", "@"}},
            {KeyCode.Alpha3, new List<string>{"3", "#"}},
            {KeyCode.Alpha4, new List<string>{"4", "$"}},
            {KeyCode.Alpha5, new List<string>{"5", "%"}},
            {KeyCode.Alpha6, new List<string>{"6", "^"}},
            {KeyCode.Alpha7, new List<string>{"7", "&"}},
            {KeyCode.Alpha8, new List<string>{"8", "*"}},
            {KeyCode.Alpha9, new List<string>{"9", "("}},
            {KeyCode.Alpha0, new List<string>{"0", ")"}},
            {KeyCode.Minus, new List<string>{"-", "_"}},
            {KeyCode.Equals, new List<string>{"=", "+"}},
            {KeyCode.LeftBracket, new List<string>{"[", "{"}},
            {KeyCode.RightBracket, new List<string>{"]", "}"}},
            {KeyCode.Semicolon, new List<string>{";", ":"}},
            {KeyCode.Quote, new List<string>{"'", "\""}},
            {KeyCode.Comma, new List<string>{",", "<"}},
            {KeyCode.Period, new List<string>{".", ">"}},
            {KeyCode.Slash, new List<string>{"/", "?"}},
            {KeyCode.Backslash, new List<string>{"\\", "|"}},
            // 无shift
            {KeyCode.Space, new List<string>{" "}},
            {KeyCode.Tab, new List<string>{"    "}},
            {KeyCode.KeypadDivide, new List<string>{"/"}},
            {KeyCode.KeypadMultiply, new List<string>{"*"}},
            {KeyCode.KeypadMinus, new List<string>{"-"}},
            {KeyCode.KeypadPlus, new List<string>{"+"}},
            {KeyCode.Keypad1, new List<string>{"1"}},
            {KeyCode.Keypad2, new List<string>{"2"}},
            {KeyCode.Keypad3, new List<string>{"3"}},
            {KeyCode.Keypad4, new List<string>{"4"}},
            {KeyCode.Keypad5, new List<string>{"5"}},
            {KeyCode.Keypad6, new List<string>{"6"}},
            {KeyCode.Keypad7, new List<string>{"7"}},
            {KeyCode.Keypad8, new List<string>{"8"}},
            {KeyCode.Keypad9, new List<string>{"9"}},
            {KeyCode.Keypad0, new List<string>{"0"}},
            {KeyCode.KeypadPeriod, new List<string>{"."}},
        };
        public static string GetDefaultKeyCodeString(KeyCode keyCode, bool shift, out int goSize) 
        {
            goSize = 0;
            if(KeyCode.A <= keyCode && keyCode <= KeyCode.Z) 
            {
                goSize = 1;
                return ((char)((keyCode - KeyCode.A) + (shift ? 'A' : 'a'))).ToString(); 
            }

            List<string> strMap = null;
            if(!m_mapDefaultKeyCodeToString.TryGetValue(keyCode, out strMap)) 
            {
                return null;
            }
            
            var ansStr = strMap.Count == 1 ? strMap[0] : strMap[shift ? 1 : 0];
            goSize = ansStr.Length;
            // 特殊处理
            if(ansStr == "{") 
            {
                ansStr = "{}";
            }
            return ansStr;
        }
    }
}