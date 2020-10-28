import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController, PopoverController } from '@ionic/angular';
import { UserService } from '../services/user.service';

import SignaturePad from 'signature_pad';
import { ConfirmdeletesiteauditComponent } from '../confirmdeletesiteaudit/confirmdeletesiteaudit.component';

@Component({
  selector: 'app-siteauditdetail',
  templateUrl: './siteauditdetail.page.html',
  styleUrls: ['./siteauditdetail.page.scss'],
})
export class SiteauditdetailPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;

  SiteId: any;
  ClientId: any;

  SiteName: any;
  ClientName: any;

  AuditDate: any;

  Equipment: any;
  PalletsOutstanding: any;
  PalletsCounted: any;
  WriteoffPallets: any;

  CustomerName: any;
  CustomerSignatureUrl: any;
  CustomerSignaturePad: any;
  @ViewChild( "CustomerSignature", { static: true } ) csignaturePad;

  RepName: any;
  RepSignatureUrl: any;
  RepSignaturePad: any;
  @ViewChild( "RepSignature", { static: true } ) rsignaturePad;

  PalletAuditor: any;
  PalletAuditorSignUrl: any;
  PalletAuditorSignPad: any;
  @ViewChild( "PalletAuditorSign", { static: true } ) psignaturePad;

  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService, public popCtrl: PopoverController )
  {
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }

    if ( this.auth.SiteAudit == undefined )
    {
      this.auth.GoToPage( "siteaudit", true );
    }

    this.menuCtrl.enable( true );
  }

  ngOnInit()
  {
    this.SiteId = this.auth.SiteAudit.SiteId;
    this.ClientId = this.auth.SiteAudit.ClientId;

    this.AuditDate = this.auth.GetISODateString( new Date( this.auth.SiteAudit.AuditDate ) );

    this.Equipment = this.auth.SiteAudit.Equipment;
    this.PalletsOutstanding = this.auth.SiteAudit.PalletsOutstanding;
    this.PalletsCounted = this.auth.SiteAudit.PalletsCounted;
    this.WriteoffPallets = this.auth.SiteAudit.WriteoffPallets;

    this.CustomerName = this.auth.SiteAudit.CustomerName;

    if ( this.auth.SiteAudit.CustomerSignatureId > 0 )
    {
      this.CustomerSignatureUrl = `${this.auth.APIUrl}Account/ViewImage?id=${this.auth.SiteAudit.CustomerSignatureId}`;
    }

    this.RepName = this.auth.SiteAudit.RepName;

    if ( this.auth.SiteAudit.RepSignatureId > 0 )
    {
      this.RepSignatureUrl = `${this.auth.APIUrl}Account/ViewImage?id=${this.auth.SiteAudit.RepSignatureId}`;
    }

    this.PalletAuditor = this.auth.SiteAudit.PalletAuditor;

    if ( this.auth.SiteAudit.PalletSignatureId > 0 )
    {
      this.PalletAuditorSignUrl = `${this.auth.APIUrl}Account/ViewImage?id=${this.auth.SiteAudit.PalletSignatureId}`;
    }
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;

    this.CustomerSignaturePad = new SignaturePad( this.csignaturePad.nativeElement );
    this.RepSignaturePad = new SignaturePad( this.rsignaturePad.nativeElement );
    this.PalletAuditorSignPad = new SignaturePad( this.psignaturePad.nativeElement );
  }

  ClearPad( sig: number )
  {
    if ( sig == 1 )
    {
      this.CustomerSignaturePad.clear();

      if ( this.CustomerSignatureUrl != undefined )
      {
        this.CustomerSignatureUrl = undefined;
      }
    }
    else if ( sig == 2 )
    {
      this.RepSignaturePad.clear();

      if ( this.RepSignatureUrl != undefined )
      {
        this.RepSignatureUrl = undefined;
      }
    }
    else if ( sig == 3 )
    {
      this.PalletAuditorSignPad.clear();

      if ( this.PalletAuditorSignUrl != undefined )
      {
        this.PalletAuditorSignUrl = undefined;
      }
    }
  }
  
  async Update()
  {
    if ( !this.Valid() )
    {
      return;
    }
    
    var audit = this.Construct();

    await this.auth.UpdateSiteAudit( audit, "Updating your Site Audit..." );

    if ( this.auth.ServerResult != undefined && this.auth.ServerResult.Code  == -1 )
    {
      this.auth.ShowError( this.auth.ServerResult.Message );
      
      return;
    }

    if ( !this.CustomerSignaturePad.isEmpty() )
    {
      await this.auth.UploadSignature( this.auth.SiteAudit.Id, "CustomerSignature", this.CustomerSignaturePad.toDataURL( "image/png" ), "file", "siteaudit-customer-signature.png", "image/png", "Uploading Customer Signature.." );
    }
    if ( !this.RepSignaturePad.isEmpty() )
    {
      await this.auth.UploadSignature( this.auth.SiteAudit.Id, "RepSignature", this.RepSignaturePad.toDataURL( "image/png" ), "file", "siteaudit-rep-signature.png", "image/png", "Uploading Sales Rep Signature.." );
    }
    if ( !this.PalletAuditorSignPad.isEmpty() )
    {
      await this.auth.UploadSignature( this.auth.SiteAudit.Id, "PalletAuditorSignature", this.PalletAuditorSignPad.toDataURL( "image/png" ), "file", "siteaudit-pallet-auditor-signature.png", "image/png", "Uploading Pallet Auditor Signature.." );
    }

    this.auth.RefreshSiteAudits = true;

    this.auth.GoToPage( "siteaudit", true );
  }

  Valid()
  {
    var valid = true;

    // Required fields
    if ( this.AuditDate === undefined || this.SiteId === undefined || this.ClientId === undefined || this.Equipment === undefined || this.PalletsOutstanding === undefined || this.PalletsCounted === undefined || this.WriteoffPallets === undefined || this.CustomerName === undefined || this.RepName === undefined || this.PalletAuditor === undefined )
    {
      this.auth.ShowError( "Please complete the required information." );

      return false;
    }

    if ( this.CustomerSignaturePad.isEmpty() && this.CustomerSignatureUrl == undefined )
    {
      this.auth.ShowError( "Customer signature is required, please ask Customer to sign." );

      return false;
    }

    if ( this.RepSignaturePad.isEmpty() && this.RepSignatureUrl == undefined )
    {
      this.auth.ShowError( "Your signature is required, please sign under Sales Rep signature." );

      return false;
    }

    if ( this.PalletAuditorSignPad.isEmpty() && this.PalletAuditorSignUrl == undefined )
    {
      this.auth.ShowError( "Pallet Auditor signature is required, please ask Pallet Auditor to sign." );

      return false;
    }

    return valid;
  }

  private Construct()
  {
    var aa = `${this.AuditDate}`.replace( "+02:00", "" );

    var audit =`Id=${this.auth.SiteAudit.Id}&SiteId=${this.SiteId}&ClientId=${this.ClientId}&AuditDate=${aa}&Equipment=${this.Equipment}&PalletsOutstanding=${this.PalletsOutstanding}&PalletsCounted=${this.PalletsCounted}&WriteoffPallets=${this.WriteoffPallets}&CustomerName=${this.CustomerName}&RepName=${this.RepName}&PalletAuditor=${this.PalletAuditor}`;

    return audit;
  }

  async Delete()
  {
    const pop = await this.popCtrl.create
    ({
      translucent: false,
      backdropDismiss: false,
      component: ConfirmdeletesiteauditComponent
    });

    pop.onDidDismiss().then( ( r ) => 
    {
      if ( this.auth.RemoveSiteAuditConfirmed )
      {
        this.RemoveSiteAuditConfirmed( this.auth.SiteAudit.Id );

        this.auth.RemoveSiteAuditConfirmed = false;
      }
    });

    return await pop.present();
  }

  async RemoveSiteAuditConfirmed( id: any )
  {
    await this.auth.DeleteSiteAudit( id, "Deleting site audit..." );

    var i = this.auth.SiteAudits.findIndex( p => p.Id == id );

    this.auth.SiteAudits.splice( i, 1 );

    this.auth.GoToPage( "siteaudit", true );
  }
}
