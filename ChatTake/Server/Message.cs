namespace Server
{
    public class Message
    {
        private Client sender;
        private string messageBody;
        private string userId;

        public string UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
            }
        }

        public string MessageBody
        {
            get
            {
                return messageBody;
            }
            set
            {
                messageBody = value;
            }
        }

        public Client Sender
        {
            get
            {
                return sender;
            }
            set
            {
                sender = value;
            }
        }

        public Message(Client sender, string Body)
        {
            this.sender = sender;
            messageBody = Body;
            UserId = sender.UserId;
        }
    }
}
