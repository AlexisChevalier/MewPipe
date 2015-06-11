﻿using System;
using System.Collections.Generic;
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
					//TODO: Refactor this
					User user;
					if (UserManager.IsUserRegistered(excelUser.UserName))
					{
						user = UserManager.GetUserByUserName(excelUser.UserName);
						Console.WriteLine("User {0} already exists in database, skipping ...", excelUser.FullName);
					}
					else
					{
						user = UserManager.RegisterUser(excelUser.UserName);
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
					mewpipeVideo.Author = excelVideo.Author;

					//TODO: Refactor this
					User user;
					if (UserManager.IsUserRegistered(mewpipeVideo.Author))
					{
						user = UserManager.GetUserByUserName(mewpipeVideo.Author);
					}
					else
					{
						user = UserManager.RegisterUser(mewpipeVideo.Author);
						Console.WriteLine("User {0} successfully added into the database !", mewpipeVideo.Author);
					}

					//users.Add(mewpipeVideo.Author, user);

					videos.Add(mewpipeVideo);
				}

				// Likes and dislikes
				Console.WriteLine("Faking like and dislikes ...");
				videos.Shuffle(); // Shuffling videos
				// Faking likes and likes
				VideoManager.FakeImpressions(videos, excelUsers, users);

				// Uploading videos into MewPipe
				Console.WriteLine("Uploading videos into MewPipe ...");
				foreach (var mewpipeVideo in videos)
				{
					if (!VideoManager.IsVideoUploaded(mewpipeVideo.Title))
					{
						Console.Write("Uploading {0} ...", mewpipeVideo.Title);
						VideoManager.UploadToMewPipe(mewpipeVideo);
						Console.WriteLine(" Uploaded !");
					}

					Console.Write("Updating {0} impressions ...", mewpipeVideo.Title);
					VideoManager.UpdateImpressions(mewpipeVideo);
					Console.WriteLine(" Updated !");
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