import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { AlertController, MenuController } from '@ionic/angular';

@Component({
  selector: 'app-outstandingpalletspersite',
  templateUrl: './outstandingpalletspersite.page.html',
  styleUrls: ['./outstandingpalletspersite.page.scss'],
})
export class OutstandingpalletspersitePage implements OnInit
{
  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService )
  {
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }

    this.menuCtrl.enable( true );
  }

  async ngOnInit()
  {
    await this.ListOutstandingPalletsPerSite();
  }

  async ListOutstandingPalletsPerSite()
  {
    await this.auth.ListOutstandingPalletsPerSite();
  }

}
