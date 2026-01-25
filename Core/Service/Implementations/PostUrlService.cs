using Microsoft.Extensions.Options;
using ServiceAbstraction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Common;

namespace Service.Implementations
{
    public class PostUrlService(IOptions<AppSettings> options) : IPostUrlService
    {
        private readonly AppSettings _settings = options.Value;
        public string BuildPostUrl(int postId)
        => $"{_settings.BaseUrl}/posts/{postId}";
    }
}
