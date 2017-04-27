using UnityEngine;
using System;
using System.Collections.Generic;

public class ColoredString {
    private string _color;
    private string _text;

    public ColoredString (string color, string text) {
        _color = color;
        _text = text;
    }

    public string GetColoredText () {
        return ( "<color=" + _color + ">" + _text + "</color>" );
    }

    public string GetText () {
        return _text;
    }
}

public class MultiColoredString {
    private List<ColoredString> _coloredStrings = new List<ColoredString>();

    public MultiColoredString () { }

    public void Append (ColoredString coloredString) {
        _coloredStrings.Add( coloredString );
    }

    public string GetColoredText () {
        string stringResult = "";

        foreach ( var coloredString in _coloredStrings ) {
            stringResult += coloredString.GetColoredText();
        }

        return stringResult;
    }

    public string GetText () {
        string stringResult = "";

        foreach ( var text in _coloredStrings ) {
            stringResult += text.GetText();
        }

        return stringResult;
    }
}

public class ShotFeedbackUnit : MonoBehaviour {    

    private const int SHADOW_UNITS = 1;

    private GUIText _guiText = null;
    private GUIText[] _shadowTexts = new GUIText[ SHADOW_UNITS ];

    #region MonoBehaviour

    void Awake () {
        _guiText = this.GetComponent<GUIText>();

        int shadowUnits = 0;
        foreach ( Transform child in transform ) {
            if ( shadowUnits < SHADOW_UNITS ) {
                _shadowTexts[ shadowUnits ] = child.gameObject.GetComponent<GUIText>();
                shadowUnits++;
            }
            else {
                throw new IndexOutOfRangeException( "Demasiadas sombras!!" );
            }
        }
	}

    void Update () {
        HandleMovementAnimation();
        HandleAlphaAnimation();
        HandleColorAnimation();
    }

    #endregion

    #region Public Interface

    public void SetPosition (Vector3 position) {

        Vector3 rectifiedPosition = new Vector3(position.x, position.y, position.z);
        this.transform.localScale = Vector3.zero;
        this.transform.position = rectifiedPosition;


        Vector3 shadowPosition = new Vector3(position.x, position.y, position.z - 1.0f);
        foreach ( var shadowText in _shadowTexts ) {
            shadowText.transform.localPosition = shadowPosition;
        }
    }

    public void SetText (string text) {
        _guiText.text = text;

        foreach ( var shadowText in _shadowTexts ) {
            shadowText.text = text;
        }
    }

    public void SetMulticoloredText (MultiColoredString text) {
        _guiText.text = text.GetColoredText();

        foreach ( var shadowText in _shadowTexts ) {
            shadowText.text = text.GetText();
        }
    }

    public void SetColor (Color textColor) {
        _guiText.color = textColor;
    }

    public void SetFontSize (int fontSize) {
        _guiText.fontSize = fontSize;

        foreach ( var shadowText in _shadowTexts ) {
            shadowText.fontSize = fontSize;
        }
    }

    public void SetAlpha (float alpha) {
        SetGUIAlpha( _guiText, alpha );
        
        foreach ( var shadowText in _shadowTexts ) {
            SetGUIAlpha( shadowText, alpha );
        }
    }    

    public void SetShadowEnabled (bool bEnabled) {
        foreach ( var shadowText in _shadowTexts ) {
            shadowText.gameObject.SetActive( bEnabled );
        }
    }

    public void SetLifetime (float lifetime) {
        Destroy( this.gameObject, lifetime );
    }

    public void SetShadowThickness (int thickness) {
        _shadowTexts[ 0 ].pixelOffset = new Vector2( thickness, -thickness );
        //_shadowTexts[ 1 ].pixelOffset = new Vector2( -thickness, thickness );
        //_shadowTexts[ 2 ].pixelOffset = new Vector2( thickness, -thickness );
        //_shadowTexts[ 3 ].pixelOffset = new Vector2( thickness, thickness );
    }

    public void AdjustGUISize () {
        // Escalamos los tamaños en funcion de la resolución de pantalla        
        ifcBase.Scale( this.gameObject );
        ifcBase.ScaleGUIOBject( this.gameObject );
    }

    public void SetMovementAnimation (Vector3 moveDirection, float moveSpeed) {
        _hasMovementAnimation = true;
        _movementDirection = moveDirection;
        _movementSpeed = moveSpeed;
    }

    public void SetAlphaAnimation (float dstAlpha, float alphaSpeed) {
        _hasAlphaAnimation = true;
        _scrAlpha = _guiText.color.a;
        _dstAlpha = dstAlpha;
        _currentAlphaLerpTime = 0.0f;
        _alphaSpeed = alphaSpeed;
    }

    public void SetColorAnimation (Color dstColor, float colorSpeed) {
        _hasColorAnimation = true;

        _srcColor = _guiText.color;
        _dstColor = dstColor;
        _currentColorLerpTime = 0.0f;
        _colorSpeed = colorSpeed;
    }

    #endregion    

    #region Animations

    private bool _hasMovementAnimation = false;
    private Vector3 _movementDirection = Vector3.zero;
    private float _movementSpeed = 0.0f;

    private void HandleMovementAnimation () {
        if ( _hasMovementAnimation ) {
            SetPosition( this.transform.position + ( _movementDirection * _movementSpeed * Time.deltaTime ) );
        }
    }

    private bool _hasAlphaAnimation = false;
    private float _scrAlpha = 0.0f;
    private float _dstAlpha = 0.0f;
    private float _currentAlphaLerpTime = 0.0f;
    private float _alphaSpeed = 0.0f;

    private void HandleAlphaAnimation () {
        if ( _hasAlphaAnimation ) {
            _currentAlphaLerpTime += Time.deltaTime;
            SetAlpha( Mathf.Lerp( _scrAlpha, _dstAlpha, _currentAlphaLerpTime * _alphaSpeed ) );
        }
    }

    private bool _hasColorAnimation = false;
    private Color _srcColor;
    private Color _dstColor;
    private float _currentColorLerpTime = 0.0f;
    private float _colorSpeed = 0.0f;

    private void HandleColorAnimation () {
        if ( _hasColorAnimation ) {
            _currentColorLerpTime += Time.deltaTime;            
            SetColor( Color.Lerp( _srcColor, _dstColor, _currentColorLerpTime * _colorSpeed ) );
        }
    }

    #endregion

    private void SetGUIAlpha (GUIText guiText, float alpha) {
        Color rectifiedAlphaColor = guiText.color;
        rectifiedAlphaColor.a = alpha;
        guiText.color = rectifiedAlphaColor;
    }    
}
