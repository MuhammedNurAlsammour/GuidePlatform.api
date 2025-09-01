namespace GuidePlatform.Application.Abstractions.RabbitMQ
{
	/// <summary>
	/// RabbitMQ mesaj gönderme işlemleri için servis interface
	/// </summary>
	public interface IRabbitMQService
	{
		/// <summary>
		/// Belirtilen kuyruğa mesaj gönderir
		/// </summary>
		/// <param name="queueName">Kuyruk adı</param>
		/// <param name="message">Gönderilecek mesaj</param>
		/// <returns>İşlem sonucu</returns>
		Task<string> SendMessage(string queueName, string message);
	}
}
