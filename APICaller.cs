using Microsoft.AspNetCore.Mvc;
using System;

namespace odgaspcore6api
{
	public class APICallerService
	{
		private readonly HttpClient _httpClient;

		public APICallerService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<string> GetExternalDataAsync(string url)
		{
			var response = await _httpClient.GetAsync(url);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> GetPagedDataAsync(string url, int page, int pageSize)
		{
			var formattedUrl = $"{url}?page={page}&pageSize={pageSize}";
			var response = await _httpClient.GetAsync(formattedUrl);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
	}
}