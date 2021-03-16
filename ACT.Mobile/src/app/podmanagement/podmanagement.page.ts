import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController, PopoverController } from '@ionic/angular';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-podmanagement',
  templateUrl: './podmanagement.page.html',
  styleUrls: ['./podmanagement.page.scss'],
})
export class PodmanagementPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;

  OutstandingShipments: any = [];

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
    await this.GetOutstandingShipments();
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async ionViewWillEnter()
  {
    if ( this.auth.RefreshShipments )
    {
      this.auth.RefreshShipments = false;

      this.Skip = 0;
      this.OutstandingShipments = [];
      
      await this.GetOutstandingShipments();
      
      //window.location.reload();
    }
  }

  async GetOutstandingShipments()
  {
    var not = await this.auth.GetOutstandingShipments( this.Query );

    var useOutstandingShipments = this.auth.OutstandingShipments;

    if ( this.Query != "" )
    {
      useOutstandingShipments = not;
    }

    if ( not != undefined && not.length > this.OutstandingShipments.length )
    {
      not = [];

      var c = this.auth.Take;

      for ( var i = this.Skip; i < useOutstandingShipments.length; i++ )
      {
        if ( c <= 0 )
        {
          break;
        }

        c--;

        not.push( useOutstandingShipments[ i ] );
      }

      this.Skip += not.length;
  
      this.OutstandingShipments = this.OutstandingShipments.concat( not );
    }
  }

  async loadData( event:any )
  {
    await this.GetOutstandingShipments();

    event.target.complete();
  }

  async Search()
  {
    this.Skip = 0;
    this.OutstandingShipments = [];

    await this.GetOutstandingShipments();
  }

  async Refresh( refresher )
  {
    await this.GetOutstandingShipments();
    
    refresher.target.complete();
  }

  async GoToOutstandingShipment ( id:number )
  {
    var i = this.auth.OutstandingShipments.findIndex( n => n.Id == id );

    this.auth.OutstandingShipment = this.auth.OutstandingShipments[ i ];

    this.auth.GoToPage( "outstandingshipmentdetail" );
  }

}
