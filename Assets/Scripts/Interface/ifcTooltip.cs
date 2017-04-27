using UnityEngine;
using System.Collections;

public class ifcTooltip : ifcBase
{
    

    public static ifcTooltip instance { get; protected set; }
    void Awake() {
        instance = this;
        gameObject.SetActive(false);
    }

    void Start() {
        getComponentByName("Cerrar").GetComponent<btnButton>().action = (_name) => {
            getComponentByName("Cerrar").GetComponent<btnButton>().reset();
            gameObject.SetActive(false);
        };
    }

    public void showLogro(string _logro) {
        LogrosDescription.descLogro desc = cntLogros.instance.m_logros.getLogroByCode(_logro);
        transform.Find("txtNombreLogro").GetComponent<GUIText>().text = desc.m_name;
        GUIText gt = transform.Find("txtDescripcion").GetComponent<GUIText>();
        int lines = 0;
        gt.text = warp(desc.m_descripcion, 200, gt.font, gt.fontSize, out lines);
        transform.Find("txtDescripcion").GetComponent<txtText>().Fix();
        lines = gt.text.Contains("\n") ? 2 : 1;
        Rect rtop, rmid;
        Vector2 offNombre = Vector2.zero;
        Vector2 offDescripcion = Vector2.zero;
        
        if (lines == 1) {
			rtop = new Rect(-122.5f*ifcBase.scaleFactor, 95*ifcBase.scaleFactor, 245*ifcBase.scaleFactor, 30*ifcBase.scaleFactor);
			rmid = new Rect(-122.5f*ifcBase.scaleFactor, 10*ifcBase.scaleFactor, 245*ifcBase.scaleFactor, 85*ifcBase.scaleFactor);
			offNombre = new Vector2(0, 115)*ifcBase.scaleFactor;
			offDescripcion = new Vector2(0, 75)*ifcBase.scaleFactor;
        }
        else {
			rtop = new Rect(-122.5f*ifcBase.scaleFactor, 135*ifcBase.scaleFactor, 245*ifcBase.scaleFactor, 30*ifcBase.scaleFactor);
			rmid = new Rect(-122.5f*ifcBase.scaleFactor, 10*ifcBase.scaleFactor, 245*ifcBase.scaleFactor, 125*ifcBase.scaleFactor);
			offNombre = new Vector2(0, 155)*ifcBase.scaleFactor;
			offDescripcion = new Vector2(0, 115)*ifcBase.scaleFactor;
        }

        transform.Find("txtNombreLogro").GetComponent<GUIText>().pixelOffset = offNombre;
        transform.Find("txtDescripcion").GetComponent<GUIText>().pixelOffset = offDescripcion;
        transform.Find("Top").GetComponent<GUITexture>().pixelInset = rtop;
        transform.Find("Centro").GetComponent<GUITexture>().pixelInset = rmid;
        transform.Find("txtPremio").GetComponent<GUIText>().text = cntLogros.instance.m_logros.getPremioDesc(desc);
        gameObject.SetActive(true);
    }

    void Update() {
        Vector3 v = Camera.main.ScreenToViewportPoint(Input.mousePosition) - transform.position;
        v.z = 0;
        if (v.sqrMagnitude > 0.1f * 0.1f) {
            getComponentByName("Cerrar").GetComponent<btnButton>().reset();
            gameObject.SetActive(false);
        }
    }

    public static string warp(string _text, float _width, Font _font, int _size, out int _lines){
        _text=_text.Trim('\n', '\r');
        _lines = 0;
        string[] words = _text.Split(' ');
        float acum = 0;
        string final = "";
        string comp = "";
        for (int i = 0; i < words.Length; ++i) {
            float len = wordSize(words[i], _font, _size);
            acum += len;
            if (acum < _width)
            {
                comp += words[i] + " ";
                
            }
            else
            {
                _lines++;
                final += comp + "\n";
                comp = words[i] + " ";
                acum = len;
            }
        }
        if (acum != 0)
        {
            
            _lines++;
            final += comp + "\n";
        }
        return final;
    }

    public static float wordSize( string _text, Font _font, int _size ) {
        CharacterInfo ci;
        float width = 0;

        _font.RequestCharactersInTexture(_text, _size);
        _text+=" ";
        for (int i = 0; i < _text.Length; ++i){
            _font.GetCharacterInfo(_text[i], out ci, _size);
            width += ci.width;
        }
        return width;
    }
}
