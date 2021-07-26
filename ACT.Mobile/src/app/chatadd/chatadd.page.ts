import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController, PopoverController } from '@ionic/angular';
import { ChatService } from '../services/chat.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-chatadd',
  templateUrl: './chatadd.page.html',
  styleUrls: ['./chatadd.page.scss'],
})
export class ChataddPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;

  Details: any;
  DepartmentId: any;

  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService, public chatservice: ChatService, public popCtrl: PopoverController )
  {
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }
    
    this.menuCtrl.enable( true );
  }

  ngOnInit()
  {

  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async Send()
  {
    if ( !this.Valid() )
    {
      return;
    }
    
    var chat = this.Construct();

    var ticket = await this.chatservice.Send( chat, "Sending.." );

    if ( typeof( ticket.Code ) != undefined && ticket.Code  == -1 )
    {
      this.auth.ShowError( ticket.Message );
      
      return;
    }

    this.auth.RefreshChats = true;
    this.chatservice.Chat = ticket;
    this.chatservice.GoToChat = true;
    this.chatservice.ChatId = ticket.Id;
    this.chatservice.Chats.unshift( ticket );

    this.auth.GoToPage( "chats", true );
  }

  Valid()
  {
    var valid = true;

    // Required fields
    if ( this.Details === undefined || this.DepartmentId === undefined )
    {
      this.auth.ShowError( "Please complete the required information." );

      return false;
    }

    return valid;
  }

  Construct()
  {
    var msg =`DepartmentId=${this.DepartmentId}&Details=${this.Details}&IsClose=False&IsSupport=False`;

    return msg;
  }
}
