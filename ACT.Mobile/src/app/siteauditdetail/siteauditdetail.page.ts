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
    this.CustomerSignatureUrl = this.auth.SiteAudit.CustomerSignature;

    this.RepName = this.auth.SiteAudit.RepName;
    this.RepSignatureUrl = this.auth.SiteAudit.RepSignature;

    this.PalletAuditor = this.auth.SiteAudit.PalletAuditor;
    this.PalletAuditorSignUrl = this.auth.SiteAudit.PalletAuditorSign;
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;

    this.CustomerSignaturePad = new SignaturePad( this.csignaturePad.nativeElement );
    this.RepSignaturePad = new SignaturePad( this.rsignaturePad.nativeElement );
    this.PalletAuditorSignPad = new SignaturePad( this.psignaturePad.nativeElement );

    if ( this.CustomerSignatureUrl != null )
    {
      const blob = new Blob( [ this.CustomerSignatureUrl ], { type: "image/jpeg" } );
      const urlCreator = window.URL || window.webkitURL;
      var base64 = urlCreator.createObjectURL( blob );

      this.CustomerSignaturePad.fromDataURL( base64 );
    }
    if ( this.RepSignatureUrl != null )
    {
      const blob = new Blob( [ this.RepSignatureUrl ], { type: "image/jpeg" } );
      const urlCreator = window.URL || window.webkitURL;
      var base64 = urlCreator.createObjectURL( blob );

      this.RepSignaturePad.fromDataURL( base64 );
    }
    if ( this.PalletAuditorSignUrl != null )
    {
      const blob = new Blob( [ this.PalletAuditorSignUrl ], { type: "image/jpeg" } );
      const urlCreator = window.URL || window.webkitURL;
      var base64 = urlCreator.createObjectURL( blob );

      this.PalletAuditorSignPad.fromDataURL( base64 );
    }
  }

  ClearPad( sig: number )
  {
    if ( sig == 1 )
    {
      this.CustomerSignaturePad.clear();
    }
    else if ( sig == 2 )
    {
      this.RepSignaturePad.clear();
    }
    else if ( sig == 3 )
    {
      this.PalletAuditorSignPad.clear();
    }
  }
  
  async Update()
  {
    if ( !this.Valid() )
    {
      return;
    }
    
    var audit = this.Construct();

    var resp = await this.auth.UpdateSiteAudit( audit, "Updating your Site Audit..." );

    if ( typeof( resp.Code ) != undefined && resp.Code  == -1 )
    {
      this.auth.ShowError( resp.Message );
      
      return;
    }

    var i = this.auth.SiteAudits.findIndex( p => p.Id == this.auth.SiteAudit.Id );

    this.auth.SiteAudits[ i ] = resp;

    if ( !this.CustomerSignaturePad.isEmpty() )
    {
      await this.auth.UploadSignature( this.auth.SiteAudit.Id, 1, this.CustomerSignaturePad.toDataURL( "image/jpeg" ), "file", "siteaudit-customer-signature.jpg", "image/jpeg", "Uploading Customer Signature.." );
    }
    if ( !this.RepSignaturePad.isEmpty() )
    {
      await this.auth.UploadSignature( this.auth.SiteAudit.Id, 2, this.RepSignaturePad.toDataURL( "image/jpeg" ), "file", "siteaudit-rep-signature.jpg", "image/jpeg", "Uploading Sales Rep Signature.." );
    }
    if ( !this.PalletAuditorSignPad.isEmpty() )
    {
      await this.auth.UploadSignature( this.auth.SiteAudit.Id, 3, this.PalletAuditorSignPad.toDataURL( "image/jpeg" ), "file", "siteaudit-pallet-auditor-signature.jpg", "image/jpeg", "Uploading Pallet Auditor Signature.." );
    }

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
