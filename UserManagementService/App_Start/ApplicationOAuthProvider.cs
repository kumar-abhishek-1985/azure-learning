using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagementDB;
using System.Linq;

namespace UserManagementService
{
    //public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    //{
    //    private readonly string _publicClientId;

    //    public ApplicationOAuthProvider(string publicClientId)
    //    {
    //        if (publicClientId == null)
    //        {
    //            throw new ArgumentNullException("publicClientId");
    //        }

    //        _publicClientId = publicClientId;
    //    }

    //    public override async Task GrantResourceOwnerCredentials
    //    (OAuthGrantResourceOwnerCredentialsContext context)
    //    {
    //        //**Replace below user authentication code as per your Entity Framework Model ***
    //        using (var obj = new UserManagementDBEntities())
    //        {

    //            var entry = obj.UserManagements.FirstOrDefault(item => item.Username == context.UserName && item.Password == context.Password);


    //            if (entry == null)
    //            {
    //                context.SetError("invalid_grant",
    //                "The user name or password is incorrect.");
    //                return;
    //            }
    //        }


    //        ClaimsIdentity oAuthIdentity =
    //        new ClaimsIdentity(context.Options.AuthenticationType);
    //        ClaimsIdentity cookiesIdentity =
    //        new ClaimsIdentity(context.Options.AuthenticationType);

    //        var claims = new List<Claim>();
    //        claims.Add(new Claim(ClaimTypes.Name, context.UserName));
    //        claims.Add(new Claim(ClaimTypes.Role, "Normal"));

    //        AuthenticationProperties properties = CreateProperties(context.UserName);
    //        AuthenticationTicket ticket =
    //        new AuthenticationTicket(oAuthIdentity, properties);
    //        context.Validated(ticket);
    //        context.Request.Context.Authentication.SignIn(cookiesIdentity);
    //    }

    //    public override Task TokenEndpoint(OAuthTokenEndpointContext context)
    //    {
    //        foreach (KeyValuePair<string,string> property in context.Properties.Dictionary)
    //        {
    //            context.AdditionalResponseParameters.Add(property.Key, property.Value);
    //        }

    //        return Task.FromResult<object>(null);
    //    }

    //    public override Task ValidateClientAuthentication
    //    (OAuthValidateClientAuthenticationContext context)
    //    {
    //        // Resource owner password credentials does not provide a client ID.
    //        if (context.ClientId == null)
    //        {
    //            context.Validated();
    //        }

    //        return Task.FromResult <object> (null);
    //    }

    //    public override Task ValidateClientRedirectUri
    //    (OAuthValidateClientRedirectUriContext context)
    //    {
    //        if (context.ClientId == _publicClientId)
    //        {
    //            Uri expectedRootUri = new Uri(context.Request.Uri, "/");

    //            if (expectedRootUri.AbsoluteUri == context.RedirectUri)
    //            {
    //                context.Validated();
    //            }
    //        }

    //        return Task.FromResult <object> (null);
    //    }

    //    public static AuthenticationProperties CreateProperties(string userName)
    //    {
    //        IDictionary <string, string>
    //           data = new Dictionary<string, string>
    //        {
    //            { "userName", userName }
    //        };
    //        return new AuthenticationProperties(data);
    //    }
    //}
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            using (var obj = new UserManagementDBEntities())
            {
                var entry = obj.UserManagements.FirstOrDefault(item => item.Username == context.UserName && item.Password == context.Password);
                if (entry == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, entry.Id.ToString()));
                context.Validated(identity);
            }
        }
    }
}