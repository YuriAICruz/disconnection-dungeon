using System;

namespace Graphene.WebSocketsNetworking
{
    public class Message
    {
        public uint id;
        public string uid;
        public string message;

        public Message(uint id, string message, Guid uid)
        {
            this.id = id;
            this.uid = uid.ToString();
            this.message = message;
        }
    }
}