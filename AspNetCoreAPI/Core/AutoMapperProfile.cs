using ASPNetCoreAPI.Entities;
using ASPNetCoreAPI.Models;
using AutoMapper;
using Microsoft.Build.Framework;

namespace ASPNetCoreAPI.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserEntity, UserAuthenticateResponse>();
            CreateMap<UserRegisterRequest , UserEntity>();
            CreateMap<UserAuthenticateRequest, UserEntity>();
            CreateMap<UserUpdateRequest, UserEntity>().ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) return false;
                    return prop.GetType() != typeof(string) || !string.IsNullOrEmpty((string) prop);
                }
            ));
        }
    }
}