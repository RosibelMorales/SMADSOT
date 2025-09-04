namespace Smadot.Web.Models
{
	public class JsonResultDto
	{
		public string ErrorDescription { get; set; }
		public bool Error { get; set; }
		public object Result { get; set; }
	}
}
