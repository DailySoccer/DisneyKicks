using UnityEngine;
using System.Collections.Generic;

public class txtText : MonoBehaviour
{


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public int baseWidth = 0;

    /// <summary>
    /// Si es MAYOR QUE "0", rellena el campo "text" con el texto que corresponda del LocalizacionManager
    /// </summary>
    public int idTextoTraducido = 0;

    /// <summary>
    /// indica si este texto debe estar en mayusculas
    /// </summary>
    public bool mayusculas = false;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public void Start() {
        // comprobar si hay que obtener una traduccion de este texto
        if (idTextoTraducido > 0) {
            string texto = LocalizacionManager.instance.GetTexto(idTextoTraducido);

            // comprobar si el texto debe ir en mayusculas
            if (mayusculas)
                texto = texto.ToUpper();

            transform.GetComponent<GUIText>().text = texto;
        }
    }


    public void SetText(string _texto)
    {
        GetComponent<GUIText>().text = _texto;
        if(baseWidth == 0) return;
        Fix();
    }

    public void Fix()
    {
        if(baseWidth == 0) return;

        string original = GetComponent<GUIText>().text;

        char[] delimitor = new char[2]{' ','\n'};
        string[] words = original.Split(delimitor);
        words = SplitTags(words);
        string def = "";

        for (int i = 0; i < words.Length ; i++)
        {
            string word = words[i];
            bool addSpace = (i != 0);// && (i != words.Length-1);
            GetComponent<GUIText>().text = def + (addSpace ? " " : "") + word;
            if(GetComponent<GUIText>().GetScreenRect().width > (baseWidth))
            {
                def += "\n" + word;
            }
            else
            {
                def += (addSpace ? " " : "") + word;
            }
        }
        GetComponent<GUIText>().text = def;
    }

    string[] SplitTags(string[] _words) //solo para colores de momento, para simplificar que hay mucho curro!
    {
        //List<ColorTag> tags = new List<ColorTag>();
        string currentTag = string.Empty;
        for(int i = 0; i < _words.Length ; i++)
        {
            int i_st = -1;
            int i_ls = -1;
            bool tagged = false;
            bool tagstart = false;
            for(int i2 = 0; i2 < _words[i].Length ; i2++)
            {
                if(_words[i][i2] == '<')
                {
                    i_st = i2;
                }
                else if(_words[i][i2] == '>')
                {
                    if(i_st != -1)
                    {
                        tagged = true;
                        i_ls = i2;
                        string tag = _words[i].Substring(i_st, i_ls-i_st+1);
                        if(tag[1] == '/')
                        {
                            tagstart = true;
                        }
                        else
                        {
                            tagstart = false;
                            currentTag = tag;
                        }
                        i_ls = -1;
                        i_st = -1;
                    }
                }
            }
            if(!tagged && currentTag != string.Empty)
            {
                _words[i] = currentTag + _words[i] + "</color>";
            }
            else if(tagged)
            {
                if(tagstart)
                {
                    if(_words[i][0] != '<') //pequeño arreglo para que no ponga 2 veces el tag cuando este estaba pegado a la palabra
                        _words[i] = currentTag + _words[i];
                    currentTag = string.Empty;
                }
                else
                {
                    _words[i] += "</color>";
                }
            }
        }
        return _words;
    }


# if UNITY_EDITOR
    void OnDrawGizmos()
    {
        float dist = 0.5f;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist)),
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3(baseWidth, 0, 0)));
    }
# endif

}
