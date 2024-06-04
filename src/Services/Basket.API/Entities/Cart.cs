namespace Basket.API.Entities
{
    public class Cart
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public Cart()
        {

        }
        public Cart(string username)
        {
            UserName = username;
        }
        public decimal TotalPrice => Items.Sum(item => item.ItemPrice * item.Quantity);
        public DateTimeOffset LastModifiedDate { get; set; } = DateTimeOffset.UtcNow;
        public string? JobId { get; set; }
    }
}
