using System;

namespace Epitaph.Signaling.Entities
{
    public class Member
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string RoomId { get; set; }
    }
}
