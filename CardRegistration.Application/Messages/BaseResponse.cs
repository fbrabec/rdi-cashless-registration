using CardRegistration.Application.Dtos;

namespace CardRegistration.Application.Messages
{
    public class BaseResponse
    {
        public ErrorDto Error { get; set; }

        public void AddError(string code, string message)
        {
            Error = new ErrorDto
            {
                Code = code,
                Message = message
            };
        }
    }
}
