using UnityEngine;
using System.Collections.Generic;

public class ifcLogroInGame : ifcBase
{
    public static ifcLogroInGame instance { get; protected set; }
    public static Queue<string> m_logroConseguido = new Queue<string>();
    public LogrosDescription m_logros;
    bool m_onAction = false;
    void Awake(){
        m_onAction = false;
        instance = this;
    }

    void Update(){
        if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.F1)) {
            Debug.Log(">>> DEPRECATED: ESTE CHEAT ESTA DESHABILITADO POR VIEJUNO");
            //Interfaz.sendRound(false, Interfaz.ResultType.encaja, 5000, DifficultyService.GetRatioRecompensa());
        }
        if (m_logroConseguido.Count != 0 && !m_onAction) {
            string logro = m_logroConseguido.Dequeue();

            Texture txt = Resources.Load("Icos/" + logro, typeof(Texture)) as Texture;
            GUITexture gtx = transform.Find("anillo_logro/icon").GetComponent<GUITexture>();
            gtx.texture = txt;
            gtx.pixelInset = new Rect((-txt.width / 2.0f - 368) * ifcBase.scaleFactor, (-txt.height / 2.0f) * ifcBase.scaleFactor, (txt.width) * ifcBase.scaleFactor, (txt.height) * ifcBase.scaleFactor);
            LogrosDescription.descLogro  desc = m_logros.getLogroByCode(logro);

            transform.Find("txtNombreLogro").GetComponent<GUIText>().text = desc.m_name;
            GUIText gt = transform.Find("txtDescripcion").GetComponent<GUIText>();

            int lines = 0;
            gt.text = ifcTooltip.warp(desc.m_descripcion, 285.0f, gt.font, gt.fontSize, out lines);
            transform.Find("txtDescripcion").GetComponent<txtText>().Fix();
            transform.Find("txtPremio").GetComponent<GUIText>().text = m_logros.getPremioDesc(desc);

			GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.logroDesbloqueadoClip);

            m_onAction = true;
            SuperTweener.InWaitOut(gameObject, 0.5f, new Vector3(1, 0.58f, 0), 1.5f, (GameObject _target3) => { m_onAction = false; });
        }
    }

}

