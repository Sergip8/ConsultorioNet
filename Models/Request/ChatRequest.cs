public class MedicalChatRequest
    {
        public string Message { get; set; }
        public List<ChatMessageDto> ConversationHistory { get; set; }
        public string userId { get; set; } // Opcional para seguimiento
    }

       public class ChatMessageDto
    {
        public string role { get; set; }
        public string content { get; set; }

    }