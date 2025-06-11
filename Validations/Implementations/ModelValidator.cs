using RentMateAPI.Exceptions;
using RentMateAPI.Repositories.Interfaces;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Validations.Implementations
{
    public class ModelValidator<T> : IModelValidator<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public ModelValidator(IRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsModelExist(int modelId)
        {
            var isExist = await _repository.IsExistAsync(modelId);
            if (!isExist)
                throw new NotFoundException($"{typeof(T).Name} with ID {modelId} was not found.");

            return true;
        }

        public async Task<T> IsModelExistReturn(int modelId)
        {
            var model = await _repository.GetByIdAsync(modelId);
            if (model is null)
                throw new NotFoundException($"{typeof(T).Name} with ID {modelId} was not found.");
            return model;
        }
    }
}
