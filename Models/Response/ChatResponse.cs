    public class MedicalChatResponse
    {
        public string Reply { get; set; }
        public Boolean Complete { get; set; }
  
        public DoctorAvailabilityChatResponse? DoctorAvailability { get; set; }

    }

     public class DeepSeekApiResponse
        {
            public Choice[] choices { get; set; }
            
            public class Choice
            {
                public Message message { get; set; }
            }

            public class Message
            {
                public string content { get; set; }
            }
        }