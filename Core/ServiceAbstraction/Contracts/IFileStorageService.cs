using Microsoft.AspNetCore.Http;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(IFormFile file, MediaType type);
        Task DeleteAsync(string relativePath);

    }
}
