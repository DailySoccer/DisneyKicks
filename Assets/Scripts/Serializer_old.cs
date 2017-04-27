using System;
using UnityEngine;

/*public class bytes {
    public ushort Length { get; set; }
    public byte[] Buffer;

    public bytes(ushort _max) {
        Buffer = new byte[_max];
        Length = _max;
    }
}*/


public static class Serializer_old {

    public static ushort ByteDeserialize(Type _type, object _ref, byte[] _buffer, ushort _idx) {
        ushort len;

        try {
            System.Reflection.FieldInfo[] fields = _ref.GetType().GetFields();
            foreach (System.Reflection.FieldInfo field in fields) {
                if (field.IsNotSerialized) continue;
                if (field.IsStatic) continue;
                if (field.FieldType.IsArray) {
                    len = BitConverter.ToUInt16( _buffer, _idx ); _idx += 2; // elementos
                    Array arr = Activator.CreateInstance( field.FieldType, len ) as Array;
                    for (int i = 0; i < len; ++i) {
                        object ele = Activator.CreateInstance( field.FieldType.GetElementType() );
                        _idx = ByteDeserialize( field.FieldType.GetElementType(), ele, _buffer, _idx );
                        Debug.Log("Deserialize!! " + ele + "  " + ele.GetType());
                        arr.SetValue( ele, i );
                    }
                    field.SetValue( _ref, arr );
                } else {
                    if (field.FieldType == typeof( byte )) {
                        field.SetValue( _ref, (byte)BitConverter.ToChar( _buffer, _idx ) );
                        _idx += 1;
                    } else if (field.FieldType == typeof( sbyte )) {
                        field.SetValue( _ref, (sbyte)BitConverter.ToChar( _buffer, _idx ) );
                        _idx += 1;
                    } else if (field.FieldType == typeof( bool )) {
                        field.SetValue(_ref, _buffer[_idx] == 1);
                        _idx += 1;
                    } else if (field.FieldType == typeof( char )) {
                        field.SetValue( _ref, (char)BitConverter.ToChar( _buffer, _idx ) );
                        _idx += 2;
                    } else if (field.FieldType == typeof( short )) {
                        field.SetValue( _ref, BitConverter.ToInt16( _buffer, _idx ) );
                        _idx += 2;
                    } else if (field.FieldType == typeof( ushort )) {
                        field.SetValue( _ref, BitConverter.ToUInt16( _buffer, _idx ) );
                        _idx += 2;
                    } else if (field.FieldType == typeof( int )) {
                        field.SetValue( _ref, BitConverter.ToInt32( _buffer, _idx ) );
                        _idx += 4;
                    } else if (field.FieldType == typeof( uint )) {
                        field.SetValue( _ref, BitConverter.ToUInt32( _buffer, _idx ) );
                        _idx += 4;
                    } else if (field.FieldType == typeof( float )) {
                        field.SetValue( _ref, BitConverter.ToSingle( _buffer, _idx ) );
                        _idx += 4;
                    } else if (field.FieldType == typeof( string )) {
                        len = BitConverter.ToUInt16( _buffer, _idx ); _idx += 2;
                        string str = System.Text.Encoding.UTF8.GetString( _buffer, _idx, len );
                        field.SetValue( _ref, str ); _idx += len;
                    } else if (field.FieldType == typeof( bytes )) {
                        len = BitConverter.ToUInt16( _buffer, _idx ); _idx += 2;
                        bytes tmp = new bytes( len );
                        Array.Copy( _buffer, _idx, tmp.Buffer, 0, len ); _idx += len;
                        field.SetValue( _ref, tmp );
                    } else {
                        object obj = Activator.CreateInstance( field.FieldType );
                        _idx = ByteDeserialize( field.FieldType, obj, _buffer, _idx );
                        field.SetValue( _ref, obj );
                    }
                }
            }
        } catch (Exception e) {
            UnityEngine.Debug.LogError( "Exception deserializing: " + e.ToString() );
        }

        return _idx;
    }

