using UnityEngine;
using System;
using System.Collections;

//#if  !UNITY_ANDROID
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public delegate void SharkDelegate();
public delegate void SharkErrorDelegate(string _err);

public class Shark : MonoBehaviour
{

    static Shark m_instance;
    public static Shark instance { get { return m_instance; } }

    public string m_URL;
    public int m_Port;
    Socket m_socket = null;
    const int cTamBuffer = 1024;

    byte[] m_eBuffer = new byte[cTamBuffer];
    byte[] m_sBuffer = new byte[cTamBuffer];

    bool m_conectado = false;

    private string m_connectionId = null;

    public string ConnectionID {
        get {
            if (m_conectado) {
                return m_connectionId;
            } else {
                return null;
            }
        }
        set { m_connectionId = value; }
    }

    public SharkDelegate OnConnect = null;
    public SharkDelegate OnDisconnect = null;
    SharkErrorDelegate OnError = (string _err) => { Debug.Log("ERROR EN RED: " + _err); };

    // Use this for initialization
    void Awake() {
        if (m_instance != null) {
            Destroy(gameObject);
            return;
        }
        registrarMensajes();
        m_instance = this;
    }

    void Start() {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_socket == null) return;
        try
        {
            if (SocketConnected(m_socket))
            {
                SocketError errorCode;
                int cnt = 0;
                while (m_socket != null && cnt < 10)
                { // datos suficientes para un mensaje. 
                    if (m_socket.Available >= 2)
                    {
                        m_socket.Receive(m_eBuffer, 0, 2, SocketFlags.Peek, out errorCode);
                        if (errorCode == SocketError.Success)
                        {
                            deserialize();
                        }
                        else
                            if (errorCode != SocketError.WouldBlock && errorCode != SocketError.Success)
                                throw (new Exception("Connection reset " + errorCode));
                    }
                    else
                        break;
                    cnt++;
                }
            }
            else
            {
                throw (new Exception("Disconnected."));
            }
        }
        catch (Exception e)
        {
            Debug.LogError( "Error: " + e.ToString() );
            ErrorHub.ThrowError(8, (_name) => { Application.LoadLevel("Menus");});
            Desconectar();
            if (OnDisconnect != null) OnDisconnect();
        }
    }

    public void Conectar()
    {
        if (m_conectado) Desconectar();
        StartCoroutine(IniciaConexion());
    }

    public void Desconectar()
    {
        if (m_socket != null)
        {
            try
            {
                if (SocketConnected(m_socket))
                {
                    m_socket.Shutdown(SocketShutdown.Both);
                    m_socket.Close();
                }
                m_socket = null;
            }
            catch (Exception e)
            {
                Debug.LogError("Error en la desconexion " + e);
            }
        }
        m_conectado = false;
    }

    IEnumerator IniciaConexion()
    {
        IPAddress[] addresses = Dns.GetHostAddresses(m_URL);
        try
        {
            if (addresses.Length > 0)
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.ReceiveBufferSize = 2048;
                m_socket.NoDelay = true;

                StartCoroutine(conectarSocket(new IPEndPoint(addresses[0], m_Port)));
            }
            else
            {
                if (OnError != null) OnError("Error al resolver la URL.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error al inicia la red. " + e);
            if (OnError != null) OnError("Error al inicia la red. " + e);
            ErrorHub.ThrowError(7, (_name) => { Application.LoadLevel("menu");});
        }
        yield return true;
    }

    IEnumerator conectarSocket(IPEndPoint ipEndPoint)
    {
        try
        {
            m_socket.Connect(ipEndPoint);
            if (m_socket.Connected)
            {
                m_socket.Blocking = false;
                m_conectado = true;
                if (OnConnect != null) OnConnect();
            }
            else
            {
                m_socket.Shutdown(SocketShutdown.Both);
                m_socket.Close();
                m_socket = null;
                if (OnError != null) OnError("Error al conectar.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("CNetwork.beginConnect inicia la conexion. " + e);
            ErrorHub.ThrowError(7, (_name) => { Application.LoadLevel("Menus");});
            if (OnError != null) OnError("CNetwork.beginConnect inicia la conexion. " + e);
        }
        yield return true;
    }

    public bool enviar(MensajeBase _msg)
    {
        if (m_socket == null) return false;
        try{
            int len;
            serialize(_msg, out len);
            m_socket.Send(m_sBuffer, len, SocketFlags.None);
        }
        catch (Exception e)
        {
            Debug.Log("error al enviar el mensaje " + e);
            if (OnDisconnect != null) OnDisconnect();
            Desconectar();
            return false;
        }
        return true;
    }

#region mensajeria
    Dictionary<ushort, MensajeBase> m_mensajes = new Dictionary<ushort, MensajeBase>();
    void registrarMensajes()
    {
        foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
        {
            object[] obj = type.GetCustomAttributes(typeof(NetMessage), true);
            if (obj.Length > 0)
            {
                MsgType id = (obj[0] as NetMessage).ID;
                MensajeBase msg = Activator.CreateInstance(type) as MensajeBase;
                msg.ID = id;
                ushort uid = (ushort)id;
                if (!m_mensajes.ContainsKey(uid)) m_mensajes.Add(uid, msg);
            }
        }
/*
        Debug.Log( "NetMessage attributes registered" );

        Debug.Log( "Id -> Type" );
        foreach (var pair in m_mensajes) {
            Debug.Log( "  " + (MsgType)pair.Key + " -> " + pair.Value );
        }
*/
    }

    public T mensaje<T>() where T : MensajeBase
    {
        foreach (var item in m_mensajes)
            if (item.Value.GetType() == typeof(T))
                return (T)item.Value;
        return default(T);
    }

    public void deserialize()
    {
        ushort len = BitConverter.ToUInt16(m_eBuffer, 0);
        if (m_socket.Available >= len)
        {
            m_socket.Receive(m_eBuffer, len, SocketFlags.None); // Saca el paquete completo.
            ushort uid = BitConverter.ToUInt16(m_eBuffer, 2);
            ushort idx = 4;
            MensajeBase msg = (MensajeBase)m_mensajes[uid];
            if (msg == null) return;
            msg.Len = len;
            msg.ID = (MsgType)uid;
            idx = Serializer.ByteDeserialize( msg.GetType(), msg, m_eBuffer, idx );
            Debug.Log ("Received message: " + msg.GetType());
            msg.process();
        }
    }

    bool SocketConnected(Socket s)
    {
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        bool part2 = (s.Available == 0);
        if (part1 && part2)
            return false;
        else
            return true;
    }

    public byte[] serialize(MensajeBase _msg, out int len)
    {
        len = 0;
        if (_msg == null) return null;
        ushort idx = 4;
        idx = Serializer.ByteSerialize( _msg.GetType(), _msg, ref m_sBuffer, idx );
        _msg.Len = idx;
        Array.Copy(BitConverter.GetBytes(_msg.Len), 0, m_sBuffer, 0, 2); // Longitud
        Array.Copy(BitConverter.GetBytes((ushort)_msg.ID), 0, m_sBuffer, 2, 2); // Identificador del paquete.
        len = idx;
        Debug.Log ("Sending message: " + _msg.GetType());
        return m_sBuffer;
    }

#endregion
}

//#endif