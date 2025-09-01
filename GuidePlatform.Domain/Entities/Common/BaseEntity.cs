namespace GuidePlatform.Domain.Entities.Common
{
	public class BaseEntity
	{
		public Guid Id { get; set; }
		public Guid? AuthUserId { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? CreateUserId { get; set; }
		public Guid? UpdateUserId { get; set; }
		public DateTime RowCreatedDate { get; set; }
		virtual public DateTime RowUpdatedDate { get; set; }
		public bool RowIsActive { get; set; }
		public bool RowIsDeleted { get; set; }

		public bool RowActiveAndNotDeleted => RowIsActive && !RowIsDeleted;
		public bool RowIsNotDeleted => !RowIsDeleted;
	}
}