    public static ushort ByteSerialize(Type _type, object _ref, ref byte[] _buffer, ushort _idx) {
        System.Reflection.FieldInfo[] fields = _type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields) {
            if (field.IsNotSerialized) continue;
            if (field.IsStatic) continue;
            if (field.FieldType.IsArray) {
                Array array = field.GetValue( _ref ) as Array;
                int len = array.Length;
                Array.Copy( BitConverter.GetBytes( len ), 0, _buffer, _idx, 2 ); _idx += 2;
                foreach (object obj in array)
                    _idx = ByteSerialize( obj.GetType(), obj, ref _buffer, _idx );
            } else {
                if (field.FieldType == typeof( byte )) {
                    Array.Copy( BitConverter.GetBytes( (byte)field.GetValue( _ref ) ), 0, _buffer, _idx, 1 );
                    _idx += 1;
                } else if (field.FieldType == typeof( sbyte )) {
                    Array.Copy( BitConverter.GetBytes( (sbyte)field.GetValue( _ref ) ), 0, _buffer, _idx, 1 );
                    _idx += 1;
                } else if (field.FieldType == typeof( bool )) {
                    _buffer[_idx] = (byte)((bool)field.GetValue(_ref) ? 1 : 0);
                    _idx += 1;
                } else if (field.FieldType == typeof( char )) {
                    Array.Copy( BitConverter.GetBytes( (char)field.GetValue( _ref ) ), 0, _buffer, _idx, 2 );
                    _idx += 2;
                } else if (field.FieldType == typeof( short )) {
                    Array.Copy( BitConverter.GetBytes( (short)field.GetValue( _ref ) ), 0, _buffer, _idx, 2 );
                    _idx += 2;
                } else if (field.FieldType == typeof( ushort )) {
                    Array.Copy( BitConverter.GetBytes( (ushort)field.GetValue( _ref ) ), 0, _buffer, _idx, 2 );
                    _idx += 2;
                } else if (field.FieldType == typeof( int )) {
                    Array.Copy( BitConverter.GetBytes( (int)field.GetValue( _ref ) ), 0, _buffer, _idx, 4 );
                    _idx += 4;
                } else if (field.FieldType == typeof( uint )) {
                    Array.Copy( BitConverter.GetBytes( (uint)field.GetValue( _ref ) ), 0, _buffer, _idx, 4 );
                    _idx += 4;
                } else if (field.FieldType == typeof( float )) {
                    Array.Copy( BitConverter.GetBytes( (float)field.GetValue( _ref ) ), 0, _buffer, _idx, 4 );
                    _idx += 4;
                } else if (field.FieldType == typeof( string )) {
                    string tmpStr = (string)field.GetValue( _ref );
                    if (tmpStr != string.Empty && tmpStr != null) {
                        byte[] str = System.Text.Encoding.UTF8.GetBytes( tmpStr );
                        Array.Copy( BitConverter.GetBytes( str.Length ), 0, _buffer, _idx, 2 ); ;
                        Array.Copy( str, 0, _buffer, _idx + 2, str.Length ); _idx += (ushort)(str.Length + 2);
                    } else {
                        Array.Copy( BitConverter.GetBytes( (ushort)0 ), 0, _buffer, _idx, 2 ); _idx += 2;
                    }
                } else if (field.FieldType == typeof( bytes )) {
                    bytes tmp = field.GetValue( _ref ) as  bytes;
                    Array.Copy( BitConverter.GetBytes( tmp.Length ), 0, _buffer, _idx, 2 ); _idx += 2;
                    Array.Copy( tmp.Buffer as byte[], 0, _buffer, _idx, tmp.Length ); _idx += tmp.Length;
                } else {
                    _idx = ByteSerialize( field.FieldType, field.GetValue( _ref ), ref _buffer, _idx );
                }
            }
        }

        return _idx;
    }

}
