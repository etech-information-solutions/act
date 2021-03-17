
import { Storage } from '@ionic/storage';
import { Injectable, ViewChild } from '@angular/core';
import { AppVersion } from '@ionic-native/app-version/ngx';
import { FileOpener } from '@ionic-native/file-opener/ngx';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Platform, LoadingController, AlertController, PopoverController, NavController } from '@ionic/angular';
import { FileTransfer, FileUploadOptions, FileTransferObject } from '@ionic-native/file-transfer/ngx';
import { PhotoViewer } from '@ionic-native/photo-viewer/ngx';
import { ContactactComponent } from '../contactact/contactact.component';
import { CallNumber } from '@ionic-native/call-number/ngx';

@Injectable({ providedIn: 'root' })
export class UserService
{
  ContentPage: any;

  DeviceUser: any;
  CurrentUser: any;
  UpdatedUser: any;

  PSPs: any;
  Rules: any;
  Roles: any;
  Sites: any;
  Clients: any;

  SiteAudit: any;
  SiteAudits: any;
  Notification: any;
  Notifications: any;
  APIAuthentication: any;
  OutstandingPallets: any;
  OutstandingShipment: any;
  OutstandingShipments: any;
  APIAuthenticationHeader: any;

  APIKey: any = "_12345testing_";
  //APIUrl: any = "http://act.loc/";
  APIUrl: any = "http://www.testact.co.za/";

  Time: any = [];

  InvoiceStatus: any;

  AppName: any;
  AppVersion: any;
  AppVersionCode: any;

  IsIOS: boolean;
  IsAndroid: boolean;

  DoDial: boolean = false;
  ShowScroll: boolean = false;

  // Force Refresh
  RefreshNotifications: boolean = false;

  // Pin
  ResetPin: boolean = false;
  ForgotPin: boolean = false;
  PinCreated: boolean = false;
  PinCreationSkipped: boolean = false;
  

  // Paging
  Take: number = 20;


  // 
  ExitApp: boolean = false;
  RefreshShipments: boolean = false;
  RefreshSiteAudits: boolean = false;
  RemoveSiteAuditConfirmed: boolean = false;

  IsTransporter: boolean = false;

  SelfieUrl: string = "../../assets/imgs/user-profile.png";

  MonthShortNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

  OutstandingPalletsPerSite: any;
  OutstandingPalletsPerClient: any;

  OutstandingPalletsPerSiteData: any;
  OutstandingPalletsPerClientData: any;

  ServerResult: any;

  constructor( public http: HttpClient, public fp: FileOpener, public platform: Platform, public storage: Storage, public loadingCtrl: LoadingController, public transfer: FileTransfer, public appVersion: AppVersion, public alertCtrl: AlertController, public imgViewer: PhotoViewer, public popCtrl: PopoverController, public callNumber: CallNumber, public navCtrl: NavController ) 
  { 
    this.InvoiceStatus = [
      { "key": "Declined", "value": "0" },
      { "key": "Paid", "value": "1" },
      { "key": "Processing", "value": "2" }];

    for ( var i = 0; i < 24; i++ )
    {
      var hr = ( i < 10 ) ? "0" + i : i;
      var am = ( i < 12 ) ? "am" : "pm";

      this.Time.push( { key: `${hr}:00 ${am}`, value: `${hr}:00` } );
    }

    this.SetDefaults();
  }

  OnScroll( e:any )
  {
    this.ShowScroll = ( e.detail.scrollTop >= 100 );
  }

  ScrollToTop() 
  {
    this.ContentPage.scrollToTop( 1000 );
  }

  async SetDefaults() 
  {
    await this.GetUser();
    await this.SetRules();
    await this.SetRoles();
    await this.SetDeviceUser();
    await this.SetAppDetails();

    await this.SetSelfieUrl();
  }

  async SetSelfieUrl()
  {
    if ( this.DeviceUser != undefined && this.DeviceUser.SelfieUrl != undefined )
    {
      this.SelfieUrl = this.APIUrl + this.DeviceUser.SelfieUrl;
    }
  }

  async SetAppDetails()
  {
    this.IsIOS = this.platform.is( "ios" );
    this.IsAndroid = this.platform.is( "android" );

    this.appVersion.getAppName().then( v =>
    {
      this.AppName = v;
    });

    this.appVersion.getVersionNumber().then( v =>
    {
      this.AppVersion = v;
    });

    this.appVersion.getVersionCode().then( v =>
    {
      this.AppVersionCode = v;
    });
  }

