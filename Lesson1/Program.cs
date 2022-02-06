using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    /// <summary>
    /// DTO for blog response deserialization
    /// </summary>
    public class BlogResponse
    {
        [JsonPropertyName("userId")]
        public int User { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }

        public BlogResponse() { }
        public override string ToString()
        {
            return $"{User}\r\n{Id}\r\n{Title}\r\n{Body}\r\n\r\n";
        }
    }
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string url = @"https://jsonplaceholder.typicode.com";
        static readonly UriBuilder ub = new UriBuilder(url);
        static async Task<BlogResponse> GetBlogAsync(int id)
        {
            ub.Path = $"posts/{id}";
            try
            {
                string responseBody = await client.GetStringAsync(ub.Uri);
                return JsonSerializer.Deserialize<BlogResponse>(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message :{0} ", ex.Message);
            }
            return null;
        }
        static async Task<IEnumerable<BlogResponse>> GetBlogsInRangeAsync(IEnumerable<int> ids)
        {
            var getBlogs = new List<Task<BlogResponse>>();
            foreach (var id in ids)
            {
                getBlogs.Add(GetBlogAsync(id));
            }
            return await Task.WhenAll(getBlogs);
        }
        static async void SaveResponseAsync(IEnumerable<BlogResponse> bg)
        {
            string text = string.Empty;
            foreach (var blog in bg)
            {
                text += blog.ToString();
            }
            using (StreamWriter sw = File.CreateText(AppDomain.CurrentDomain.BaseDirectory + "result.txt"))
            {
                await sw.WriteLineAsync(text);
            }
        }
        static void Main(string[] args)
        {
            List<int> ids = new List<int>();
            int from = 4;
            int to = 13;
            for (int i = from; i <= to; i++)
            {
                ids.Add(i);
            }
            var blogs = GetBlogsInRangeAsync(ids);
            SaveResponseAsync(blogs.Result);
            Console.ReadLine();
        }

    }
}