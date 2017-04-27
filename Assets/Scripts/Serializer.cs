using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    public class bytes {
        public ushort Length { get; set; }
        public byte[] Buffer;

        public bytes(ushort _max) {
            Buffer = new byte[_max];
            Length = _max;
        }
    }

    public static class Serializer {
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
                            //¿Porque esta mierda de ñapa a continuacion?
                            //porque a algun crack, en alguna parte del mundo
                            //le parecio que un string no era un tipo primitivo
                            //ni tampoco un array
                            //¿que es para ti un string, amigo desconocido?
                            //esta chapuza te la dedico :)
                            if((field.FieldType.GetElementType() == typeof( string ))) {
                                ushort strlen = BitConverter.ToUInt16( _buffer, _idx ); _idx += 2;
                                string str = System.Text.Encoding.UTF8.GetString( _buffer, _idx, strlen );
                                arr.SetValue( str, i );
                                _idx += strlen;
                            }
                            else
                            {
                                object ele = Activator.CreateInstance( field.FieldType.GetElementType() );
                                if(field.FieldType.GetElementType().IsPrimitive){
                                    _idx = ByteDeserializeLiteral( field.FieldType.GetElementType(), ref ele, _buffer, _idx);
                                }
                                else {
                                    _idx = ByteDeserialize( field.FieldType.GetElementType(), ele, _buffer, _idx );
                                }
                                arr.SetValue( ele, i );
                            }
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
                Console.WriteLine( e.ToString() );
            }

            return _idx;
        }

        public static ushort ByteSerialize(Type _type, object _ref, ref byte[] _buffer, ushort _idx) {
            if(_type.IsPrimitive || (_type == typeof( string )))
            {
                return ByteSerializeLiteral(_type, _ref, ref _buffer, _idx);
            }

            System.Reflection.FieldInfo[] fields = _type.GetFields();

            foreach (System.Reflection.FieldInfo field in fields) {
                try
                {
                    if (field.IsNotSerialized) continue;
                    if (field.IsStatic) continue;
                    if (field.FieldType.IsArray)
                    {
                        Array array = field.GetValue(_ref) as Array;
                        int len = array.Length;
                        Array.Copy(BitConverter.GetBytes(len), 0, _buffer, _idx, 2); _idx += 2;
                        foreach (object obj in array)
                        {
                            _idx = ByteSerialize(obj.GetType(), obj, ref _buffer, _idx);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Deserializing property:" + field.FieldType);
                        _idx = ByteSerialize(field.FieldType, field.GetValue(_ref), ref _buffer, _idx);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " Campo " + _type + ":" + field);
                    throw e;
                }
            }

            return _idx;
        }

        public static ushort ByteSerializeLiteral(Type _type, object _ref, ref byte[] _buffer, ushort _idx) {
            if (_type == typeof(byte))
            {
                Array.Copy(BitConverter.GetBytes((byte)_ref), 0, _buffer, _idx, 1);
                _idx += 1;
            }
            else if (_type == typeof(sbyte))
            {
                Array.Copy(BitConverter.GetBytes((sbyte)_ref), 0, _buffer, _idx, 1);
                _idx += 1;
            }
            else if (_type == typeof(bool))
            {
                _buffer[_idx] = (byte)((bool)_ref ? 1 : 0);
                _idx += 1;
            }
            else if (_type == typeof(char))
            {
                Array.Copy(BitConverter.GetBytes((char)_ref), 0, _buffer, _idx, 2);
                _idx += 2;
            }
            else if (_type == typeof(short))
            {
                Array.Copy(BitConverter.GetBytes((short)_ref), 0, _buffer, _idx, 2);
                _idx += 2;
            }
            else if (_type == typeof(ushort))
            {
                Array.Copy(BitConverter.GetBytes((ushort)_ref), 0, _buffer, _idx, 2);
                _idx += 2;
            }
            else if (_type == typeof(int))
            {
                Array.Copy(BitConverter.GetBytes((int)_ref), 0, _buffer, _idx, 4);
                _idx += 4;
            }
            else if (_type == typeof(uint))
            {
                Array.Copy(BitConverter.GetBytes((uint)_ref), 0, _buffer, _idx, 4);
                _idx += 4;
            }
            else if (_type == typeof(float))
            {
                Array.Copy(BitConverter.GetBytes((float)_ref), 0, _buffer, _idx, 4);
                _idx += 4;
            }
            else if (_type == typeof(string))
            {
                string tmpStr = (string)_ref;
                if (tmpStr != string.Empty && tmpStr != null)
                {
                    byte[] str = System.Text.Encoding.UTF8.GetBytes(tmpStr);
                    Array.Copy(BitConverter.GetBytes(str.Length), 0, _buffer, _idx, 2); ;
                    Array.Copy(str, 0, _buffer, _idx + 2, str.Length); _idx += (ushort)(str.Length + 2);
                }
                else
                {
                    Array.Copy(BitConverter.GetBytes((ushort)0), 0, _buffer, _idx, 2); _idx += 2;
                }
            }
            else if (_type == typeof(bytes))
            {
                bytes tmp = _ref as bytes;
                Array.Copy(BitConverter.GetBytes(tmp.Length), 0, _buffer, _idx, 2); _idx += 2;
                Array.Copy(tmp.Buffer as byte[], 0, _buffer, _idx, tmp.Length); _idx += tmp.Length;
            }
            else
            {
                _idx = ByteSerialize(_type, _ref, ref _buffer, _idx);
            }

            return _idx;
        }

        public static ushort ByteDeserializeLiteral(Type _type, ref object _ref, byte[] _buffer, ushort _idx) {
            ushort len;
            if (_type == typeof( byte )) {
                _ref = (byte)BitConverter.ToChar( _buffer, _idx );
                _idx += 1;
            } else if (_type == typeof( sbyte )) {
                _ref = (sbyte)BitConverter.ToChar( _buffer, _idx );
                _idx += 1;
            } else if (_type == typeof( bool )) {
                _ref = (_buffer[_idx] == 1);
                _idx += 1;
            } else if (_type == typeof( char )) {
                _ref = (char)BitConverter.ToChar( _buffer, _idx );
                _idx += 2;
            } else if (_type == typeof( short )) {
                _ref = BitConverter.ToInt16( _buffer, _idx );
                _idx += 2;
            } else if (_type == typeof( ushort )) {
                _ref = BitConverter.ToUInt16( _buffer, _idx );
                _idx += 2;
            } else if (_type == typeof( int )) {
                _ref = BitConverter.ToInt32( _buffer, _idx );
                _idx += 4;
            } else if (_type == typeof( uint )) {
                _ref = BitConverter.ToUInt32( _buffer, _idx );
                _idx += 4;
            } else if (_type == typeof( float )) {
                _ref = BitConverter.ToSingle( _buffer, _idx );
                _idx += 4;
            } else if (_type == typeof( string )) {
                len = BitConverter.ToUInt16( _buffer, _idx ); _idx += 2;
                string str = System.Text.Encoding.UTF8.GetString( _buffer, _idx, len );
                _ref = str;
                _idx += len;
            } else if (_type == typeof( bytes )) {
                len = BitConverter.ToUInt16( _buffer, _idx ); _idx += 2;
                bytes tmp = new bytes( len );
                Array.Copy( _buffer, _idx, tmp.Buffer, 0, len ); _idx += len;
                _ref = tmp;
            } else {
                object obj = Activator.CreateInstance( _type );
                _idx = ByteDeserialize( _type, obj, _buffer, _idx );
                _ref = obj;
            }
            return _idx;
        }

    }
