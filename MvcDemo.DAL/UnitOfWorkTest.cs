using System;

namespace MvcDemo.DAL
{
    public class UnitOfWorkTest : IUnitOfWork
    {
        #region Constructors

        public UnitOfWorkTest()
        {
            if (Current == null)
            {
                Current = this;
            }
        }

        #endregion


        #region Properties

        public UnitOfWorkTest Current { get; private set; }

        #endregion


        #region Repositories

        public IPersonRepository PersonRepository { get; set; }

        #endregion


        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
