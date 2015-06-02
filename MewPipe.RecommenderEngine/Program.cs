using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            var pearsonEngine = new PearsonScoreEngine();
            
            var mewPipeDataSource = new MewPipeDataSource();

            var realData = mewPipeDataSource.GetData();

            var data = GenerateFakeData();

            var results = pearsonEngine.GetTopMatches(data, "Superman Returns").ToArray();

            var mostInteresting = results[0];

            Console.Out.WriteLine(results);
        }

        private static Dictionary<string, VideoUserRatingData[]> GenerateFakeData()
        {
            var data = new Dictionary<string, VideoUserRatingData[]>
            {
                {
                    "Just My Luck", new[]
                    {
                        new VideoUserRatingData
                        {
                            UserId = "Lisa Rose",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Mick LaSalle",
                            Rating = 2.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Claudia Puig",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Gene Seymour",
                            Rating = 1.5,
                        }
                    }
                },
                {
                    "Lady in the Water", new[]
                    {
                        new VideoUserRatingData
                        {
                            UserId = "Mick LaSalle",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Lisa Rose",
                            Rating = 2.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Michael Phillips",
                            Rating = 2.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Jack Matthews",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Gene Seymour",
                            Rating = 3.0,
                        }
                    }
                },
                {
                    "The Night Listener", new[]
                    {
                        new VideoUserRatingData
                        {
                            UserId = "Mick LaSalle",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Claudia Puig",
                            Rating = 4.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Michael Phillips",
                            Rating = 4.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Lisa Rose",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Gene Seymour",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Jack Matthews",
                            Rating = 3.0,
                        }
                    }
                },
                {
                    "Superman Returns", new[]
                    {
                        new VideoUserRatingData
                        {
                            UserId = "Mick LaSalle",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Claudia Puig",
                            Rating = 4.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Michael Phillips",
                            Rating = 3.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Lisa Rose",
                            Rating = 3.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Toby",
                            Rating = 4.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Gene Seymour",
                            Rating = 5.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Jack Matthews",
                            Rating = 5.0,
                        }
                    }
                },
                {
                    "You, Me and Dupree", new[]
                    {
                        new VideoUserRatingData
                        {
                            UserId = "Mick LaSalle",
                            Rating = 2.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Claudia Puig",
                            Rating = 2.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Lisa Rose",
                            Rating = 2.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Toby",
                            Rating = 1.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Gene Seymour",
                            Rating = 3.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Jack Matthews",
                            Rating = 3.5,
                        }
                    }
                },
                {
                    "Snakes on a Plane", new[]
                    {
                        new VideoUserRatingData
                        {
                            UserId = "Mick LaSalle",
                            Rating = 4.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Claudia Puig",
                            Rating = 3.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Michael Phillips",
                            Rating = 3.0,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Lisa Rose",
                            Rating = 3.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Toby",
                            Rating = 4.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Gene Seymour",
                            Rating = 3.5,
                        },
                        new VideoUserRatingData
                        {
                            UserId = "Jack Matthews",
                            Rating = 4.0,
                        }
                    }
                }
            };


            return data;
        }
    }
}
