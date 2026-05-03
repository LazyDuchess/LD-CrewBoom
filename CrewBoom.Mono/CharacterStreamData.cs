using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CrewBoomMono
{
    public class CharacterStreamData
    {
        private const byte Version = 0;
        public string Id;
        public UnlockType UnlockType;
        public BrcMovestyle DefaultMoveStyle;
        public string Name;
        public string GrafTitle;
        public string GrafAuthor;
        public bool HasGraffiti;
        public BrcCharacter IdleDance;
        public string BoEIdleDance;
        public bool BoEIdleDanceVanilla;
        public Texture2D GraffitiTexture = null;

        public void FromCharacter(CharacterDefinition character)
        {
            Id = character.Id;
            UnlockType = character.UnlockType;
            DefaultMoveStyle = character.DefaultMovestyle;
            Name = character.CharacterName;
            HasGraffiti = character.Graffiti != null;
            GrafTitle = character.GraffitiName;
            GrafAuthor = character.GraffitiArtist;
            IdleDance = character.BounceAnimation;
            BoEIdleDance = character.BoEBounceAnimation;
            BoEIdleDanceVanilla = character.BoEBounceAnimationVanilla;
            if (HasGraffiti)
            {
                CopyGraffitiTexture(character.Graffiti.mainTexture as Texture2D);
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(Id);
            writer.Write((int)UnlockType);
            writer.Write((int)DefaultMoveStyle);
            writer.Write(Name);
            writer.Write(HasGraffiti);
            writer.Write(GrafTitle);
            writer.Write(GrafAuthor);
            writer.Write((int)IdleDance);
            writer.Write(BoEIdleDance);
            writer.Write(BoEIdleDanceVanilla);
            if (HasGraffiti)
            {
                var grafData = GraffitiTexture.EncodeToPNG();
                writer.Write(grafData.Length);
                writer.Write(grafData);
            }
        }

        public void Read(BinaryReader reader, bool grafreadable = false)
        {
            var version = reader.ReadByte();
            Id = reader.ReadString();
            UnlockType = (UnlockType)reader.ReadInt32();
            DefaultMoveStyle = (BrcMovestyle)reader.ReadInt32();
            Name = reader.ReadString();
            HasGraffiti = reader.ReadBoolean();
            GrafTitle = reader.ReadString();
            GrafAuthor = reader.ReadString();
            IdleDance = (BrcCharacter)reader.ReadInt32();
            BoEIdleDance = reader.ReadString();
            BoEIdleDanceVanilla = reader.ReadBoolean();
            if (HasGraffiti)
            {
                var dataLen = reader.ReadInt32();
                var pngData = reader.ReadBytes(dataLen);

                GraffitiTexture = new Texture2D(2, 2);
                GraffitiTexture.LoadImage(pngData, !grafreadable);
            }
        }

        void CopyGraffitiTexture(Texture2D source)
        {
            RenderTexture rt = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB);

            Graphics.Blit(source, rt);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            GraffitiTexture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
            GraffitiTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            GraffitiTexture.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
        }

        public void Release()
        {
            if (GraffitiTexture != null)
                UnityEngine.Object.DestroyImmediate(GraffitiTexture);
        }
    }
}
