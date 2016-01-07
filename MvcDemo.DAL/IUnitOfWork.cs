
namespace MvcDemo.DAL
{
    public interface IUnitOfWork
    {
        void Save();
        IPersonRepository PersonRepository { get; }
    }
}
