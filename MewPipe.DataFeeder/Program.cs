using System;
using System.Collections.Generic;
using System.Linq;
using MewPipe.DataFeeder.Entities;
using MewPipe.DataFeeder.Utils;
using MewPipe.Logic.Models;

namespace MewPipe.DataFeeder
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				ShowUsage();
			}
			else
			{
				// Users
				var usersXlsxPath = args[0];
				Console.WriteLine("Getting the users from the excel file.");
				var excelUsers = ExcelManager.GetUsers(usersXlsxPath);
				Console.WriteLine("Found {0} users in the excel file.", excelUsers.Count);

				Console.WriteLine("Creating accounts for all users ...");
				var users = new Dictionary<string, User>();
				foreach (var excelUser in excelUsers)
				{
					User user;
					if (UserManager.IsUserRegistered(excelUser.UserName))
					{
						user = UserManager.GetUserByUserName(excelUser.UserName);
						Console.WriteLine("User {0} already exists in database, skipping ...", excelUser.FullName);
					}
					else
					{
						user = UserManager.RegisterUser(excelUser);
						Console.WriteLine("User {0} successfully added into the database !", excelUser.FullName);
					}

					users.Add(excelUser.FullName, user);
				}

				// Videos
				var videosXlsxPath = args[1];
				Console.WriteLine("Getting the videos from the excel file.");
				var excelVideos = ExcelManager.GetVideos(videosXlsxPath);
				Console.WriteLine("Found {0} videos in the excel file.", excelVideos.Count);

				Console.WriteLine("Starting the downloads (The first initialisation may take few minutes) ...");
				var videos = new List<MewPipeVideo>();
				foreach (var excelVideo in excelVideos)
				{
					// Downloading videos or getting em from the cache folder
					var mewpipeVideo = VideoManager.Download(excelVideo.Url);
					mewpipeVideo.Category = excelVideo.Category;
					videos.Add(mewpipeVideo);
				}

				// Likes and dislikes
				Console.WriteLine("Faking like and dislikes ...");
				videos.Shuffle(); // Shuffling videos
				foreach (var mewpipeVideo in videos)
				{
					// Faking likes and likes
					VideoManager.FakeImpressions(mewpipeVideo, excelUsers, users);
					var likes = mewpipeVideo.Impressions.Count(impression => impression.Type == Impression.ImpressionType.Good);
					var dislikes = mewpipeVideo.Impressions.Count(impression => impression.Type == Impression.ImpressionType.Bad);
					Console.WriteLine("{0}  {1} likes - {2} dislikes.", mewpipeVideo.Title, likes, dislikes);
				}
			}

			Console.Write("\nPress any key to continue ...");
			Console.ReadKey();
		}

		private static void ShowUsage()
		{
			Console.WriteLine("USAGE: DataFeeder.exe <pathToUsersXlsxFile> <pathToVideosXlsxFile>");
		}
	}
}