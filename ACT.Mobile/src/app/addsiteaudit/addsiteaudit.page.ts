import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController, PopoverController } from '@ionic/angular';
import SignaturePad from 'signature_pad';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-addsiteaudit',
  templateUrl: './addsiteaudit.page.html',
  styleUrls: ['./addsiteaudit.page.scss'],
})
export class AddsiteauditPage implements OnInit
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

    this.menuCtrl.enable( true );
  }

  ngOnInit()
  {
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
  
  async Create()
  {
    if ( !this.Valid() )
    {
      return;
    }
    
    var audit = this.Construct();

    var resp = await this.auth.CreateSiteAudit( audit, "Creating your Site Audit..." );

    if ( typeof( resp.Code ) != undefined && resp.Code  == -1 )
    {
      this.auth.ShowError( resp.Message );
      
      return;
    }

    this.auth.SiteAudits.push( resp );

    if ( !this.CustomerSignaturePad.isEmpty() )
    {
      await this.auth.UploadSignature( resp.Id, "CustomerSignature", this.CustomerSignaturePad.toDataURL( "image/png" ), "file", "siteaudit-customer-signature.png", "image/png", "Uploading Customer Signature.." );
    }
    if ( !this.RepSignaturePad.isEmpty() )
    {
      await this.auth.UploadSignature( resp.Id, "RepSignature", this.RepSignaturePad.toDataURL( "image/png" ), "file", "siteaudit-rep-signature.png", "image/png", "Uploading Sales Rep Signature.." );
    }
    if ( !this.PalletAuditorSignPad.isEmpty() )
    {
      await this.auth.UploadSignature( resp.Id, "PalletAuditorSignature", this.PalletAuditorSignPad.toDataURL( "image/png" ), "file", "siteaudit-pallet-auditor-signature.png", "image/png", "Uploading Pallet Auditor Signature.." );
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

    var audit =`SiteId=${this.SiteId}&ClientId=${this.ClientId}&AuditDate=${aa}&Equipment=${this.Equipment}&PalletsOutstanding=${this.PalletsOutstanding}&PalletsCounted=${this.PalletsCounted}&WriteoffPallets=${this.WriteoffPallets}&CustomerName=${this.CustomerName}&RepName=${this.RepName}&PalletAuditor=${this.PalletAuditor}`;

    return audit;
  }
}