  async ShowLoading( message: string = undefined )
  {
    let loading = await this.loadingCtrl.create(
    { 
      spinner: "bubbles",
      message: message
    } );
  
    loading.present();

    return loading;
  }

  async Login( login: any ) 
  {
    if ( login.email === undefined || login.password === undefined ) 
    {
      this.CurrentUser = { Code: -1, Message: `Email address and password required.` };

      return;
    }

    var loading = await this.ShowLoading();

    try
    {
      var url = `${this.APIUrl}/api/Account/Login?email=${login.email}&password=${login.password}&apikey=${this.APIKey}`;
      this.CurrentUser = await this.http.get<iUserModel>( url ).toPromise();

      this.DeviceUser = this.CurrentUser;
      this.CurrentUser.Password = login.password;

      // Store User Details
      this.storage.set( "DeviceUser", this.CurrentUser );
      this.storage.set( "CurrentUser", this.CurrentUser );

      if ( this.CurrentUser != undefined && this.CurrentUser.RoleType == 6 )
      {
        this.IsTransporter = true;
      }
    }
    catch( error )
    {
      loading.dismiss();

      this.CurrentUser = { Code: -1, Message: JSON.stringify( error ) };

      return;
    }

    loading.dismiss();
  }

  async PinIn( pin: string ) 
  {
    var loading = await this.ShowLoading();

    try
    {
      var url = `${this.APIUrl}/api/Account/LoginByPin?pin=${pin}&email=${this.DeviceUser.Email}&apikey=${this.APIKey}`;
      this.UpdatedUser = await this.http.get<iUserModel>( url ).toPromise();

      if ( this.UpdatedUser != undefined && this.UpdatedUser.Code == 1 )
      {
        this.DeviceUser = this.CurrentUser = this.UpdatedUser;

        // Store User Details
        this.storage.set( "DeviceUser", this.CurrentUser );
        this.storage.set( "CurrentUser", this.CurrentUser );

        if ( this.CurrentUser != undefined && this.CurrentUser.RoleType == 6 )
        {
          this.IsTransporter = true;
        }
      }
    }
    catch( error )
    {
      loading.dismiss();

      this.UpdatedUser = { Code: -1, Message: JSON.stringify( error ) };

      return;
    }

    loading.dismiss();
  }

  async RemovePin()
  {
    this.DeviceUser.Pin = undefined;
    this.storage.set( "DeviceUser", this.DeviceUser );
  }

  SearchNotifications( query:string = "" )
  {
    var resp = [];

    query = query.toLowerCase();

    for ( let r of this.Notifications )
    {
      if ( ( r.Message != undefined && r.Message.toLowerCase().includes( query ) ) )
      {
        resp.push( r );
      }
    }

    return resp;
  }

  async GetNotifications( query:string = "" )
  {
    if ( this.Notifications != undefined && this.Notifications.length > 0 )
    {
      if ( query != "" )
      {
        return this.SearchNotifications( query );
      }

      return this.Notifications;
    }

    var loading = await this.ShowLoading( "Fetching notifications..." );

    try
    {
      var url = `${this.APIUrl}/api/Account/Notifications?Id=${this.CurrentUser.Id}&apikey=${this.APIKey}`;

      this.Notifications = await this.http.get<iNotificationModel>( url ).toPromise();

      if ( query != "" )
      {
        return this.SearchNotifications( query );
      }
    }
    catch( error )
    {
      loading.dismiss();

      if ( this.Notifications == undefined )
      {
        this.Notifications = {};
      }

      this.Notifications.ResponseCode = -1;
      this.Notifications.Description = JSON.stringify( error );

      return [];
    }

    loading.dismiss();

    return this.Notifications;
  }

  SearchSiteAudits( query:string = "" )
  {
    var resp = [];

    query = query.toLowerCase();

    for ( let r of this.SiteAudits )
    {
      if ( ( r.ClientName != undefined && r.ClientName.toLowerCase().includes( query ) ) ||
           ( r.SiteName != undefined && r.SiteName.toLowerCase().includes( query ) ) ||
           ( r.PSPName != undefined && r.PSPName.toLowerCase().includes( query ) ) ||
           ( r.CustomerName != undefined && r.CustomerName.toLowerCase().includes( query ) ) ||
           ( r.RepName != undefined && r.RepName.toLowerCase().includes( query ) ) ||
           ( r.PalletAuditor != undefined && r.PalletAuditor.toLowerCase().includes( query ) ) ||
           ( r.Equipment != undefined && r.Equipment.toLowerCase().includes( query ) ) )
      {
        resp.push( r );
      }
    }

    return resp;
  }

