using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperTweener : MonoBehaviour
{
    public const float HALFPI = (Mathf.PI / 2.0f);
    static class Sine
    {
        public static float EaseIn(float s) { return Mathf.Sin(s * HALFPI - HALFPI) + 1.0f; }
        public static float EaseOut(float s) { return Mathf.Sin(s * HALFPI); }
        public static float EaseInOut(float s) { return (Mathf.Sin(s * Mathf.PI - HALFPI) + 1) / 2.0f; }
    }
    static class Power{
        public static float EaseIn(float s, int power) { 
            return Mathf.Pow(s, power); 
        }
        public static float EaseOut(float s, int power) {
            float sign = power % 2 == 0 ? -1.0f : 1.0f;
        return sign * (Mathf.Pow(s - 1, power) + sign); 
        }
        public static float EaseInOut(float s, int power) 
        { 
            s *= 2.0f; 
            if (s < 1.0f) return EaseIn(s, power) / 2.0f; 
            float sign = power % 2 == 0 ? -1.0f : 1.0f;
        return sign / 2.0f * (Mathf.Pow(s - 2.0f, power) + sign * 2.0f); 
        }
    }
    #region Linear
    static public float LinearNone(float k)
    {
        return k;
    }
    #endregion
    #region Quadratic
    static public float QuadraticIn(float k)
    {
        return Power.EaseIn(k, 2);
    }

    static public float QuadraticOut(float k)
    {
        return Power.EaseOut(k, 2);
    }

    static public float QuadraticInOut(float k)
    {
        return Power.EaseInOut(k, 2);
    }

    #endregion
    #region Cubic
    static public float CubicIn(float k)
    {
        return Power.EaseIn(k, 3);
    }

    static public float CubicOut(float k)
    {
        return Power.EaseOut(k, 3);
    }

    static public float CubicInOut(float k)
    {
        return Power.EaseInOut(k, 3);
    }
    #endregion
    #region Quart
    static public float QuartIn(float k)
    {
        return Power.EaseIn(k, 4);
    }

    static public float QuartOut(float k)
    {
        return Power.EaseOut(k, 4);
    }

    static public float QuartInOut(float k)
    {
        return Power.EaseInOut(k, 4);
    }
    #endregion
    #region Quint
    static public float QuintIn(float k)
    {
        return Power.EaseIn(k, 5);
    }

    static public float QuintOut(float k)
    {
        return Power.EaseOut(k, 5);
    }

    static public float QuintInOut(float k)
    {
        return Power.EaseInOut(k, 5);
    }
    #endregion
    #region Sinusoidal
    public static float SinusoidalIn(float k) {
        return -1.0f * Mathf.Cos( k * (Mathf.PI / 2.0f)) + 1.0f;
    }

    public static float SinusoidalOut(float k) {
        return Mathf.Sin(k * (Mathf.PI / 2.0f));
    }

    public static float SinusoidalInOut(float k) {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = SinusoidalIn(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = SinusoidalOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    static public float SinusoidalOutIn(float k) {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = SinusoidalOut(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = SinusoidalIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }
    #endregion
    #region Exponential
    public static float ExponentialIn(float k) {
        return Mathf.Pow(2.0f, 10.0f * (k - 1.0f)) - 0.001f;
    }

    public static float ExponentialOut(float k) {
        return 1.001f * (-Mathf.Pow(2.0f, -10.0f * k) + 1.0f);
    }

    public static float ExponentialInOut(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = ExponentialIn(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = ExponentialOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float ExponentialOutIn(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = ExponentialOut(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = ExponentialIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }
    #endregion
    #region Circular
    public static float CircularIn(float k) {
        return -1.0f * (Mathf.Sqrt(1.0f - (k * k)) - 1.0f);
    }

    public static float CircularOut(float k) {
        return Mathf.Sqrt(1.0f - (k - 1.0f) * (k - 1.0f));
    }

    public static float CircularInOut(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = CircularIn(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = CircularOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float CircularOutIn(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = CircularOut(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = CircularIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }
    #endregion
    #region Elastic
    public static float ElasticIn(float k) {
        return (-1.0f * Mathf.Pow(2.0f, 10.0f * (k - 1.0f)) * Mathf.Sin(((k - 1.0f) - 0.075f) * (2.0f * Mathf.PI) / 0.3f));
    }

    public static float ElasticOut(float k) {
        return 1.0f * Mathf.Pow(2.0f, -10.0f * k) * Mathf.Sin((k - 0.075f) * (2.0f * Mathf.PI) / 0.3f) + 1.0f;
    }

    public static float ElasticInOut(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = ElasticIn(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = ElasticOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float ElasticOutIn(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = ElasticOut(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = ElasticIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }
    #endregion
    #region Back
    public static float BackIn(float k) {
        return k * k * ((1.7016f + 1.0f) * k - 1.7016f);
    }

    public static float BackOut(float k) {
        return (k - 1.0f) * (k - 1.0f) * ((1.7016f + 1.0f) * (k - 1.0f) + 1.7016f) + 1.0f;
    }

    public static float BackInOut(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = BackIn(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = BackOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float BackOutIn(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = BackOut(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = BackIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }
    #endregion
    #region Bounce
    public static float BounceIn(float k){
        k = 1.0f - k;
        if (k < 1.0f / 2.75f)
            k = 7.5625f * k * k;
        else if (k < 2.0f / 2.75f) {
            k = k - (1.5f / 2.75f);
            k = 7.5625f * k * k + 0.75f;
        }
        else if (k < 2.5f / 2.75f) {
            k = k - (2.25f / 2.75f);
            k = 7.5625f * k * k + 0.9375f;
        }
        else {
            k = k - (2.625f / 2.75f);
            k = 7.5625f * k * k + 0.984375f;
        }
        return 1.0f - k;
    }

    public static float BounceOut(float k) {
        if (k < 1.0f / 2.75f)
            k = 7.5625f * k * k;
        else if (k < 2.0f / 2.75f)
        {
            k = k - (1.5f / 2.75f);
            k = 7.5625f * k * k + 0.75f;
        }
        else if (k < 2.5f / 2.75f)
        {
            k = k - (2.25f / 2.75f);
            k = 7.5625f * k * k + 0.9375f;
        }
        else
        {
            k = k - (2.625f / 2.75f);
            k = 7.5625f * k * k + 0.984375f;
        }
        return k;
    }

    public static float BounceInOut(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = BounceIn(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = BounceOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float BounceOutIn(float k) {
        if (k <= 0.5f) {
            k = k * 2.0f;
            k = BounceOut(k);
            return k / 2.0f;
        }
        else {
            k = (k - 0.5f) * 2.0f;
            k = BounceIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }   
    #endregion

    public class action
    {
        public delegate void callback(GameObject _target);
        public delegate float Easing(float _time);

        protected GameObject target { get; set; }
        protected float ttime { get; set; }
        protected float dtime { get; set; }
        protected bool deleted { get; set; }
        protected Easing easing { get; set; }
        protected callback onEndCallback { get; set; }
        protected callback onFrameCallback { get; set; }        

        protected action(GameObject _target, float _time, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
        {
            if (_easing == null) _easing = SuperTweener.LinearNone;

            target = _target;
            easing = _easing;
            ttime = _time;
            dtime = 0;
            onEndCallback = _onEndCallback;
            onFrameCallback = _onFrameCallback;
            SuperTweener.Add(this); // A�ade la accion a la lista.
        }

        public void preupdate(float _delta){
            dtime+=_delta;
            if (dtime > ttime) dtime = ttime; 
        }

        public void finishTime()
        {
          dtime = ttime;
        }

        public virtual bool update() {
            if (dtime == ttime)
            {
                if (onEndCallback != null) onEndCallback(target);
                return true;
            }
            if (onFrameCallback != null) onFrameCallback(target);
            return false; 
        }

    };

    public class move : action
    {
        Vector3 m_org, m_dst;

        public move(GameObject _target, float _time, Vector3 _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback)
        {
            m_org = _target.transform.position;
            m_dst = _dst;
        }

        public override bool update()
        {
            if(target != null) target.transform.position = Vector3.Lerp(m_org, m_dst, easing(dtime / ttime));
            return base.update();
        }
    }

    public class MoveGuitextPixelOffset: action {
        
        Vector2 m_org, m_dst;

        public MoveGuitextPixelOffset(GameObject _target, float _time, Vector2 _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback) {
            m_org = _target.GetComponent<GUIText>().pixelOffset;
            m_dst = _dst;
        }

        public override bool update()
        {
            if (target != null)
                target.GetComponent<GUIText>().pixelOffset = Vector2.Lerp(m_org, m_dst, easing(dtime / ttime));
            return base.update();
        }
    }

    public class MoveGuitexturePixelInset: action {
        Rect m_org, m_dst;

        public MoveGuitexturePixelInset(GameObject _target, float _time, Rect _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback) {
            m_org = _target.GetComponent<GUITexture>().pixelInset;
            m_dst = _dst;
        }

        public override bool update() {
            if (target != null)
                target.GetComponent<GUITexture>().pixelInset = new Rect(
                    Mathf.Lerp(m_org.xMin, m_dst.xMin, easing(dtime / ttime)),
                    Mathf.Lerp(m_org.yMin, m_dst.yMin, easing(dtime / ttime)),
                    Mathf.Lerp(m_org.width, m_dst.width, easing(dtime / ttime)),
                    Mathf.Lerp(m_org.height, m_dst.height, easing(dtime / ttime)));
            return base.update();
        }
    }

    public class moveLocal : action
    {
        Vector3 m_org, m_dst;

        public moveLocal(GameObject _target, float _time, Vector3 _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback)
        {
            m_org = _target.transform.localPosition;
            m_dst = _dst;
        }

        public override bool update()
        {
            target.transform.localPosition = Vector3.Lerp(m_org, m_dst, easing(dtime / ttime));
            return base.update();
        }
    }

    public class scale : action
    {
        Vector3 m_org, m_dst;

        public scale(GameObject _target, float _time, Vector3 _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback)
        {
            m_org = _target.transform.localScale;
            m_dst = _dst;
        }

        public override bool update()
        {
            target.transform.localScale = Vector3.Lerp(m_org, m_dst, easing(dtime / ttime));
            return base.update();
        }
    }

    public class none : action
    {
        public none(GameObject _target, float _time, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, null, _onEndCallback, _onFrameCallback)
        {
        }
    }

    public class rotate : action {
        Quaternion m_org, m_dst;
        public rotate(GameObject _target, float _time, Quaternion _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback)
        {
            m_org = _target.transform.rotation;
            m_dst = _dst;
        }

        public override bool update() {
            target.transform.rotation = Quaternion.Lerp(m_org, m_dst, easing(dtime / ttime));
            return base.update();
        }
    }

    public class GUItextureColor : action
    {
        Color m_org, m_dst;
        public GUItextureColor(GameObject _target, float _time, Color _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback)
        {
            m_org = _target.GetComponent<GUITexture>().color;
            m_dst = _dst;
        }

        public override bool update() {
            if (target != null && target.GetComponent<GUITexture>() != null)
                target.GetComponent<GUITexture>().color = Color.Lerp(m_org, m_dst, easing(dtime / ttime));
            return base.update();
        }
    }

    public class GUITextColor : action
    {
        Color m_org, m_dst;
        public GUITextColor(GameObject _target, float _time, Color _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback)
        {
            m_org = _target.GetComponent<GUIText>().color;
            m_dst = _dst;
        }

        public override bool update()
        {
            target.GetComponent<GUIText>().color = Color.Lerp(m_org, m_dst, easing(dtime / ttime));
            return base.update();
        }
    }


    public class volume : action
    {
        float m_org, m_dst;

        public volume(GameObject _target, float _time, float _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback)
        {
            m_org = _target.GetComponent<AudioSource>().volume;
            m_dst = _dst;
        }

        public override bool update()
        {
            float e = easing(dtime / ttime);
            target.GetComponent<AudioSource>().volume = m_org * (1.0f - e) + m_dst * e;
            return base.update();
        }
    }

    public class animationSpeed : action
    {
        float m_org, m_dst;
        AnimationState m_anmState;

        public animationSpeed(GameObject _target, string _animName , float _time, float _dst, Easing _easing = null, callback _onEndCallback = null, callback _onFrameCallback = null)
            : base(_target, _time, _easing, _onEndCallback, _onFrameCallback) {
            m_anmState = target.GetComponent<Animation>()[_animName];
            m_org = m_anmState.speed;
            m_dst = _dst;
        }

        public override bool update() {
            float e = easing(dtime / ttime);
            m_anmState.speed = m_org * (1.0f - e) + m_dst * e;
            return base.update();
        }
    }    

    static SuperTweener m_instance;
    public static SuperTweener instance { get { if (m_instance == null) m_instance = new GameObject("SuperTweener").AddComponent<SuperTweener>(); return m_instance; } }

    public static void Add(action _action) {
        m_actions.Add(_action);
        if (!instance.gameObject.activeSelf)
        {
            instance.gameObject.SetActive(true);
            oldTimer = Time.realtimeSinceStartup;
        }
    }

    static List<action> m_actions = new List<action>();
  // Use this for initialization
  void Awake () {
        GameObject.DontDestroyOnLoad(gameObject);
        m_instance = this;
        gameObject.SetActive(false);
  }

    void Start()
    {
        oldTimer = Time.realtimeSinceStartup;
    }

	// Update is called once per frame
    static float oldTimer;
    void Update ()
    {
        float t = Time.realtimeSinceStartup - oldTimer;
        t = Time.unscaledDeltaTime;
        oldTimer = Time.realtimeSinceStartup;
        action[] actions = m_actions.ToArray();
        for( int i=0;i<actions.Length;++i){
            actions[i].preupdate(t);
            if( actions[i].update() ){
                // Si el update retona positivo, es hora de sacar la accion.
                m_actions.Remove(actions[i]);
                if(m_actions.Count==0)
                    gameObject.SetActive(false);
            }
        }
    }

    public static void Flush()
    {
        m_actions.Clear();
    }

    public static void Kill(action _action)
    {
        if (_action != null)
        {
            if (m_actions.Contains(_action))
                m_actions.Remove(_action);
            if (m_actions.Count == 0)
                m_instance.gameObject.SetActive(false);
        }
    }

    public static void InWaitOut(GameObject _gameObject, float _time, Vector3 _dst, float _waitTime, action.callback _onEndCallback = null)
    {
        Vector3 org = _gameObject.transform.position;
        new SuperTweener.move(_gameObject, _time, _dst, SuperTweener.QuintOut, (GameObject _target) =>
        {
            new SuperTweener.move(_gameObject, _waitTime, _dst, SuperTweener.LinearNone, (GameObject _target2) =>
            {
                new SuperTweener.move(_gameObject, _time, org, SuperTweener.QuintIn, _onEndCallback);
            });
        });
    }

    public static void FadeInWaitFadeOut(GameObject _gameObject, float _time, action.callback _onEndCallback = null, float _alpha=0.5f)
    {
        if (_gameObject.GetComponent<GUITexture>() != null)
        {
            Color org = _gameObject.GetComponent<GUITexture>().color;
            Color dst = _gameObject.GetComponent<GUITexture>().color;
            org.a = 0;
            dst.a = _alpha;
            _gameObject.GetComponent<GUITexture>().color = org;
            new SuperTweener.GUItextureColor(_gameObject, _time, dst, SuperTweener.QuintOut, (GameObject _target) =>
            {
                new SuperTweener.GUItextureColor(_gameObject, _time * 3.0f, dst, SuperTweener.LinearNone, (GameObject _target2) =>
                {
                    new SuperTweener.GUItextureColor(_gameObject, _time, org, SuperTweener.QuintIn, _onEndCallback);
                });
            });
        }
        else
        {
            if (_gameObject.GetComponent<GUIText>() != null)
            {
                Color org = _gameObject.GetComponent<GUIText>().color;
                Color dst = _gameObject.GetComponent<GUIText>().color;
                org.a = 0;
                dst.a = 1;
                _gameObject.GetComponent<GUIText>().color = org;
                new SuperTweener.GUITextColor(_gameObject, _time, dst, SuperTweener.QuintOut, (GameObject _target) =>
                {
                    new SuperTweener.GUITextColor(_gameObject, _time * 3.0f, dst, SuperTweener.LinearNone, (GameObject _target2) =>
                    {
                        new SuperTweener.GUITextColor(_gameObject, _time, org, SuperTweener.QuintIn, _onEndCallback);
                    });
                });
            }
        }

    }

}
