import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController, PopoverController } from '@ionic/angular';
import { UserService } from '../services/user.service';

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
  CustomerSignature: any;

  RepName: any;
  RepSignature: any;

  PalletAuditor: any;
  PalletAuditorSign: any;

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
    this.CustomerSignature = this.auth.SiteAudit.CustomerSignature;

    this.RepName = this.auth.SiteAudit.RepName;
    this.RepSignature = this.auth.SiteAudit.RepSignature;

    this.PalletAuditor = this.auth.SiteAudit.PalletAuditor;
    this.PalletAuditorSign = this.auth.SiteAudit.PalletAuditorSign;
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async Update()
  {

  }

  async Delete()
  {

  }
}
