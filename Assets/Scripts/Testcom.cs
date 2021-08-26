using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuaLanguage;

/*
def getFunc (str, close) {
    _f = fun() {
       close = close + 1;
       __log("闭包 " + str + " " + close);
   }
   
   _f
}

f1 = getFunc ("闭包1", 0)
f2 = getFunc ("闭包2", 0)

f1()
f1()
f1()

f2()
*/
/// <summary>
/// /////////////////////////////////////////////////////////////////////////////////
/// </summary>
/*
class vector2 {
    x = 0;
    y = 0;
    // 打印方法
    def print() {
        __log("vector2: " + x + " " + y)
    }
    // 归一化方法
    def normalize() {
        sum = x * x + y * y;
        if (sum <= 0) {
            x = y = 0;
            __log("零向量！");
        } else {
            curSqrt = __sqrt(sum)
            x = x / curSqrt;
            y = y / curSqrt;
        }      
  
        this;
    }
}

class vector3 extends vector2 {
    z = 0;
    // 打印方法
    def print() {
        __log("vector3: " + x + " " + y + " " + z)
    }
    // 归一化方法
    def normalize() {
        sum = x * x + y * y + z * z;
        if (sum <= 0) {
            x = y = z = 0;
            __log("零向量！");
        } else {
            curSqrt = __sqrt(sum)
            x = x / curSqrt;
            y = y / curSqrt;
            z = z / curSqrt;
        }

        this;
    }
}

p2d = vector2.new
p2d.x = 5;
p2d.y = 10;
p2d.normalize().print();
// p2d.print()

p3d = vector3.new
p3d.x = 5;
p3d.y = 10;
p3d.z = 15;
p3d.normalize().print();
// p3d.print()
*/
/// <summary>
/// /////////////////////////////////////////////////////////////////////////////////
/// </summary>
/*
class Object {
    // ToString
    def ToString() {
        __logError("这个类没有实现ToString！");
    }
}
class List extends Object {
    _arr = [-1, -1, -1, -1, -1, -1, -1, -1];
    _size = 0;
    def _EnsureBounds(id) {
        ok = 1;
        if (id < 0) {
            ok = 0;
        } else {
            if (id >= _size) {
                ok = 0;
            }
        }
        
        ok;
    }
    // 添加一个元素
    def Add(obj) {
        _arr[_size] = obj;
        _size = _size + 1;
    }
    def RemoveAt(id) {
        if (this._EnsureBounds(id) == 0) {
            __logError("数组越界 ！尺寸为 " + _size + "，请求的id为 " + id);
        } else {
            // 往前拷贝数组
            _pos = id + 1;
            while(_pos < _size) {
                _arr[_pos - 1] = _arr[_pos];
                _pos = _pos + 1;
            }
            // 删除最后一个
            _size = _size - 1;
        }
    }
    def Get(id) {
        ans = 0;
        if (this._EnsureBounds(id) == 0) {
            __logError("数组越界 ！尺寸为 " + _size + "，请求的id为 " + id);
        } else {
            ans = _arr[id];
        }

        ans;
    }
    // 重写ToString
    def ToString() {
        str = "List: [";
        _i = 0;
        while (_i < _size) {
            str = str + "index " + _i + ": " + _arr[_i] + ", ";
            _i = _i + 1;
        }
        str = str + "]";
        str;
    }
}
// 测试List基本功能
list = List.new;
list.Add(5);
list.Add(6);
list.Add(7);
__log(list.ToString());
list.RemoveAt(5);
list.RemoveAt(1);
__log(list.ToString());
*/

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
