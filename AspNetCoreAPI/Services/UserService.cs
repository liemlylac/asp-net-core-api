using System.Collections.Generic;
using System.Linq;
using ASPNetCoreAPI.Authorization;
using ASPNetCoreAPI.Core;
using ASPNetCoreAPI.Entities;
using ASPNetCoreAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace ASPNetCoreAPI.Services
{
    public interface IUserService
    {
        UserAuthenticateResponse Authenticate(UserAuthenticateRequest model);
        IEnumerable<UserEntity> GetAll();
        UserEntity GetById(long id);
        void Register(UserRegisterRequest model);
        void ChangePassword(long id, UserChangePasswordRequest model);
        void Update(long id, UserUpdateRequest model);
        void UpdateActive(UserUpdateActiveRequest model);
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        private IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;

        public UserService(
            DataContext context,
            IJwtUtils jwtUtils,
            IMapper mapper
        )
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
        }

        private UserEntity GetUser(long id)
        {
            if (_context.UserEntities == null)
            {
                throw new KeyNotFoundException("UserEntities is not found");
            }
            UserEntity user = _context.UserEntities.Find(id) 
                              ?? throw new KeyNotFoundException("User entity is not found");
            return user;
        }

        public UserAuthenticateResponse Authenticate(UserAuthenticateRequest model)
        {
            UserEntity user = _context.UserEntities.SingleOrDefault(x => x.Username == model.Username)
                              ?? throw new BadHttpRequestException("Username or password is incorrect");

            var response = _mapper.Map<UserAuthenticateResponse>(user);
            response.AccessToken = _jwtUtils.GenerateAccessToken(user);
            response.RefreshToken = _jwtUtils.GenerateRefreshToken(user);
            return response;
        }

        public IEnumerable<UserEntity> GetAll()
        {
            return _context.UserEntities;
        }

        public UserEntity GetById(long id)
        {
            return GetUser(id);
        }

        public void Register(UserRegisterRequest model)
        {
            if (_context.UserEntities.Any(user => user.Username == model.Username))
            {
                throw new BadHttpRequestException($"Username '{model.Username}' is already taken");
            }

            // map model to user entity
            UserEntity userEntity = _mapper.Map<UserEntity>(model);

            // hash password
            userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save user
            _context.UserEntities.Add(userEntity);
            _context.SaveChanges();
        }

        public void ChangePassword(long id, UserChangePasswordRequest model)
        {
            UserEntity user = GetUser(id);
            string oldPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.OldPassword);
            if (oldPasswordHash != user.PasswordHash)
            {
                throw new BadHttpRequestException("Invalid old password");
            }

            if (model.NewPassword != model.RepeatPassword)
            {
                throw new BadHttpRequestException("Repeat password does not match");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _mapper.Map(model, user);
            _context.UserEntities.Update(user);
            _context.SaveChanges();
        }

        public void Update(long id, UserUpdateRequest model)
        {
            UserEntity user = GetUser(id);
            _mapper.Map(model, user);
            _context.UserEntities.Update(user);
            _context.SaveChanges();
        }

        public void UpdateActive(UserUpdateActiveRequest model)
        {
            UserEntity user = GetUser(model.Ids.First());
            user.Active = model.Active;
            _context.UserEntities.Update(user);
            _context.SaveChanges();
        }
    }
}