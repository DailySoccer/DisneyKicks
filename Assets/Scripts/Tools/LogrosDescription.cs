using UnityEngine;
using System.Collections;

/*public enum premios
{
    nada,
    equipacionCasilla2,
    equipacionCasilla3,
    equipacionCasilla4,
    equipacionCasilla5,
    equipacionIniesta2,
    equipacionIniesta3,
    equipacionIniesta4,
    equipacionIniesta5,
    puntosBBVA10,
    puntosBBVA20,
    puntosBBVA50,
    puntosBBVA100,
    puntosBBVA500,
    puntosBBVA1000,
    puntosBBVA2500
};*/

[System.Serializable]
public class LogrosDescription : ScriptableObject
{
    [System.Serializable]
    public class descLogro
    {
        [SerializeField]
        public string m_name;
        [SerializeField]
        public string m_codigo;
        [SerializeField]
        public string m_descripcion;
        [SerializeField]
        public int m_premio;
        [SerializeField]
        public bool m_desbloqueado;

        public descLogro(string _codigo, string _name, string _descripcion, int _premio)
        {
            m_codigo = _codigo;
            m_name = _name;
            m_descripcion = _descripcion;
            m_premio = _premio;
        }
    };
    [SerializeField]
    public descLogro[] m_lista;


    public descLogro getLogroByCode(string _codigo) {
        foreach (descLogro logro in m_lista)
            if (logro.m_codigo == _codigo)
                return logro;
        return null;
    }

    public int Code2ID(string _codigo)
    {
        for (int i = 0; i < m_lista.Length;++i )
            if( m_lista[i].m_codigo == _codigo )
                return i;
        return -1;
    }

/*    public int getPremioPoints(premios _premio)
    {
        switch (_premio)
        {
            case premios.puntosBBVA10:
                return 10;
            case premios.puntosBBVA20:
                return 20;
            case premios.puntosBBVA50:
                return 50;
            case premios.puntosBBVA100:
                return 100;
            case premios.puntosBBVA500:
                return 500;
            case premios.puntosBBVA1000:
                return 1000;
            case premios.puntosBBVA2500:
                return 2500;
        }
        return 0;
    }*/


    public string getPremioDesc(descLogro _logro)
    {
        return string.Format("Obtienes {0} ptos. BBVA.", _logro.m_premio);
    }
}
