import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController, PopoverController } from '@ionic/angular';
import { ChatService } from '../services/chat.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-chatdetail',
  templateUrl: './chatdetail.page.html',
  styleUrls: ['./chatdetail.page.scss'],
})
export class ChatdetailPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;

  Details: any;

  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService, public chatservice: ChatService, public popCtrl: PopoverController )
  {
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }
    
    this.menuCtrl.enable( true );

    this.chatservice.GoToChat = false;
  }

  async ngOnInit()
  {
    this.MarkAsRead();

    this.content.scrollToBottom();
  }

  async MarkAsRead()
  {
    await this.chatservice.MarkAsRead( this.chatservice.Chat.Id );

    var x = this.chatservice.Chats.findIndex( n => n.Id == this.chatservice.Chat.Id );

    for ( var i = 0; i < this.chatservice.Chat.Messages.length; i++ )
    {
      var msg = this.chatservice.Chat.Messages[ i ];

      if ( msg.ReceiverUserId == this.auth.CurrentUser.Id && msg.Status == 0 )
      {
        msg.Status = 1;

        this.chatservice.Chat.Messages[ i ] = msg;

        // Update chat
        this.chatservice.Chats[ x ].Messages[ i ] = msg;
      }
    }

    if ( this.chatservice.Chat.MessageSenderUserId != this.auth.CurrentUser.Id )
    {
      this.chatservice.Chat.MessageStatus = 1;
    }

    this.chatservice.RefreshTicket( this.chatservice.Chat.Id, this.content );
  }

  async Send()
  {
    if ( this.Details == undefined || this.Details === "" )
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

    var i = this.chatservice.Chats.findIndex( n => n.Id == ticket.Id );

    // Update chat
    this.auth.RefreshChats = true;
    this.chatservice.Chat = ticket;
    this.chatservice.Chats[ i ] = ticket;
    this.chatservice.Chats[ i ].Messages = ticket.Messages;

    // Update view
    this.Details = "";

    this.content.scrollToBottom();
  }

  Construct()
  {
    var receiverUserId = ( this.chatservice.Chat.OwnerUserId == this.auth.CurrentUser.Id ) ? this.chatservice.Chat.SupportUserId : this.chatservice.Chat.OwnerUserId;

    var isSupport = ( this.chatservice.Chat.SupportUserId == this.auth.CurrentUser.Id );

    var msg =`ReceiverUserId=${receiverUserId}&TicketId=${this.chatservice.Chat.Id}&Details=${this.Details}&IsClose=False&IsSupport=${isSupport}`;

    return msg;
  }
}
