import { Component, OnInit } from '@angular/core';

import { Platform } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { UserService } from './services/user.service';

import { FirebaseX } from '@ionic-native/firebase-x/ngx';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss']
})
export class AppComponent implements OnInit
{
  public selectedIndex = 0;
  public appPages = [
    {
      title: 'Home',
      url: '/home',
      icon: 'home',
      strict: true
    },
    {
      title: 'Site Audit',
      url: '/siteaudit',
      icon: 'shield-checkmark',
      strict: true
    },
    {
      title: 'Outstanding Report',
      url: '/outstandingreport',
      icon: 'mail',
      strict: false
    },
    {
      title: 'POD Management',
      url: '/podmanagement',
      icon: 'document-text',
      strict: false
    },
    {
      title: 'Notifications',
      url: '/notifications',
      icon: 'notifications',
      strict: false
    },
    {
      title: 'Settings',
      url: '/settings',
      icon: 'settings',
      strict: false
    },
    {
      title: 'Chats',
      url: '/chats',
      icon: 'chatbubbles',
      strict: false
    },
    {
      title: 'Logout',
      url: '/logout',
      icon: 'log-out',
      strict: false
    }
  ];

  constructor( private platform: Platform, private splashScreen: SplashScreen, private statusBar: StatusBar, public auth: UserService, public firebase: FirebaseX )
  {
    this.initializeApp();
  }

  initializeApp()
  {
    this.platform.ready().then(() =>
    {
      this.statusBar.styleDefault();
      this.splashScreen.hide();

      // Get token
      this.firebase.getToken().then( token =>
      {
        this.auth.UpdateDeviceId( token );

        alert( token );
      }).catch( err =>
      {
        alert( err );
      });


      // Handle received notification
      this.firebase.onMessageReceived().subscribe( n =>
      {
        alert(n);

      }, err =>
      {
        alert( err );
      });
    });
  }

  ngOnInit()
  {
    const path = window.location.pathname.split('/')[1];

    if (path !== undefined)
    {
      this.selectedIndex = this.appPages.findIndex(page => page.title.toLowerCase() === path.toLowerCase());
    }
  }
}
