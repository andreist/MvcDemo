using MvcDemo.DAL;

namespace MvcDemo.BLL
{
    public class GenericBl
    {
        public GenericBl()
        {
            Uow = UnitOfWork.Current;
        }

        public GenericBl(IUnitOfWork uow)
        {
            Uow = uow;
        }

        internal IUnitOfWork Uow;
    }
}
