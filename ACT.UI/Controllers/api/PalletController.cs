using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Data.Entity;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Models.Simple;
using ACT.Core.Services;
using ACT.Data.Models;
using System.Web.Hosting;

namespace ACT.UI.Controllers.api
{
    public class PalletController : ApiController
    {
        BaseController baseC = new BaseController();

        // GET api/Pallet/GetOutstandingPalletsPerClient
        [HttpGet]
        public List<SimpleOutstandingPallets> ListOutstandingPalletsPerClient( string email, string apikey )
        {
            CustomSearchModel csm = new CustomSearchModel() { BalanceStatus = BalanceStatus.NotBalanced };

            List<SimpleOutstandingPallets> response = new List<SimpleOutstandingPallets>();

            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                clservice.CurrentUser = clservice.GetUser( email );

                response = clservice.ListOutstandingPalletsPerClient( csm );

                if ( !response.NullableAny() )
                {
                    return response;
                }

                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, 1 );
                //csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                List<SimpleOutstandingPallets> response30 = clservice.ListOutstandingPalletsPerClient( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Past30 = response30?.FirstOrDefault( r => r.ClientId == s.ClientId )?.Total ?? 0;
                }
            }

            return response;
        }

        // GET api/Pallet/GetOutstandingPalletsPerSite
        [HttpGet]
        public List<SimpleOutstandingPallets> ListOutstandingPalletsPerSite( string email, string apikey )
        {
            List<SimpleOutstandingPallets> response = new List<SimpleOutstandingPallets>();

            CustomSearchModel csm = new CustomSearchModel() { BalanceStatus = BalanceStatus.NotBalanced };

            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                clservice.CurrentUser = clservice.GetUser( email );

                response = clservice.ListOutstandingPalletsPerSite( csm );

                if ( !response.NullableAny() )
                {
                    return response;
                }

                // 0-3 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                List<SimpleOutstandingPallets> rep = clservice.ListOutstandingPalletsPerSite( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Month1 = rep?.FirstOrDefault( r => r.SiteId == s.SiteId )?.Total ?? 0;
                }

                // 4-6 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month ) );

                rep = clservice.ListOutstandingPalletsPerSite( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Month2 = rep?.FirstOrDefault( r => r.SiteId == s.SiteId )?.Total ?? 0;
                }

                // 7-12 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month ) );

                rep = clservice.ListOutstandingPalletsPerSite( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Month3 = rep?.FirstOrDefault( r => r.SiteId == s.SiteId )?.Total ?? 0;
                }

                // +12 Months
                csm.FromDate = null;
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month ) );

                rep = clservice.ListOutstandingPalletsPerSite( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Month4 = rep?.FirstOrDefault( r => r.SiteId == s.SiteId )?.Total ?? 0;
                }
            }

            return response;
        }


        [HttpGet]
        public List<SiteAuditCustomModel> SiteAudits( string email, string apikey )
        {
            using ( SiteAuditService saservice = new SiteAuditService() )
            {
                saservice.CurrentUser = saservice.GetUser( email );

                List<SiteAuditCustomModel> response = saservice.List1( new PagingModel() { Take = int.MaxValue }, new CustomSearchModel() { Status = Status.Active } );

                return response;
            }
        }

        [HttpPost]
        public ResponseModel DeleteSiteAudit( SiteAuditCustomModel model )
        {
            using ( SiteAuditService saservice = new SiteAuditService() )
            {
                saservice.CurrentUser = saservice.GetUser( model.Email );

                SiteAudit audit = saservice.GetById( model.Id );

                if ( audit == null )
                {
                    return new ResponseModel() { Code = -1, Description = "Site Audit could not be found" };
                }

                audit.Status = ( int ) Status.Inactive;

                saservice.Update( audit );

                return new ResponseModel() { Code = 1, Description = "Site Audit deleted" };
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateSiteAudit( SiteAuditCustomModel model )
        {
            using ( SiteAuditService saservice = new SiteAuditService() )
            {
                saservice.CurrentUser = saservice.GetUser( model.Email );

                SiteAudit audit = new SiteAudit()
                {
                    SiteId = model.SiteId,
                    RepName = model.RepName,
                    ClientId = model.ClientId,
                    AuditDate = model.AuditDate,
                    Equipment = model.Equipment,
                    Status = ( int ) Status.Active,
                    CustomerName = model.CustomerName,
                    PalletAuditor = model.PalletAuditor,
                    PalletsCounted = model.PalletsCounted,
                    WriteoffPallets = model.WriteoffPallets,
                    PalletsOutstanding = model.PalletsOutstanding,
                };

                audit = saservice.Create( audit );

                return Request.CreateResponse( HttpStatusCode.OK, audit );
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateSiteAudit( SiteAuditCustomModel model )
        {
            using ( SiteAuditService saservice = new SiteAuditService() )
            {
                saservice.CurrentUser = saservice.GetUser( model.Email );

                SiteAudit audit = saservice.GetById( model.Id );

                if ( audit == null )
                {
                    return Request.CreateResponse( HttpStatusCode.OK, new ResponseModel() { Code = -1, Description = "Site Audit could not be found" } );
                }

                audit.SiteId = model.SiteId;
                audit.RepName = model.RepName;
                audit.ClientId = model.ClientId;
                audit.AuditDate = model.AuditDate;
                audit.Equipment = model.Equipment;
                audit.CustomerName = model.CustomerName;
                audit.PalletAuditor = model.PalletAuditor;
                audit.PalletsCounted = model.PalletsCounted;
                audit.WriteoffPallets = model.WriteoffPallets;
                audit.PalletsOutstanding = model.PalletsOutstanding;

                saservice.Update( audit );

                List<SiteAuditCustomModel> siteAudits = SiteAudits( model.Email, model.APIKey );

                return Request.CreateResponse( HttpStatusCode.OK, new { SiteAudits = siteAudits, Code = 1, Description = "Site Audit updated" } );
            }
        }

        [HttpPost]
        public HttpResponseMessage UploadSignature( int id, string type, string email, string apikey )
        {
            try
            {
                using ( ImageService iservice = new ImageService() )
                using ( SiteAuditService saservice = new SiteAuditService() )
                {
                    saservice.CurrentUser = saservice.GetUser( email );

                    SiteAudit audit = saservice.GetById( id );

                    if ( audit == null )
                    {
                        return Request.CreateResponse( HttpStatusCode.OK, new ResponseModel() { Code = -1, Description = "Site Audit could not be found" } );
                    }

                    HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[ 0 ] : null;

                    if ( file == null )
                    {
                        return Request.CreateResponse( HttpStatusCode.OK, new ResponseModel() { Code = -1, Description = "No file uploaded!" } );
                    }

                    // Create folder
                    string path = HostingEnvironment.MapPath( $"~/{VariableExtension.SystemRules.ImagesLocation}/SiteAudits/{audit.Id}/" );

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    string ext = Path.GetExtension( file.FileName ),
                           name = file.FileName.Replace( ext, "" );

                    // Check if a logo already exists?
                    Image img = iservice.Get( audit.Id, type, true );

                    if ( img != null )
                    {
                        baseC.DeleteImage( img.Id );
                    }

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    Image image = new Image()
                    {
                        Name = name,
                        IsMain = true,
                        Extension = ext,
                        ObjectType = type,
                        ObjectId = audit.Id,
                        Description = type,
                        Size = file.ContentLength,
                        Location = $"SiteAudits/{audit.Id}/{now}-{file.FileName}"
                    };

                    image = iservice.Create( image );

                    string fullpath = Path.Combine( path, $"{now}-{file.FileName}" );

                    file.SaveAs( fullpath );

                    if ( type == "CustomerSignature" )
                    {
                        audit.CustomerSignatureId = image.Id;
                    }
                    else if ( type == "RepSignature" )
                    {
                        audit.RepSignatureId = image.Id;
                    }
                    else if ( type == "PalletAuditorSignature" )
                    {
                        audit.PalletAuditorSignatureId = image.Id;
                    }

                    saservice.Update( audit );

                    return Request.CreateResponse( HttpStatusCode.OK, saservice.OldObject );
                }
            }
            catch ( Exception ex )
            {
                return Request.CreateResponse( HttpStatusCode.OK, ex.Message.ToString() );
            }
        }

        [HttpGet]
        public HttpResponseMessage OutstandingPallets( string email, string apikey )
        {
            List<OutstandingPalletsModel> resp = baseC.GetOutstandingPallets( new PagingModel(), new CustomSearchModel(), email );

            return Request.CreateResponse( HttpStatusCode.OK, resp );
        }

        [HttpGet]
        public HttpResponseMessage OutstandingShipments( string email, string apikey )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                service.CurrentUser = service.GetUser( email );

                CustomSearchModel csm = new CustomSearchModel();

                PagingModel pm = new PagingModel() { SortBy = "cl.PCNNumber, cl.PRNNumber, cl.PODNumber", Skip = 0, Take = int.MaxValue };

                List<ClientLoadCustomModel> model = service.ListOutstandingShipments( pm, csm );

                return Request.CreateResponse( HttpStatusCode.OK, model );
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateShipment( ClientLoadCustomModel model )
        {
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                clservice.CurrentUser = clservice.GetUser( model.Email );

                ClientLoad load = clservice.GetById( model.Id );

                if ( load == null )
                {
                    return Request.CreateResponse( HttpStatusCode.OK, new ResponseModel() { Code = -1, Description = "Shipment could not be found" } );
                }

                load.OutstandingReasonId = null;
                load.PCNNumber = model.PCNNumber;
                load.PODNumber = model.PODNumber;
                load.PRNNumber = model.PRNNumber;
                load.PODStatus = ( int ) Status.Active;

                clservice.Update( load );

                CustomSearchModel csm = new CustomSearchModel();

                PagingModel pm = new PagingModel() { SortBy = "cl.PCNNumber, cl.PRNNumber, cl.PODNumber" };

                List<ClientLoadCustomModel> loads = clservice.ListOutstandingShipments( pm, csm );

                return Request.CreateResponse( HttpStatusCode.OK, new { OutstandingShipments = loads, Code = 1, Description = "Shipment updated" } );
            }
        }

        [HttpPost]
        public HttpResponseMessage UploadShipment( int id, string apikey, string objecttype, string comment, string email )
        {
            try
            {
                using ( ImageService iservice = new ImageService() )
                using ( ClientLoadService saservice = new ClientLoadService() )
                {
                    saservice.CurrentUser = saservice.GetUser( email );

                    ClientLoad load = saservice.GetById( id );

                    if ( load == null )
                    {
                        return Request.CreateResponse( HttpStatusCode.OK, new ResponseModel() { Code = -1, Description = "Shipment could not be found" } );
                    }

                    HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[ 0 ] : null;

                    if ( file == null )
                    {
                        return Request.CreateResponse( HttpStatusCode.OK, new ResponseModel() { Code = -1, Description = "No file uploaded!" } );
                    }

                    // Create folder
                    string path = HostingEnvironment.MapPath( $"~/{VariableExtension.SystemRules.ImagesLocation}/ClientLoads/{load.LoadNumber?.Trim()}/" );

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    string ext = Path.GetExtension( file.FileName ),
                           name = file.FileName.Replace( ext, "" );

                    // Check if a logo already exists?
                    Image img = iservice.Get( load.Id, objecttype, true );

                    if ( img != null )
                    {
                        baseC.DeleteImage( img.Id );
                    }

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    Image image = new Image()
                    {
                        Name = name,
                        ObjectId = load.Id,
                        ObjectType = objecttype,
                        Size = file.ContentLength,
                        Description = comment,
                        IsMain = true,
                        Extension = ext,
                        Location = $"ClientLoads/{load.LoadNumber?.Trim()}/{now}-{file.FileName}"
                    };

                    iservice.Create( image );

                    string fullpath = Path.Combine( path, $"{now}-{file.FileName}" );

                    file.SaveAs( fullpath );

                    return Request.CreateResponse( HttpStatusCode.OK, saservice.OldObject );
                }
            }
            catch ( Exception ex )
            {
                return Request.CreateResponse( HttpStatusCode.OK, ex.Message.ToString() );
            }
        }
    }
}