  async GetSiteAudits( query:string = "" )
  {
    if ( this.SiteAudits != undefined && this.SiteAudits.length > 0 )
    {
      if ( query != "" )
      {
        return this.SearchSiteAudits( query );
      }

      return this.SiteAudits;
    }

    var loading = await this.ShowLoading( "Fetching site audits..." );

    try
    {
      var url = `${this.APIUrl}/api/Pallet/SiteAudits?email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;

      this.SiteAudits = await this.http.get<iCommonModel>( url ).toPromise();

      if ( query != "" )
      {
        return this.SearchSiteAudits( query );
      }
    }
    catch( error )
    {
      loading.dismiss();

      if ( this.SiteAudits == undefined )
      {
        this.SiteAudits = {};
      }

      this.SiteAudits.ResponseCode = -1;
      this.SiteAudits.Description = JSON.stringify( error );

      return [];
    }

    loading.dismiss();

    return this.SiteAudits;
  }

  SearchOutstandingPallets( query:string = "" )
  {
    var resp = [];

    query = query.toLowerCase();

    for ( let r of this.OutstandingPallets )
    {
      if ( ( r.ClientName != undefined && r.ClientName.toLowerCase().includes( query ) ) ||
           ( r.SiteName != undefined && r.SiteName.toLowerCase().includes( query ) ) ||
           ( r.SubSiteName != undefined && r.SubSiteName.toLowerCase().includes( query ) ) ||
           ( r.LoadNumber != undefined && r.LoadNumber.toLowerCase().includes( query ) ) ||
           ( r.DeliveryNote != undefined && r.DeliveryNote.toLowerCase().includes( query ) ) ||
           ( r.ReferenceNumber != undefined && r.ReferenceNumber.toLowerCase().includes( query ) ) ||
           ( r.ReceiverNumber != undefined && r.ReceiverNumber.toLowerCase().includes( query ) ) ||
           ( r.Equipment != undefined && r.Equipment.toLowerCase().includes( query ) ) ||
           ( r.PODNumber != undefined && r.PODNumber.toLowerCase().includes( query ) ) ||
           ( r.PCNNumber != undefined && r.PCNNumber.toLowerCase().includes( query ) ) ||
           ( r.PRNNumber != undefined && r.PRNNumber.toLowerCase().includes( query ) ) ||
           ( r.ChepInvoiceNo != undefined && r.ChepInvoiceNo.toLowerCase().includes( query ) ) ||
           ( r.DocketNumber != undefined && r.DocketNumber.toLowerCase().includes( query ) ) ||
           ( r.THAN != undefined && r.THAN.toLowerCase().includes( query ) ) ||
           ( r.ChepCompensationNo != undefined && r.ChepCompensationNo.toLowerCase().includes( query ) ) ||
           ( r.TransporterName != undefined && r.TransporterName.toLowerCase().includes( query ) ) ||
           ( r.VehicleRegistration != undefined && r.VehicleRegistration.toLowerCase().includes( query ) )
          )
      {
        resp.push( r );
      }
    }

    return resp;
  }

  async GetOutstandingPallets( query:string = "" )
  {
    if ( this.OutstandingPallets != undefined && this.OutstandingPallets.length > 0 )
    {
      if ( query != "" )
      {
        return this.SearchOutstandingPallets( query );
      }

      return this.OutstandingPallets;
    }

    var loading = await this.ShowLoading( "Fetching report..." );

    try
    {
      var url = `${this.APIUrl}/api/Pallet/OutstandingPallets?email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;

      this.OutstandingPallets = await this.http.get<iCommonModel>( url ).toPromise();

      if ( query != "" )
      {
        return this.SearchOutstandingPallets( query );
      }
    }
    catch( error )
    {
      loading.dismiss();

      if ( this.OutstandingPallets == undefined )
      {
        this.OutstandingPallets = {};
      }

      this.OutstandingPallets.ResponseCode = -1;
      this.OutstandingPallets.Description = JSON.stringify( error );

      return [];
    }

    loading.dismiss();

    return this.OutstandingPallets;
  }

