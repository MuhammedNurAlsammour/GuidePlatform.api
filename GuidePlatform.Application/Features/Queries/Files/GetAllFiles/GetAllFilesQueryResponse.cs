using GuidePlatform.Application.Dtos.ResponseDtos.Files;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Files.GetAllFiles
{
  public class GetAllFilesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<FilesDTO> files { get; set; } = new();
  }
}
