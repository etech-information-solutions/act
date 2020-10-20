import { Component, OnInit } from '@angular/core';

import { Platform } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { UserService } from './services/user.service';

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
      icon: 'home'
    },
    {
      title: 'Site Audit',
      url: '/siteaudit',
      icon: 'site-audit'
    },
    {
      title: 'Outstanding Report',
      url: '/outstandingreport',
      icon: 'outstandingreport'
    },
    {
      title: 'POD Management',
      url: '/podmanagement',
      icon: 'podmanagement'
    },
    {
      title: 'Notifications',
      url: '/notifications',
      icon: 'notifications'
    },
    {
      title: 'Settings',
      url: '/settings',
      icon: 'settings'
    },
    {
      title: 'Logout',
      url: '/logout',
      icon: 'log-out'
    }
  ];

  constructor( private platform: Platform, private splashScreen: SplashScreen, private statusBar: StatusBar, private auth: UserService )
  {
    this.initializeApp();
  }

  initializeApp()
  {
    this.platform.ready().then(() =>
    {
      this.statusBar.styleDefault();
      this.splashScreen.hide();
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
