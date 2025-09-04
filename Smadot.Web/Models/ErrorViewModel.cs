namespace Smadot.FrontEnd.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TypeError { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}