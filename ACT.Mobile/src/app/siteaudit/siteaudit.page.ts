import { Component, OnInit, ViewChild } from '@angular/core';
import { MenuController, IonContent, AlertController, PopoverController } from '@ionic/angular';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-siteaudit',
  templateUrl: './siteaudit.page.html',
  styleUrls: ['./siteaudit.page.scss'],
})
export class SiteauditPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;

  SiteAudits: any = [];

  Skip: number = 0;
  Query: string = "";

  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService, public popCtrl: PopoverController )
  { 
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }

    this.menuCtrl.enable( true );
  }

  async ngOnInit()
  {
    await this.auth.SetPSPs();
    await this.auth.SetSites();
    await this.auth.SetClients();
    await this.GetSiteAudits();
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async ionViewWillEnter()
  {
    if ( this.auth.RefreshSiteAudits )
    {
      this.auth.RefreshShipments = false;

      this.Skip = 0;
      this.SiteAudits = [];
      
      await this.GetSiteAudits();
      
      //window.location.reload();
    }
  }

  async GetSiteAudits()
  {
    var not = await this.auth.GetSiteAudits( this.Query );

    var useSiteAudits = this.auth.SiteAudits;

    if ( this.Query != "" )
    {
      useSiteAudits = not;
    }

    if ( not != undefined && not.length > this.SiteAudits.length )
    {
      not = [];

      var c = this.auth.Take;

      for ( var i = this.Skip; i < useSiteAudits.length; i++ )
      {
        if ( c <= 0 )
        {
          break;
        }

        c--;

        not.push( useSiteAudits[ i ] );
      }

      this.Skip += not.length;
  
      this.SiteAudits = this.SiteAudits.concat( not );
    }
  }

  async loadData( event:any )
  {
    await this.GetSiteAudits();

    event.target.complete();
  }

  async Search()
  {
    this.Skip = 0;
    this.SiteAudits = [];

    await this.GetSiteAudits();
  }

  async Refresh( refresher )
  {
    await this.GetSiteAudits();
    
    refresher.target.complete();
  }

  async GoToSiteAudit ( id:number )
  {
    var i = this.auth.SiteAudits.findIndex( n => n.Id == id );

    this.auth.SiteAudit = this.auth.SiteAudits[ i ];

    this.auth.GoToPage( "siteauditdetail" );
  }
  
  async Add()
  {
    this.auth.GoToPage( "addsiteaudit" );
  }

}
