﻿using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    public abstract class EgController : Controller
    {
        public abstract string EgName { get; }
        protected DSConfiguration Config { get; }
        protected IRequestItemsService RequestItemsService { get; }

        public EgController(DSConfiguration config, IRequestItemsService requestItemsService)
        {
            Config = config;
            RequestItemsService = requestItemsService;
            ViewBag.csrfToken = "";
        }

        public IActionResult Get()
        {
            // Check that the token is valid and will remain valid for awhile to enable the
            // user to fill out the form. If the token is not available, now is the time
            // to have the user authenticate or re-authenticate.
            bool tokenOk = CheckToken();

            ViewBag.showDoc = true;
            if (tokenOk)
            {               
                //addSpecialAttributes(model);
                ViewBag.envelopeOk = RequestItemsService.EnvelopeId != null;
                ViewBag.documentsOk = RequestItemsService.EnvelopeDocuments != null;
                ViewBag.documentOptions = RequestItemsService.EnvelopeDocuments != null? 
                    RequestItemsService.EnvelopeDocuments.Documents: null;
                ViewBag.gatewayOk = Config.GatewayAccountId != null && Config.GatewayAccountId.Length > 25;
                ViewBag.templateOk = RequestItemsService.TemplateId != null;
                ViewBag.source = CreateSourcePath();
                ViewBag.documentation = Config.documentation + EgName;
                ViewBag.showDoc = Config.documentation != null;
                
                return View(EgName);
            }

            RequestItemsService.EgName = EgName;
                        
            return Redirect("/ds/mustAuthenticate");
        }

        private dynamic CreateSourcePath()
        {
            var source = this.GetType().Name;
            return Config.githubExampleUrl + source + ".cs";
        }

        private bool CheckToken()
        {
            //TODO: add token expiration time validation check like node program
            return HttpContext.User.Identity.IsAuthenticated;
        }
    }
}