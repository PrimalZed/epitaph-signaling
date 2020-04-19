using Epitaph.Signaling.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Epitaph.Signaling.Hubs
{
    public class SignalHub : Hub
    {
        private readonly IRoomsService _roomsService;

        public string MemberId
        {
            get
            {
                if (!Context.Items.TryGetValue("MemberId", out object value)) return null;
                return (string)value;
            }
            set { Context.Items["MemberId"] = value; }
        }

        public SignalHub(IRoomsService roomsService)
        {
            _roomsService = roomsService
                ?? throw new ArgumentNullException(nameof(roomsService));
        }

        public async Task Create(string name, string password)
        {
            var roomId = _roomsService.CreateRoom(name, password);

            await JoinHelper(roomId);
        }

        public async Task Join(string roomId, string password)
        {
            if (!_roomsService.ValidateRoom(roomId, password))
            {
                throw new HubException("Invalid room/password");
            }

            await JoinHelper(roomId);
        }

        private async Task JoinHelper(string roomId)
        {
            await Leave();

            var memberId = _roomsService.JoinRoom(roomId);

            MemberId = memberId;

            await Groups.AddToGroupAsync(Context.ConnectionId, memberId);

            var members = _roomsService.GetMembers(memberId);

            await Clients.Caller.SendAsync("members", members);
        }

        public async Task Relay(string to, string type, object content)
        {
            _roomsService.EnsureRoommates(MemberId, to);

            await Clients.Group(to).SendAsync("relay", type, MemberId, content);
        }

        public async Task Leave()
        {
            if (MemberId != null)
            {
                _roomsService.LeaveRoom(MemberId);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, MemberId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Leave();

            await base.OnDisconnectedAsync(exception);
        }
    }
}
