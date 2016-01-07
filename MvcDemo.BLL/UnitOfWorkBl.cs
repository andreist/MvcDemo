using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MvcDemo.DAL;

namespace MvcDemo.BLL
{
    public class UnitOfWorkBl
    {
        public static void Save()
        {
            UnitOfWork.Current.Save();
        }

        public static void InitUow()
        {
            if (UnitOfWork.Current == null)
            {
                UnitOfWork uow = new UnitOfWork();
            }
        }

        public static void DisposeUow()
        {
            if (UnitOfWork.Current != null)
            {
                UnitOfWork.Current.Dispose();
            }
        }
    }
}
