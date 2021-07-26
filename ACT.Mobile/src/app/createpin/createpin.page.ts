import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { ModalController, NavController, AlertController } from '@ionic/angular';

@Component({
  selector: 'app-createpin',
  templateUrl: './createpin.page.html',
  styleUrls: ['./createpin.page.scss'],
})
export class CreatepinPage implements OnInit 
{
  Pin: string = "";
  ConfirmPin: string = "";
  ConfirmingPin: boolean = false;
  ConfirmPinFailed: boolean = false;

  Pin1:string = "../../assets/imgs/open-circle.png";
  Pin2:string = "../../assets/imgs/open-circle.png";
  Pin3:string = "../../assets/imgs/open-circle.png";
  Pin4:string = "../../assets/imgs/open-circle.png";
  Pin5:string = "../../assets/imgs/open-circle.png";

  
  Number1:string = "../../assets/imgs/n1.png";
  Number2:string = "../../assets/imgs/n2.png";
  Number3:string = "../../assets/imgs/n3.png";
  Number4:string = "../../assets/imgs/n4.png";
  Number5:string = "../../assets/imgs/n5.png";
  Number6:string = "../../assets/imgs/n6.png";
  Number7:string = "../../assets/imgs/n7.png";
  Number8:string = "../../assets/imgs/n8.png";
  Number9:string = "../../assets/imgs/n9.png";
  Number0:string = "../../assets/imgs/n0.png";
  NumberB:string = "../../assets/imgs/nb.png";

  SelfieUrl: string = "../../assets/imgs/user-profile.png";

  constructor( public auth: UserService, public mCtrl: ModalController, public navCtrl: NavController, public alertCtrl: AlertController ) 
  { 

  }

  ngOnInit() 
  {
    if ( this.auth.DeviceUser != undefined && this.auth.DeviceUser.SelfieUrl != undefined )
    {
      this.SelfieUrl = this.auth.APIUrl + this.auth.DeviceUser.SelfieUrl;
    }
  }

  async SetPin( n: number )
  {
    this.ConfirmPinFailed = false;

    var l = 0;

    if ( n > -1 )
    {
      if ( this.ConfirmingPin )
      {
        this.ConfirmPin += n + "";

        l = this.ConfirmPin.length;
      }
      else
      {
        this.Pin += n + "";

        l = this.Pin.length;
      }
    }
    else if( this.Pin.length > 0 || this.ConfirmPin.length > 0 )
    {
      var arr = ( this.ConfirmingPin ) ? this.ConfirmPin.split( "" ) : this.Pin.split( "" );
      
      arr.splice( arr.length -1, 1 );

      if( this.ConfirmingPin )
      {
        this.ConfirmPin = arr.join( "" );

        l = this.ConfirmPin.length + 1;
      }
      else
      {
        this.Pin = arr.join( "" );

        l = this.Pin.length + 1;
      }
    }

    switch( l )
    {
      case 1:
        this.Pin1 = ( n > -1 ) ? "../../assets/imgs/closed-circle.png" : "../../assets/imgs/open-circle.png";
      break;

      case 2:
        this.Pin2 = ( n > -1 ) ? "../../assets/imgs/closed-circle.png" : "../../assets/imgs/open-circle.png";
      break;

      case 3:
        this.Pin3 = ( n > -1 ) ? "../../assets/imgs/closed-circle.png" : "../../assets/imgs/open-circle.png";
      break;

      case 4:
        this.Pin4 = ( n > -1 ) ? "../../assets/imgs/closed-circle.png" : "../../assets/imgs/open-circle.png";
      break;

      case 5:
        this.Pin5 = ( n > -1 ) ? "../../assets/imgs/closed-circle.png" : "../../assets/imgs/open-circle.png";
      break;
    }

    if ( this.Pin.length == 5 && this.ConfirmPin.length == 0 )
    {
      await this.SetConfirmPin();
    }
    else if ( this.Pin.length == 5 && this.ConfirmPin.length == 5 && ( this.Pin != this.ConfirmPin ) )
    {
      await this.Restart();

      this.ConfirmPinFailed = true;
    }
    else if ( this.Pin.length == 5 && this.ConfirmPin.length == 5 && ( this.Pin == this.ConfirmPin ) )
    {
      this.ConfirmPinFailed = false;

      this.UpdatePin();
    }
  }

  async SetConfirmPin()
  {
    this.ConfirmingPin = true;

    this.Pin1 = this.Pin2 = this.Pin3 = this.Pin4 = this.Pin5 = "../../assets/imgs/open-circle.png";
  }

  async Restart()
  {
    this.ConfirmingPin = false;
    this.auth.PinCreated = false;
    this.ConfirmPinFailed = false;

    this.Pin = this.ConfirmPin = "";
    this.Pin1 = this.Pin2 = this.Pin3 = this.Pin4 = this.Pin5 = "../../assets/imgs/open-circle.png";
  }

  async UpdatePin()
  {
    await this.auth.UpdatePin( this.Pin, this.ConfirmPin );

    if ( this.auth.UpdatedUser.Code != 1  )
    {
      this.ShowMessage( "Oops!", this.auth.UpdatedUser.Message );

      this.Restart();

      return;
    }

    this.auth.PinCreated = true;
    this.auth.PinCreationSkipped = false;

    this.navCtrl.navigateRoot( "home" );
    this.mCtrl.dismiss();
  }

  async Skip()
  {
    this.auth.PinCreated = false;
    this.auth.PinCreationSkipped = true;

    this.mCtrl.dismiss();
    this.navCtrl.navigateRoot( "home" );
  }

  async ShowMessage( title:string, message:string ) 
  {
    let alert = await this.alertCtrl.create(
    {
      header: title,
      message: message,
      buttons: [ 'OK' ]
    });

    await alert.present();
  }
}
