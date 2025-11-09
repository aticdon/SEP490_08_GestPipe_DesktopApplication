using AutoMapper;
using GestPipe.Backend.Mappings;
using GestPipe.Backend.Models;
using GestPipe.Backend.Models.DTOs;
using GestPipe.Backend.Models.DTOs.Auth;
using GestPipe.Backend.Models.DTOs.Profile;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<UserProfile> _profilesCollection;
        private readonly IMapper _mapper;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(
            IMongoClient mongoClient,
            IOptions<MongoDbSettings> dbSettings,
            IMapper mapper,
            ILogger<ProfileService> logger)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _usersCollection = database.GetCollection<User>("Users");
            _profilesCollection = database.GetCollection<UserProfile>("User_Profiles");
            _mapper = mapper;
            _logger = logger;
        }

        // ✅ VIEW PROFILE
        public async Task<AuthResponseDto> GetUserProfileAsync(string userId)
        {
            try
            {
                if (!ObjectId.TryParse(userId, out _))
                    return new AuthResponseDto { Success = false, Message = "Invalid user ID." };

                var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    return new AuthResponseDto { Success = false, Message = "User not found." };

                var profile = await _profilesCollection.Find(p => p.UserId == userId).FirstOrDefaultAsync();
                if (profile == null)
                    return new AuthResponseDto { Success = false, Message = "Profile not found." };

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Profile retrieved successfully.",
                    UserId = userId,
                    Email = user.Email,
                    Data = new
                    {
                        Profile = _mapper.Map<UserProfileDto>(profile),
                        User = _mapper.Map<UserDto>(user)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile: {UserId}", userId);
                return new AuthResponseDto { Success = false, Message = "Error retrieving profile." };
            }
        }

        // ✅ UPDATE PROFILE
        public async Task<AuthResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateDto)
        {
            try
            {
                if (!ObjectId.TryParse(userId, out _))
                    return new AuthResponseDto { Success = false, Message = "Invalid user ID." };

                var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    return new AuthResponseDto { Success = false, Message = "User not found." };

                var profile = await _profilesCollection.Find(p => p.UserId == userId).FirstOrDefaultAsync();
                if (profile == null)
                    return new AuthResponseDto { Success = false, Message = "Profile not found." };

                // ✅ Map using AutoMapper
                _mapper.Map(updateDto, profile);
                profile.UpdatedAt = DateTime.UtcNow;

                // ✅ Update AvatarUrl nếu có
                if (!string.IsNullOrEmpty(updateDto.AvatarUrl))
                {
                    profile.ProfileImage = updateDto.AvatarUrl;
                    user.AvatarUrl = updateDto.AvatarUrl;
                    await _usersCollection.ReplaceOneAsync(u => u.Id == userId, user);
                }

                var result = await _profilesCollection.ReplaceOneAsync(p => p.Id == profile.Id, profile);

                if (result.ModifiedCount == 0)
                    return new AuthResponseDto { Success = false, Message = "Failed to update profile." };

                _logger.LogInformation("Profile updated: {UserId}", userId);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Profile updated successfully.",
                    UserId = userId,
                    Email = user.Email,
                    Data = new
                    {
                        Profile = _mapper.Map<UserProfileDto>(profile),
                        User = _mapper.Map<UserDto>(user)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile: {UserId}", userId);
                return new AuthResponseDto { Success = false, Message = "Error updating profile." };
            }
        }

        // ✅ CHANGE PASSWORD
        public async Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ObjectId.TryParse(userId, out _))
                    return new AuthResponseDto { Success = false, Message = "Invalid user ID." };

                var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    return new AuthResponseDto { Success = false, Message = "User not found." };

                // ✅ Verify old password
                if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid old password for user: {UserId}", userId);
                    return new AuthResponseDto { Success = false, Message = "Old password is incorrect." };
                }

                // ✅ Hash new password
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

                var update = Builders<User>.Update.Set(u => u.PasswordHash, newPasswordHash);
                var result = await _usersCollection.UpdateOneAsync(u => u.Id == userId, update);

                if (result.ModifiedCount == 0)
                    return new AuthResponseDto { Success = false, Message = "Failed to change password." };

                _logger.LogInformation("Password changed: {UserId}", userId);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Password changed successfully.",
                    UserId = userId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password: {UserId}", userId);
                return new AuthResponseDto { Success = false, Message = "Error changing password." };
            }
        }
    }
}