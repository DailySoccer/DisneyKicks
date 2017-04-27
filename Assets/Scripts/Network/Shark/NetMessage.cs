using System;

public class NetMessage : Attribute
{
  MsgType m_ID;
  public MsgType ID { get { return m_ID; } }
  public NetMessage(MsgType _ID) { m_ID = _ID; }
}
