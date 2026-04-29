using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public void Read(BinaryReader reader)
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
        }
    }
}
