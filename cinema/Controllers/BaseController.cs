using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using WebApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace WebApplication.Controllers
{
    public class BaseController : Controller
    {
        protected DbAccess _dbAccess = null;
        protected GlobalOption _globalOption = null;
        private InnerSession _innerSession = null;
        public BaseController(IOptions<GlobalOption> globalOptions)
        {
            _globalOption = globalOptions.Value;
            _dbAccess = new DbAccess(_globalOption);
        }

        public InnerSession Session
        {
            get
            {
                InsureSession();
                return _innerSession;
            }
        }

        private void InsureSession()
        {
            if (_innerSession == null)
            {
                _innerSession = new InnerSession(this.ControllerContext.HttpContext.Session);
            }
        }
    }

    public class InnerSession
    {
        private ISession session = null;
        public InnerSession(ISession session)
        {
            this.session = session;
        }

        public void Clear()
        {
            this.session.Clear();
        }

        public T Get<T>(string key)
        {
            return this.session.GetItem<T>(key);
        }

        public object this[string key]
        {
            get
            {
                return this.session.GetObjectFromJson(key);
            }
            set
            {
                this.session.SetObjectAsJson(key, value);
            }
        }
    }
}