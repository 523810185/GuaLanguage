using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuaLanguage;

public class Testcom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var l = new Lexer(" int 5; x = \"ddss\"; z[0] = 5 ^ tt;");
        for(int i = 0;i<2;i++) 
        {
            // l.readLine();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
