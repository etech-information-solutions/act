import { Component, OnInit, ViewChild } from '@angular/core';
import { NavController, MenuController, AlertController, IonContent, ActionSheetController } from '@ionic/angular';
import { UserService } from '../services/user.service';
import { Camera, CameraOptions } from '@ionic-native/camera/ngx';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.page.html',
  styleUrls: ['./settings.page.scss'],
})
export class SettingsPage implements OnInit 
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;
  
  DisableBranch:boolean = false;

  // Form data
  Pin: string;
  Name: string;
  Surname: string;

  Cell: string;
  Email: string;

  AcceptTnC: boolean;

  // Uploads
  SelfieLoaded: boolean = false;
  IdPassportLoaded: boolean = false;

  SelfieUrl: string = "../assets/imgs/no-preview.png";

  AppName: any;
  AppVersion: any;
  AppVersionCode: any;

  constructor( public navCtrl: NavController, public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService, public camera: Camera, public asCtrl: ActionSheetController ) 
  { 
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.navCtrl.navigateRoot( "login" );
    }

    this.menuCtrl.enable( true );

    this.Set();
  }

  ngOnInit() 
  {
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async Set()
  {
    if ( this.auth.CurrentUser.SelfieUrl != undefined && this.auth.CurrentUser.SelfieUrl != null && this.auth.CurrentUser.SelfieUrl != "" )
    {
      this.SelfieUrl = this.auth.APIUrl + this.auth.CurrentUser.SelfieUrl;
    }

    // My Details
    this.Name = this.auth.CurrentUser.Name;
    this.Surname = this.auth.CurrentUser.Surname;

    // Contact Details
    this.Cell = this.auth.CurrentUser.Cell;
    this.Email = this.auth.CurrentUser.Email;
  }

  UploadDocument( source, isSelfie )
  {
    const options: CameraOptions =
    {
      quality: 100,
      destinationType: this.camera.DestinationType.DATA_URL,
      encodingType: this.camera.EncodingType.JPEG,
      mediaType: this.camera.MediaType.PICTURE,
      correctOrientation: true,
      sourceType: source
    };
    
    this.camera.getPicture( options ).then( ( imageData ) =>
    {
      // imageData is either a base64 encoded string or a file URI
      // If it's base64 (DATA_URL):

      if ( isSelfie )
      {
        this.SelfieUrl = 'data:image/jpeg;base64,' + imageData;
        this.SelfieLoaded = true;
      }
    }, ( err ) =>
    {
      this.SelfieLoaded = ( isSelfie ) ? false: this.SelfieLoaded;

     this.auth.ShowError( JSON.stringify( err ) );
    });
  }

  async Update()
  {
    if ( !this.Valid() )
    {
      return;
    }

    var agent = this.Construct();

    await this.auth.Update( agent, "Updating your account..." );

    if ( this.auth.UpdatedUser != undefined && this.auth.UpdatedUser.Code == 1 )
    {
      var agentId = this.auth.UpdatedUser.Id;
      var msg = this.auth.UpdatedUser.Message;

      // Upload Selfie!
      if ( this.SelfieLoaded )
      {
        await this.auth.UploadDocument( agentId, this.SelfieUrl, "file", "selfie.jpg", "image/jpeg", "Uploading your selfie..." );

        if ( this.auth.UpdatedUser != null && this.auth.UpdatedUser.Code == -1 )
        {
          msg = `${msg} BUT, your selfie could not be uploaded. Don't you worry about it for now...`;
        }
      }

      // Notify agent
      this.auth.ShowMessage( "Hoooray...!!!", msg );
    }
    else if ( this.auth.UpdatedUser != undefined  && this.auth.UpdatedUser.Message != "" )
    {
      this.auth.ShowMessage( "Error", this.auth.UpdatedUser.Message );
    }
  }

  async PresentPhotoOptions( isSelfie: boolean )
  {
    const acc = await this.asCtrl.create
    ({
      buttons: [{
        text: 'Take Photo',
        icon: 'camera',
        handler: () =>
        {
          acc.dismiss();

          this.UploadDocument( 1, isSelfie );
        }
      },{
        text: 'Choose Photo',
        icon: 'images',
        handler: () =>
        {
          acc.dismiss();

          this.UploadDocument( 0, isSelfie );
        }
      },{ 
        icon: 'close',
        text: 'Cancel', 
        role: 'cancel' 
      }]
    });

    await acc.present();
  }

  Valid()
  {
    var valid = true;

    // Required inputs
    if ( this.Name === undefined || this.Surname === undefined || this.Cell === undefined || this.Email === undefined )
    {
      this.auth.ShowError( "Please complete all required '*' inputs" );

      return false;
    }

    // Checked TnC?
    if ( !this.AcceptTnC )
    {
      this.auth.ShowMessage( "Terms & Conditions", "One last critical thing, please Accept our T & C for signing up with us." );

      return false;
    }

    return valid;
  }

  private Construct()
  {
    var agent =`Id=${this.auth.CurrentUser.Id}&Name=${this.Name}&Surname=${this.Surname}&Email=${this.Email}&Cell=${this.Cell}&IsAccpetedTC=${this.AcceptTnC}`

    return agent;
  }

  async Refresh( refresher )
  {
    await this.Set();
    
    refresher.target.complete();
  }

  async ShowTnC()
  {
    var tnc = "Some sleak awesome Terms and Conditions you cannot help but accept.";

    this.auth.ShowMessage( "Terms and Conditions", tnc );
  }
  
  async GoToPage( page:string )
  {
    if ( page == 'home' )
    {
      this.navCtrl.navigateRoot( page );
    }
    else
    {
      this.navCtrl.navigateForward( page );
    }
  }

}
