//
// using System.Net.Http.Headers;
// using System.Net.Mime;
//
// namespace AdminWebsite;
//
// public static class AdminWebApi
// {
//     private const string ApiBaseUri = "http://localhost:5282";
//
//     public static HttpClient InitializeClient()
//     {
//         var client = new HttpClient { BaseAddress = new Uri(ApiBaseUri) };
//         client.DefaultRequestHeaders.Clear();
//         client.DefaultRequestHeaders.Accept.Add(
//             new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
//
//         return client;
//     }
// }