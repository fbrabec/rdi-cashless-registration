using AutoMapper;
using CardRegistration.Application.Messages;
using CardRegistration.Domain.Entities;

namespace CardRegistration.Application.Profiles
{
    public class CardProfile : Profile
    {
        public CardProfile()
        {
            CreateMap<RegisterCardRequest, CardEntity>().ReverseMap();
        }
    }
}
