using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.Base
{
	/// <summary>
	/// Base class for all paginated query requests
	/// </summary>
	public abstract class BasePaginatedQueryRequest : BaseQueryRequest
	{
		[Range(0, int.MaxValue, ErrorMessage = "Sayfa numarası sıfır veya pozitif olmalıdır")]
		public int Page { get; set; } = 0;

		[Range(1, 100, ErrorMessage = "Sayfa boyutu 1 ile 100 arasında olmalıdır")]
		public int Size { get; set; } = 10;

		/// <summary>
		/// Geçerli sayfa numarasını döndürür
		/// </summary>
		public int GetValidatedPage()
		{
			return Math.Max(0, Page);
		}

		/// <summary>
		/// Geçerli sayfa boyutunu döndürür
		/// </summary>
		public int GetValidatedSize()
		{
			return Math.Max(1, Math.Min(100, Size));
		}

		/// <summary>
		/// Skip değerini hesaplar (Page * Size)
		/// </summary>
		public int GetSkip()
		{
			return GetValidatedPage() * GetValidatedSize();
		}

		/// <summary>
		/// Take değerini hesaplar (Size)
		/// </summary>
		public int GetTake()
		{
			return GetValidatedSize();
		}

		/// <summary>
		/// AuthUserId'yi GUID olarak döndürür
		/// </summary>
		public Guid? GetAuthUserIdAsGuid()
		{
			return AuthUserId;
		}

		/// <summary>
		/// AuthCustomerId'yi GUID olarak döndürür
		/// </summary>
		public Guid? GetAuthCustomerIdAsGuid()
		{
			return AuthCustomerId;
		}
	}
}
