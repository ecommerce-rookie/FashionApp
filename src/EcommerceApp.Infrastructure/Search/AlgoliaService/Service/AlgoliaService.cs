using Algolia.Search.Clients;
using Domain.Repositories.BaseRepositories;
using Infrastructure.Search.AlgoliaService.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Search.AlgoliaService.Service
{
    public class AlgoliaService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SearchClient _clientWrite;
        private readonly SearchClient _clientRead;
        private readonly AlgoliaSetting _algoliaSetting;

        public AlgoliaService(IUnitOfWork unitOfWork, IOptions<AlgoliaSetting> options)
        {
            _unitOfWork = unitOfWork;
            _algoliaSetting = options.Value;
            _clientWrite = new SearchClient(_algoliaSetting.ApplicationId, _algoliaSetting.WriteApiKey);
            _clientRead = new SearchClient(_algoliaSetting.ApplicationId, _algoliaSetting.SearchApiKey);
        }

        public async Task<bool> AddOrUpdateRecord(string name, string objectID, dynamic model)
        {
            try
            {
                // Create a client
                var response = await _clientWrite.AddOrUpdateObjectAsync(
                          name.ToString(),
                          objectID,
                          model
                        );

                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteRecord(string name, string objectID)
        {
            try
            {
                // Create a client
                var response = await _clientWrite.DeleteObjectAsync(
                          name.ToString(),
                          objectID
                        );

                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
