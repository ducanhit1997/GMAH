namespace GMAH.Models.ViewModels
{
    public class BaseResponse
    {
        public BaseResponse()
        {

        }

        public BaseResponse(string errorMsg)
        {
            IsSuccess = false;
            Message = errorMsg;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public dynamic Object { get; set; }
    }
}
