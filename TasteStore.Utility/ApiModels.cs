namespace TasteStore.Utility
{
    public class CreateOrderApiResponse
    {
        public int OrderId { get; set; }

        public string Message { get; set; }
    }

    public class CreateOrderApiRequest
    {
        public string SessionId { get; set; }

        public string UserId { get; set; }
    }
}
