using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Text;
using System.Security.Cryptography;
using System.Net;

using MvcDemo.BLL;
using MvcDemo.Common;

namespace MvcDemo.Controllers
{
    public class PersonController : BaseController
    {
        public ActionResult Index()
        {
            LoadSettings();
            return View();
        }

        [HttpGet]
        [HandleJsonException]
        public ActionResult BindData(string sidx, string sord, int page, int rows, bool search, string filters)
        {
            BindDataParam = new BindDataParamDto { Sidx = sidx, Sord = sord, Page = page, Rows = rows, Filters = filters };

            Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            Response.Cache.SetMaxAge(new TimeSpan(0));

            // TO DO - Implement a global initialisation
            IPersonBl personBl = new PersonBl();

            CustomDataSource<PersonDto> dataSource = personBl.BindData(sidx, sord, page, rows, search, filters);

            // to be able to use ToString() below which is NOT exist in the LINQ to Entity
            // we should include in queryDetails only the properies which we will use below
            var queryDetails = (from item in dataSource.RecordList
                                select new { item.Id, item.FirstName, item.LastName, item.Age}).ToList();
            var retult = new
            {
                total = (dataSource.TotalRecords + rows - 1) / rows,
                page,
                records = dataSource.TotalRecords,
                rows = (from item in queryDetails
                        select new[] {
                            item.Id + "",
                            item.FirstName,
                            item.LastName,
                            item.Age + ""
                        }).ToList()
            };

            // calculate MD5 from the returned data and use it as ETag
            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(retult);
            byte[] inputBytes = Encoding.ASCII.GetBytes(str);
            byte[] hash = MD5.Create().ComputeHash(inputBytes);
            string newETag = Convert.ToBase64String(hash);
            Response.Cache.SetETag(newETag);
            // compare ETag of the data which already has the client with ETag of response
            string incomingEtag = Request.Headers["If-None-Match"];
            if (String.Compare(incomingEtag, newETag, StringComparison.Ordinal) == 0)
            {
                // we don't need return the data which the client already have
                Response.SuppressContent = true;
                Response.StatusCode = (int)HttpStatusCode.NotModified;
                return null;
            }

            return Json(retult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id != 0)
            {
                IPersonBl personBl = new PersonBl();
                PersonDto personDto = personBl.GetById(id);

                PersonModel personModel = new PersonModel()
                {
                    Id = personDto.Id,
                    FirstName = personDto.FirstName,
                    LastName = personDto.LastName,
                    Age = personDto.Age,
                    Sex = personDto.Sex
                };

                if (personModel != null)
                    return View(personModel);
            }

            return View(new PersonModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonModel personModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IPersonBl personBl = new PersonBl();

                    PersonDto personDto = new PersonDto
                    {
                        Id = personModel.Id,
                        FirstName = personModel.FirstName,
                        LastName = personModel.LastName,
                        Age = personModel.Age,
                        Sex = personModel.Sex
                    };

                    if (personModel.Id == 0)
                        personBl.Insert(personDto);
                    else
                        personBl.Update(personDto);

                    UnitOfWorkBl.Save();

                    return null;
                }
                else
                {
                    return View(personModel);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult UpdateData(string oper, string id)
        {
            if (String.Compare(oper, "del", StringComparison.Ordinal) == 0)
            {
                int iOut = 0;
                if (int.TryParse(id, out iOut))
                {
                    IPersonBl personBl = new PersonBl();
                    personBl.DeleteById(iOut);

                    UnitOfWorkBl.Save();

                    // no exception, the item was successfully deleted
                    return Json(null);
                }
                ArgumentException exDataNotDeleted = new ArgumentException("No data could be deleted", "id");
                throw exDataNotDeleted;
            }
            ArgumentException exUnknownOperation = new ArgumentException("Unknown editing operation", "oper");
            throw exUnknownOperation;
        }

        public ActionResult EditRow(int id)
        {
            return RedirectToAction("Edit", "Person", new { id = id });
        }

        public ActionResult AddNewRow()
        {
            return RedirectToAction("Edit", "Person", new { id = 0 });
        }

        public JsonResult GetFirstNames(string term)
        {
            Response.Cache.SetMaxAge(new TimeSpan(0));

            IPersonBl personBl = new PersonBl();
            return Json(personBl.GetIListWithFirstNames(term),
                        JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastNames(string term)
        {
            Response.Cache.SetMaxAge(new TimeSpan(0));

            IPersonBl personBl = new PersonBl();
            return Json(personBl.GetIListWithLastNames(term),
                        JsonRequestBehavior.AllowGet);
        }

        #region Methods

        private void LoadSettings()
        {
            // Default values
            ViewBag.SectionToDisplayId = 1;
            LoadBindDataParms();
        }

        private void LoadBindDataParms()
        {
            BindDataParamDto bindDataParam = BindDataParam;

            ViewBag.Sortname = bindDataParam.Sidx;
            ViewBag.Sortorder = bindDataParam.Sord;
            ViewBag.Page = bindDataParam.Page;
            ViewBag.RowNum = bindDataParam.Rows;
            ViewBag.Filters = bindDataParam.Filters;
            ViewBag.Search = bindDataParam.Search;
        }

        #endregion


        #region Properties

        private const string BindDataParamExtraStateKey = "BindDataParam";

        private BindDataParamDto BindDataParam
        {
            get
            {
                // Try to get the object from session
                BindDataParamDto bindDataParam = GetExtraState<BindDataParamDto>(BindDataParamExtraStateKey);
                if (bindDataParam == default(BindDataParamDto))
                {
                    // Save the object in session with default values
                    // When we set the values of the object this values are save automatically in the session
                    bindDataParam = new BindDataParamDto { Sidx = "FirstName", Sord = "asc", Page = 1, Rows = 10, Filters = "" };
                    SaveExtraState(BindDataParamExtraStateKey, bindDataParam);
                }
                return bindDataParam;
            }
            set
            {
                SaveExtraState(BindDataParamExtraStateKey, value);
            }
        }

        #endregion

    }
}
