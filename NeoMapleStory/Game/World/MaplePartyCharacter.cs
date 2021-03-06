﻿using System.Drawing;
using System.Linq;
using NeoMapleStory.Game.Client;

namespace NeoMapleStory.Game.World
{
    public class MaplePartyCharacter
    {

        public string CharacterName { get; }
        public int CharacterId { get; private set; }
        public int Level { get; private set; }
        public int ChannelId { get; private set; }
        public int JobId { get; private set; }
        public int MapId { get; private set; }
        public bool Gender { get; private set; }
        public bool IsMarried { get; private set; }
        public int DoorTown { get; private set; } = 999999999;
        public int DoorTarget { get; private set; } = 999999999;
        public Point DoorPosition { get; private set; } = new Point(0, 0);
        public bool IsOnline { get; set; }

        public MaplePartyCharacter(MapleCharacter maplechar)
        {
            CharacterName = maplechar.Name;
            Level = maplechar.Level;
            ChannelId = maplechar.Client.ChannelId;
            CharacterId = maplechar.Id;
            JobId = maplechar.Job.JobId;
            MapId = maplechar.Map.MapId;
            IsOnline = true;
            Gender = maplechar.Gender;
            IsMarried = maplechar.IsMarried;
            if (maplechar.Doors.Any())
            {
                DoorTown = maplechar.Doors[0].Town.MapId;
                DoorTarget = maplechar.Doors[0].TargetMap.MapId;
                DoorPosition = maplechar.Doors[0].TargetMapPosition;
            }
        }

        public MaplePartyCharacter()
        {
            CharacterName = null;
        }

        public static bool operator ==(MaplePartyCharacter left, MaplePartyCharacter right)
        {
            if (left?.CharacterName == null || right?.CharacterName == null)
                return false;
            return left.CharacterName == right.CharacterName;
        }

        public static bool operator !=(MaplePartyCharacter left, MaplePartyCharacter right)
        {
            return !(left == right);
        }

        protected bool Equals(MaplePartyCharacter other)
        {
            return string.Equals(CharacterName, other.CharacterName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MaplePartyCharacter)obj);
        }

        public override int GetHashCode()
        {
            return CharacterName.GetHashCode();
        }
    }
}