﻿namespace RentMateAPI.DTOModels.DTOMessage
{
    public class SenderDto
    {
        public int SenderId { get; set; }

        public string SenderName { get; set; } = null!;

        public byte[]? SenderImage { get; set; }

        //public string LastMessage { get; set; } = null!;

        //public bool IsAnyUnseenMessages { get; set; }

        //public int NumberOfUnseenMessages { get; set; }
    }
}
