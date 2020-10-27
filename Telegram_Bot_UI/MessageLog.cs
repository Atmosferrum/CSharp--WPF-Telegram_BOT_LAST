namespace Telegram_Bot_UI
{
    struct MessageLog
    {
        public string Time { get; set; }
        public long ID { get; set; }
        public string Message { get; set; }
        public string FirstName { get; set; }

        public MessageLog(string Time, long ID, string FirstName, string Message)
        {
            this.Time = Time;
            this.ID = ID;            
            this.FirstName = FirstName;
            this.Message = Message;
        }
    }
}
