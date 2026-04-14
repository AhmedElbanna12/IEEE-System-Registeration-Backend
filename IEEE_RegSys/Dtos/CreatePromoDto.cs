namespace IEEE_RegSys.Dtos
{
    public class CreatePromoDto
    {
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public int UsageLimit { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
