using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using UserManagementDB;

namespace UserManagementService.Controllers
{
    [CustomAuthorizeAttribute]
    [RoutePrefix("api/account")]
    public class UserManagementController : ApiController
    {
        private UserManagementDBEntities db = new UserManagementDBEntities();

        // GET: api/UserManagements
        [Route("users")]
        [HttpGet]
        public IQueryable<UserManagement> GetUserManagements()
        {
            return db.UserManagements;
        }

        // GET: api/UserManagements/5
        [ResponseType(typeof(UserManagement))]
        [Route("user")]
        [HttpGet]
        public IHttpActionResult GetUserManagement(int id)
        {
            int userID;
            Int32.TryParse(Request.Properties[ClaimTypes.NameIdentifier].ToString(), out userID);
            if (id != userID)
                return BadRequest("Unauthorized");
            UserManagement userManagement = db.UserManagements.Find(id);
            if (userManagement == null)
            {
                return NotFound();
            }
            return Ok(userManagement);
        }

        // PUT: api/UserManagements/5
        [ResponseType(typeof(void))]
        [Route("user")]
        [HttpPut]
        public IHttpActionResult PutUserManagement(int id, UserManagement userManagement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userManagement.Id)
            {
                return BadRequest();
            }
            int userID;
            Int32.TryParse(Request.Properties[ClaimTypes.NameIdentifier].ToString(), out userID);
            if (id != userID)
                return BadRequest("Un Authorized");


            try
            {
                var userData = db.UserManagements.First(item => item.Id == id);
                userData.FirstName = userManagement.FirstName;
                userData.LastName = userManagement.LastName;
                userData.Password = userManagement.Password;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserManagementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // POST: api/UserManagements
        [Route("user")]
        [AllowAnonymous]
        [ResponseType(typeof(UserManagement))]
        [HttpPost]
        public IHttpActionResult PostUserManagement(UserManagement userManagement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserManagements.Add(userManagement);
            db.SaveChanges();

            return Ok(userManagement);
        }

        // DELETE: api/UserManagements/5
        [ResponseType(typeof(UserManagement))]
        [Route("user")]
        [HttpDelete]
        public IHttpActionResult DeleteUserManagement(int id)
        {
            int userID;
            Int32.TryParse(Request.Properties[ClaimTypes.NameIdentifier].ToString(), out userID);
            if (id != userID)
                return BadRequest("Unauthorized");
            UserManagement userManagement = db.UserManagements.Find(id);
            if (userManagement == null)
            {
                return NotFound();
            }

            db.UserManagements.Remove(userManagement);
            db.SaveChanges();

            return Ok(userManagement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserManagementExists(int id)
        {
            return db.UserManagements.Count(e => e.Id == id) > 0;
        }
    }
    public class CustomAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext)) return;

            ClaimsPrincipal principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            var name = principal.Claims.First(item => item.Type == ClaimTypes.Name).Value;
            actionContext.Request.Properties.Add(ClaimTypes.Name, name);
            var userid = principal.Claims.First(item => item.Type == ClaimTypes.NameIdentifier).Value;
            actionContext.Request.Properties.Add(ClaimTypes.NameIdentifier, userid);
            base.OnAuthorization(actionContext);
        }
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}