using System;
using System.Security.Cryptography;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;

namespace MewPipe.DataFeeder.Utils
{
	public static class UserManager
	{
		private static UnitOfWork _unitOfWork = UnitOfWorkSingleton.GetInstance();

		public static User GetUserByUserName(string username)
		{
			return _unitOfWork.UserRepository.GetOne(user => user.UserName.Equals(username));
		}

		public static bool IsUserRegistered(string username)
		{
			return GetUserByUserName(username) != null;
		}

		public static User RegisterUser(string username)
		{
			var user = new User
			{
				UserName = username,
				EmailConfirmed = true,
				PasswordHash = HashPassword("bypass")
			};
			_unitOfWork.UserRepository.Insert(user);

			_unitOfWork.Save();

			return user;
		}

		#region Private Helpers

		private static string HashPassword(string password)
		{
			const int saltSize = 16;
			const int bytesRequired = 32;
			byte[] array = new byte[1 + saltSize + bytesRequired];
			const int iterations = 1000; // 1000, afaik, which is the min recommended for Rfc2898DeriveBytes
			using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltSize, iterations))
			{
				byte[] salt = pbkdf2.Salt;
				Buffer.BlockCopy(salt, 0, array, 1, saltSize);
				byte[] bytes = pbkdf2.GetBytes(bytesRequired);
				Buffer.BlockCopy(bytes, 0, array, saltSize + 1, bytesRequired);
			}
			return Convert.ToBase64String(array);
		}

		#endregion
	}
}