namespace GuaLanguage.Utility 
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FontUtility
    {
        public static float LINE_HEIGHT = 13f;
        public static float TEXT_MARGIN_LEFT = 4f;
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
    }
}