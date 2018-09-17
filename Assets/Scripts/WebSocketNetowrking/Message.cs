using System;

namespace Graphene.WebSocketsNetworking
{
    public class Message
    {
        public uint id;
        public uint objectId;
        public string uid;
        public string message;

        public Message(uint id, string message, Guid uid, uint objectId = 0)
        {
            this.id = id;
            this.objectId = objectId;
            this.uid = uid.ToString();
            this.message = message;
        }
    }
}