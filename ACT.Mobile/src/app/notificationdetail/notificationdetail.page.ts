import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController } from '@ionic/angular';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-notificationdetail',
  templateUrl: './notificationdetail.page.html',
  styleUrls: ['./notificationdetail.page.scss'],
})
export class NotificationdetailPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;
  

  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService )
  {
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }

    if ( this.auth.Notification == undefined )
    {
      this.auth.GoToPage( "notifications", true );
    }

    this.menuCtrl.enable( true );

    this.MarkNotificationAsRead();
  }


  ngOnInit() 
  {

  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async MarkNotificationAsRead()
  {
    if ( this.auth.Notification.Status == 1 )
    {
      return;
    }

    this.auth.RefreshNotifications = true;
    
    await this.auth.MarkNotificationAsRead( this.auth.Notification.Id );
  }

}
