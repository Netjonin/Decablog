using DecaBlog.Models.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DecaBlog.Services.Interfaces
{
  public  interface ICloudinaryService
    {
        Task<PhotoUploadResult> UploadPhoto(IFormFile photo);
        Task<bool> DeletePhoto(string id);
    }
}
