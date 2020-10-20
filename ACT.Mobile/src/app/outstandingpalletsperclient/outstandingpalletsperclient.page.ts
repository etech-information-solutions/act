import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { AlertController, MenuController } from '@ionic/angular';

@Component({
  selector: 'app-outstandingpalletsperclient',
  templateUrl: './outstandingpalletsperclient.page.html',
  styleUrls: ['./outstandingpalletsperclient.page.scss'],
})
export class OutstandingpalletsperclientPage implements OnInit
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
    await this.ListOutstandingPalletsPerClient();
  }

  async ListOutstandingPalletsPerClient()
  {
    await this.auth.ListOutstandingPalletsPerClient();
  }

}
