import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertController, IonContent, MenuController, PopoverController } from '@ionic/angular';
import { ChatService } from '../services/chat.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-chats',
  templateUrl: './chats.page.html',
  styleUrls: ['./chats.page.scss'],
})
export class ChatsPage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;

  Chats: any = [];

  Skip: number = 0;
  Query: string = "";

  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService, public chatservice: ChatService, public popCtrl: PopoverController )
  {
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }

    if ( this.chatservice.GoToChat )
    {
      this.auth.GoToPage( "chatdetail" );
    }
    
    this.menuCtrl.enable( true );
  }

  async ngOnInit()
  {
    await this.List();
  }

  async ionViewWillEnter()
  {
    if ( this.chatservice.GoToChat )
    {
      this.auth.GoToPage( "chatdetail" );
    }

    if ( this.auth.RefreshChats )
    {
      this.auth.RefreshChats = false;

      this.Skip = 0;
      this.Chats = [];
      
      await this.List();
    }

    this.chatservice.RefreshChats();
  }

  async List()
  {
    var not = await this.chatservice.List( this.Query );

    /*var useChats = this.chatservice.Chats;

    if ( this.Query != "" )
    {
      useChats = not;
    }

    if ( not != undefined && not.length > this.Chats.length )
    {
      not = [];

      var c = this.auth.Take;

      for ( var i = this.Skip; i < useChats.length; i++ )
      {
        if ( c <= 0 )
        {
          break;
        }

        c--;

        not.push( useChats[ i ] );
      }

      this.Skip += not.length;
  
      this.Chats = this.Chats.concat( not );
    }*/
  }

  async loadData( event:any )
  {
    await this.List();

    event.target.complete();
  }

  async Search()
  {
    this.Skip = 0;
    this.Chats = [];

    await this.List();
  }

  async Refresh( refresher )
  {
    await this.List();
    
    refresher.target.complete();
  }

  async GoToChat ( id:number )
  {
    var i = this.chatservice.Chats.findIndex( n => n.Id == id );

    this.chatservice.ChatId = id;
    this.chatservice.Chat = this.chatservice.Chats[ i ];

    this.auth.GoToPage( "chatdetail" );
  }

  async Add()
  {
    this.auth.GoToPage( "chatadd" );
  }
}
