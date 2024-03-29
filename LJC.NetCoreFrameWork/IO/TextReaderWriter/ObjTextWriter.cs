﻿using LJC.NetCoreFrameWork.Comm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LJC.NetCoreFrameWork.IO.TextReaderWriter
{
    public class ObjTextWriter : ObjTextReaderWriterBase, IDisposable
    {
        private StreamWriter _sw;

        private ObjTextWriter(string textfile, ObjTextReaderWriterEncodeType encodetype)
        {
            this.readwritePath = textfile;
            var fs = File.Open(textfile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _sw = new StreamWriter(fs, Encoding.UTF8);
            int firstchar = fs.ReadByte();
            if (firstchar == -1)
            {
                this._encodeType = encodetype;
                fs.WriteByte((byte)encodetype);
            }
            else
            {
                this._encodeType = (ObjTextReaderWriterEncodeType)firstchar;
                _sw.BaseStream.Position = _sw.BaseStream.Length;

                while (true)
                {
                    if (CheckHasEndSpan(_sw.BaseStream))
                    {
                        _sw.BaseStream.Position -= 3;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //Flush();
            _canReadFromBack = CanReadFormBack;
            if (_canReadFromBack)
            {
                if (PostionLast(_sw.BaseStream))
                {
                    _sw.BaseStream.Position += 6;
                }
                else
                {
                    _sw.BaseStream.Position = 1;
                }
            }
        }


        public static ObjTextWriter CreateWriter(string textfile, ObjTextReaderWriterEncodeType encodetype = ObjTextReaderWriterEncodeType.json)
        {
            return new ObjTextWriter(textfile, encodetype);
        }

        /// <summary>
        /// 用结束符填充空格
        /// </summary>
        public void FillSpace(long count)
        {
            var oldcount = count;
            lock (this)
            {
                while (count-- > 0)
                {
                    this._sw.BaseStream.Write(endSpanChar, 0, 3);
                }
                //this._sw.BaseStream.Position += 3 * oldcount;
            }
        }

        public void Flush()
        {
            if (!_isdispose)
            {
                lock (this)
                {
                    _sw.Flush();
                }
            }
        }

        /// <summary>
        /// 预追加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="append">返回null，则自动加到文件后面</param>
        /// <returns></returns>
        public Tuple<long, long> PreAppendObject<T>(T obj, Func<byte[], Stream, Tuple<long, long>> append) where T : class
        {
            Tuple<long, long> offset;

            using (MemoryStream ms0 = new MemoryStream())
            {
                using (System.IO.StreamWriter tempms = new StreamWriter(ms0))
                {
                    switch (_encodeType)
                    {
                        case ObjTextReaderWriterEncodeType.protobuf:
                        case ObjTextReaderWriterEncodeType.protobufex:
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    throw new NotImplementedException();
                                }
                                break;
                            }
                        case ObjTextReaderWriterEncodeType.jsonbuf:
                        case ObjTextReaderWriterEncodeType.jsonbufex:
                            {
                                var json = JsonUtil<T>.Serialize(obj);
                                offset = Append(tempms.BaseStream, Encoding.UTF8.GetBytes(json), true);
                                break;
                            }
                        case ObjTextReaderWriterEncodeType.entitybuf:
                        case ObjTextReaderWriterEncodeType.entitybufex:
                            {
                                var buf = EntityBuf.EntityBufCore.Serialize(obj);
                                offset = Append(tempms.BaseStream, buf, true);
                                break;
                            }
                        case ObjTextReaderWriterEncodeType.entitybuf2:
                            {
                                var buf = EntityBuf2.EntityBufCore2.Serialize(obj);
                                offset = Append(tempms.BaseStream, buf, true);
                                break;
                            }
                        default:
                            {
                                string str = JsonUtil<T>.Serialize(obj);
                                if (ObjTextReaderWriterEncodeType.jsongzip == this._encodeType)
                                {
                                    var jsonByte = Encoding.UTF8.GetBytes(str);
                                    var compressbytes = GZip.Compress(jsonByte);
                                    offset = Append(tempms.BaseStream, compressbytes, false);
                                }
                                else
                                {
                                    offset = Append(tempms, str);
                                }
                                break;
                            }
                    }

                    var bytes = ms0.ToArray();
                    lock (this)
                    {
                        offset = append(bytes, _sw.BaseStream);

                    }

                    if (offset == null)
                    {
                        lock (this)
                        {
                            var start = _sw.BaseStream.Position;
                            _sw.BaseStream.Write(bytes, 0, bytes.Length);
                            offset = new Tuple<long, long>(start, _sw.BaseStream.Position);
                        }

                    }
                }
            }

            return offset;
        }

        public Tuple<long, long> AppendObject<T>(T obj) where T : class
        {
            Tuple<long, long> offset;
            switch (this._encodeType)
            {
                case ObjTextReaderWriterEncodeType.protobuf:
                case ObjTextReaderWriterEncodeType.protobufex:
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    }
                case ObjTextReaderWriterEncodeType.jsonbuf:
                case ObjTextReaderWriterEncodeType.jsonbufex:
                    {
                        var json = JsonUtil<T>.Serialize(obj);
                        offset = Append(Encoding.UTF8.GetBytes(json), true);
                        break;
                    }
                case ObjTextReaderWriterEncodeType.entitybuf:
                case ObjTextReaderWriterEncodeType.entitybufex:
                    {
                        var buf = EntityBuf.EntityBufCore.Serialize(obj);
                        offset = Append(buf, true);
                        break;
                    }
                case ObjTextReaderWriterEncodeType.entitybuf2:
                    {
                        var buf = EntityBuf2.EntityBufCore2.Serialize(obj);
                        offset = Append(buf, true);
                        break;
                    }
                default:
                    {
                        string str = JsonUtil<T>.Serialize(obj);
                        if (ObjTextReaderWriterEncodeType.jsongzip == this._encodeType)
                        {
                            var jsonByte = Encoding.UTF8.GetBytes(str);
                            var compressbytes = GZip.Compress(jsonByte);
                            offset = Append(compressbytes, false);
                        }
                        else
                        {
                            offset = Append(str);
                        }
                        break;
                    }
            }

            return offset;
        }

        private Tuple<long, long> Append(StreamWriter s, string objtr)
        {
            long offset = s.BaseStream.Position;
            s.WriteLine();
            s.Write(objtr);
            s.Write(splitChar);

            return new Tuple<long, long>(offset, s.BaseStream.Position);
        }

        private Tuple<long, long> Append(string objtr)
        {
            if (string.IsNullOrEmpty(objtr))
                return new Tuple<long, long>(0, 0);

            lock (this)
            {
                return Append(_sw, objtr);
            }
        }

        public void SetPosition(long pos)
        {
            _sw.BaseStream.Position = pos;

        }

        public long GetWritePosition()
        {
            return _sw.BaseStream.Position;
        }

        private Tuple<long, long> Append(Stream s, byte[] objstream, bool writesplit)
        {
            var lenbyte = BitConverter.GetBytes(objstream.Length);

            long offset = s.Position;
            s.Write(lenbyte, 0, lenbyte.Length);
            s.Write(objstream, 0, objstream.Length);
            if (_canReadFromBack)
            {
                s.Write(lenbyte, 0, lenbyte.Length);
            }

            if (writesplit)
            {
                s.Write(ObjTextReaderWriterBase.splitBytes, 0, 2);
            }

            return new Tuple<long, long>(offset, s.Position);
        }

        private Tuple<long, long> Append(byte[] objstream, bool writesplit)
        {
            if (objstream == null)
                return new Tuple<long, long>(0, 0);

            lock (this)
            {
                return Append(_sw.BaseStream, objstream, writesplit);
            }
        }

        public Tuple<long, long> Override(long start, byte[] bytes)
        {
            lock (this)
            {
                _sw.BaseStream.Position = start;
                _sw.BaseStream.Write(bytes, 0, bytes.Length);

                return new Tuple<long, long>(start, _sw.BaseStream.Position);
            }
        }

        public Tuple<long, long> Override(long start, byte[] bytes, int len)
        {
            lock (this)
            {
                _sw.BaseStream.Position = start;
                _sw.BaseStream.Write(bytes, 0, len);

                return new Tuple<long, long>(start, _sw.BaseStream.Position);
            }
        }

        private bool _isdispose = false;
        public void Dispose(bool disposing)
        {
            if (!_isdispose)
            {
                if (_sw != null)
                {
                    lock (this)
                    {
                        try
                        {
                            _sw.Close();
                        }
                        catch
                        {

                        }
                    }
                }
                GC.SuppressFinalize(this);
            }

            _isdispose = true;
        }

        public bool Isdispose
        {
            get
            {
                return _isdispose;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~ObjTextWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object Tag
        {
            get;
            set;
        }
    }
}
