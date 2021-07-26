import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController, PopoverController } from '@ionic/angular';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-outstandingreport',
  templateUrl: './outstandingreport.page.html',
  styleUrls: ['./outstandingreport.page.scss'],
})
export class OutstandingreportPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;

  OutstandingPallets: any = [];

  Skip: number = 0;
  Query: string = "";
  
  Years: any = [];
  ThisYear: number = new Date().getFullYear();
  OldestYear: number = new Date().getFullYear();


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
    await this.GetOutstandingPallets();
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async GetOutstandingPallets()
  {
    var not = await this.auth.GetOutstandingPallets( this.Query );

    var useOutstandingPallets = this.auth.OutstandingPallets;

    if ( this.Query != "" )
    {
      useOutstandingPallets = not;
    }

    if ( not != undefined && not.length > this.OutstandingPallets.length )
    {
      not = [];

      var c = this.auth.Take;

      for ( var i = this.Skip; i < useOutstandingPallets.length; i++ )
      {
        if ( c <= 0 )
        {
          break;
        }

        c--;

        not.push( useOutstandingPallets[ i ] );
      }

      this.Skip += not.length;
  
      this.OutstandingPallets = this.OutstandingPallets.concat( not );
    }
  }

  async loadData( event:any )
  {
    await this.GetOutstandingPallets();

    event.target.complete();
  }

  async Search()
  {
    this.Skip = 0;
    this.OutstandingPallets = [];

    await this.GetOutstandingPallets();
  }

  async Refresh( refresher )
  {
    await this.GetOutstandingPallets();
    
    refresher.target.complete();
  }
}
