
namespace MvcDemo.DAL
{
    public interface IUnitOfWorks
    {
        void Save();
        PersonRepository PersonRepository { get; }
    }
}
