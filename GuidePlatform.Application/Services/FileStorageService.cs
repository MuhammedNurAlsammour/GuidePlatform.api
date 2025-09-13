using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Services
{
  /// <summary>
  /// Dosya depolama servisi - File storage service
  /// wwwroot klasöründe dosyaları yönetir
  /// </summary>
  public interface IFileStorageService
  {
    /// <summary>
    /// Resmi wwwroot'a kaydeder ve URL döndürür - Saves image to wwwroot and returns URL
    /// </summary>
    Task<TransactionResultPack<string>> SaveImageAsync(byte[] imageData, string fileName, string subFolder = "images");

    /// <summary>
    /// Resmi siler - Deletes image
    /// </summary>
    Task<TransactionResultPack<bool>> DeleteImageAsync(string filePath);

    /// <summary>
    /// Resim URL'ini oluşturur - Creates image URL
    /// </summary>
    string CreateImageUrl(string fileName, string subFolder = "images");

    /// <summary>
    /// Dosya yolunu oluşturur - Creates file path
    /// </summary>
    string CreateFilePath(string fileName, string subFolder = "images");
  }

  public class FileStorageService : IFileStorageService
  {
    private readonly string _webRootPath;
    private readonly string _baseUrl;

    public FileStorageService(string webRootPath, string baseUrl)
    {
      _webRootPath = webRootPath ?? throw new ArgumentNullException(nameof(webRootPath));
      _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
    }

    public async Task<TransactionResultPack<string>> SaveImageAsync(byte[] imageData, string fileName, string subFolder = "images")
    {
      try
      {
        // Dosya adını doğrula - Validate file name
        if (string.IsNullOrEmpty(fileName))
        {
          return ResultFactory.CreateErrorResult<string>(
              null,
              null,
              "Hata / Geçersiz Dosya Adı",
              "Dosya adı boş olamaz.",
              "File name cannot be empty."
          );
        }

        // Güvenli dosya adı oluştur - Create safe file name
        var safeFileName = CreateSafeFileName(fileName);
        var filePath = CreateFilePath(safeFileName, subFolder);
        var directoryPath = Path.GetDirectoryName(filePath);

        // Klasörü oluştur - Create directory
        if (!Directory.Exists(directoryPath))
        {
          Directory.CreateDirectory(directoryPath);
        }

        // Dosyayı kaydet - Save file
        await File.WriteAllBytesAsync(filePath, imageData);

        // URL oluştur - Create URL
        var imageUrl = CreateImageUrl(safeFileName, subFolder);

        return ResultFactory.CreateSuccessResult<string>(
            imageUrl,
            null,
            null,
            "İşlem Başarılı",
            "Resim başarıyla kaydedildi.",
            $"Resim {safeFileName} başarıyla kaydedildi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<string>(
            null,
            null,
            "Hata / Resim Kaydetme",
            "Resim kaydedilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    public async Task<TransactionResultPack<bool>> DeleteImageAsync(string filePath)
    {
      try
      {
        if (string.IsNullOrEmpty(filePath))
        {
          return ResultFactory.CreateErrorResult<bool>(
              null,
              null,
              "Hata / Geçersiz Dosya Yolu",
              "Dosya yolu boş olamaz.",
              "File path cannot be empty."
          );
        }

        // wwwroot içinde mi kontrol et - Check if inside wwwroot
        var fullPath = Path.GetFullPath(filePath);
        var webRootFullPath = Path.GetFullPath(_webRootPath);

        if (!fullPath.StartsWith(webRootFullPath))
        {
          return ResultFactory.CreateErrorResult<bool>(
              null,
              null,
              "Hata / Güvenlik",
              "Dosya yolu güvenli değil.",
              "File path is not secure."
          );
        }

        if (File.Exists(fullPath))
        {
          File.Delete(fullPath);
        }

        return ResultFactory.CreateSuccessResult<bool>(
            true,
            null,
            null,
            "İşlem Başarılı",
            "Dosya başarıyla silindi.",
            $"Dosya {filePath} başarıyla silindi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<bool>(
            null,
            null,
            "Hata / Dosya Silme",
            "Dosya silinirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    public string CreateImageUrl(string fileName, string subFolder = "images")
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException("Dosya adı boş olamaz.", nameof(fileName));

      var safeFileName = CreateSafeFileName(fileName);
      var relativePath = $"{subFolder.TrimStart('/').TrimEnd('/')}/{safeFileName}";
      return $"{_baseUrl.TrimEnd('/')}/{relativePath}";
    }

    public string CreateFilePath(string fileName, string subFolder = "images")
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException("Dosya adı boş olamaz.", nameof(fileName));

      var safeFileName = CreateSafeFileName(fileName);
      var relativePath = Path.Combine(subFolder.TrimStart('/').TrimEnd('/'), safeFileName);
      return Path.Combine(_webRootPath, relativePath);
    }

    /// <summary>
    /// Güvenli dosya adı oluşturur - Creates safe file name
    /// </summary>
    private string CreateSafeFileName(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return Guid.NewGuid().ToString();

      // Dosya uzantısını al - Get file extension
      var extension = Path.GetExtension(fileName);
      var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

      // Güvenli karakterlerle değiştir - Replace with safe characters
      var safeName = System.Text.RegularExpressions.Regex.Replace(
          nameWithoutExtension,
          @"[^a-zA-Z0-9\-_]",
          "_"
      );

      // GUID ekle (benzersizlik için) - Add GUID for uniqueness
      var uniqueName = $"{safeName}_{Guid.NewGuid():N}";

      return $"{uniqueName}{extension}";
    }
  }
}
