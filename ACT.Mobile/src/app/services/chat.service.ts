import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserService } from './user.service';

@Injectable({ providedIn: 'root' })
export class ChatService
{
  Chat: any;
  Chats: any;
  ChatId: any;

  Departments: any;

  GoToChat: boolean = false;

  Timeout: any;
  Interval: number = 10000;
  OInterval: number = 10000;

  constructor( public auth: UserService )
  {
    this.GetDepartments();
  }

  async GetDepartments()
  {
    try
    {
      if ( this.Departments != undefined )
      {
        return;
      }

      var url = `${this.auth.APIUrl}/api/Chat/GetDepartments?apikey=${this.auth.APIKey}`;
      this.Departments = await this.auth.http.get<iCommonModel>( url ).toPromise();
    }
    catch( error )
    {
      return;
    }
  }

  Search( query:string = "" )
  {
    var resp = [];

    query = query.toLowerCase();

    for ( let r of this.Chats )
    {
      if ( ( r.Number != undefined && r.Number.toLowerCase().includes( query ) ) ||
           ( r.Message != undefined && r.Message.toLowerCase().includes( query ) ) ||
           ( r.OwnerName != undefined && r.OwnerName.toLowerCase().includes( query ) ) ||
           ( r.SupportName != undefined && r.SupportName.toLowerCase().includes( query ) ) ||
           ( r.DepartmentName != undefined && r.DepartmentName.toLowerCase().includes( query ) ) )
      {
        resp.push( r );
      }
    }

    return resp;
  }

  async List( query:string = "" )
  {
    if ( this.Chats != undefined && this.Chats.length > 0 )
    {
      if ( query != "" )
      {
        return this.Search( query );
      }

      return this.Chats;
    }

    var loading = await this.auth.ShowLoading( "Fetching your chats..." );

    try
    {
      var url = `${this.auth.APIUrl}/api/Chat/List?email=${this.auth.CurrentUser.Email}&UserId=${this.auth.CurrentUser.Id}&apikey=${this.auth.APIKey}`;

      this.Chats = await this.auth.http.get<iCommonModel>( url ).toPromise();

      if ( query != "" )
      {
        return this.Search( query );
      }
    }
    catch( error )
    {
      loading.dismiss();

      if ( this.Chats == undefined )
      {
        this.Chats = {};
      }

      this.Chats.ResponseCode = -1;
      this.Chats.Description = JSON.stringify( error );

      return [];
    }

    loading.dismiss();

    return this.Chats;
  }

  async Send( chat: any, message: string = undefined )
  {
    var loading = await this.auth.ShowLoading( message );

    // API Call here
    try
    {
      chat = `${chat}&email=${this.auth.CurrentUser.Email}&SenderUserId=${this.auth.CurrentUser.Id}&apikey=${this.auth.APIKey}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      // Update site audit
      var resp = await this.auth.http.post<iCommonModel>( this.auth.APIUrl + "/api/Chat/Send", chat, httpOptions ).toPromise();

      loading.dismiss();

      return resp;
    }
    catch( error )
    {
      loading.dismiss();

      return { Id: -1, Code: -1, Message: JSON.stringify( error ), Messages: [] };
    }
  }

  async Receive( ticketId: number, receiverUserId: number )
  {
    
  }

  async RefreshChats()
  {
    var me = this;

    clearTimeout( me.Timeout );

    if ( window.location.pathname.indexOf( "chats" ) < 0 )
    {
      return;
    }

    me.Timeout = setInterval( async function ()
    {
      var url = `${me.auth.APIUrl}/api/Chat/List?email=${me.auth.CurrentUser.Email}&UserId=${me.auth.CurrentUser.Id}&apikey=${me.auth.APIKey}`;

      var chats = await me.auth.http.get<iCommonModel>( url ).toPromise();

      if ( chats != me.Chats )
      {
        me.Chats = chats;
        me.auth.RefreshChats = true;

        me.Interval = me.OInterval;
      }
      else
      {
        //me.Interval = me.Interval * 2;
      }

      me.RefreshChats();

    }, me.Interval );
  }

  async RefreshTicket( ticketId: number, content: any )
  {
    var me = this;

    clearTimeout( me.Timeout );

    if ( window.location.pathname.indexOf( "chatdetail" ) < 0 ) // Only run this sync if we're on the chatdetail page
    {
      return;
    }

    var x = me.Chats.findIndex( n => n.Id == me.Chat.Id );

    me.Timeout = setInterval( async function ()
    {
      var url = `${me.auth.APIUrl}/api/Chat/Receive?email=${me.auth.CurrentUser.Email}&apikey=${me.auth.APIKey}&ticketId=${ticketId}`;

      var ticket = await me.auth.http.get<iCommonModel>( url ).toPromise();

      if ( me.Chats[ x ].Messages.length < ticket.Messages.length )
      {
        me.Chats[ x ].Messages = ticket.Messages;

        me.Interval = me.OInterval;

        content.scrollToBottom();
      }
      else
      {
        //me.Interval = me.Interval * 2;
      }

      me.RefreshTicket( ticketId, content );

    }, me.Interval );
  }

  async MarkAsRead( ticketId: number )
  {
    // API Call here
    try
    {
      var p = `email=${this.auth.CurrentUser.Email}&apikey=${this.auth.APIKey}&ticketId=${ticketId}&receiverUserId=${this.auth.CurrentUser.Id}`;

      let httpOptions = 
      {
        headers: new HttpHeaders(
        {
          "Content-Type": "application/x-www-form-urlencoded"
        })
      };

      // Update site audit
      await this.auth.http.post<iCommonModel>( this.auth.APIUrl + "/api/Chat/MarkAsRead", p, httpOptions ).toPromise();
    }
    catch( error )
    {
    }
  }
}

export interface iCommonModel
{
  Id: any;
  Code: any;
  Message: any;
  Messages: any;
}