using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using CrewBoomMono;

namespace CrewBoom.Mono
{
    public class EmbeddedBundle : IDisposable
    {
        private const string Magic = "STREAM";
        private const byte Version = 0;

        private string _path;
        private FileStream _stream;

        public EmbeddedBundle(string bundlePath)
        {
            _path = bundlePath;
        }

        public void OpenRead()
        {
            Close();
            _stream = new FileStream(_path, FileMode.Open, FileAccess.Read);
        }

        public void OpenWrite()
        {
            Close();
            _stream = new FileStream(_path, FileMode.Open, FileAccess.Write);
        }

        public void Close()
        {
            if (_stream != null) _stream.Dispose();
            _stream = null;
        }

        public void Dispose()
        {
            if (_stream != null)
                _stream.Dispose();
        }

        public AssetBundle LoadAssetBundle()
        {
            Close();
            return AssetBundle.LoadFromFile(_path);
        }

        public void AppendStreamData(CharacterStreamData streamData)
        {
            if (_stream == null)
                throw new IOException("There isn't any stream open to write!");

            if (!_stream.CanWrite)
                throw new IOException("The FileStream can't write!");

            _stream.Seek(0, SeekOrigin.End);
            var streamDataOffset = (int)_stream.Position;

            using (var writer = new BinaryWriter(_stream))
            {
                streamData.Write(writer);
                writer.Write(streamDataOffset);
                writer.Write(Encoding.ASCII.GetBytes(Magic));
                writer.Write(Version);
            }
        }

        public bool TryRetrieveStreamData(out CharacterStreamData streamData)
        {
            if (_stream == null)
                throw new IOException("There isn't any stream open to read!");

            if (!_stream.CanRead)
                throw new IOException("The FileStream can't read!");

            streamData = null;
            
            // Seek to and read the footer, containing our magic and the file version.
            var footerLength = Magic.Length + 1;
            _stream.Seek(-footerLength, SeekOrigin.End);

            var footer = new byte[footerLength];
            _stream.Read(footer, 0, footerLength);

            var magic = Encoding.ASCII.GetString(footer, 0, Magic.Length);

            if (magic != Magic) return false;

            var version = footer[6];

            if (version < 0 || version > Version)
                throw new IOException($"Unrecognized Stream Data version {version}");

            // Verified the footer and version, read the offset of the stream data.
            _stream.Seek(-footerLength - 4, SeekOrigin.End);

            var offsetBytes = new byte[4];
            _stream.Read(offsetBytes, 0, 4);
            var offset = BitConverter.ToInt32(offsetBytes, 0);

            _stream.Seek(offset, SeekOrigin.Begin);

            using (var reader = new BinaryReader(_stream))
            {
                streamData = new CharacterStreamData();
                streamData.Read(reader);
            }

            return true;
        }
    }
}