  SearchOutstandingShipments( query:string = "" )
  {
    var resp = [];

    query = query.toLowerCase();

    for ( let r of this.OutstandingShipments )
    {
      if ( ( r.ClientName != undefined && r.ClientName.toLowerCase().includes( query ) ) ||
           ( r.SiteName != undefined && r.SiteName.toLowerCase().includes( query ) ) ||
           ( r.SubSiteName != undefined && r.SubSiteName.toLowerCase().includes( query ) ) ||
           ( r.LoadNumber != undefined && r.LoadNumber.toLowerCase().includes( query ) ) ||
           ( r.DeliveryNote != undefined && r.DeliveryNote.toLowerCase().includes( query ) ) ||
           ( r.ReferenceNumber != undefined && r.ReferenceNumber.toLowerCase().includes( query ) ) ||
           ( r.ReceiverNumber != undefined && r.ReceiverNumber.toLowerCase().includes( query ) ) ||
           ( r.Equipment != undefined && r.Equipment.toLowerCase().includes( query ) ) ||
           ( r.PODNumber != undefined && r.PODNumber.toLowerCase().includes( query ) ) ||
           ( r.PCNNumber != undefined && r.PCNNumber.toLowerCase().includes( query ) ) ||
           ( r.PRNNumber != undefined && r.PRNNumber.toLowerCase().includes( query ) ) ||
           ( r.ChepInvoiceNo != undefined && r.ChepInvoiceNo.toLowerCase().includes( query ) ) ||
           ( r.DocketNumber != undefined && r.DocketNumber.toLowerCase().includes( query ) ) ||
           ( r.THAN != undefined && r.THAN.toLowerCase().includes( query ) ) ||
           ( r.ChepCompensationNo != undefined && r.ChepCompensationNo.toLowerCase().includes( query ) ) ||
           ( r.TransporterName != undefined && r.TransporterName.toLowerCase().includes( query ) ) ||
           ( r.VehicleRegistration != undefined && r.VehicleRegistration.toLowerCase().includes( query ) )
          )
      {
        resp.push( r );
      }
    }

    return resp;
  }

  async GetOutstandingShipments( query:string = "" )
  {
    if ( this.OutstandingShipments != undefined && this.OutstandingShipments.length > 0 )
    {
      if ( query != "" )
      {
        return this.SearchOutstandingShipments( query );
      }

      return this.OutstandingShipments;
    }

    var loading = await this.ShowLoading( "Fetching outstanding shipments..." );

    try
    {
      var url = `${this.APIUrl}/api/Pallet/OutstandingShipments?email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;

      this.OutstandingShipments = await this.http.get<iCommonModel>( url ).toPromise();

      if ( query != "" )
      {
        return this.SearchOutstandingShipments( query );
      }
    }
    catch( error )
    {
      loading.dismiss();

      if ( this.OutstandingShipments == undefined )
      {
        this.OutstandingShipments = {};
      }

      this.OutstandingShipments.ResponseCode = -1;
      this.OutstandingShipments.Description = JSON.stringify( error );

      return [];
    }

    loading.dismiss();

    return this.OutstandingShipments;
  }

  async LogOut() 
  {
    this.CurrentUser = null;  

    await this.storage.remove( "CurrentUser" ).then( () =>
    {
    });
  }

  async ForgotPassword( email: string )
  {
    if ( email === undefined ) 
    {
      this.UpdatedUser = { Code: -1, Message: "Email address is required." };

      return;
    }

    var loading = await this.ShowLoading();

    // API Call here
    try
    {
      var url = `${this.APIUrl}/api/Account/ForgotPassword?email=${email}&apikey=${this.APIKey}`;
      this.UpdatedUser = await this.http.get<iUserModel>(url).toPromise();
    }
    catch(error)
    {
      loading.dismiss();

      this.UpdatedUser = { Code: -1, Message: JSON.stringify( error ) };

      return;
    }

    loading.dismiss();
  }

  async SetDeviceUser() 
  {
    await this.storage.get( "DeviceUser" ).then( ( user ) =>
    {
      this.DeviceUser = user;
    });
  }

  async SetCurrentUser( user: any ) 
  {
    this.CurrentUser = user;
    await this.storage.set( "CurrentUser", user );
  }

  async GetUser() 
  {
    await this.storage.get( "CurrentUser" ).then( ( user ) =>
    {
      this.CurrentUser = user;

      if ( this.CurrentUser != undefined && this.CurrentUser.RoleType == 6 )
      {
        this.IsTransporter = true;
      }
    });
  }

  async SetRules()
  {
    try
    {
      var url = `${this.APIUrl}/api/Account/GetRules?apikey=${this.APIKey}`;
      this.Rules = await this.http.get<iSystemRulesModel>( url ).toPromise();

      await this.storage.set( "Rules", this.Rules );
    }
    catch( error )
    {
      return;
    }
  }

  async SetRoles()
  {
    try
    {
      var url = `${this.APIUrl}/api/Account/GetRoles?apikey=${this.APIKey}`;
      this.Roles = await this.http.get<iSystemRulesModel>( url ).toPromise();

      await this.storage.set( "Roles", this.Roles );
    }
    catch( error )
    {
      return;
    }
  }

  async SetPSPs()
  {
    try
    {
      var url = `${this.APIUrl}/api/Account/GetPSPs?email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;
      this.PSPs = await this.http.get<iCommonModel>( url ).toPromise();

      await this.storage.set( "PSPs", this.PSPs );
    }
    catch( error )
    {
      return;
    }
  }

