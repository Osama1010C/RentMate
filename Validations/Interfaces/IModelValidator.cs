namespace RentMateAPI.Validations.Interfaces
{
    public interface IModelValidator<T> where T : class
    {
        Task<bool> IsModelExist(int modelId);
        Task<T> IsModelExistReturn(int modelId);
    }
}
