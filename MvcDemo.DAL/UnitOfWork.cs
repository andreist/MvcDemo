using System;
using System.Web;
using NLog;

namespace MvcDemo.DAL
{
    public class UnitOfWork : IDisposable, IUnitOfWorks
    {
        #region Constructors

        public UnitOfWork()
        {
            if (Current == null)
            {
                UnitOfWorkId = new Guid();
                Current = this;
                Logger.Log(LogLevel.Info, "new UnitOfWork " + UnitOfWorkId);
            }
        }

        #endregion


        #region Properties

        // ===  Dynamic Properties  ===

        private Boolean _isDisposed = false;

        public Guid UnitOfWorkId
        {
            get; 
            private set;
        }

        private Lazy<MvcDemoEntities> _entities = new Lazy<MvcDemoEntities>();
        public MvcDemoEntities Entities
        {
            get { return _entities.Value; }
        }

        private Lazy<Logger> _logger = new Lazy<Logger>(LogManager.GetCurrentClassLogger);
        public Logger Logger
        {
            get { return _logger.Value; }
        }

        // ===  Static Properties  ===

        [ThreadStatic]
        private static UnitOfWork _threadStaticCurrent;

        private static string _currentKey = typeof(UnitOfWork).FullName + "~Current";

        public static UnitOfWork Current
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[_currentKey] as UnitOfWork;
                }
                return _threadStaticCurrent;
            }
            private set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items[_currentKey] = value;
                else
                    _threadStaticCurrent = value;
            }
        }

        #endregion


        #region Events

        public event EventHandler<EventArgs> Disposing;
        public event EventHandler<EventArgs> Disposed;

        #endregion


        #region Methods

        /// <summary>Save changes to the database.</summary>
        public void Save()
        {
            Entities.SaveChanges();
        }

        protected virtual void OnDisposing(EventArgs e)
        {
            if (Disposing != null)
            {
                Disposing.Invoke(this, e);
            }
        }
        protected virtual void OnDisposed(EventArgs e)
        {
            if (Disposed != null)
            {
                Disposed.Invoke(this, e);
            }
        }

        /// <summary>
        /// Call the dispose of current scope
        /// </summary>
        /// <exception cref="System.Exception">Occurs when this UnitOfWork is already disposed</exception>
        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                throw new Exception("UnitOfWork has already been disposed");
            }

            OnDisposing(EventArgs.Empty);

            if (_entities.IsValueCreated)
            {
                _entities.Value.Dispose();
            }

            _isDisposed = true;
            OnDisposed(EventArgs.Empty);
        }

        #endregion


        #region Repositories

        private PersonRepository _personRepository;
        public PersonRepository PersonRepository
        {
            get
            {
                if (_personRepository == null)
                    _personRepository = new PersonRepository(Entities);
                return _personRepository;
            }
        }

        #endregion

    }
}