  async SetSites()
  {
    try
    {
      var url = `${this.APIUrl}/api/Account/GetSites?email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;
      this.Sites = await this.http.get<iCommonModel>( url ).toPromise();

      await this.storage.set( "Sites", this.Sites );
    }
    catch( error )
    {
      return;
    }
  }

  async SetClients()
  {
    try
    {
      var url = `${this.APIUrl}/api/Account/GetClients?email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;
      this.Clients = await this.http.get<iCommonModel>( url ).toPromise();

      await this.storage.set( "Clients", this.Clients );
    }
    catch( error )
    {
      return;
    }
  }

  async Register( agent: any, message: string = undefined ) 
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      agent = `${agent}&apikey=${this.APIKey}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      this.UpdatedUser = await this.http.post<iUserModel>(this.APIUrl + "/api/Account/Register", agent, httpOptions).toPromise();
    }
    catch(error)
    {
      loading.dismiss();
      this.UpdatedUser = { Code: -1, Message: JSON.stringify( error ) };

      return;
    }

    loading.dismiss();
  }

  async MarkNotificationAsRead( id:any ) 
  {
    var loading = await this.ShowLoading();

    // API Call here
    try
    {
      var p = `id=${id}&userid=${this.CurrentUser.Id}&apikey=${this.APIKey}`;

      var url = `id=${id}&userid=${this.CurrentUser.Id}&apikey=${this.APIKey}`;

      var result = await this.http.post<iNotificationModel>( this.APIUrl + "/api/Account/MarkNotificationAsRead?" + url, p, this.APIAuthenticationHeader ).toPromise();
    }
    catch(error)
    {
      loading.dismiss();

      return;
    }

    loading.dismiss();
  }

  async Update( agent: any, message: string = undefined ) 
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      agent = `${agent}&apikey=${this.APIKey}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      this.UpdatedUser = await this.http.post<iUserModel>( this.APIUrl + "/api/Account/Update", agent, httpOptions ).toPromise();

