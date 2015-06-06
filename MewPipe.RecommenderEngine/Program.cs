using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                ProcessRecommendations();
            }
        }

        private static void ProcessRecommendations()
        {
            var watch = Stopwatch.StartNew();
            var pearsonEngine = new PearsonScoreEngine();
            var contentEngine = new ContentScoreEngine();
            var mewPipeDataSource = new MewPipeDataSource();

            //1 - RECUPERATION
            var realData = mewPipeDataSource.GetData();

            //2 - RESULTS
            foreach (var video in realData.VideoRatingDatas)
            {
                var finalArray = new KeyValuePair<string, double>[0];

                var socialResults = pearsonEngine.GetTopMatches(realData.VideoUserRatingDatas,
                    realData.VideoRatingDatas.First(r => r.Value.VideoId == video.Key).Value);
                var contentResults = contentEngine.GetTopMatches(realData.VideoRatingDatas,
                    realData.VideoRatingDatas.First(r => r.Value.VideoId == video.Key).Value);

                if (contentResults.Count > 0)
                {
                    finalArray = contentResults.ToArray();
                }

                //3 - COMBINATION
                //TODO

                mewPipeDataSource.SaveData(video.Key, finalArray);
            }

            watch.Stop();
            Console.Out.WriteLine("[INFO][" + DateTime.Now.ToLongDateString() + "] Recommendations database updated in " + watch.ElapsedMilliseconds + " ms");
        }

        private static DataSourceResult GenerateFakeData()
        {

            var data = new DataSourceResult
            {
                VideoUserRatingDatas = new Dictionary<string, VideoUserRatingData[]>
                {
                    {
                        "Just My Luck", new[]
                        {
                            new VideoUserRatingData
                            {
                                UserId = "Lisa Rose",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Mick LaSalle",
                                SocialRating = 2.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Claudia Puig",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Gene Seymour",
                                SocialRating = 1.5,
                            }
                        }
                    },
                    {
                        "Lady in the Water", new[]
                        {
                            new VideoUserRatingData
                            {
                                UserId = "Mick LaSalle",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Lisa Rose",
                                SocialRating = 2.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Michael Phillips",
                                SocialRating = 2.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Jack Matthews",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Gene Seymour",
                                SocialRating = 3.0,
                            }
                        }
                    },
                    {
                        "The Night Listener", new[]
                        {
                            new VideoUserRatingData
                            {
                                UserId = "Mick LaSalle",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Claudia Puig",
                                SocialRating = 4.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Michael Phillips",
                                SocialRating = 4.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Lisa Rose",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Gene Seymour",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Jack Matthews",
                                SocialRating = 3.0,
                            }
                        }
                    },
                    {
                        "Superman Returns", new[]
                        {
                            new VideoUserRatingData
                            {
                                UserId = "Mick LaSalle",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Claudia Puig",
                                SocialRating = 4.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Michael Phillips",
                                SocialRating = 3.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Lisa Rose",
                                SocialRating = 3.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Toby",
                                SocialRating = 4.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Gene Seymour",
                                SocialRating = 5.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Jack Matthews",
                                SocialRating = 5.0,
                            }
                        }
                    },
                    {
                        "You, Me and Dupree", new[]
                        {
                            new VideoUserRatingData
                            {
                                UserId = "Mick LaSalle",
                                SocialRating = 2.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Claudia Puig",
                                SocialRating = 2.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Lisa Rose",
                                SocialRating = 2.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Toby",
                                SocialRating = 1.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Gene Seymour",
                                SocialRating = 3.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Jack Matthews",
                                SocialRating = 3.5,
                            }
                        }
                    },
                    {
                        "Snakes on a Plane", new[]
                        {
                            new VideoUserRatingData
                            {
                                UserId = "Mick LaSalle",
                                SocialRating = 4.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Claudia Puig",
                                SocialRating = 3.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Michael Phillips",
                                SocialRating = 3.0,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Lisa Rose",
                                SocialRating = 3.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Toby",
                                SocialRating = 4.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Gene Seymour",
                                SocialRating = 3.5,
                            },
                            new VideoUserRatingData
                            {
                                UserId = "Jack Matthews",
                                SocialRating = 4.0,
                            }
                        }
                    }
                },
                VideoRatingDatas = new Dictionary<string, VideoRatingData>
                {
                    {
                        "Just My Luck", new VideoRatingData
                        {
                            Title = "Just My Luck",
                            Category = "Movie",
                            UploaderId = "alexis",
                            Description = "Manhattanite Ashley is known to many as the luckiest woman around. After a chance encounter with a down-and-out young man, however, she realizes that she's swapped her fortune for his.",
                            VideoId = "Just My Luck",
                            Tags = new []
                            {
                                "Comedy",
                                "Fantasy",
                                "Romance"
                            }
                        }
                    },
                    {
                        "Lady in the Water", new VideoRatingData
                        {
                            Title = "Lady in the Water",
                            Category = "Movie",
                            UploaderId = "alexis",
                            Description = "Apartment building superintendent Cleveland Heep rescues what he thinks is a young woman from the pool he maintains. When he discovers that she is actually a character from a bedtime story who is trying to make the journey back to her home, he works with his tenants to protect his new friend from the creatures that are determined to keep her in our world.",
                            VideoId = "Lady in the Water",
                            Tags = new []
                            {
                                "Fantasy",
                                "Mistery",
                                "Thriller"
                            }
                        }
                    },
                    {
                        "The Night Listener", new VideoRatingData
                        {
                            Title = "The Night Listener",
                            Category = "Movie",
                            UploaderId = "alexis",
                            Description = "In the midst of his crumbling relationship, a radio show host begins speaking to his biggest fan, a young boy, via the telephone. But when questions about the boy's identity come up, the host's life is thrown into chaos.",
                            VideoId = "The Night Listener",
                            Tags = new []
                            {
                                "Crime",
                                "Mistery",
                                "Thriller"
                            }
                        }
                    },
                    {
                        "Superman Returns", new VideoRatingData
                        {
                            Title = "Superman Returns",
                            Category = "Movie",
                            UploaderId = "alexis",
                            Description = "Superman reappears after a long absence, but is challenged by an old foe who uses Kryptonian technology for world domination.",
                            VideoId = "Superman Returns",
                            Tags = new []
                            {
                                "Action",
                                "Adventure",
                                "Fantasy"
                            }
                        }
                    },
                    {
                        "You, Me and Dupree", new VideoRatingData
                        {
                            Title = "You, Me and Dupree",
                            Category = "Movie",
                            UploaderId = "jack",
                            Description = "A best man (Wilson) stays on as a houseguest with the newlyweds, much to the couple's annoyance.",
                            VideoId = "You, Me and Dupree",
                            Tags = new []
                            {
                                "Comedy",
                                "Romance"
                            }
                        }
                    },
                    {
                        "Snakes on a Plane", new VideoRatingData
                        {
                            Title = "Snakes on a Plane",
                            Category = "Movie",
                            UploaderId = "jack",
                            Description = "An FBI agent takes on a plane full of deadly and poisonous snakes, deliberately released to kill a witness being flown from Honolulu to Los Angeles to testify against a mob boss.",
                            VideoId = "Snakes on a Plane",
                            Tags = new []
                            {
                                "Action",
                                "Thriller"
                            }
                        }
                    }
                }
            };



            return data;
        }
    }
}
