import { Component, OnInit, ViewChild } from '@angular/core';
import { ActionSheetController, AlertController, IonContent, MenuController } from '@ionic/angular';
import { UserService } from '../services/user.service';
import { Camera, CameraOptions } from '@ionic-native/camera/ngx';

@Component({
  selector: 'app-outstandingshipmentdetail',
  templateUrl: './outstandingshipmentdetail.page.html',
  styleUrls: ['./outstandingshipmentdetail.page.scss'],
})
export class OutstandingshipmentdetailPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;

  PCNNumber: any;
  PODNumber: any;
  PRNNumber: any;

  // Uploads
  PCNUrl: any = "../assets/imgs/no-preview.png";
  PODUrl: any = "../assets/imgs/no-preview.png";
  PRNUrl: any = "../assets/imgs/no-preview.png";

  // Comments
  PCNComment: any;
  PODComment: any;
  PRNComment: any;

  PCNLoaded: boolean = false;
  PODLoaded: boolean = false;
  PRNLoaded: boolean = false;

  PreviewUrl: string = "../assets/imgs/no-preview.png";


  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService, public camera: Camera, public asCtrl: ActionSheetController )
  {
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }

    if ( this.auth.OutstandingShipment == undefined )
    {
      this.auth.GoToPage( "podmanagement", true );
    }

    this.menuCtrl.enable( true );

    this.Set();
  }

  async ngOnInit()
  {
    
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async Set()
  {
    if ( this.auth.OutstandingShipment.Images != undefined && this.auth.OutstandingShipment.Images.length > 0 )
    {
      // Current images
      this.auth.OutstandingShipment.Images.forEach( d =>
      {
        if ( d.ObjectType == "PCNNumber" )
        {
          this.PCNComment = d.Description;
          this.PCNUrl = this.auth.APIUrl + "Account/ViewImage?id=" + d.Id;
        }
        else if ( d.ObjectType == "PODNumber" )
        {
          this.PODComment = d.Description;
          this.PODUrl = this.auth.APIUrl + "Account/ViewImage?id=" + d.Id;
        }
        else if ( d.ObjectType == "PRNNumber" )
        {
          this.PRNComment = d.Description;
          this.PRNUrl = this.auth.APIUrl + "Account/ViewImage?id=" + d.Id;
        }
      });
    }

    // Shipment Details
    this.PCNNumber = this.auth.OutstandingShipment.PCNNumber;
    this.PODNumber = this.auth.OutstandingShipment.PODNumber;
    this.PRNNumber = this.auth.OutstandingShipment.PRNNumber;
  }

  async PresentPhotoOptions( type: number )
  {
    const acc = await this.asCtrl.create
    ({
      buttons: [{
        text: 'Take Photo',
        icon: 'camera',
        handler: () =>
        {
          acc.dismiss();

          this.UploadDocument( 1, type );
        }
      },{
        text: 'Choose Photo',
        icon: 'images',
        handler: () =>
        {
          acc.dismiss();

          this.UploadDocument( 0, type );
        }
      },{ 
        icon: 'close',
        text: 'Cancel', 
        role: 'cancel' 
      }]
    });

    await acc.present();
  }

  UploadDocument( source: number, type: number )
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

      if ( type == 1 )
      {
        this.PCNUrl = 'data:image/jpeg;base64,' + imageData;
        this.PCNLoaded = true;
      }
      else if ( type == 2 )
      {
        this.PODUrl = 'data:image/jpeg;base64,' + imageData;
        this.PODLoaded = true;
      }
      else if ( type == 3 )
      {
        this.PRNUrl = 'data:image/jpeg;base64,' + imageData;
        this.PRNLoaded = true;
      }
    }, ( err ) =>
    {
      this.PCNLoaded = ( type == 1 ) ? false: this.PCNLoaded;
      this.PODLoaded = ( type == 2 ) ? false: this.PODLoaded;
      this.PRNLoaded = ( type == 3 ) ? false: this.PRNLoaded;

     this.auth.ShowError( JSON.stringify( err ) );
    });
  }

  async Update()
  {
    if ( !this.Valid() )
    {
      return;
    }

    var load = this.Construct();

    await this.auth.UpdateShipment( load, "Updating your shipment details..." );

    if ( this.auth.ServerResult != undefined && this.auth.ServerResult.Code == 1 )
    {
      // Upload PCN!
      if ( this.PCNLoaded )
      {
        await this.auth.UploadShipment( this.auth.OutstandingShipment.Id, "PCNNumber", this.PCNComment, this.PCNUrl, "file", "pcn.jpg", "image/jpeg", "Uploading your PCN..." );
      }
      // Upload POD!
      if ( this.PODLoaded )
      {
        await this.auth.UploadShipment( this.auth.OutstandingShipment.Id, "PODNumber", this.PODComment, this.PODUrl, "file", "pod.jpg", "image/jpeg", "Uploading your POD..." );
      }
      // Upload PRN!
      if ( this.PRNLoaded )
      {
        await this.auth.UploadShipment( this.auth.OutstandingShipment.Id, "PRNNumber", this.PRNComment, this.PRNUrl, "file", "prn.jpg", "image/jpeg", "Uploading your PRN..." );
      }

      this.auth.RefreshShipments = true;

      this.auth.GoToPage( "podmanagement", true );
    }
    else if ( this.auth.ServerResult != undefined  && this.auth.ServerResult.Message != "" )
    {
      this.auth.ShowMessage( "Error", this.auth.ServerResult.Message );
    }
  }

  Valid()
  {
    var valid = true;

    // Required inputs
    if ( this.PODNumber === undefined && this.PCNNumber === undefined && this.PRNNumber === undefined )
    {
      this.auth.ShowError( "Nothing has been entered, please enter details for POD, PCN, and PRN" );

      return false;
    }

    // Any uploads
    if ( !this.PODLoaded || !this.PCNLoaded || !this.PRNLoaded )
    {
      this.auth.ShowError( "Please upload snapshots of POD, PCN, and PRN" );
      
      return false;
    }

    return valid;
  }

  Construct()
  {
    var load =`Id=${this.auth.OutstandingShipment.Id}&PODNumber=${this.PODNumber}&PODComment=${this.PODComment}&PCNNumber=${this.PCNNumber}&PCNComment=${this.PCNComment}&PRNNumber=${this.PRNNumber}&PRNComment=${this.PRNComment}`

    return load;
  }

}
