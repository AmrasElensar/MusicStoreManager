using MusicStoreManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web;
using MusicStoreManager.Models;
using Marvin.StreamExtensions;
using MusicStoreManager.Entities;

namespace MusicStoreManager.ApiClient
{
    public partial class MusicStoreApiClient
    {
        private HttpClient _httpClient;
        private string _baseUrl = "";

        public MusicStoreApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            BaseUrl = "http://localhost:52757";
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();

        }

        private string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }


        public async Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync(int? pageNumber, int? pageSize, string genre, string searchQuery, string orderBy, string fields, string accept)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/albums?");
            if (pageNumber != null)
            {
                urlBuilder_.Append("PageNumber=").Append(System.Uri.EscapeDataString(ConvertToString(pageNumber, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (pageSize != null)
            {
                urlBuilder_.Append("PageSize=").Append(System.Uri.EscapeDataString(ConvertToString(pageSize, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (genre != null)
            {
                urlBuilder_.Append("Genre=").Append(System.Uri.EscapeDataString(ConvertToString(genre, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (searchQuery != null)
            {
                urlBuilder_.Append("SearchQuery=").Append(System.Uri.EscapeDataString(ConvertToString(searchQuery, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (orderBy != null)
            {
                urlBuilder_.Append("OrderBy=").Append(System.Uri.EscapeDataString(ConvertToString(orderBy, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (fields != null)
            {
                urlBuilder_.Append("Fields=").Append(System.Uri.EscapeDataString(ConvertToString(fields, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            using (var request_ = new HttpRequestMessage())
            {
                request_.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request_.Method = new HttpMethod("GET");
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

                var stream = await response_.Content.ReadAsStreamAsync();
                response_.EnsureSuccessStatusCode();

                return stream.ReadAndDeserializeFromJson<List<AlbumDto>>();
            }

        }


       

        public async Task<AlbumDto> GetAlbumAsync(int id, int artistId)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/albums/artist/{artistId}/{id}?");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
            urlBuilder_.Replace("{artistId}", System.Uri.EscapeDataString(ConvertToString(artistId, System.Globalization.CultureInfo.InvariantCulture)));
            urlBuilder_.Length--;

            var client_ = _httpClient;

            using (var request_ = new HttpRequestMessage())
            {
                request_.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request_.Method = new HttpMethod("GET");
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);

                var response_ = await client_.SendAsync(request_, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

                var stream = await response_.Content.ReadAsStreamAsync();
                response_.EnsureSuccessStatusCode();

                return stream.ReadAndDeserializeFromJson<AlbumDto>();

            }
        }

        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is System.Enum)
            {
                string name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute))
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
                }
            }
            else if (value is bool)
            {
                return Convert.ToString(value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[])value);
            }
            else if (value != null && value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((Array)value);
                return string.Join(",", Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }

            return Convert.ToString(value, cultureInfo);
        }
    }
}
