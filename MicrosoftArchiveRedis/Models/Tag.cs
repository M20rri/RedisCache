namespace MicrosoftArchiveRedis.Models
{
    public class Tag
    {
        public int organizationId { get; set; }
        public int promotionId { get; set; }
        public int organizationPromotionId { get; set; }
    }

    public class TagWithOwnerEntityType
    {
        public int ownerEntityId { get; set; }
        public string ownerContextType { get; set; }
    }
}