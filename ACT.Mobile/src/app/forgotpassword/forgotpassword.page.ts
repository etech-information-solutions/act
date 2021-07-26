import { Component, OnInit, ViewChild } from '@angular/core';
import { NavController, MenuController, AlertController, IonContent } from '@ionic/angular';

import { UserService } from '../services/user.service';

@Component({
  selector: 'app-forgotpassword',
  templateUrl: './forgotpassword.page.html',
  styleUrls: ['./forgotpassword.page.scss'],
})
export class ForgotpasswordPage implements OnInit 
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;
  
  Email: any;

  constructor( public navCtrl: NavController, public auth: UserService, public menuCtrl: MenuController, public alertCtrl: AlertController ) 
  { 
    if ( this.auth.CurrentUser != null && this.auth.CurrentUser.Id > 0 )
    {
      this.navCtrl.navigateRoot( "home" );
    }

    this.menuCtrl.enable(false);
  }

  ngOnInit() 
  {

  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }


  async ForgotPassword()
  {
    await this.auth.ForgotPassword( this.Email );

    if ( this.auth.UpdatedUser != undefined && this.auth.UpdatedUser.Code == 1)
    {
      this.Email = undefined;

      this.ShowMessage( "Email Sent", this.auth.UpdatedUser.Message );
    }
    else if ( this.auth.UpdatedUser != undefined  && this.auth.UpdatedUser.Message != "" )
    {
      this.ShowMessage( "Oops!", this.auth.UpdatedUser.Message );
    }
  }

  async ShowMessage( title:string, message:string ) 
  {
    let alert = await this.alertCtrl.create({
      header: title,
      message: message,
      buttons: ['OK']
    });
    
    await alert.present();
  }
}
