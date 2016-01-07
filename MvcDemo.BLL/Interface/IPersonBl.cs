using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using MvcDemo.Common;


namespace MvcDemo.BLL
{
    public interface IPersonBl
    {
        PersonDto GetById(int id);
        CustomDataSource<PersonDto> BindData(string sidx, string sord, int page, int rows, bool search, string filters);
        IList GetIListWithFirstNames(string firstNameContained);
        IList GetIListWithLastNames(string lastNameContained);
        void DeleteById(int id);
        void Insert(PersonDto personDto);
        void Update(PersonDto personDto);
    }
}