      if ( this.UpdatedUser.Code == 1 )
      {
        this.CurrentUser = this.DeviceUser = this.UpdatedUser;

        await this.SetSelfieUrl();
      }
    }
    catch( error )
    {
      loading.dismiss();

      this.UpdatedUser = { Code: -1, Message: JSON.stringify( error ) };

      return;
    }

    loading.dismiss();
  }

  async UpdateSiteAudit( audit: any, message: string = undefined )
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      audit = `${audit}&email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      // Update site audit
      this.ServerResult = await this.http.post<iCommonModel>( this.APIUrl + "/api/Pallet/UpdateSiteAudit", audit, httpOptions ).toPromise();

      loading.dismiss();

      if ( this.ServerResult.Code == 1 )
      {
        this.SiteAudits = this.ServerResult.SiteAudits;
      }
    }
    catch( error )
    {
      loading.dismiss();

      this.ServerResult = { Code: -1, Message: JSON.stringify( error ) };
    }
  }

  async CreateSiteAudit( audit: any, message: string = undefined )
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      audit = `${audit}&email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      // Update site audit
      var resp = await this.http.post<iCommonModel>( this.APIUrl + "/api/Pallet/CreateSiteAudit", audit, httpOptions ).toPromise();

      loading.dismiss();

      return resp;
    }
    catch( error )
    {
      loading.dismiss();

      return { Id: -1, Code: -1, Message: JSON.stringify( error ) };
    }
  }

  async UploadSignature( id: string, type: string, fileurl: any, fileKey: string, fileName: string, mimeType: string, message: string = undefined )
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      const fileTransfer: FileTransferObject = this.transfer.create();

      let options: FileUploadOptions =
      {
        fileKey: fileKey,
        fileName: fileName,
        chunkedMode: false,
        mimeType: mimeType,
        params: {
          id: id,
          type: type,
          apikey: this.APIKey,
          email: this.CurrentUser.Email
        }
      };

      await fileTransfer.upload( fileurl, `${this.APIUrl}/api/Pallet/UploadSignature?id=${id}&type=${type}&apikey=${this.APIKey}&email=${this.CurrentUser.Email}`, options ).then( ( data ) =>
      {
        //this.ServerResult.ResponseCode = 1;

      }, ( err ) =>
      {
        //this.ServerResult.ResponseCode = 1;
        //this.ServerResult.Description = JSON.stringify( err );
      });
    }
    catch( error )
    {
      //this.ServerResult.ResponseCode = 1;
      //this.ServerResult.Description = JSON.stringify( error );
    }

    loading.dismiss();
  }

  async DeleteSiteAudit( id: any, message: string = undefined )
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      var p = `id=${id}&email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      var resp = await this.http.post<iCommonModel>( this.APIUrl + "/api/Pallet/DeleteSiteAudit", p, httpOptions ).toPromise();

      loading.dismiss();

      return resp;
    }
    catch( error )
    {
      loading.dismiss();

      return { Code: -1, Message: JSON.stringify( error ) };
    }
  }

  async UpdatePin( pin: string, confirmPin: string )
  {
    var loading = await this.ShowLoading( "Updating pin..." );

    // API Call here
    try
    {
      var p = `id=${this.CurrentUser.Id}&pin=${pin}&confirmpin=${confirmPin}&apikey=${this.APIKey}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      this.UpdatedUser = await this.http.post<iUserModel>( this.APIUrl + "/api/Account/UpdatePin", p, httpOptions ).toPromise();

      if ( this.UpdatedUser.Code == 1 )
      {
        this.DeviceUser = this.UpdatedUser;
        this.CurrentUser = this.UpdatedUser;

        await this.storage.set( "DeviceUser", this.CurrentUser );
        await this.storage.set( "CurrentUser", this.CurrentUser );
      }
    }
    catch( error )
    {
      loading.dismiss();

      this.UpdatedUser = { Code: -1, Message: JSON.stringify( error ) };

      return;
    }

    loading.dismiss();
  }

  async DontShowWelcomeAgain( chk:boolean )
  {
    // API Call here
    try
    {
      var welcome = `Id=${this.CurrentUser.Id}&DontShowWelcome=${chk}&apikey=${this.APIKey}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      await this.http.post<iUserModel>( this.APIUrl + "/api/Account/DontShowWelcomeAgain", welcome, httpOptions ).toPromise();
    }
    catch( error )
    {
      return;
    }
  }

  async UploadDocument( agentId: number, fileurl: any, fileKey: string, fileName: string, mimeType: string, message: string = undefined )
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      const fileTransfer: FileTransferObject = this.transfer.create();

      let options: FileUploadOptions =
      {
        fileKey: fileKey,
        fileName: fileName,
        chunkedMode: false,
        mimeType: mimeType,
        headers: {
          agentId: agentId,
          apikey: this.APIKey
        }
      };

      await fileTransfer.upload( fileurl, `${this.APIUrl}/api/Account/UploadDocument?agentId=${agentId}&apikey=${this.APIKey}`, options ).then( ( data ) =>
      {
        this.UpdatedUser = data;

      }, ( err ) =>
      {
        this.UpdatedUser = { Code: 1, Message: JSON.stringify( err ) };
      });
    }
    catch( error )
    {
      this.UpdatedUser = { Code: -1, Message: JSON.stringify( error ) };
    }

    loading.dismiss();
  }

  async UploadDocument1( agentId: number, fileurl: any, fileKey: string, fileName: string, mimeType: string )
  {
    // API Call here
    try
    {
      const fileTransfer: FileTransferObject = this.transfer.create();

      let options: FileUploadOptions =
      {
        fileKey: fileKey,
        fileName: fileName,
        chunkedMode: false,
        mimeType: mimeType,
        headers: {
          agentId: agentId,
          apikey: this.APIKey
        }
      };

      fileTransfer.upload( fileurl, `${this.APIUrl}/api/Account/UploadDocument?agentId=${agentId}&apikey=${this.APIKey}`, options ).then( ( data ) =>
      {
        

      }, ( err ) =>
      {
        
      });
    }
    catch( error )
    {
      
    }
  }

  async UpdateShipment( load: any, message: string = undefined ) 
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      load = `${load}&apikey=${this.APIKey}&email=${this.CurrentUser.Email}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      this.ServerResult = await this.http.post<iCommonModel>( this.APIUrl + "/api/Pallet/UpdateShipment", load, httpOptions ).toPromise();

      if ( this.ServerResult.Code == 1 )
      {
        this.OutstandingShipments = this.ServerResult.OutstandingShipments;
      }
    }
    catch( error )
    {
      loading.dismiss();

      this.ServerResult = { Code: -1, Message: JSON.stringify( error ) };

      return;
    }

    loading.dismiss();
  }

  async UploadShipment( id: number, objectType: string, comment: string, fileurl: any, fileKey: string, fileName: string, mimeType: string, message: string = undefined )
  {
    var loading = await this.ShowLoading( message );

    // API Call here
    try
    {
      const fileTransfer: FileTransferObject = this.transfer.create();

      let options: FileUploadOptions =
      {
        fileKey: fileKey,
        fileName: fileName,
        chunkedMode: false,
        mimeType: mimeType,
        params: {
          id: id,
          apikey: this.APIKey,
          objecttype: objectType,
          comment: comment,
          email: this.CurrentUser.Email,
        }
      };

      await fileTransfer.upload( fileurl, `${this.APIUrl}/api/Pallet/UploadShipment?id=${id}&apikey=${this.APIKey}&objecttype=${objectType}&comment=${comment}&email=${this.CurrentUser.Email}`, options ).then( ( data ) =>
      {
        this.ServerResult = data;

      }, ( err ) =>
      {
        this.ServerResult = { Code: 1, Message: JSON.stringify( err ) };
      });
    }
    catch( error )
    {
      this.ServerResult = { Code: -1, Message: JSON.stringify( error ) };
    }

    loading.dismiss();
  }

  DataURItoBlob( fileurl:any, mimeType: string )
  {
    const byteString = window.atob( fileurl );
    const arrayBuffer = new ArrayBuffer( byteString.length );
    const int8Array = new Uint8Array( arrayBuffer );

    for ( let i = 0; i < byteString.length; i++ )
    {
      int8Array[ i ] = byteString.charCodeAt( i );
    }
    
    return new Blob( [ int8Array ], { type: mimeType } );
  }

  GetISODateString( date: Date = new Date() )
  {
    try
    {
      return `${date.getFullYear()}-${this.Do0Check( date.getMonth() + 1 )}-${this.Do0Check( date.getDate() )}T${this.Do0Check( date.getHours() )}:${this.Do0Check( date.getMinutes() )}`;
    }
    catch
    {
      return new Date().toISOString();
    }
  }

  GetShortDate( date:any, format:string = "" )
  {
    if( date == undefined )
    {
      return '';
    }

    var d = new Date( date );

    if ( format != "" )
    {
      switch ( format )
      {
        case 'yyyy/MM/dd':
          return `${d.getFullYear()}/${this.Do0Check( d.getMonth() + 1 )}/${this.Do0Check( d.getDate() )}`;
      }
    }

    return `${this.Do0Check( d.getDate() )}/${this.Do0Check( d.getMonth() + 1 )}/${d.getFullYear()}`;
  }

  GetShortTime( date:any )
  {
    var d = new Date( date );

    return `${this.Do0Check( d.getHours() )}:${this.Do0Check( d.getMinutes() )}`;
  }

  Do0Check( digit: number )
  {
    return ( digit < 10 ) ? `0${digit}` : digit + "";
  }

  GetTime( time: string )
  {
    return `${time.split( ':' )[ 0 ]}:${time.split( ':' )[ 1 ]}`;
  }

  GetInvoiceStatusDescription( status:any )
  {
    if ( status == undefined )
    {
      return status;
    }

    var i = this.InvoiceStatus.findIndex( s => s.value == status );

    if ( i < 0 )
    {
      return status;
    }

    return this.InvoiceStatus[ i ].key;
  }

  ToDecimal( number: number, decimals: number )
  {
    if ( number == undefined )
    {
      number = 0;
    }

    return number.toFixed( decimals );
  }

  async AuthenticateAPI()
  {
    var url = `${this.APIUrl}/api/Authorisation/Authenticate`;
    
    var body = `APIKey=${this.APIKey}`;

    let httpOptions = 
    {
      headers: new HttpHeaders
      ({
        "Content-Type": "application/x-www-form-urlencoded"
      })
    };

    this.APIAuthentication = await this.http.post<iAPIAuthentionModel>( url, body, httpOptions ).toPromise();

    if ( this.APIAuthentication.Authorized )
    {
      let httpOptions = 
      {
        headers: new HttpHeaders
        ({
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      this.APIAuthenticationHeader = httpOptions;
    }
    else
    {
      this.APIAuthentication = null;

      await this.AuthenticateAPI();
    }
  }

  HideAccountNumber( acc:string )
  {
    if ( acc == undefined || acc == "" )
    {
      return acc;
    }

    var hide = acc.length - 2;

    var arr = acc.split( '' );

    for ( var i = 0; i < hide; i++ )
    {
        arr[ i ] = "*";
    }

    return arr.join( "" );
  }

  async OpenImage( url: string )
  {
    this.imgViewer.show( url );
  }

  async OnShowContact()
  {
    const pop = await this.popCtrl.create
    ({
      translucent: true,
      backdropDismiss: false,
      component: ContactactComponent,
      componentProps: { ContactNumber: this.Rules.ContactNumber }
    });

    pop.onDidDismiss().then( ( r ) => 
    {
      if ( r.data == 1 )
      {
        this.callNumber.callNumber( this.Rules.ContactNumber, true )
        .then(res => console.log('Launched dialer!', res))
        .catch(err => this.ShowError('Error launching dialer' + JSON.stringify( err )));
      }
    });

    return await pop.present();
  }

  async GoToPage( page:string, back: boolean = false )
  {
    if ( page == 'home' || page == 'login' )
    {
      this.navCtrl.navigateRoot( page );
    }
    else if ( back )
    {
      this.navCtrl.navigateBack( page );
    }
    else
    {
      this.navCtrl.navigateForward( page );
    }
  }

  async ListOutstandingPalletsPerSite()
  {
    let loading = await this.ShowLoading();

    try
    {
      if ( this.OutstandingPalletsPerSite != undefined && this.OutstandingPalletsPerSite.length > 0 )
      {
        loading.dismiss();

        return this.OutstandingPalletsPerSite;
      }

      var url = `${this.APIUrl}/api/Pallet/ListOutstandingPalletsPerSite?email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;
      this.OutstandingPalletsPerSite = await this.http.get<iPalletModel>( url ).toPromise();
    }
    catch( error )
    {
      loading.dismiss();

      this.ShowMessage( "Oops!", JSON.stringify( error ) );
    }

    loading.dismiss();

    return this.OutstandingPalletsPerSite;
  }

  async ListOutstandingPalletsPerClient()
  {
    let loading = await this.ShowLoading();

    try
    {
      if ( this.OutstandingPalletsPerClient != undefined && this.OutstandingPalletsPerClient.length > 0 )
      {
        loading.dismiss();
        
        return this.OutstandingPalletsPerClient;
      }

      var url = `${this.APIUrl}/api/Pallet/ListOutstandingPalletsPerClient?email=${this.CurrentUser.Email}&apikey=${this.APIKey}`;
      this.OutstandingPalletsPerClient = await this.http.get<iPalletModel>( url ).toPromise();
    }
    catch( error )
    {
      loading.dismiss();

      this.ShowMessage( "Oops!", JSON.stringify( error ) );
    }

    loading.dismiss();

    return this.OutstandingPalletsPerClient;
  }

  async ShowMessage( title:string, message:string ) 
  {
    let alert = await this.alertCtrl.create(
    {
      header: title,
      message: message,
      buttons: [ 'OK' ]
    });

    await alert.present();
  }

  async ShowError( text ) 
  {
    let alert = await this.alertCtrl.create(
    {
      header: "Oops!",
      message: text,
      buttons: ["OK"]
    });

    await alert.present();
  }
}


export interface iUserModel
{
  // Control
  Code: any;
  Message: string;

  // Properties
  Id: any;
  Status: any;
  Pin: string;
  Cell: string;
  Name: string;
  Email: string;
  Selfie: string;
  Surname: string;
  Password: string;
  IdNumber: string;
  DisplayName: string;
  
  Bank: any;
  Comments: any;
  Addresses: any;

  DoNotShowWelcome: boolean;
}

export interface iAPIAuthentionModel
{
  // Properties
  Authorized: boolean;
  AccessToken: string;
}

export interface iNotificationModel
{
  Id: any;
  Status: any;
  CreatedOn: any;
  Message: string;
  ResponseCode: any;
  Description: string;
}

export interface iNumberModel
{
  Number: any;
  ResponseCode: any;
  Description: string;
}

export interface iSystemRulesModel
{
  LogoffSeconds: any;
  AutoLogoff: any;
  ActivationEmail: any;
}

export interface iPalletModel
{
  Cli: any;
  AutoLogoff: any;
  ActivationEmail: any;
}

export interface iCommonModel
{
  Id: any;
  Code: any;
  Message: any;
}