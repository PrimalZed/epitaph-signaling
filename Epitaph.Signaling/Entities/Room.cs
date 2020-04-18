using System;

namespace Epitaph.Signaling.Entities
{
    public class Room
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Password { get; set; }
        public string HostId { get; set; }
    }
}
