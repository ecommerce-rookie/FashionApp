namespace Infrastructure.HttpClients
{
    public interface IHttpService
    {
        Task<string?> UpdateStatusUser(Guid userId, string status);
    }
}
