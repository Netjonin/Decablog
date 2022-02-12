using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DecaBlog.Configurations;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DecaBlog.Services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private Cloudinary _cloud;
        public CloudinaryService(IOptions<CloudinarySettings> cloudSettings)
        {
            Account cloudAccount = new Account
            {
                ApiKey = cloudSettings.Value.ApiKey,
                ApiSecret = cloudSettings.Value.ApiSecret,
                Cloud = cloudSettings.Value.CloudName
            };
            _cloud = new Cloudinary(cloudAccount);
        }

        public async Task<bool> DeletePhoto(string id)
        {
            var deletionParams = new DeletionParams(id);
            deletionParams.ResourceType = ResourceType.Image;
            var delRes = await _cloud.DestroyAsync(deletionParams);
            if (delRes.StatusCode == System.Net.HttpStatusCode.OK && delRes.Result.ToLower() == "ok")
                return true;
            return false;
        }

        public async Task<PhotoUploadResult> UploadPhoto(IFormFile photo)
        {
            var imageUploadParams = new ImageUploadParams
            {
                File = new FileDescription(photo.FileName, photo.OpenReadStream()),
                Transformation = new Transformation().Width(300).Height(300).Gravity("faces").Crop("fill")
            };
            var res = await _cloud.UploadAsync(imageUploadParams);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
                return new PhotoUploadResult { PublicId = res.PublicId, Url = res.Url.ToString() };
            return null;
        }
    }
}
