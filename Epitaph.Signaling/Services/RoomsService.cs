using Epitaph.Signaling.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Epitaph.Signaling.Services
{
    public interface IRoomsService
    {
        IEnumerable<KeyValuePair<string, string>> GetRooms();
        string CreateRoom(string name, string password);
        bool ValidateRoom(string roomid, string password);
        string JoinRoom(string roomId);
        IEnumerable<string> GetMembers(string memberId);
        void EnsureRoommates(string a, string b);
        void LeaveRoom(string memberId);
    }

    public class RoomsService : IRoomsService
    {
        private IDictionary<string, Room> _rooms = new Dictionary<string, Room>();
        private IDictionary<string, Member> _members = new Dictionary<string, Member>();

        private object _lock = new object();

        public IEnumerable<KeyValuePair<string, string>> GetRooms()
        {
            return _rooms
                .Select((pair) => new KeyValuePair<string, string>(pair.Key, pair.Value.Name));
        }

        public string CreateRoom(string name, string password)
        {
            lock (_lock)
            {
                var room = new Room
                {
                    Name = name,
                    Password = password
                };

                _rooms.Add(room.Id, room);

                return room.Id;
            }
        }

        public bool ValidateRoom(string roomId, string password)
        {
            if (!_rooms.ContainsKey(roomId))
            {
                return false;   
            }

            var room = _rooms[roomId];

            if (room.Password != password)
            {
                return false;
            }

            return true;
        }

        public string JoinRoom(string roomId)
        {
            lock (_lock)
            {
                if (!_rooms.ContainsKey(roomId))
                {
                    throw new KeyNotFoundException();
                }

                var member = new Member
                {
                    RoomId = roomId
                };

                _members.Add(member.Id, member);

                return member.Id;
            }
        }

        public IEnumerable<string> GetMembers(string memberId)
        {
            if (!_members.ContainsKey(memberId))
            {
                throw new KeyNotFoundException();
            }

            var roomId = _members[memberId].RoomId;

            return _members
                .Where((pair) => pair.Value.RoomId == roomId)
                .Where((pair) => pair.Key != memberId)
                .Select((pair) => pair.Key);
        }

        public void EnsureRoommates(string a, string b)
        {
            if (!_members.ContainsKey(a))
            {
                throw new KeyNotFoundException();
            }
            if (!_members.ContainsKey(b))
            {
                throw new KeyNotFoundException();
            }

            if (_members[a].RoomId != _members[b].RoomId)
            {
                throw new System.Exception("Members are not in the same room");
            }
        }

        public void LeaveRoom(string memberId)
        {
            lock (_lock)
            {
                if (!_members.ContainsKey(memberId))
                {
                    return;
                }

                var roomId = _members[memberId].RoomId;
                _members.Remove(memberId);

                if (!_members.Values.Any((member) => member.RoomId == roomId) && _rooms.ContainsKey(roomId))
                {
                    _rooms.Remove(roomId);
                }
            }
        }
    }
}
