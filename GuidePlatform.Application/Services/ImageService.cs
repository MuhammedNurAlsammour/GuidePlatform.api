using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Services
{
	public class ImageUploadDto
	{
		public string? PhotoPath { get; set; }
		public string? PhotoBase64 { get; set; }
	}

	public interface IImageService
	{
		Task<(byte[] originalImage, byte[] thumbnail)> ProcessImageAsync(ImageUploadDto imageUploadDto);
		string GetBase64Image(byte[] imageData, string contentType = "image/jpeg");
	}

	public class ImageService : IImageService
	{
		private const int MaxPhotoSizeBytes = 2 * 1024 * 1024; // 2 MB
		private const int ThumbnailSize = 100; // 100x100 piksel

		public async Task<(byte[] originalImage, byte[] thumbnail)> ProcessImageAsync(ImageUploadDto imageUploadDto)
		{
			try
			{
				// التحقق من وجود بيانات الصورة
				if (string.IsNullOrEmpty(imageUploadDto.PhotoPath) && string.IsNullOrEmpty(imageUploadDto.PhotoBase64))
				{
					throw new ArgumentException("Fotoğraf verisi bulunamadı! PhotoPath veya PhotoBase64 alanlarından en az biri doldurulmalıdır.");
				}

				// إعطاء الأولوية لـ PhotoBase64 إذا كان كلاهما موجودين
				if (!string.IsNullOrEmpty(imageUploadDto.PhotoBase64))
				{
					// استخدام PhotoBase64 فقط
					imageUploadDto.PhotoPath = null;
				}
				else if (!string.IsNullOrEmpty(imageUploadDto.PhotoPath))
				{
					// استخدام PhotoPath فقط
					imageUploadDto.PhotoBase64 = null;
				}

				// التحقق من طول البيانات
				if (!string.IsNullOrEmpty(imageUploadDto.PhotoPath) && imageUploadDto.PhotoPath.Length > 2000)
				{
					throw new ArgumentException("PhotoPath çok uzun! Maksimum 2000 karakter olabilir.");
				}

				if (!string.IsNullOrEmpty(imageUploadDto.PhotoBase64) && imageUploadDto.PhotoBase64.Length > 50 * 1024 * 1024)
				{
					throw new ArgumentException("PhotoBase64 çok büyük! Maksimum 50MB olabilir.");
				}

				byte[] photoData = await ReadPhoto(imageUploadDto);
				ValidatePhotoSize(photoData, MaxPhotoSizeBytes);

				byte[] thumbnailData = await GenerateThumbnailAsync(photoData);

				return (photoData, thumbnailData);
			}
			catch (Exception ex)
			{
				throw new Exception($"Fotoğraf işlenirken hata oluştu: {ex.Message}", ex);
			}
		}

		public string GetBase64Image(byte[] imageData, string contentType = "image/jpeg")
		{
			if (imageData == null || imageData.Length == 0)
			{
				throw new ArgumentException("Resim verisi boş olamaz!");
			}

			string base64Image = Convert.ToBase64String(imageData);
			return $"data:{contentType};base64,{base64Image}";
		}

		private async Task<byte[]> ReadPhoto(ImageUploadDto dto)
		{
			try
			{
				if (Uri.TryCreate(dto.PhotoPath, UriKind.Absolute, out Uri uriResult) &&
					(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) &&
					dto.PhotoPath is not null)
				{
					using var httpClient = new HttpClient();
					httpClient.Timeout = TimeSpan.FromSeconds(30); // 30 saniye timeout
					try
					{
						var response = await httpClient.GetAsync(dto.PhotoPath);
						if (!response.IsSuccessStatusCode)
						{
							throw new FileNotFoundException($"Resim URL'den yüklenemedi. HTTP Status: {response.StatusCode} - {dto.PhotoPath}");
						}

						var contentType = response.Content.Headers.ContentType?.MediaType;
						if (contentType == null || !contentType.StartsWith("image/"))
						{
							throw new ArgumentException($"Geçersiz içerik türü: {contentType}. Sadece resim dosyaları desteklenir.");
						}

						return await response.Content.ReadAsByteArrayAsync();
					}
					catch (HttpRequestException ex)
					{
						throw new FileNotFoundException($"Resim URL'den yüklenemedi: {dto.PhotoPath}", dto.PhotoPath, ex);
					}
					catch (TaskCanceledException ex)
					{
						throw new FileNotFoundException($"Resim URL'den yükleme zaman aşımına uğradı: {dto.PhotoPath}", dto.PhotoPath, ex);
					}
				}
				else if (dto.PhotoPath is not null)
				{
					// إذا كان PhotoPath موجود ولكن ليس URL صحيح
					throw new ArgumentException($"Geçersiz PhotoPath formatı: {dto.PhotoPath}. Geçerli bir HTTP/HTTPS URL olmalıdır.");
				}
				else if (dto.PhotoBase64 is not null)
				{
					try
					{
						var base64Data = dto.PhotoBase64.Contains(",")
							? dto.PhotoBase64.Substring(dto.PhotoBase64.IndexOf(",") + 1)
							: dto.PhotoBase64;

						if (string.IsNullOrEmpty(base64Data))
						{
							throw new ArgumentException("Base64 verisi boş olamaz!");
						}

						if (base64Data.Length % 4 != 0)
						{
							throw new ArgumentException("Geçersiz Base64 formatı: Uzunluk 4'ün katı olmalıdır.");
						}

						return Convert.FromBase64String(base64Data);
					}
					catch (FormatException ex)
					{
						throw new ArgumentException($"Geçersiz Base64 formatı: {ex.Message}", ex);
					}
				}

				throw new FileNotFoundException("Fotoğraf verisi bulunamadı! PhotoPath veya PhotoBase64 alanlarından en az biri geçerli olmalıdır.");
			}
			catch (Exception ex)
			{
				throw new Exception($"Fotoğraf okunurken hata oluştu: {ex.Message}", ex);
			}
		}

		private static void ValidatePhotoSize(byte[] photoData, int maxPhotoSizeBytes)
		{
			if (photoData == null || photoData.Length == 0)
			{
				throw new ArgumentException("Fotoğraf verisi boş olamaz!");
			}

			if (photoData.Length > maxPhotoSizeBytes)
			{
				var currentSizeMB = photoData.Length / (1024.0 * 1024.0);
				var maxSizeMB = maxPhotoSizeBytes / (1024.0 * 1024.0);
				throw new ArgumentException($"Fotoğrafın maksimum boyutu {maxSizeMB:F1} MB olabilir! Mevcut boyut: {currentSizeMB:F2} MB");
			}

			// التحقق من الحد الأدنى للحجم
			if (photoData.Length < 100)
			{
				throw new ArgumentException("Fotoğraf çok küçük! Minimum 100 byte olmalıdır.");
			}
		}

		private async Task<byte[]> GenerateThumbnailAsync(byte[] originalImage)
		{
			try
			{
				if (originalImage == null || originalImage.Length == 0)
				{
					throw new ArgumentException("Orijinal resim verisi boş olamaz!");
				}

				using (var image = Image.Load(originalImage))
				{
					// التحقق من أبعاد الصورة
					if (image.Width < 10 || image.Height < 10)
					{
						throw new ArgumentException($"Resim çok küçük! Minimum boyut: 10x10 piksel. Mevcut boyut: {image.Width}x{image.Height} piksel");
					}

					image.Mutate(x => x
						.Resize(new ResizeOptions
						{
							Size = new Size(ThumbnailSize, ThumbnailSize),
							Mode = ResizeMode.Max
						}));

					using (var ms = new MemoryStream())
					{
						await image.SaveAsJpegAsync(ms);
						var thumbnailData = ms.ToArray();

						if (thumbnailData.Length == 0)
						{
							throw new Exception("Küçük resim oluşturulamadı!");
						}

						return thumbnailData;
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Küçük resim oluşturulurken hata oluştu: {ex.Message}", ex);
			}
		}
	}
}
