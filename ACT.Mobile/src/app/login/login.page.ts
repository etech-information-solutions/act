import { Component, OnInit, ViewChild } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { NavController, MenuController, AlertController, IonContent, ModalController, PopoverController, Platform } from '@ionic/angular';

import { FingerprintAIO } from '@ionic-native/fingerprint-aio/ngx';
import { CallNumber } from '@ionic-native/call-number/ngx';

import { UserService } from '../services/user.service';
import { CreatepinPage } from '../createpin/createpin.page';
import { ResetpinconfirmComponent } from '../resetpinconfirm/resetpinconfirm.component';
import { ExitappconfirmComponent } from '../exitappconfirm/exitappconfirm.component';
import { ContactactComponent } from '../contactact/contactact.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
})
export class LoginPage implements OnInit 
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;
  
  Pin: string = "";
  Email:  string = "";
  Password:  string = "";
  FingerPrintAvailable: boolean = false;

  SelfieUrl: string = "../../assets/imgs/user-profile.png";

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

  Subscription:any;

  constructor( public navCtrl: NavController, public activatedRoute: ActivatedRoute, public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService, public fp: FingerprintAIO, public mCtrl: ModalController, public popCtrl: PopoverController, public plat: Platform ) 
  {
    if ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id > 0 )
    {
      this.navCtrl.navigateRoot( "home" );
    }

    this.menuCtrl.enable( false );
  }

  async ngOnInit()
  {
    await this.plat.ready();
    await this.auth.SetDeviceUser();

    if ( this.auth.DeviceUser != undefined )
    {
      const available = await this.fp.isAvailable();
  
      if ( available == "OK" || available == "finger" || available == "face" || available == "biometric" )
      {
        this.FingerPrintAvailable = true;
      }
      else
      {
        this.ShowMessage( "Fingerprint Outcome", available );
      }
    }

    this.auth.ExitApp = false;
  }

  ionViewDidEnter()
  {
    this.Subscription = this.plat.backButton.subscribe( () =>
    {
      this.OnExitAppConfirm();
    });
  }

  ionViewWillLeave()
  {
    this.Subscription.unsubscribe();
  }

  async OnExitAppConfirm()
  {
    const pop = await this.popCtrl.create
    ({
      translucent: true,
      backdropDismiss: false,
      component: ExitappconfirmComponent
    });

    pop.onDidDismiss().then( ( r ) => 
    {
      if ( this.auth.ExitApp )
      {
        navigator[ "app" ].exitApp();
      }
    });

    return await pop.present();
  }

  async ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async ShowFingerPrint()
  {
    try
    {
      this.fp.show
      ({
        title: 'Biometric Authentication', // (Android Only) | optional | Default: "<APP_NAME> Biometric Sign On"
        //clientSecret: 'AIzaSyAeQFyrVUFUjsQOK',
        subtitle: 'Please authenticate', // optional | Default: null
        fallbackButtonTitle: 'Use Pin', // optional | When disableBackup is false defaults to "Use Pin".
        description: 'Please authenticate',
        disableBackup: false,  // optional | default: false
      })
      .then( ( result: any ) => 
      {
        if ( result == "success" )
        {
          this.auth.CurrentUser = this.auth.DeviceUser;

          this.navCtrl.navigateRoot( "home" );
        }
        else
        {
          // Fingerprint/Face was not successfully verified
          this.ShowMessage( "Oops!", JSON.stringify( result ) );
        }
      })
      .catch( ( error: any ) => 
      {
        // Fingerprint/Face was not successfully verified
        this.ShowMessage( "Oops!", JSON.stringify( error ) );
      });
    }
    catch( err )
    {
      this.ShowMessage( "Oops!", JSON.stringify( err ) );
    }
  }

  async SetPin( n: number )
  {
    var l = 0;

    if ( n > -1 )
    {
      this.Pin += n + "";

      l = this.Pin.length;
    }
    else if ( this.Pin.length > 0 )
    {
      var arr = this.Pin.split( "" );
      
      arr.splice( arr.length -1, 1 );

      this.Pin = arr.join( "" );

      l = this.Pin.length + 1;
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

    if ( this.Pin.length == 5 )
    {
      await this.PinIn();
    }
  }

  async PinIn()
  {
    await this.auth.PinIn( this.Pin );

    if ( this.auth.UpdatedUser != undefined && this.auth.UpdatedUser.Code == 1 )
    {
      this.navCtrl.navigateRoot( "home" );
    }
    else if ( this.auth.UpdatedUser != undefined  && this.auth.UpdatedUser.Message != "" )
    {
      this.Restart();

      this.ShowMessage( "Oops", this.auth.UpdatedUser.Message );
    }
  }
  

  async Restart()
  {
    this.Pin = "";
    this.Pin1 = this.Pin2 = this.Pin3 = this.Pin4 = this.Pin5 = "../../assets/imgs/open-circle.png";
  }

  async SignIn()
  {
    await this.auth.Login( { email: this.Email, password: this.Password } );

    if ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Code == 1)
    {
      this.Email = undefined;
      this.Password = undefined;

      await this.DoCreatePin();
    }
    else if ( this.auth.CurrentUser != undefined  && this.auth.CurrentUser.Message != "" )
    {
      this.ShowMessage( "Error", this.auth.CurrentUser.Message );
    }
  }

  async DoCreatePin()
  {
    const modal = await this.mCtrl.create
    ({
      backdropDismiss: false,
      component: CreatepinPage
    });

    modal.onDidDismiss().then( (r) => 
    {
      
    });

    return await modal.present();
  }

  SignUp()
  {
    this.navCtrl.navigateForward( "register" );
  }

  async ForgotPin()
  {
    const pop = await this.popCtrl.create
    ({
      translucent: true,
      backdropDismiss: false,
      component: ResetpinconfirmComponent
    });

    pop.onDidDismiss().then( (r) => 
    {
      if ( this.auth.ResetPin )
      {
        this.auth.RemovePin();
      }
    });

    return await pop.present();
  }

  ForgotPassword()
  {
    this.navCtrl.navigateForward( "forgotpassword" );
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
