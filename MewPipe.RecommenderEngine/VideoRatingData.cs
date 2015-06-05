using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public class VideoRatingData
    {
        public bool NotIndexed { get; set; }
        public string VideoId { get; set; }
        public string UploaderId { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }


        public double GetContentScore(VideoRatingData other)
        {
            var score = 0.0;

            double titleScore = LevenshteinDistance(other.Title, Title);
            if (titleScore > 0)
            {
                titleScore = titleScore / (other.Title.Length > Title.Length ? other.Title.Length : Title.Length);
            }
            titleScore = 1 - titleScore;
            score += titleScore;

            double descriptionScore = LevenshteinDistance(other.Description, Description);
            if (descriptionScore > 0)
            {
                descriptionScore = descriptionScore / (other.Description.Length > Description.Length ? other.Description.Length : Description.Length);
            }
            descriptionScore = 1 - descriptionScore;
            score += descriptionScore;

            if (Category.Equals(other.Category))
            {
                score += 1;
            }

            var commonTags = Tags.Count(tag => other.Tags.Contains(tag));
            var maxTags = (Tags.Length > other.Tags.Length ? Tags.Length : other.Tags.Length);
            if (maxTags > 0)
            {
                var tagsScore = commonTags / maxTags;
                if (tagsScore == 1)
                {
                    score += 1;
                }
                else if (tagsScore > 0.5)
                {
                    score += 0.5;
                }
                else if (tagsScore > 0)
                {
                    score += 0.25;
                }   
            }

            if (UploaderId == other.UploaderId)
            {
                score += 1;
            }

            score = score / 5;

            return score;
        }

        public static int LevenshteinDistance(string s, string t)
        {
            // degenerate cases
            if (s == t) return 0;
            if (s.Length == 0) return t.Length;
            if (t.Length == 0) return s.Length;

            // create two work vectors of integer distances
            var v0 = new int[t.Length + 1];
            var v1 = new int[t.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (var i = 0; i < v0.Length; i++)
            {
                v0[i] = i;
            }

            for (var i = 0; i < s.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (var j = 0; j < t.Length; j++)
                {
                    var cost = (s[i] == t[j]) ? 0 : 1;
                    v1[j + 1] = Minimum(v1[j] + 1, v0[j + 1] + 1, v0[j] + cost);
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (var j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[t.Length];
        }

        public static int Minimum(int a, int b, int c)
        {
            var min = a;

            if (b < a)
            {
                min = b;
            }

            if (c < min)
            {
                min = c;
            }

            return min;
        }
    }
}
