using System;

namespace LostAndFound.Models
{
    public class ItemRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ItemId { get; set; }
        public string RequestedName { get; set; } = string.Empty;
        public string RequestedDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RequesterName { get; set; } = string.Empty;
        public string RequesterPhone { get; set; } = string.Empty;
        public string RequesterEmail { get; set; } = string.Empty;
        public string LinkedItemName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
